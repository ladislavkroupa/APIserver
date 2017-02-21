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
    public class RoomMemberController : ApiController
    {
        private MyContext db = new MyContext();
        private TokenRepository TokenRepositor = new TokenRepository();
        // GET api/RoomMember
        public IQueryable<RoomMember> GetRoomMembers()
        {
            return db.RoomMembers;
        }

        // GET api/RoomMember/5
        [ResponseType(typeof(RoomMember))]
        public async Task<IHttpActionResult> GetRoomMember(int id)
        {
            RoomMember roommember = await db.RoomMembers.FindAsync(id);
            if (roommember == null)
            {
                return NotFound();
            }

            return Ok(roommember);
        }

        // PUT api/RoomMember/5
        public async Task<IHttpActionResult> PutRoomMember(int id, RoomMember roommember)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != roommember.Id)
            {
                return BadRequest();
            }

            db.Entry(roommember).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomMemberExists(id))
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

        public class PostRoomMemberClass
        {
            public int IdRoom { get; set; }
            public string UserName { get; set; }
            public int idUser { get; set; }
            public string token { get; set; }
        }


        // POST api/RoomMember
        [ResponseType(typeof(RoomMember))]
        public async Task<IHttpActionResult> PostRoomMember(PostRoomMemberClass POSTroommember)
        {
            if (!TokenRepositor.checkToken(POSTroommember.idUser, POSTroommember.token))
            {
                return NotFound();
            }
            User u = db.Users.Where(x => x.UserName == POSTroommember.UserName).FirstOrDefault();
            if (u == null)
            {
                return NotFound();
            }
            if (db.Rooms.Where(x => x.Id == POSTroommember.IdRoom && x.Private == true).FirstOrDefault() != null && db.RoomMembers.Where(x => x.IdRoom == POSTroommember.IdRoom && x.IdUser == POSTroommember.idUser).FirstOrDefault() == null)
            {
                return NotFound();
            }
            if (db.RoomMembers.Where(x => x.IdRoom == POSTroommember.IdRoom && x.IdUser == u.Id).FirstOrDefault() !=null)
            {
                return BadRequest("User already in chatroom");
            }


            RoomMember roommember = new RoomMember();
            roommember.IdRoom = POSTroommember.IdRoom;
            roommember.IdUser = u.Id;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.RoomMembers.Add(roommember);
            await db.SaveChangesAsync();

            return Ok("User Added");
        }
        public class DeleteRoomMemberClass
        {
            public int IdRoom { get; set; }
            public int idUser { get; set; }
            public string token { get; set; }
        }
        // DELETE api/RoomMember/5
        [HttpDelete, Route("api/leaveRoom")]
        [ResponseType(typeof(RoomMember))]
        public async Task<IHttpActionResult> DeleteRoomMember(DeleteRoomMemberClass deleteRoomMember)
        {
            if (!TokenRepositor.checkToken(deleteRoomMember.idUser, deleteRoomMember.token))
            {
                return NotFound();
            }
            RoomMember roommember = db.RoomMembers.Where(x => x.IdUser == deleteRoomMember.idUser && x.IdRoom == deleteRoomMember.IdRoom).FirstOrDefault();
            if (roommember == null)
            {
                return NotFound();
            }

            db.RoomMembers.Remove(roommember);
            await db.SaveChangesAsync();
            string delete = "";
            if (db.RoomMembers.Where(x => x.IdRoom == deleteRoomMember.IdRoom).Count() == 0)
            {
                Room r = await db.Rooms.FindAsync(deleteRoomMember.IdRoom);
                db.Rooms.Remove(r);
                delete = ",room deleted";
            }

            await db.SaveChangesAsync();

            return Ok("User left Room" + delete);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RoomMemberExists(int id)
        {
            return db.RoomMembers.Count(e => e.Id == id) > 0;
        }
    }
}