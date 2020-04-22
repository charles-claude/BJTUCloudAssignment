using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService;
using UserService.Models;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserContext _context;

        public UsersController(UserContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserItem>>> Getusers()
        {
            return await _context.users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserItem>> GetUserItem(long id)
        {
            var userItem = await _context.users.FindAsync(id);

            if (userItem == null)
            {
                return NotFound();
            }

            return userItem;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserItem(long id, UserItem userItem)
        {
            if (id != userItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(userItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<UserItem>> PostUserItem(UserItem userItem)
        {
            _context.users.Add(userItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetuserItem), new { id = userItem.Id }, userItem);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<UserItem>> DeleteUserItem(long id)
        {
            var userItem = await _context.users.FindAsync(id);
            if (userItem == null)
            {
                return NotFound();
            }

            _context.users.Remove(userItem);
            await _context.SaveChangesAsync();

            return userItem;
        }

        private bool UserItemExists(long id)
        {
            return _context.users.Any(e => e.Id == id);
        }
    }
}
