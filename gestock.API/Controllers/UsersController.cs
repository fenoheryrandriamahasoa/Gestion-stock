using gestock.API.Data;
using gestock.API.DTOs;
using gestock.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace gestock.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        
        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();

            var usersDto = users.Select(u => new UserDto
            {
                UserId = u.UserId,
                Username = u.Username,
                Role = u.Role,
                IsActive = u.IsActive
            }).ToList();

            return Ok(usersDto);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]                                    
        public async Task<ActionResult<UserDto>> GetUser(int id)  
        {
            var user = await _context.Users.FindAsync(id);   

            if (user == null)
            {
                return NotFound();
            }

            var userDto = new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Role = user.Role,
                IsActive = user.IsActive
            };

            return Ok(userDto);
        }

        
        // POST: api/Users/login
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user == null)
            {
                return Unauthorized(new { message = "Identifiant incorrect" });
            }

            if (!user.IsActive)
            {
                return Unauthorized(new { message = "Compte désactivé" });
            }

            // Vérifier le mot de passe haché
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Mot de passe incorrect" });
            }

            var userDto = new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Role = user.Role,
                IsActive = user.IsActive
            };

            return Ok(userDto);
        }

        
        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<UserDto>> PostUser(User user)
        {
            // Vérifier si le username existe déjà
            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
            {
                return Conflict(new { message = "Ce nom d'utilisateur existe déjà" });
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var userDto = new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Role = user.Role,
                IsActive = user.IsActive
            };

            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, userDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.UserId)
                return BadRequest();

            var existingUser = await _context.Users.AsNoTracking()
                                                .FirstOrDefaultAsync(u => u.UserId == id);
            if (existingUser == null)
                return NotFound();

            // ✅ Si mot de passe vide → garder l'ancien hash
            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                user.PasswordHash = existingUser.PasswordHash;
            }
            // ✅ Si mot de passe changé → re-hasher
            else if (user.PasswordHash != existingUser.PasswordHash)
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Users.Any(e => e.UserId == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
       
    }
}

