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

namespace Server.Controllers
{
    public class UserController : ApiController
    {
        private MyContext db = new MyContext();
        private TokenRepository TokenRepositor = new TokenRepository();

        // GET api/User
        public IQueryable<User> GetUsers()
        {
            return db.Users;
        }

        // GET api/User/5
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> GetUser(int id, string token)
        {
            if (!TokenRepositor.checkToken(id, token))
            {
                return NotFound();
            }
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            user.Password = null;
            return Ok(user);
        }

        // PUT api/User/5
        public async Task<IHttpActionResult> PutUser(int id, User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.Id)
            {
                return BadRequest();
            }

            db.Entry(user).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        public class PostUserClass
        {
            public string UserName { get; set; }
            public string Password { get; set; }
        }
        // POST api/User
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> PostUser(PostUserClass postuser)
        {
            User user = new User();
            user.UserName = postuser.UserName;
            user.Password = postuser.Password;
            if (db.Users.Where(x => x.UserName == postuser.UserName).FirstOrDefault()!= null)
            {
                return BadRequest("Account already exists.");
            }
            if (postuser.UserName == null || postuser.UserName == "" || postuser.Password == null || postuser.Password == "")
            {
                return BadRequest("Please fill all inputs."); 
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Users.Add(user);
            await db.SaveChangesAsync();

            return Ok("User registered!");
        }

        // DELETE api/User/5
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> DeleteUser(int id)
        {
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            db.Users.Remove(user);
            await db.SaveChangesAsync();

            return Ok(user);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserExists(int id)
        {
            return db.Users.Count(e => e.Id == id) > 0;
        }
    }
}