using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lab3.Models;
using Lab3.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RentalController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RentalController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/Rental
        /// <summary>
        /// POST https://localhost:5001/api/Rental
        /// {
        /// "UserID": 1,
        /// "BookId": 1
        /// }
        /// </summary>
        /// <returns>
        /// 201
        /// {
        /// "id": 1,
        /// "userID": 1,
        /// "user":{
        ///     "id": 1,
        ///     "name": "Mateusz",
        ///     "surname": "Krawczyk",
        ///     "eMail": "krawat10@gmail.com"
        /// },
        /// "bookID": 1,
        /// "book":{
        ///     "id": 1,
        ///     "title": "Harry",
        ///     "author": "JK Rowling",
        ///     "isRented": true
        /// },
        /// "isCurrentlyRented": true
        /// }
        /// </returns>
        [HttpPost]
        public async Task<ActionResult> Post(RentBookRequest request)
        {
            var user = await _context.Users.FindAsync(request.UserID);

            if (user == null)
            {
                return NotFound($"User with id: {request.UserID} does not exists");
            }

            var book = await _context.Books.FindAsync(request.BookId);

            if (book == null)
            {
                return NotFound($"Book with id: {request.BookId} does not exists");
            }

            var isRented = await _context.UserBooks
                .AnyAsync(userBook =>
                    userBook.BookID == request.BookId &&
                    userBook.IsCurrentlyRented);

            if (isRented)
            {
                return Conflict($"Book with id: {request.BookId} is already rented");
            }

            book.IsRented = true;
            var entity = new UserBook
            {
                Book = book,
                User = user,
                IsCurrentlyRented = true
            };

            _context.UserBooks.Add(entity);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserBooks", "Users", new {id = user.ID}, entity);
        }
    }
}