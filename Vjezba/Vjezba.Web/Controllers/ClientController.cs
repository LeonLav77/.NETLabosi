// Vjezba.Web/Controllers/ClientController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Vjezba.DAL;
using Vjezba.Model;
using Vjezba.Web.Models;
using Vjezba.Model.Helpers;

namespace Vjezba.Web.Controllers
{
    public class ClientController : BaseController
    {
        private readonly ClientManagerDbContext _context;

        public ClientController(ClientManagerDbContext context, UserManager<AppUser> userManager)
            : base(userManager)
        {
            _context = context;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            var clientQuery = _context.Clients.Include(c => c.City);

            var clientsInMemory = clientQuery.AsEnumerable();
            
            ViewBag.ActiveTab = 1;
            
            return View(clientsInMemory.OrderBy(c => c.ID).ToList());
        }

        public IActionResult Details(int? id = null)
        {
            if (id == null)
                return NotFound();
                
            var model = _context.Clients
                .Include(c => c.City)
                .FirstOrDefault(c => c.ID == id.Value);
                
            if (model == null)
                return NotFound();
                
            return View(model);
        }

        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Create()
        {
            FillDropDownValues();

            return View(new Client());
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Create(Client client)
        {
            try 
            {
                if(!ModelState.IsValid){
                    FillDropDownValues();
                    return View(client);
                }
                
                client.CreatedById = UserId;
                client.UpdatedById = UserId;
                
                _context.Clients.Add(client);
                
                _context.SaveChanges();
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during create: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                
                ModelState.AddModelError("", "Error saving client: " + ex.InnerException?.Message ?? ex.Message);
            }
            
            FillDropDownValues();
            return View(client);
        }

        [Authorize(Roles = "Admin,Manager")]
        [ActionName("Edit")]
        public IActionResult EditGet(int id)
        {
            var client = _context.Clients.FirstOrDefault(c => c.ID == id);
            
            if (client == null)
                return NotFound();
                
            FillDropDownValues();            
            return View(client);
        }

        [HttpPost]
        [ActionName("Edit")]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult EditPost(int id)
        {
            Client client = _context.Clients.Find(id);
            
            if (client == null)
                return NotFound();

            TryUpdateModelAsync(client);
            
            client.UpdatedById = UserId;
            
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var client = _context.Clients.Find(id);
            if (client == null)
                return NotFound();

            _context.Clients.Remove(client);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        private void FillDropDownValues()
        {
            var cities = _context.Cities.ToList();
            ViewBag.Cities = cities;
        }

        // POST: Client/UploadAttachment
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadAttachment(int clientId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty");

            var client = await _context.Clients.FindAsync(clientId);
            if (client == null)
                return NotFound($"Client with ID {clientId} not found");

            try
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "clients", clientId.ToString());
                Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                var attachment = new Attachment
                {
                    FileName = file.FileName,
                    FilePath = Path.Combine("uploads", "clients", clientId.ToString(), uniqueFileName),
                    UploadDate = DateTime.Now,
                    ClientID = clientId
                };

                _context.Attachments.Add(attachment);
                await _context.SaveChangesAsync();

                return Ok(new { fileName = file.FileName, id = attachment.ID });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: Client/GetAttachments
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAttachments(int clientId)
        {
            try
            {
                var attachments = await _context.Attachments
                    .Where(a => a.ClientID == clientId)
                    .OrderByDescending(a => a.UploadDate)
                    .ToListAsync();

                return PartialView("_AttachmentList", attachments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: Client/DeleteAttachment
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteAttachment(int id)
        {
            try
            {
                var attachment = await _context.Attachments.FindAsync(id);
                if (attachment == null)
                    return NotFound();

                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", attachment.FilePath);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                // Remove from database
                _context.Attachments.Remove(attachment);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> IndexAjax([FromBody] ClientFilterModel filter)
        {
            IQueryable<Client> query = _context.Clients.Include(c => c.City);

            List<Client> clients = await query.ToListAsync();
            
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