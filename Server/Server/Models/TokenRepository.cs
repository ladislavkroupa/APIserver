using Server.Models.tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.Models
{
    public class TokenRepository
    {
        private MyContext _context = new MyContext();
        public bool checkToken(int IdUser, string TokenHash)
        {
            bool check = true;
            Token t = _context.Tokens.Where(x => x.TokenHash == TokenHash).FirstOrDefault();
            if (t == null)
            {
                check = false;
            }
            else if (t.IdUser != IdUser)
            {
                check = false;
            }

            return check;
        }
    }
}