using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.Models.tables
{
    public class Token
    {
        public int Id { get; set; }
        public int IdUser { get; set; }
        public string TokenHash { get; set; }
    }
}