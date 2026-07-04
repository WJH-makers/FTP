namespace FTP
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            server = new Label();
            userName = new Label();
            password = new Label();
            button_Connect = new Button();
            button_Disconnect = new Button();
            txtServer = new TextBox();
            txtUserName = new TextBox();
            txtPassword = new TextBox();
            Dictioanry = new Label();
            list_Dictionary = new ListView();
            UploadAddress = new Label();
            downloadAddress = new Label();
            UpFileName = new Label();
            downFileName = new Label();
            txtUpAddress = new TextBox();
            txtUpFileName = new TextBox();
            button_Upload = new Button();
            button_Download = new Button();
            button_Resume = new Button();
            Dictionary = new Label();
            txtDownPath = new TextBox();
            txtDownAddress = new TextBox();
            SuspendLayout();
            // 
            // server
            // 
            server.AutoSize = true;
            server.Location = new Point(12, 23);
            server.Name = "server";
            server.Size = new Size(84, 20);
            server.TabIndex = 0;
            server.Text = "服务器地址";
            // 
            // userName
            // 
            userName.AutoSize = true;
            userName.Location = new Point(249, 23);
            userName.Name = "userName";
            userName.Size = new Size(54, 20);
            userName.TabIndex = 1;
            userName.Text = "用户名";
            // 
            // password
            // 
            password.AutoSize = true;
            password.Location = new Point(453, 23);
            password.Name = "password";
            password.Size = new Size(39, 20);
            password.TabIndex = 2;
            password.Text = "密码";
            password.Click += password_Click;
            // 
            // button_Connect
            // 
            button_Connect.Location = new Point(669, 20);
            button_Connect.Name = "button_Connect";
            button_Connect.Size = new Size(94, 29);
            button_Connect.TabIndex = 3;
            button_Connect.Text = "连接";
            button_Connect.UseVisualStyleBackColor = true;
            button_Connect.Click += button_Connect_Click;
            // 
            // button_Disconnect
            // 
            button_Disconnect.Location = new Point(669, 55);
            button_Disconnect.Name = "button_Disconnect";
            button_Disconnect.Size = new Size(94, 29);
            button_Disconnect.TabIndex = 4;
            button_Disconnect.Text = "断开";
            button_Disconnect.UseVisualStyleBackColor = true;
            button_Disconnect.Click += button_Disconnect_Click;
            // 
            // txtServer
            // 
            txtServer.Location = new Point(102, 20);
            txtServer.Name = "txtServer";
            txtServer.Size = new Size(125, 27);
            txtServer.TabIndex = 5;
            // 
            // txtUserName
            // 
            txtUserName.Location = new Point(309, 20);
            txtUserName.Name = "txtUserName";
            txtUserName.Size = new Size(125, 27);
            txtUserName.TabIndex = 6;
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(498, 20);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(125, 27);
            txtPassword.TabIndex = 7;
            // 
            // Dictioanry
            // 
            Dictioanry.AutoSize = true;
            Dictioanry.Location = new Point(320, 64);
            Dictioanry.Name = "Dictioanry";
            Dictioanry.Size = new Size(0, 20);
            Dictioanry.TabIndex = 8;
            Dictioanry.Click += Dictioanry_Click;
            // 
            // list_Dictionary
            // 
            list_Dictionary.Location = new Point(26, 101);
            list_Dictionary.Name = "list_Dictionary";
            list_Dictionary.Size = new Size(751, 224);
            list_Dictionary.TabIndex = 9;
            list_Dictionary.UseCompatibleStateImageBehavior = false;
            list_Dictionary.View = View.Details;
            list_Dictionary.SelectedIndexChanged += list_Dictionary_SelectedIndexChanged;
            list_Dictionary.Columns.Add("目录", 200, HorizontalAlignment.Left); // 添加一列
            // 
            // UploadAddress
            // 
            UploadAddress.AutoSize = true;
            UploadAddress.Location = new Point(12, 350);
            UploadAddress.Name = "UploadAddress";
            UploadAddress.Size = new Size(99, 20);
            UploadAddress.TabIndex = 10;
            UploadAddress.Text = "上传文件地址";
            UploadAddress.Click += UploadAddress_Click;
            // 
            // downloadAddress
            // 
            downloadAddress.AutoSize = true;
            downloadAddress.Location = new Point(12, 407);
            downloadAddress.Name = "downloadAddress";
            downloadAddress.Size = new Size(99, 20);
            downloadAddress.TabIndex = 11;
            downloadAddress.Text = "下载文件地址";
            downloadAddress.Click += downloadAddress_Click;
            // 
            // UpFileName
            // 
            UpFileName.AutoSize = true;
            UpFileName.Location = new Point(394, 351);
            UpFileName.Name = "UpFileName";
            UpFileName.Size = new Size(69, 20);
            UpFileName.TabIndex = 12;
            UpFileName.Text = "新文件名";
            UpFileName.Click += UpFileName_Click;
            // 
            // downFileName
            // 
            downFileName.AutoSize = true;
            downFileName.Location = new Point(394, 410);
            downFileName.Name = "downFileName";
            downFileName.Size = new Size(69, 20);
            downFileName.TabIndex = 13;
            downFileName.Text = "存储路径";
            // 
            // txtUpAddress
            // 
            txtUpAddress.Location = new Point(117, 349);
            txtUpAddress.Name = "txtUpAddress";
            txtUpAddress.Size = new Size(271, 27);
            txtUpAddress.TabIndex = 14;
            txtUpAddress.TextChanged += txtUpAddress_TextChanged;
            // 
            // txtUpFileName
            // 
            txtUpFileName.Location = new Point(469, 350);
            txtUpFileName.Name = "txtUpFileName";
            txtUpFileName.Size = new Size(124, 27);
            txtUpFileName.TabIndex = 16;
            txtUpFileName.TextChanged += txtUpFileName_TextChanged;
            // 
            // button_Upload
            // 
            button_Upload.Location = new Point(599, 350);
            button_Upload.Name = "button_Upload";
            button_Upload.Size = new Size(94, 30);
            button_Upload.TabIndex = 18;
            button_Upload.Text = "上传";
            button_Upload.UseVisualStyleBackColor = true;
            button_Upload.Click += button_Upload_Click;
            // 
            // button_Download
            // 
            button_Download.Location = new Point(601, 401);
            button_Download.Name = "button_Download";
            button_Download.Size = new Size(92, 29);
            button_Download.TabIndex = 19;
            button_Download.Text = "下载";
            button_Download.UseVisualStyleBackColor = true;
            button_Download.Click += button_Download_Click;
            // 
            // button_Resume
            // 
            button_Resume.Location = new Point(697, 347);
            button_Resume.Name = "button_Resume";
            button_Resume.Size = new Size(80, 87);
            button_Resume.TabIndex = 20;
            button_Resume.Text = "断点重传";
            button_Resume.UseVisualStyleBackColor = true;
            button_Resume.Click += button_Resume_Click;
            // 
            // Dictionary
            // 
            Dictionary.AutoSize = true;
            Dictionary.Location = new Point(369, 75);
            Dictionary.Name = "Dictionary";
            Dictionary.Size = new Size(84, 20);
            Dictionary.TabIndex = 21;
            Dictionary.Text = "服务器目录";
            // 
            // txtDownPath
            // 
            txtDownPath.Location = new Point(469, 401);
            txtDownPath.Name = "txtDownPath";
            txtDownPath.Size = new Size(125, 27);
            txtDownPath.TabIndex = 22;
            // 
            // txtDownAddress
            // 
            txtDownAddress.Location = new Point(117, 404);
            txtDownAddress.Name = "txtDownAddress";
            txtDownAddress.Size = new Size(271, 27);
            txtDownAddress.TabIndex = 15;
            txtDownAddress.TextChanged += txtDownAddress_TextChanged;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(txtDownPath);
            Controls.Add(Dictionary);
            Controls.Add(button_Resume);
            Controls.Add(button_Download);
            Controls.Add(button_Upload);
            Controls.Add(txtUpFileName);
            Controls.Add(txtDownAddress);
            Controls.Add(txtUpAddress);
            Controls.Add(downFileName);
            Controls.Add(UpFileName);
            Controls.Add(downloadAddress);
            Controls.Add(UploadAddress);
            Controls.Add(list_Dictionary);
            Controls.Add(Dictioanry);
            Controls.Add(txtPassword);
            Controls.Add(txtUserName);
            Controls.Add(txtServer);
            Controls.Add(button_Disconnect);
            Controls.Add(button_Connect);
            Controls.Add(password);
            Controls.Add(userName);
            Controls.Add(server);
            Name = "Form1";
            Text = "FTP_client";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label server;
        private Label userName;
        private Label password;
        private Button button_Connect;
        private Button button_Disconnect;
        private TextBox txtServer;
        private TextBox txtUserName;
        private TextBox txtPassword;
        private Label Dictioanry;
        private ListView list_Dictionary;
        private Label UploadAddress;
        private Label downloadAddress;
        private Label UpFileName;
        private Label downFileName;
        private TextBox txtUpAddress;
        private TextBox txtUpFileName;
        private Button button_Upload;
        private Button button_Download;
        private Button button_Resume;
        private Label Dictionary;
        private TextBox txtDownPath;
        private TextBox txtDownAddress;
    }
}
