// See https://aka.ms/new-console-template for more information
class Program
{
    static void Main(string[] args)
    {
        //string dbPath = "ftp_client.db";
        string server = "127.0.0.1"; // 本地测试服务器地址
        string username = "user";
        string password = "password";
        FtpClient ftpClient = new FtpClient(server, username, password);

        if (ftpClient.Connect())
        {
            Console.WriteLine("连接成功");
            var files = ftpClient.ListDirectory(" ");
            foreach (var file in files)
            {
                Console.WriteLine(file);
            }
            // 下载文件
            string remoteDownloadFilePath = "x.pdf";
            string localDownloadFilePath = "C:\\Users\\万佳泓\\OneDrive\\桌面\\x.pdf";
            long resumePosition = ftpClient.GetResumePosition(localDownloadFilePath);
            ftpClient.DownloadFile(remoteDownloadFilePath, localDownloadFilePath);
            Console.WriteLine("文件下载成功");

            // 断开连接并重新连接
            ftpClient.dataConnect();


            //上传文件
            string localUploadFilePath = "C:\\Users\\万佳泓\\OneDrive\\桌面\\2024计算机网络课程设计--软件设计要求.docx";
            string remoteUploadFilePath = "y.docx";
            resumePosition = ftpClient.GetResumePosition(localUploadFilePath);
            ftpClient.UploadFile(localUploadFilePath, remoteUploadFilePath);
            Console.WriteLine("文件上传成功");
        }
        else
        {
            Console.WriteLine("连接失败");
        }
    }
}