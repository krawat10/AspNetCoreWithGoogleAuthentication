using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab3;
using Lab3.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace Lab3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        /// <summary>
        /// GET https://localhost:5001/api/Users
        /// </summary>
        /// <returns>
        /// 200
        ///[
        /// {
        /// "id": 1,
        /// "name": "Mateusz",
        /// "surname": "Krawczyk",
        /// "eMail": "krawat10@gmail.com"
        /// }
        /// ]
        /// </returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        /// <summary>
        /// GET: api/Users/5
        /// </summary>
        /// <returns>
        /// 200
        /// {
        /// "id": 1,
        /// "name": "Mateusz",
        /// "surname": "Krawczyk",
        /// "eMail": "krawat10@gmail.com"
        /// }
    /// </returns>
    [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        /// <summary>
        /// GET: https://localhost:5001/api/Users/1/Books
        /// </summary>
        /// <returns>
        /// 200
        ///[
        /// {
        /// "id": 1,
        /// "title": "Harry",
        /// "author": "JK Rowling",
        /// "isRented": true
        /// }
        /// ]
        /// </returns>
        [HttpGet("{id}/Books")]
        public async Task<ActionResult<IEnumerable<Book>>> GetUserBooks(int id)
        {
            var user = await _context.Users
                .Include(usr => usr.UserBooks)
                .ThenInclude(books => books.Book)
                .SingleOrDefaultAsync(usr => usr.ID == id);

            if (user == null)
            {
                return NotFound();
            }

            var userBooks = user.UserBooks
                .Where(userBook => userBook.IsCurrentlyRented)
                .Select(userBook => userBook.Book);

            if (!userBooks.Any())
            {
                return NotFound();
            }

            return Ok(userBooks);
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        /// <summary>
        /// PUT: https://localhost:5001/api/Users/1
        /// </summary>
        /// <param name="user">
        ///{
        /// "Name": "Mateusz",
        /// "Surname": "Krawczyk2",
        /// "EMail": "aksoasko@o2.pl"
        /// }
    /// </param>
    /// <returns>204</returns>
    [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            user.ID = id;

            if (id != user.ID)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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

            return NoContent();
        }

        /// <summary>
        /// POST: https://localhost:5001/api/Users
        /// </summary>
        /// <param name="user">
        ///{
        /// "Name": "Mateusz"  
        /// }
        /// </param>
        /// <returns>
        ///{
        /// "id": 2,
        /// "name": "Mateusz",
        /// "surname": "Krawczyk", // from google
        /// "eMail": "krawat10@gmail.com" // from google
        /// }
        /// </returns>
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Name))
            {
                user.Name = HttpContext.User.FindFirst(ClaimTypes.GivenName).Value;
            }

            if (string.IsNullOrWhiteSpace(user.Surname))
            {
                user.Surname = HttpContext.User.FindFirst(ClaimTypes.Surname).Value;
            }

            if (string.IsNullOrWhiteSpace(user.EMail))
            {
                user.EMail = HttpContext.User.FindFirst(ClaimTypes.Email).Value;
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.ID }, user);
        }

        /// <summary>
        /// DELETE: https://localhost:5001/api/Users/1
        /// </summary>
        /// <param name="id">1</param>
        /// <returns>
        ///200
        /// {
        /// "id": 1,
        /// "name": "Mateusz",
        /// "surname": "Krawczyk2",
        /// "eMail": "aksoasko@o2.pl"
        /// }
    /// </returns>
    [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.ID == id);
        }
    }
}
