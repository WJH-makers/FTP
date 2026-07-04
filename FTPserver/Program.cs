// See https://aka.ms/new-console-template for more information
class Program
{
    static void Main(string[] args)
    {
        int port = 21;
        int dataport = 20;
        string rootDirectory = "C:\\FTPRoot";
        if (!Directory.Exists(rootDirectory))
        {
            Directory.CreateDirectory(rootDirectory);
        }
        FtpServer ftpServer = new FtpServer(port, dataport, rootDirectory, "ftp_server.db");
        ftpServer.Start();
    }
}