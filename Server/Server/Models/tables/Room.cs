using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.Models.tables
{
    public class Room
    {
        public int Id { get; set; }
        public string RoomName { get; set; }
        public bool Private { get; set; }
    }
}