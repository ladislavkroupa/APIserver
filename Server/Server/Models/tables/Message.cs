using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.Models.tables
{
    public class Message
    {
        public int Id { get; set; }
        public int IdSender { get; set; }
        public int IdRoom { get; set; }
        public string MessageText { get; set; }
        public DateTime Time { get; set; }
    }
}