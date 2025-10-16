namespace SocketChatGuiClient
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
            tbxMessage = new TextBox();
            btnSend = new Button();
            btnAddFile = new Button();
            lblAttachedFileName = new Label();
            selectFileToSendDialog = new OpenFileDialog();
            listMessages = new ListBox();
            SuspendLayout();
            // 
            // tbxMessage
            // 
            tbxMessage.Location = new Point(12, 399);
            tbxMessage.Multiline = true;
            tbxMessage.Name = "tbxMessage";
            tbxMessage.Size = new Size(629, 39);
            tbxMessage.TabIndex = 0;
            // 
            // btnSend
            // 
            btnSend.Location = new Point(647, 400);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(76, 38);
            btnSend.TabIndex = 1;
            btnSend.Text = "Відправити";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            // 
            // btnAddFile
            // 
            btnAddFile.Location = new Point(729, 400);
            btnAddFile.Name = "btnAddFile";
            btnAddFile.Size = new Size(59, 38);
            btnAddFile.TabIndex = 2;
            btnAddFile.Text = "Файл";
            btnAddFile.UseVisualStyleBackColor = true;
            btnAddFile.Click += btnAddFile_Click;
            // 
            // lblAttachedFileName
            // 
            lblAttachedFileName.AutoSize = true;
            lblAttachedFileName.Location = new Point(12, 381);
            lblAttachedFileName.Name = "lblAttachedFileName";
            lblAttachedFileName.Size = new Size(198, 15);
            lblAttachedFileName.TabIndex = 3;
            lblAttachedFileName.Text = "Назва файлу до відкправки якщо є";
            // 
            // selectFileToSendDialog
            // 
            selectFileToSendDialog.FileName = "openFileDialog1";
            // 
            // listMessages
            // 
            listMessages.FormattingEnabled = true;
            listMessages.ItemHeight = 15;
            listMessages.Items.AddRange(new object[] { "16.10.2025 19:00 [user1] Привіт", "16.10.2025 19:00 [user1] Привіт", "16.10.2025 19:00 [user1] Привіт", "16.10.2025 19:00 [user1] Привіт", "16.10.2025 19:00 [user1] Привіт", "16.10.2025 19:00 [user1] Привіт", "16.10.2025 19:00 [user1] Привіт" });
            listMessages.Location = new Point(15, 18);
            listMessages.Name = "listMessages";
            listMessages.Size = new Size(773, 349);
            listMessages.TabIndex = 4;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(listMessages);
            Controls.Add(lblAttachedFileName);
            Controls.Add(btnAddFile);
            Controls.Add(btnSend);
            Controls.Add(tbxMessage);
            Name = "Form1";
            Text = "Чат";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox tbxMessage;
        private Button btnSend;
        private Button btnAddFile;
        private Label lblAttachedFileName;
        private OpenFileDialog selectFileToSendDialog;
        private ListBox listMessages;
    }
}
