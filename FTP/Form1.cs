using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace FTP
{
    public partial class Form1 : Form
    {
        private FtpClient ftpClient;
        private bool isConnect = false;
        private bool resumeOpen = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //button_Connect.Click += new EventHandler(button_Connect_Click);
        }

        private void password_Click(object sender, EventArgs e)
        {

        }

        private void list_Dictionary_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void UpFileName_Click(object sender, EventArgs e)
        {

        }

        private void txtUpAddress_TextChanged(object sender, EventArgs e)
        {

        }

        private void UploadAddress_Click(object sender, EventArgs e)
        {

        }

        private void txtUpFileName_TextChanged(object sender, EventArgs e)
        {

        }

        private void downloadAddress_Click(object sender, EventArgs e)
        {

        }

        private void txtDownAddress_TextChanged(object sender, EventArgs e)
        {

        }

        private void button_Connect_Click(object sender, EventArgs e)
        {
            String textServer = txtServer.Text;
            String textUserName = txtUserName.Text;
            String textPassword = txtPassword.Text;
            ftpClient = new FtpClient(textServer, textUserName, textPassword);
            isConnect = ftpClient.Connect();
            if (isConnect)
            {
                var files = ftpClient.ListDirectory(" ");
                DisplayFilesInListView(files);
                MessageBox.Show("连接成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            else
            {
                MessageBox.Show("无法连接到FTP服务器", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void DisplayFilesInListView(List<string> files)
        {
            list_Dictionary.Items.Clear(); // 清空现有的项目
            foreach (string file in files)
            {
                ListViewItem item = new ListViewItem(file);
                list_Dictionary.Items.Add(item);
            }
        }

        private void Dictioanry_Click(object sender, EventArgs e)
        {

        }

        private void button_Resume_Click(object sender, EventArgs e)
        {
            if (!resumeOpen)
            {
                resumeOpen = true;
                MessageBox.Show("开启断点续传功能 ！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                resumeOpen = false;
                MessageBox.Show("关闭断点续传功能 ！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button_Disconnect_Click(object sender, EventArgs e)
        {
            ftpClient.dataDisconnect();
        }

        private void button_Upload_Click(object sender, EventArgs e)
        {
            string localUploadFilePath = txtUpAddress.Text;
            string remoteUploadFilePath = txtUpFileName.Text;
            if (isConnect)
            {
                if (resumeOpen)
                {
                    ftpClient.UploadFileWithResume(localUploadFilePath, remoteUploadFilePath);
                    MessageBox.Show("文件上传成功 ！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    ftpClient.UploadFile(localUploadFilePath, remoteUploadFilePath);
                    MessageBox.Show("文件上传成功 ！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("未连接服务器 ！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button_Download_Click(object sender, EventArgs e)
        {
            string remoteDownloadFilePath = txtDownAddress.Text;
            string localDownloadFilePath = txtDownPath.Text;
            if (isConnect)
            {
                if (resumeOpen)
                {
                    ftpClient.DownloadFileWithResume(remoteDownloadFilePath, localDownloadFilePath);
                    MessageBox.Show("文件下载成功 ！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    ftpClient.DownloadFile(remoteDownloadFilePath, localDownloadFilePath);
                    MessageBox.Show("文件下载成功 ！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("未连接服务器 ！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}

