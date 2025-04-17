using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vjezba.Model;
using Vjezba.Model.DTO;
using Vjezba.DAL;

namespace Vjezba.Controllers.Api
{
    [Route("api/client")]
    [ApiController]
    public class ClientApiController : ControllerBase
    {
        private readonly ClientManagerDbContext _context;

        public ClientApiController(ClientManagerDbContext context)
        {
            _context = context;
        }

        // GET: api/client
        [HttpGet]
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
        public async Task<ActionResult<Client>> Post([FromBody] Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = client.ID }, client);
        }



// PUT: api/client/5
[HttpPut("{id}")]
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


        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.ID == id);
        }
    }
}