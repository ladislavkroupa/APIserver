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
using Newtonsoft.Json;

namespace Server.Controllers
{
    public class MessageController : ApiController
    {
        private MyContext db = new MyContext();
        private TokenRepository TokenRepositor = new TokenRepository();
        // GET api/Message
        public IQueryable<Message> GetMessages()
        {
            return db.Messages;
        }

        // GET api/Message/5
        [HttpGet, Route("api/Messages")]
        public async Task<IHttpActionResult> GetMessage(int id, int idUser,string token)
        {
            if (!TokenRepositor.checkToken(idUser, token))
            {
                return NotFound();
            }
            if (db.Rooms.Where(x => x.Id == id && x.Private == true).FirstOrDefault() != null && db.RoomMembers.Where(x => x.IdRoom == id && x.IdUser == idUser).FirstOrDefault() == null)
            {
                return NotFound();
            }

            List<Message> message = db.Messages.Where(x => x.IdRoom ==id).ToList();
            var messages = db.Messages.Join(db.Users, x => x.IdSender, y => y.Id, (x, y) => new { x, y }).Where(xy => xy.x.IdRoom == id).OrderBy(xy => xy.x.Id).Select(xy => new {xy.x.MessageText , xy.x.Time , xy.y.UserName}).ToList();
            /*if (message == null)
            {
                return NotFound();
            }*/

            return Ok(messages);
        }

        // PUT api/Message/5
        public async Task<IHttpActionResult> PutMessage(int id, Message message)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != message.Id)
            {
                return BadRequest();
            }

            db.Entry(message).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MessageExists(id))
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

        public class PostMessageClass
        {
            public string MessagePost { get; set; }
            public int IdRoom { get; set; }
            public int idUser { get; set; }
            public string token { get; set; }
        }

        // POST api/Message
        [ResponseType(typeof(Message))]
        public async Task<IHttpActionResult> PostMessage(PostMessageClass messagepost)
        {
            if (!TokenRepositor.checkToken(messagepost.idUser, messagepost.token))
            {
                return NotFound();
            }
            Message m = new Message();
            m.MessageText = messagepost.MessagePost;
            m.IdSender = messagepost.idUser;
            m.IdRoom = messagepost.IdRoom;
            m.Time = DateTime.Now;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Messages.Add(m);
            await db.SaveChangesAsync();

            return Ok("Message send.");
        }

        // DELETE api/Message/5
        [ResponseType(typeof(Message))]
        public async Task<IHttpActionResult> DeleteMessage(int id)
        {
            Message message = await db.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            db.Messages.Remove(message);
            await db.SaveChangesAsync();

            return Ok(message);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MessageExists(int id)
        {
            return db.Messages.Count(e => e.Id == id) > 0;
        }
    }
}