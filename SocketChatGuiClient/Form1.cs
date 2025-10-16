using ChatModels;
using ChatModels.Models;

namespace SocketChatGuiClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private List<ChatMessage> Messages = [];

        private ChatClient ChatClient;

        private void UpdateMessagesList()
        {
            listMessages.Items.Clear();
            lock (this.Messages)
            {
                foreach (ChatMessage message in Messages)
                {
                    listMessages.Items.Add($"{message.Timestamp.ToString("dd.MM.yyyy HH:mm:ss")} {message.From.Username}: {message.Text}");
                }
            }
            listMessages.Refresh();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            lblAttachedFileName.Text = "";

            bool isLoggedIn = false;

            while (!isLoggedIn)
            {
                var loginForm = new LoginForm();
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    // Заповнюємо дані для підключення
                    ChatClient = new ChatClient("192.168.1.141", 5000);
                    ChatClient.UserName = loginForm.Username;
                    ChatClient.Password = loginForm.Password;

                    try
                    {
                        this.Messages = ChatClient.GetMessages();
                        UpdateMessagesList();
                        isLoggedIn = true;
                        this.Text = $"Chat - {loginForm.Username}";

                        Task.Run(async () =>
                        {
                            while (true)
                            {
                                lock (this.Messages)
                                {
                                    this.Messages.AddRange(ChatClient.GetMessages());
                                }

                                this.Invoke(() =>
                                {
                                    this.UpdateMessagesList();
                                });
                                await Task.Delay(2000);
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error", "Invalid password", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }


                }
                else
                {
                    // Якщо користувач скасував вхід, закриваємо головну форму
                    this.Close();
                    return;
                }
            }



        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbxMessage.Text))
            {
                return;
            }

            tbxMessage.ReadOnly = true;
            btnSend.Enabled = false;

            var message = new ChatMessage
            {
                From = new ChatUser { Username = ChatClient.UserName },
                Text = tbxMessage.Text,
                Timestamp = DateTime.Now
            };

            if (!string.IsNullOrEmpty(fullFilePath))
            {
                ChatClient.SendFile(fullFilePath);
                message.Filename = System.IO.Path.GetFileName(fullFilePath);
                
                fullFilePath = string.Empty;
                lblAttachedFileName.Text = string.Empty;
                btnAddFile.Text = "Файл";
                btnAddFile.ForeColor = System.Drawing.Color.Black;
            }


            ChatClient.SendMessage(message);
            lock (this.Messages)
            {
                this.Messages.Add(message);
            }

            tbxMessage.Clear();
            tbxMessage.ReadOnly = false;
            btnSend.Enabled = true;
        }

        private string fullFilePath = string.Empty;

        private void btnAddFile_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(lblAttachedFileName.Text))
            {
                var result = selectFileToSendDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    fullFilePath = selectFileToSendDialog.FileName;
                    lblAttachedFileName.Text = System.IO.Path.GetFileName(fullFilePath);

                    btnAddFile.Text = "Х";
                    btnAddFile.ForeColor = System.Drawing.Color.Red;
                }
            } else
            {
                fullFilePath = string.Empty;
                lblAttachedFileName.Text = string.Empty;
                btnAddFile.Text = "Файл";
                btnAddFile.ForeColor = System.Drawing.Color.Black;
            }
            
        }
    }
}
