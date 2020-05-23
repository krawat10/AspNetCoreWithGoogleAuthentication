using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab3.Models;
using Microsoft.AspNetCore.Authorization;

namespace Lab3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET: https://localhost:5001/api/Books
        /// </summary>
        /// <returns> 200 - [{"id":1,"title":"Harry","author":"JK Rowling","isRented":false}]</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            return await _context.Books.ToListAsync();
        }

        /// <summary>
        /// GET: https://localhost:5001/api/Books/1
        /// </summary>
        /// <returns> 200 - [{"id":1,"title":"Harry","author":"JK Rowling","isRented":false}]</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        /// <summary>
        /// GET: https://localhost:5001/api/Books/1/Users
        /// </summary>
        /// <param name="id"></param>
        /// <returns> 200 - [{"id":1,"name":"Mateusz","surname":"Krawczyk","eMail":"krawat10@gmail.com"}]</returns>
        [HttpGet("{id}/Users")]
        public async Task<ActionResult<IEnumerable<User>>> GetBookUsers(int id)
        {
            var book = await _context.Books
                .Include(b => b.UserBooks)
                .ThenInclude(b => b.User)
                .SingleOrDefaultAsync(b => b.ID == id);

            if (book == null)
            {
                return NotFound();
            }

            var users = book
                .UserBooks
                .Select(userBook => userBook.User)
                .Distinct();

            if (!users.Any())
            {
                return NotFound();
            }

            return Ok(users);
        }

        /// <summary>
        /// PUT: https://localhost:5001/api/Books/1
        /// {
        /// "Title": "Harry2",
        /// "Author": "JK Rowling"
        /// }
        /// </summary>
        /// <returns>204</returns>
    [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            book.ID = id;
            if (id != book.ID)
            {
                return BadRequest();
            }

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
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
        /// POST: https://localhost:5001/api/Books/
        /// {"title":"Harry","author":"JK Rowling"}
        /// </summary>
        /// <returns> 201 - {"id":1,"title":"Harry","author":"JK Rowling","isRented":false}</returns>
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBook", new { id = book.ID }, book);
        }

        /// <summary>
        /// DELETE: https://localhost:5001/api/Books/5
        /// </summary>
        /// <returns>
        /// 200 
        ///{
        /// "id": 1,
        /// "title": "Harry2",
        /// "author": "JK Rowling",
        /// "isRented": false
        /// }
        /// </returns>
    [HttpDelete("{id}")]
        public async Task<ActionResult<Book>> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return book;
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.ID == id);
        }
    }
}
