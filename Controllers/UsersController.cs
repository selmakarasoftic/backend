using Microsoft.AspNetCore.Mvc;
using blog_website_api.Data;
using blog_website_api.Models;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace blog_website_api.Controllers
{
    [ApiController]
    [Route("api/Users")]
    public class UsersController : ControllerBase
    {
        private readonly MongoDbContext _context;

        public UsersController(MongoDbContext context)
        {
            _context = context;
        }


        // GET: api/users/all
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users.Find(_ => true).ToListAsync();
            return Ok(users);
        }


        // GET: api/users with pagination
        // to use it in Postman http://localhost:5000/api/users?page=2&pageSize=5
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var usersQuery = _context.Users.Find(_ => true);
            var totalItems = await usersQuery.CountDocumentsAsync();
            var users = await usersQuery.Skip((page - 1) * pageSize).Limit(pageSize).ToListAsync();

            var response = new
            {
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)System.Math.Ceiling(totalItems / (double)pageSize),
                Items = users
            };

            return Ok(response);
        }


        // GET: api/users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _context.Users.Find<User>(u => u.Id == id).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        // POST: api/users
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            await _context.Users.InsertOneAsync(user);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // PUT: api/users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] User updatedUser)
        {
            var result = await _context.Users.ReplaceOneAsync(u => u.Id == id, updatedUser);
            if (result.ModifiedCount == 0)
            {
                return NotFound();
            }
            return NoContent();
        }


        // DELETE: api/users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _context.Users.DeleteOneAsync(u => u.Id == id);
            if (result.DeletedCount == 0)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}