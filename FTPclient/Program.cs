// See https://aka.ms/new-console-template for more information
class Program
{
    static void Main(string[] args)
    {
        string server = args.Length > 0 ? args[0] : "127.0.0.1";
        string username = args.Length > 1 ? args[1] : "user";
        string password = args.Length > 2 ? args[2] : "password";
        FtpClient ftpClient = new FtpClient(server, username, password);

        if (ftpClient.Connect())
        {
            Console.WriteLine("连接成功");
            var files = ftpClient.ListDirectory(" ");
            foreach (var file in files)
            {
                Console.WriteLine(file);
            }

            string remoteDownloadFilePath = "x.pdf";
            string localDownloadFilePath = "downloaded_x.pdf";
            long resumePosition = ftpClient.GetResumePosition(localDownloadFilePath);
            ftpClient.DownloadFile(remoteDownloadFilePath, localDownloadFilePath);
            Console.WriteLine("文件下载成功");

            ftpClient.dataConnect();

            string localUploadFilePath = "upload_test.docx";
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