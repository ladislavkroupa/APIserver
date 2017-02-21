using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.Models.tables
{
    public class RoomMember
    {
        public int Id { get; set; }
        public int IdUser { get; set; }
        public int IdRoom { get; set; }
    }
}