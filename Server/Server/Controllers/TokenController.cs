using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Server.Models.tables;
using Server.Models;
using System.Text;

namespace Server.Controllers
{
    public class TokenController : ApiController
    {
        private MyContext db = new MyContext();
        private TokenRepository TokenRepositor = new TokenRepository();
        // GET api/Token
        public IQueryable<Token> GetTokens()
        {
            return db.Tokens;
        }

        // GET api/Token/5
        [ResponseType(typeof(Token))]
        public async Task<IHttpActionResult> GetToken(string UserName,string Password)
        {
            User u = db.Users.Where(x => x.UserName == UserName).FirstOrDefault();
            if (u == null)
            {
                return BadRequest("Bad UserName");
            }
            if (u.Password != Password)
            {
                return BadRequest("Bad Password");
            }
            Token token = new Token();
            token.IdUser = u.Id;

            List<char> date = DateTime.Now.Ticks.ToString().ToList<char>();
            char[] chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            Random r = new Random();
            StringBuilder MyTokenBuilder = new StringBuilder("");
            while(date.Count > 0)
            {
                int i = r.Next(date.Count);
                MyTokenBuilder.Append(date[i]);
                date.RemoveAt(i);
                MyTokenBuilder.Append(chars[r.Next(chars.Count())]);
                if (date.Count % 2 == 1)
                {
                    MyTokenBuilder.Append(chars[r.Next(chars.Count())]);
                }
            }


            token.TokenHash = MyTokenBuilder.ToString();
            db.Tokens.Add(token);

            db.SaveChanges();

            return Ok(token);
        }

        // PUT api/Token/5
        public async Task<IHttpActionResult> PutToken(int id, Token token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != token.Id)
            {
                return BadRequest();
            }

            db.Entry(token).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TokenExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST api/Token
        [ResponseType(typeof(Token))]
        public async Task<IHttpActionResult> PostToken(Token token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Tokens.Add(token);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = token.Id }, token);
        }
        
        // DELETE api/Token/5
        public class DeleteTokenClass
        {
            public int idUser { get; set; }
            public string token { get; set; }
        }
        [HttpDelete, Route("api/logout")]
        [ResponseType(typeof(Token))]
        public async Task<IHttpActionResult> DeleteToken(DeleteTokenClass delToken)
        {
            if (!TokenRepositor.checkToken(delToken.idUser, delToken.token))
            {
                return NotFound();
            }
            Token token = await db.Tokens.Where(x => x.TokenHash == delToken.token).FirstOrDefaultAsync();
            if (token == null)
            {
                return NotFound();
            }

            db.Tokens.Remove(token);
            await db.SaveChangesAsync();

            return Ok("logged out, token deleted");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TokenExists(int id)
        {
            return db.Tokens.Count(e => e.Id == id) > 0;
        }
    }
}