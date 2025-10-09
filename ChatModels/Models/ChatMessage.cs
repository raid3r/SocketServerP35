using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatModels.Models;

public class ChatMessage
{
    public ChatUser From { get; set; }
    public DateTime Timestamp { get; set; }
    public string Text { get; set; }    
}
