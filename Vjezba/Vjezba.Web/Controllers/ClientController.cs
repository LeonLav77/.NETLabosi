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
    public class ClientController : Controller
    {
        private readonly ClientManagerDbContext _context;

        public ClientController(ClientManagerDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Load the base query with included related data
            var clientQuery = _context.Clients.Include(c => c.City);

            // Then switch to client-side evaluation before using FullName
            var clientsInMemory = clientQuery.AsEnumerable();
            
            // Filter by FullName in memory if needed
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

        public IActionResult Create()
        {
            // Fill dropdown values
            FillDropDownValues();
            return View(new Client());
        }

        [HttpPost]
        public IActionResult Create(Client client)
        {
            try 
            {
                 if(!ModelState.IsValid){
                    FillDropDownValues();
                    return View(client);
                 }
                // Add client to database context
                _context.Clients.Add(client);
                
                // Save changes to the database
                _context.SaveChanges();
                
                // Redirect to index
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log the actual exception
                Console.WriteLine($"Exception during create: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                
                // Add error message
                ModelState.AddModelError("", "Error saving client: " + ex.InnerException?.Message ?? ex.Message);
            }
            
            // If we get here, something went wrong
            FillDropDownValues();
            return View(client);
        }

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
        public ActionResult EditPost(int id)
        {
            Client client = _context.Clients.Find(id);

            TryUpdateModelAsync(client);
            
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // Add this method to your ClientController class
        private void FillDropDownValues()
        {
            // Create a list with a "select" empty option first
            var cities = _context.Cities.ToList();
            ViewBag.Cities = cities;
        }



          // POST: Client/UploadAttachment
        [HttpPost]
        public async Task<IActionResult> UploadAttachment(int clientId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty");

            var client = await _context.Clients.FindAsync(clientId);
            if (client == null)
                return NotFound($"Client with ID {clientId} not found");

            try
            {
                // Create directory if it doesn't exist
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "clients", clientId.ToString());
                Directory.CreateDirectory(uploadsFolder);

                // Create unique filename
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save file to disk
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // Save file info to database
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
        public async Task<IActionResult> DeleteAttachment(int id)
        {
            try
            {
                var attachment = await _context.Attachments.FindAsync(id);
                if (attachment == null)
                    return NotFound();

                // Delete file from disk
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
    }
}