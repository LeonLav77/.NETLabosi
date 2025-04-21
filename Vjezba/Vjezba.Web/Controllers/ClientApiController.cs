// Vjezba.Web/Controllers/Api/ClientApiController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vjezba.Model;
using Vjezba.Model.DTO;
using Vjezba.DAL;
using Vjezba.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Vjezba.Controllers.Api
{
    [Route("api/client")]
    [ApiController]
    public class ClientApiController : Controller
    {
        private readonly ClientManagerDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ClientApiController(ClientManagerDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/client
        [HttpGet]
        [AllowAnonymous]
        public ActionResult<IEnumerable<ClientDTO>> Get()
        {
            var clients = _context.Clients
                .Include(c => c.City)
                .Select(c => new ClientDTO
                {
                    ID = c.ID,
                    FullName = c.FullName,
                    Address = c.Address,
                    Email = c.Email,
                    City = new CityDTO
                    {
                        ID = c.City.ID,
                        Name = c.City.Name
                    }
                })
                .ToList();

            return Ok(clients);
        }

        // GET: api/client/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public ActionResult<ClientDTO> Get(int id)
        {
            var client = _context.Clients
                .Include(c => c.City)
                .Where(c => c.ID == id)
                .Select(c => new ClientDTO
                {
                    ID = c.ID,
                    FullName = c.FullName,
                    Address = c.Address,
                    Email = c.Email,
                    City = new CityDTO
                    {
                        ID = c.City.ID,
                        Name = c.City.Name
                    }
                })
                .FirstOrDefault();

            if (client == null)
            {
                return NotFound();
            }

            return Ok(client);
        }

        // GET: api/client/pretraga/q
        [HttpGet("pretraga/{q}")]
        [AllowAnonymous]
        public ActionResult<IEnumerable<ClientDTO>> Get(string q)
        {
            var clients = _context.Clients
                .Include(c => c.City)
                .Where(c => c.FirstName.Contains(q) || c.LastName.Contains(q))
                .Select(c => new ClientDTO
                {
                    ID = c.ID,
                    FullName = c.FullName,
                    Address = c.Address,
                    Email = c.Email,
                    City = new CityDTO
                    {
                        ID = c.City.ID,
                        Name = c.City.Name
                    }
                })
                .ToList();

            return Ok(clients);
        }

        // POST: api/client
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<Client>> Post([FromBody] Client client)
        {
            client.CreatedById = _userManager.GetUserId(User);
            client.UpdatedById = client.CreatedById;
            
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = client.ID }, client);
        }

        // PUT: api/client/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> PutClient(int id, [FromBody] Client client)
        {
            if (id != client.ID)
                return BadRequest("ID mismatch");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingClient = await _context.Clients.FindAsync(id);

            if (existingClient == null)
                return NotFound();

            // Ručno ažuriranje vrijednosti
            existingClient.FirstName = client.FirstName;
            existingClient.LastName = client.LastName;
            existingClient.Email = client.Email;
            existingClient.Gender = client.Gender;
            existingClient.Address = client.Address;
            existingClient.PhoneNumber = client.PhoneNumber;
            existingClient.CityID = client.CityID;
            existingClient.WorkingExperience = client.WorkingExperience;
            existingClient.DateOfBirth = client.DateOfBirth;
            existingClient.UpdatedById = _userManager.GetUserId(User);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/client/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.ID == id);
        }

        // method with IndexAjax name, that takes in a ClientFilterModel, and return a partial view
        [HttpPost("IndexAjax")]
        [AllowAnonymous]
        public async Task<IActionResult> IndexAjax([FromBody] ClientFilterModel filter)
        {
            // Start with the base query
            IQueryable<Client> query = _context.Clients.Include(c => c.City);

            // Apply filters after loading the data into memory to avoid translation issues
            // First retrieve all the data
            List<Client> clients = await query.ToListAsync();
            
            // Then filter in memory
            if (!string.IsNullOrWhiteSpace(filter.FullName))
            {
                clients = clients.Where(c => c.FullName != null && 
                    c.FullName.Contains(filter.FullName, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(filter.Address))
            {
                clients = clients.Where(c => c.Address != null && 
                    c.Address.Contains(filter.Address, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(filter.Email))
            {
                clients = clients.Where(c => c.Email != null && 
                    c.Email.Contains(filter.Email, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(filter.City))
            {
                clients = clients.Where(c => c.City != null && c.City.Name != null && 
                    c.City.Name.Contains(filter.City, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return PartialView("~/Views/Client/_IndexTable.cshtml", clients);
        }
    }
}