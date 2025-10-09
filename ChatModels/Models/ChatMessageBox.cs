using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatModels.Models;

public class ChatMessageBox
{
    public ChatUser User { get; set; }
    public string Password { get; set; }
    public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}


