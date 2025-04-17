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

        public IActionResult Index(string query = null)
        {
            // Load the base query with included related data
            var clientQuery = _context.Clients.Include(c => c.City);

            // Then switch to client-side evaluation before using FullName
            var clientsInMemory = clientQuery.AsEnumerable();
            
            // Filter by FullName in memory if needed
            if (!string.IsNullOrWhiteSpace(query))
            {
                clientsInMemory = clientsInMemory.Where(p => p.FullName.ToLower().Contains(query.ToLower()));
            }

            ViewBag.ActiveTab = 1;
            
            return View(clientsInMemory.OrderBy(c => c.ID).ToList());
        }

        [HttpPost]
        public ActionResult Index(string queryName, string queryAddress)
        {
            // Load the base query with included related data
            var clientQuery = _context.Clients.Include(c => c.City);
            
            // Switch to client-side evaluation
            var clientsInMemory = clientQuery.AsEnumerable();
            
            // Apply client-side filters
            if (!string.IsNullOrWhiteSpace(queryName))
                clientsInMemory = clientsInMemory.Where(p => p.FullName.ToLower().Contains(queryName.ToLower()));

            if (!string.IsNullOrWhiteSpace(queryAddress))
                clientsInMemory = clientsInMemory.Where(p => p.Address.ToLower().Contains(queryAddress.ToLower()));

            ViewBag.ActiveTab = 2;

            return View(clientsInMemory.OrderBy(c => c.ID).ToList());
        }

        [HttpPost]
        public ActionResult AdvancedSearch(ClientFilterModel filter)
        {
            // Start with the base query
            var clientQuery = _context.Clients.Include(c => c.City);
            
            // Switch to client-side evaluation
            var clientsInMemory = clientQuery.AsEnumerable();
            
            // Apply all filters in memory
            if (!string.IsNullOrWhiteSpace(filter.FullName))
                clientsInMemory = clientsInMemory.Where(p => p.FullName.ToLower().Contains(filter.FullName.ToLower()));

            if (!string.IsNullOrWhiteSpace(filter.Address))
                clientsInMemory = clientsInMemory.Where(p => p.Address.ToLower().Contains(filter.Address.ToLower()));

            if (!string.IsNullOrWhiteSpace(filter.Email))
                clientsInMemory = clientsInMemory.Where(p => p.Email.ToLower().Contains(filter.Email.ToLower()));

            if (!string.IsNullOrWhiteSpace(filter.City))
                clientsInMemory = clientsInMemory.Where(p => p.City != null && p.City.Name.ToLower().Contains(filter.City.ToLower()));

            // Check if the request came from our new tab
            ViewBag.ActiveTab = 4;

            ViewBag.Filter = filter;

            return View("Index", clientsInMemory.OrderBy(c => c.ID).ToList());
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
            DebugHelper.DD(client);

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
    }
}