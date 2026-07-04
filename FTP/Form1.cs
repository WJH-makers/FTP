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

        private async void button_Connect_Click(object sender, EventArgs e)
        {
            button_Connect.Enabled = false;
            String textServer = txtServer.Text;
            String textUserName = txtUserName.Text;
            String textPassword = txtPassword.Text;
            bool connected = await Task.Run(() =>
            {
                var client = new FtpClient(textServer, textUserName, textPassword);
                bool ok = client.Connect();
                if (ok)
                {
                    ftpClient = client;
                }
                return ok;
            });
            if (connected)
            {
                var files = await Task.Run(() => ftpClient.ListDirectory(" "));
                DisplayFilesInListView(files);
                MessageBox.Show("���ӳɹ���", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("�޷����ӵ�FTP������", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            button_Connect.Enabled = true;
        }
        private void DisplayFilesInListView(List<string> files)
        {
            list_Dictionary.Items.Clear(); // ������е���Ŀ
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
                MessageBox.Show("�����ϵ��������� ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                resumeOpen = false;
                MessageBox.Show("�رնϵ��������� ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button_Disconnect_Click(object sender, EventArgs e)
        {
            ftpClient.dataDisconnect();
        }

        private async void button_Upload_Click(object sender, EventArgs e)
        {
            string localUploadFilePath = txtUpAddress.Text;
            string remoteUploadFilePath = txtUpFileName.Text;
            if (isConnect)
            {
                button_Upload.Enabled = false;
                await Task.Run(() =>
                {
                    if (resumeOpen)
                    {
                        ftpClient.UploadFileWithResume(localUploadFilePath, remoteUploadFilePath);
                    }
                    else
                    {
                        ftpClient.UploadFile(localUploadFilePath, remoteUploadFilePath);
                    }
                });
                MessageBox.Show("�ļ��ϴ��ɹ� ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                button_Upload.Enabled = true;
            }
            else
            {
                MessageBox.Show("δ���ӷ����� ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void button_Download_Click(object sender, EventArgs e)
        {
            string remoteDownloadFilePath = txtDownAddress.Text;
            string localDownloadFilePath = txtDownPath.Text;
            if (isConnect)
            {
                button_Download.Enabled = false;
                await Task.Run(() =>
                {
                    if (resumeOpen)
                    {
                        ftpClient.DownloadFileWithResume(remoteDownloadFilePath, localDownloadFilePath);
                    }
                    else
                    {
                        ftpClient.DownloadFile(remoteDownloadFilePath, localDownloadFilePath);
                    }
                });
                MessageBox.Show("�ļ����سɹ� ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                button_Download.Enabled = true;
            }
            else
            {
                MessageBox.Show("δ���ӷ����� ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}

