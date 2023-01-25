using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NextMindBE.Data;
using NextMindBE.DTOs;
using NextMindBE.Model;

namespace NextMindBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/Pings
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Ping>> PostPing(PingDto ping)
        {
            var user = _context.User.FirstOrDefault(o => o.SessionId == ping.SessionId);
            if(user == null)
            {
                return BadRequest(); // TODO: Remove from the list, and lock out the app.
            }

            if (!PingTimerManager._authenticatedUsers.ContainsKey(user.SessionId))
            {
                // TODO: Notify lockout server to lock out the app.
                Console.WriteLine("Recived an inactive ping. Alarm ON!");
                return BadRequest();
            }

            var newPing = new Ping()
            {
                Name = user.Username,
                Position = ping.Position,
                Status = ping.Status,
                SessionId = user.SessionId,
                Timestamp = DateTime.UtcNow,
            };

            user.LastActive = DateTime.UtcNow;
            PingTimerManager._authenticatedUsers[user.SessionId] = user;
            _context.Ping.Add(newPing);
            _context.User.Update(user);
            await _context.SaveChangesAsync();

            return Ok();
        }

    }
}
