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
    public class RoomController : ApiController
    {
        private MyContext db = new MyContext();
        private TokenRepository TokenRepositor = new TokenRepository();
        // GET api/Room/5
        [HttpGet, Route("api/Rooms")]
        [ResponseType(typeof(List<Room>))]
        public async Task<IHttpActionResult> GetRoom(string token, int idUser)
        {
            if (!TokenRepositor.checkToken(idUser, token))
            {
                return NotFound();
            }
            List<Room> room = db.Rooms.Where(x => x.Private != true).ToList();
            if (room == null)
            {
                return NotFound();
            }

            return Ok(room);
        }
        [HttpGet, Route("api/PrivateRooms")]
        [ResponseType(typeof(List<Room>))]
        public async Task<IHttpActionResult> GetPrivateRoom(string token, int idUser)
        {
            if (!TokenRepositor.checkToken(idUser, token))
            {
                return NotFound();
            }
            List<int> memberships = db.RoomMembers.Where(x => x.IdUser == idUser).Select(u => u.IdRoom).ToList();
            if (memberships == null)
            {
                return NotFound();
            }
            List<Room> room = db.Rooms.Where(x => x.Private ==true && memberships.Contains(x.Id)).ToList();
            if (room == null)
            {
                return NotFound();
            }

            return Ok(room);
        }
        [HttpGet, Route("api/MyRooms")]
        [ResponseType(typeof(List<Room>))]
        public async Task<IHttpActionResult> GetMyRoom(string token, int idUser)
        {
            if (!TokenRepositor.checkToken(idUser, token))
            {
                return NotFound();
            }
            List<int> memberships = db.RoomMembers.Where(x => x.IdUser == idUser).Select(u => u.IdRoom).ToList();
            if (memberships == null)
            {
                return NotFound();
            }
            List<Room> room = db.Rooms.Where(x => memberships.Contains(x.Id)).ToList();
            if (room == null)
            {
                return NotFound();
            }

            return Ok(room);
        }

        // PUT api/Room/5
        public async Task<IHttpActionResult> PutRoom(int id, Room room)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != room.Id)
            {
                return BadRequest();
            }

            db.Entry(room).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomExists(id))
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

        // POST api/Room
        public class PostRoomMemberClass
        {
            public bool Private { get; set; }
            public string RoomName { get; set; }
            public int idUser { get; set; }
            public string token { get; set; }
        }

        [ResponseType(typeof(Room))]
        public async Task<IHttpActionResult> PostRoom(PostRoomMemberClass postroom)
        {
            if (!TokenRepositor.checkToken(postroom.idUser, postroom.token))
            {
                return NotFound();
            }
            Room room = new Room();
            room.RoomName = postroom.RoomName;
            room.Private = postroom.Private;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Rooms.Add(room);
            await db.SaveChangesAsync();
            RoomMember roomm = new RoomMember();
            roomm.IdRoom = room.Id;
            roomm.IdUser = postroom.idUser;
            db.RoomMembers.Add(roomm);
            await db.SaveChangesAsync();

            return Ok("Chatroom Created");
        }

        // DELETE api/Room/5
        [ResponseType(typeof(Room))]
        public async Task<IHttpActionResult> DeleteRoom(int id)
        {
            Room room = await db.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            db.Rooms.Remove(room);
            await db.SaveChangesAsync();

            return Ok(room);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RoomExists(int id)
        {
            return db.Rooms.Count(e => e.Id == id) > 0;
        }
    }
}