using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
/*
public class FtpServer
{
    private TcpListener listener;
    private TcpListener datalistener;
    private string rootDirectory;


    public FtpServer(int port, int dataport, string rootDirectory)
    {
        this.listener = new TcpListener(IPAddress.Any, port);
        this.datalistener = new TcpListener(IPAddress.Any, dataport);
        this.rootDirectory = rootDirectory;
    }

    public void Start()
    {
        listener.Start();
        datalistener.Start();
        Console.WriteLine("FTP服务器已启动，等待连接...");
        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            TcpClient dataclient = datalistener.AcceptTcpClient();
            Task.Run(() => HandleClient(client, dataclient));
        }
    }
    private void HandleClient(TcpClient client, TcpClient dataclient)
    {
        using (NetworkStream datastream = dataclient.GetStream())
        using (StreamReader datareader = new StreamReader(datastream))
        using (StreamWriter datawriter = new StreamWriter(datastream) { AutoFlush = true })
        using (NetworkStream stream = client.GetStream())
        using (StreamReader reader = new StreamReader(stream))
        using (StreamWriter writer = new StreamWriter(stream) { AutoFlush = true })
        {
            writer.WriteLine("220 Welcome to Simple FTP Server");
            string user = null;
            string pass = null;
            string currentDir = rootDirectory;
            long restPosition = 0;
            while (true)
            {
                string request = reader.ReadLine();
                Console.WriteLine("请求: " + request);
                string[] tokens = request.Split(' ');
                string command = tokens[0].ToUpperInvariant();
                string arguments = tokens.Length > 1 ? request.Substring(command.Length + 1).Trim() : null;

                switch (command)
                {
                    case "USER":
                        user = arguments;
                        writer.WriteLine("331 Username okay, need password");
                        Console.WriteLine("响应: 331 Username okay, need password");
                        break;
                    case "PASS":
                        pass = arguments;
                        writer.WriteLine("230 User logged in, proceed");
                        Console.WriteLine("响应: 230 User logged in, proceed");
                        break;
                    case "QUIT":
                        writer.WriteLine("221 Goodbye");
                        Console.WriteLine("响应: 221 Goodbye");
                        client.Close();
                        return;
                    case "REST":
                        restPosition = long.Parse(arguments);
                        writer.WriteLine("350 Restart position accepted (" + restPosition + ").");
                        Console.WriteLine("响应: 350 Restart position accepted (" + restPosition + ").");
                        break;
                    case "STOR":
                        HandleStorCommand(arguments, datastream, datawriter, currentDir, restPosition);
                        break;
                    case "RETR":
                        HandleRetrCommand(arguments, datastream, datawriter, currentDir, restPosition);
                        break;
                    case "LIST":
                        HandleListCommand(writer, currentDir);
                        break;
                    case "SIZE":
                        HandleSizeCommand(arguments, writer, currentDir);
                        break;
                    case "COMPLETE":
                        Console.WriteLine("文件完成传输 !");
                        break;
                    default:
                        writer.WriteLine("502 Command not implemented");
                        Console.WriteLine("响应: 502 Command not implemented");
                        break;
                }
            }
        }
    }
    private void HandleStorCommand(string fileName, NetworkStream stream, StreamWriter writer, string currentDir, long restPosition)
    {
        string filePath = Path.Combine(currentDir, fileName);

        using (FileStream fs = new FileStream(filePath, restPosition > 0 ? FileMode.Append : FileMode.Create))
        {
            fs.Seek(restPosition, SeekOrigin.Begin);

            byte[] buffer = new byte[1024];
            int bytesRead;
            int i = 0;
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                if (dataReceived.Contains("<EOF>"))//添加数据包大小为3的判断
                {
                    fs.Write(buffer, 0, bytesRead - 3);
                    break;
                }
                fs.Write(buffer, 0, bytesRead);
                Console.WriteLine($"Received{bytesRead}bytes(#{i})");
                i++;
            }
        }

        Console.WriteLine($"文件上传成功: {filePath}");
    }

    private void HandleRetrCommand(string fileName, NetworkStream stream, StreamWriter writer, string currentDir, long restPosition)
    {
        string filePath = Path.Combine(currentDir, fileName);
        if (File.Exists(filePath))
        {

            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                fs.Seek(restPosition, SeekOrigin.Begin);

                byte[] buffer = new byte[1024];
                int bytesRead;
                int i = 0;
                while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    stream.Write(buffer, 0, bytesRead);
                    Console.WriteLine($"Sent {bytesRead} bytes  (#{i})");
                    i++;
                }
            }

            Console.WriteLine($"文件下载成功: {filePath}");
            writer.WriteLine("<EOF>");
            writer.Flush();

        }
        else
        {
            Console.WriteLine($"文件未找到: {filePath}");
        }
    }

    private void HandleListCommand(StreamWriter writer, string currentDir)
    {
        writer.WriteLine("150 Opening ASCII mode data connection for file list");
        writer.Flush();

        string list = GetDirectoryListing(currentDir, 0);
        writer.WriteLine(list);
        writer.WriteLine("complete");
    }

    private string GetDirectoryListing(string path, int level)
    {
        string result = "";
        string indent = new string(' ', level * 2);

        foreach (var dir in Directory.GetDirectories(path))
        {
            result += $"{indent}{Path.GetFileName(dir)}/\n";
            result += GetDirectoryListing(dir, level + 1);
        }

        foreach (var file in Directory.GetFiles(path))
        {
            result += $"{indent}{Path.GetFileName(file)}\n";
        }

        return result;
    }
    private void HandleSizeCommand(string fileName, StreamWriter writer, string currentDir)
    {
        string filePath = Path.Combine(currentDir, fileName);
        if (File.Exists(filePath))
        {
            long fileSize = new FileInfo(filePath).Length;
            writer.WriteLine($"{fileSize}");
            Console.WriteLine($"响应:FileSize {fileSize}");
        }
        else
        {
            long fileSize = 0;
            writer.WriteLine($"{fileSize}");

        }
    }
}*/
using System.Data.SQLite;
using System.IO;
using System.Reflection;

public class DatabaseManager
{
    private string connectionString;

    public DatabaseManager(string dbPath)
    {
        if (!File.Exists(dbPath))
        {
            SQLiteConnection.CreateFile(dbPath);
        }

        connectionString = $"Data Source={dbPath};Version=3;";

        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            string createUserTable = "CREATE TABLE IF NOT EXISTS Users (Username TEXT PRIMARY KEY, Password TEXT)";
            using (var command = new SQLiteCommand(createUserTable, connection))
            {
                command.ExecuteNonQuery();
            }

            string createFileTable = "CREATE TABLE IF NOT EXISTS Files (FilePath TEXT PRIMARY KEY, Size INTEGER, LastModified TEXT)";
            using (var command = new SQLiteCommand(createFileTable, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }

    public void AddUser(string username, string password)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            string insertUser = "INSERT INTO Users (Username, Password) VALUES (@Username, @Password)";
            using (var command = new SQLiteCommand(insertUser, connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                var hash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(password)));
                command.Parameters.AddWithValue("@Password", hash);
                command.ExecuteNonQuery();
            }
        }
    }

    public bool ValidateUser(string username, string password)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            string query = "SELECT COUNT(1) FROM Users WHERE Username = @Username AND Password = @Password";
            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                var hash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(password)));
                command.Parameters.AddWithValue("@Password", hash);
                if (Convert.ToInt32(command.ExecuteScalar()) > 0)
                {
                    Console.WriteLine("老用户，欢迎再次光临!");
                    return true;
                }
                else
                {
                    AddUser(username, password);
                    Console.WriteLine("新用户，已经为您注册!");
                    return true;
                }
            }
        }
    }

    public void UpdateFileInfo(string filePath, long size, DateTime lastModified, int mode = 0)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            string upsertFile = "INSERT INTO Files (FilePath, Size, LastModified) VALUES (@FilePath, @Size, @LastModified) " +
                                "ON CONFLICT(FilePath) DO UPDATE SET Size = @Size, LastModified = @LastModified";
            using (var command = new SQLiteCommand(upsertFile, connection))
            {
                command.Parameters.AddWithValue("@FilePath", filePath);
                command.Parameters.AddWithValue("@Size", size);
                command.Parameters.AddWithValue("@LastModified", lastModified.ToString("yyyy-MM-dd HH:mm:ss"));
                if (mode == 0)
                {
                    Console.WriteLine("客户端上传成功");
                }
                else
                {
                    Console.WriteLine("客户端下载成功");
                }
                command.ExecuteNonQuery();
            }
        }
    }
}


public class FtpServer
{
    private TcpListener listener;
    private TcpListener datalistener;
    private string rootDirectory;
    private DatabaseManager dbManager;

    public FtpServer(int port, int dataport, string rootDirectory, string dbPath)
    {
        this.listener = new TcpListener(IPAddress.Any, port);
        this.datalistener = new TcpListener(IPAddress.Any, dataport);
        this.rootDirectory = rootDirectory;
        this.dbManager = new DatabaseManager(dbPath);
    }

    public void Start()
    {
        listener.Start();
        datalistener.Start();
        Console.WriteLine("FTP服务器已启动，等待连接...");
        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            TcpClient dataclient = datalistener.AcceptTcpClient();
            var remoteEP = dataclient.Client.RemoteEndPoint as IPEndPoint;
            var controlEP = client.Client.RemoteEndPoint as IPEndPoint;
            if (remoteEP?.Address?.ToString() != controlEP?.Address?.ToString())
            {
                Console.WriteLine("数据连接来源不匹配，拒绝连接");
                dataclient.Close();
                client.Close();
                continue;
            }
            Task.Run(() => HandleClient(client, dataclient));
        }
    }

    private void HandleClient(TcpClient client, TcpClient dataclient)
    {
        using (NetworkStream datastream = dataclient.GetStream())
        using (StreamReader datareader = new StreamReader(datastream))
        using (StreamWriter datawriter = new StreamWriter(datastream) { AutoFlush = true })
        using (NetworkStream stream = client.GetStream())
        using (StreamReader reader = new StreamReader(stream))
        using (StreamWriter writer = new StreamWriter(stream) { AutoFlush = true })
        {
            writer.WriteLine("220 Welcome to Simple FTP Server");
            string user = null;
            string pass = null;
            string currentDir = rootDirectory;
            long restPosition = 0;
            while (true)
            {
                string request = reader.ReadLine();
                if (request == null) return;
                Console.WriteLine("请求: " + (request.StartsWith("PASS") ? "PASS ***" : request));
                string[] tokens = request.Split(' ');
                string command = tokens[0].ToUpperInvariant();
                string arguments = tokens.Length > 1 ? request.Substring(command.Length + 1).Trim() : null;

                switch (command)
                {
                    case "USER":
                        user = arguments;
                        writer.WriteLine("331 Username okay, need password");
                        Console.WriteLine("响应: 331 Username okay, need password");
                        break;
                    case "PASS":
                        pass = arguments;
                        if (dbManager.ValidateUser(user, pass))
                        {
                            writer.WriteLine("230 User logged in, proceed");
                            Console.WriteLine("响应: 230 User logged in, proceed");
                        }
                        else
                        {
                            writer.WriteLine("530 Login incorrect");
                            Console.WriteLine("响应: 530 Login incorrect");
                        }
                        break;
                    case "QUIT":
                        writer.WriteLine("221 Goodbye");
                        Console.WriteLine("响应: 221 Goodbye");
                        client.Close();
                        return;
                    case "REST":
                        if (!long.TryParse(arguments, out restPosition))
                        {
                            writer.WriteLine("501 Invalid REST parameter.");
                            break;
                        }
                        writer.WriteLine("350 Restart position accepted (" + restPosition + ").");
                        Console.WriteLine("响应: 350 Restart position accepted (" + restPosition + ").");
                        break;
                    case "STOR":
                        HandleStorCommand(arguments, datastream, datawriter, currentDir, restPosition);
                        break;
                    case "RETR":
                        HandleRetrCommand(arguments, datastream, datawriter, currentDir, restPosition);
                        break;
                    case "LIST":
                        HandleListCommand(writer, currentDir);
                        break;
                    case "SIZE":
                        HandleSizeCommand(arguments, writer, currentDir);
                        break;
                    default:
                        writer.WriteLine("502 Command not implemented");
                        Console.WriteLine("响应: 502 Command not implemented");
                        break;
                }
            }
        }
    }

    private void HandleStorCommand(string fileName, NetworkStream stream, StreamWriter writer, string currentDir, long restPosition)
    {
        string filePath = Path.GetFullPath(Path.Combine(currentDir, fileName));
        if (!filePath.StartsWith(Path.GetFullPath(currentDir), StringComparison.OrdinalIgnoreCase))
        {
            writer.WriteLine("550 Path traversal detected.");
            return;
        }

        byte[] lenBuf = new byte[8];
        int read = stream.Read(lenBuf, 0, 8);
        if (read < 8)
        {
            writer.WriteLine("450 Failed to receive file length.");
            return;
        }
        long dataLen = BitConverter.ToInt64(lenBuf, 0);

        using (FileStream fs = new FileStream(filePath, restPosition > 0 ? FileMode.Append : FileMode.Create))
        {
            fs.Seek(restPosition, SeekOrigin.Begin);

            byte[] buffer = new byte[1024];
            long remaining = dataLen;
            while (remaining > 0)
            {
                int toRead = (int)Math.Min(buffer.Length, remaining);
                int bytesRead = stream.Read(buffer, 0, toRead);
                if (bytesRead <= 0) break;
                fs.Write(buffer, 0, bytesRead);
                remaining -= bytesRead;
            }
        }

        FileInfo fileInfo = new FileInfo(filePath);
        dbManager.UpdateFileInfo(filePath, fileInfo.Length, fileInfo.LastWriteTime, 0);
        Console.WriteLine($"文件上传成功: {filePath}");
    }

    private void HandleRetrCommand(string fileName, NetworkStream stream, StreamWriter writer, string currentDir, long restPosition)
    {
        string filePath = Path.GetFullPath(Path.Combine(currentDir, fileName));
        if (!filePath.StartsWith(Path.GetFullPath(currentDir), StringComparison.OrdinalIgnoreCase))
        {
            writer.WriteLine("550 Path traversal detected.");
            return;
        }
        if (File.Exists(filePath))
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                fs.Seek(restPosition, SeekOrigin.Begin);

                long len = fs.Length - restPosition;
                byte[] lenPrefix = BitConverter.GetBytes(len);
                stream.Write(lenPrefix, 0, 8);

                byte[] buffer = new byte[1024];
                int bytesRead;
                while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    stream.Write(buffer, 0, bytesRead);
                }
            }
            FileInfo fileInfo = new FileInfo(filePath);
            Console.WriteLine($"文件下载成功: {filePath}");
            dbManager.UpdateFileInfo(filePath, fileInfo.Length, fileInfo.LastWriteTime, 1);
        }
        else
        {
            Console.WriteLine($"文件未找到: {filePath}");
        }
    }

    private void HandleListCommand(StreamWriter writer, string currentDir)
    {
        writer.WriteLine("150 Opening ASCII mode data connection for file list");
        writer.Flush();

        string list = GetDirectoryListing(currentDir, 0);
        writer.WriteLine(list);
        writer.WriteLine("complete");
    }

    private string GetDirectoryListing(string path, int level)
    {
        string result = "";
        string indent = new string(' ', level * 2);

        foreach (var dir in Directory.GetDirectories(path))
        {
            result += $"{indent}{Path.GetFileName(dir)}/\n";
            result += GetDirectoryListing(dir, level + 1);
        }

        foreach (var file in Directory.GetFiles(path))
        {
            result += $"{indent}{Path.GetFileName(file)}\n";
        }

        return result;
    }

    private void HandleSizeCommand(string fileName, StreamWriter writer, string currentDir)
    {
        string filePath = Path.Combine(currentDir, fileName);
        if (File.Exists(filePath))
        {
            long fileSize = new FileInfo(filePath).Length;
            writer.WriteLine($"{fileSize}");
            Console.WriteLine($"响应: FileSize {fileSize}");
        }
        else
        {
            writer.WriteLine("0");
        }
    }
}
