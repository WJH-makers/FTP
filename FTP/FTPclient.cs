using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;



namespace FTP
{
    public class FtpClient
    {
        private string server;
        private string username;
        private string password;
        public FtpClient(string server, string username, string password)
        {
            this.server = server;
            this.username = username;
            this.password = password;

        }
        private TcpClient client;
        private NetworkStream stream;
        private StreamReader reader;
        private StreamWriter writer;

        private TcpClient dataclient;
        private NetworkStream datastream;
        private StreamReader datareader;
        private StreamWriter datawriter;


        public bool Connect()
        {
            try
            {
                client = new TcpClient(server, 21);
                dataclient = new TcpClient(server, 20);

                datastream = dataclient.GetStream();
                datareader = new StreamReader(datastream);
                datawriter = new StreamWriter(datastream) { AutoFlush = true };

                stream = client.GetStream();
                reader = new StreamReader(stream);
                writer = new StreamWriter(stream) { AutoFlush = true };

                string response = reader.ReadLine();
                Console.WriteLine("服务器响应: " + response);

                writer.WriteLine($"USER {username}");
                writer.Flush();
                response = reader.ReadLine();
                Console.WriteLine("服务器响应: " + response);

                if (!response.StartsWith("331"))
                {
                    return false;
                }

                writer.WriteLine($"PASS {password}");
                writer.Flush();
                response = reader.ReadLine();
                Console.WriteLine("服务器响应: " + response);

                if (!response.StartsWith("230"))
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("连接失败: " + ex.Message);
                return false;
            }
        }
        public void dataDisconnect()
        {
            datawriter.Close();
            datareader.Close();
            datastream.Close();
            dataclient.Close();
        }
        public void dataConnect()
        {
            dataclient = new TcpClient(server, 20);
            datastream = dataclient.GetStream();
            datareader = new StreamReader(datastream);
            datawriter = new StreamWriter(datastream) { AutoFlush = true };
        }


        public List<string> ListDirectory(string path)
        {
            writer.WriteLine($"LIST {path}");
            writer.Flush();
            List<string> files = new List<string>();

            string response = reader.ReadLine();
            while (!response.StartsWith("complete"))
            {
                if (response.StartsWith("150"))
                {
                    response = reader.ReadLine();
                    continue;
                }
                files.Add(response);
                response = reader.ReadLine();
            }
            return files;
        }



        public void DownloadFileWithResume(string remoteFilePath, string localFilePath)
        {
            long remoteFileSize = GetRemoteFileSize(remoteFilePath);
            long localFileSize = 0;

            // 检查本地文件是否存在，并获取其大小
            if (File.Exists(localFilePath))
            {
                localFileSize = new FileInfo(localFilePath).Length;
            }

            if (localFileSize < remoteFileSize)
            {
                writer.WriteLine($"REST {localFileSize}");
                writer.Flush();
                string response = reader.ReadLine(); // 读取服务器响应

                if (response.StartsWith("350"))
                {
                    writer.WriteLine($"RETR {remoteFilePath}");
                    writer.Flush();

                    using (FileStream fs = new FileStream(localFilePath, localFileSize > 0 ? FileMode.Append : FileMode.Create))
                    {
                        fs.Seek(localFileSize, SeekOrigin.Begin);
                        byte[] buffer = new byte[1024];
                        int bytesRead;
                        int i = 0;
                        while ((bytesRead = datastream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            string check = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);
                            if (check.Contains("<EOF>"))//添加数据包大小为5的判断
                            {
                                fs.Write(buffer, 0, bytesRead - 5);
                                break;
                            }
                            fs.Write(buffer, 0, bytesRead);
                            Console.WriteLine($"Received {bytesRead} bytes  (#{i})");
                            i++;


                        }
                    }
                    writer.WriteLine("complete");
                    Console.WriteLine($"文件下载成功: {localFilePath}");
                }
                else
                {
                    Console.WriteLine("REST command failed.");
                }
            }
            else
            {
                Console.WriteLine("Local file is up-to-date.");
            }
        }

        public void UploadFileWithResume(string localFilePath, string remoteFilePath)
        {
            long fileOffset = GetRemoteFileSize(remoteFilePath); // 获取服务器上的文件大小

            if (fileOffset > 0)
            {
                writer.WriteLine($"REST {fileOffset}"); // 发送REST命令
                writer.Flush();
            }

            writer.WriteLine($"STOR {remoteFilePath}"); // 发送STOR命令
            writer.Flush();

            using (FileStream fs = new FileStream(localFilePath, FileMode.Open))
            {
                fs.Seek(fileOffset, SeekOrigin.Begin); // 从偏移量处开始读取本地文件
                byte[] buffer = new byte[1024];
                int bytesRead;
                int i = 0;
                while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    datastream.Write(buffer, 0, bytesRead);
                    Console.WriteLine($"Sent {bytesRead} bytes  (#{i})");
                    i++;
                }
            }

            writer.WriteLine("complete");
            Console.WriteLine($"文件上传完成: {localFilePath}");
            byte[] eofBuffer = Encoding.ASCII.GetBytes("<EOF>");
            datastream.Write(eofBuffer, 0, eofBuffer.Length);
        }

        private long GetRemoteFileSize(string remoteFilePath)
        {
            // 这里需要实现获取远程文件大小的逻辑，可以使用FTP SIZE命令
            // 返回文件大小（字节数）
            // 示例代码:
            writer.WriteLine($"SIZE {remoteFilePath}");
            writer.Flush();
            string response = reader.ReadLine(); // 读取服务器响应
            long fileSize = 0;
            if (long.TryParse(response, out fileSize))
            {
                return fileSize;
            }
            return 0;
        }

        public void DownloadFile(string remoteFilePath, string localFilePath, long resumePosition = 0)
        {
            writer.WriteLine($"RETR {remoteFilePath}");
            writer.Flush();

            using (FileStream fs = new FileStream(localFilePath, FileMode.Create))
            {
                fs.Seek(resumePosition, SeekOrigin.Begin);
                byte[] buffer = new byte[1024];
                int bytesRead;
                int i = 0;
                while ((bytesRead = datastream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string response = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    if (response.Contains("<EOF>"))//添加数据包大小为5的判断，测试一下需不需要
                    {
                        fs.Write(buffer, 0, bytesRead - 5);
                        break;
                    }
                    fs.Write(buffer, 0, bytesRead);
                    Console.WriteLine($"Received {bytesRead} bytes  (#{i})");
                    i++;
                }
            }
            writer.WriteLine("complete");
            Console.WriteLine($"文件下载成功: {localFilePath}");
        }
        public void UploadFile(string localFilePath, string remoteFilePath, long resumePosition = 0)
        {
            writer.WriteLine($"STOR {remoteFilePath}");
            writer.Flush();

            using (FileStream fs = new FileStream(localFilePath, FileMode.Open))
            {
                fs.Seek(resumePosition, SeekOrigin.Begin);
                byte[] buffer = new byte[1024];
                int bytesRead;
                int i = 0;
                while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    datastream.Write(buffer, 0, bytesRead);
                    Console.WriteLine($"Sent {bytesRead} bytes  (#{i})");
                    i++;
                }
            }
            writer.WriteLine("complete");
            Console.WriteLine($"文件上传完成: {localFilePath}");
            byte[] eofBuffer = Encoding.ASCII.GetBytes("<EOF>");
            datastream.Write(eofBuffer, 0, eofBuffer.Length);
        }

    }

}
