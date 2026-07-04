using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
//using System.Data.SQLite;
using System.Text;

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


    public void dataConnect()
    {
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


    public void DownloadFile(string remoteFilePath, string localFilePath, long resumePosition = 0)
    {
        writer.WriteLine($"REST {resumePosition}");
        writer.Flush();
        reader.ReadLine();

        writer.WriteLine($"RETR {remoteFilePath}");
        writer.Flush();

        using (FileStream fs = new FileStream(localFilePath, resumePosition > 0 ? FileMode.Append : FileMode.Create))
        {
            fs.Seek(resumePosition, SeekOrigin.Begin);
            byte[] buffer = new byte[1024];
            int bytesRead;
            int i = 0;
            while ((bytesRead = datastream.Read(buffer, 0, buffer.Length)) > 0)
            {
                string response = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);
                if (response.Contains("EOF"))
                {
                    break;
                }
                fs.Write(buffer, 0, bytesRead);
                Console.WriteLine($"Received {bytesRead} bytes  (#{i})");
                i++;
            }
        }
        Console.WriteLine($"文件下载成功: {localFilePath}");
    }
    public void UploadFile(string localFilePath, string remoteFilePath, long resumePosition = 0)
    {
        writer.WriteLine($"REST {resumePosition}");
        writer.Flush();
        reader.ReadLine();

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
        Console.WriteLine($"文件上传完成: {localFilePath}");
        byte[] eofBuffer = Encoding.ASCII.GetBytes("EOF");
        datastream.Write(eofBuffer, 0, eofBuffer.Length);
    }
    public long GetResumePosition(string filePath)
    {
        if (File.Exists(filePath))
        {
            return new FileInfo(filePath).Length;
        }
        return 0;
    }
}