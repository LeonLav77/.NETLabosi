using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vjezba.Web.Models;
using Vjezba.Model;
using Vjezba.DAL; // Namespace for DbContext

namespace Vjezba.Web.Controllers
{
    public class ClientController : Controller
    {
        // Add database context with the correct class
        private readonly ClientManagerDbContext _context;

        // Constructor with dependency injection
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
            
            return View(clientsInMemory.ToList());
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

            return View(clientsInMemory.ToList());
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

            return View("Index", clientsInMemory.ToList());
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
            // Get cities from database for dropdown
            ViewBag.Cities = _context.Cities.ToList();
            return View(new Client());
        }

        [HttpPost]
        public IActionResult Create(Client client)
        {
            // Check if model is valid and handle required fields
            if (string.IsNullOrEmpty(client.FirstName))
            {
                ModelState.AddModelError("FirstName", "First Name is required");
            }
            
            if (string.IsNullOrEmpty(client.LastName))
            {
                ModelState.AddModelError("LastName", "Last Name is required");
            }

            // Set default Gender if not provided
            if (client.Gender == '\0')
            {
                client.Gender = 'M'; // Default to 'M' or whatever default makes sense for your app
            }
            
            if (!ModelState.IsValid)
            {
                // If validation fails, reload cities for dropdown and return to form
                ViewBag.Cities = _context.Cities.ToList();
                return View(client);
            }
            
            // Hard-code CityID to 1, 2, or 3 (not using value from dropdown)
            // Using 1 as an example, can be 2 or 3 as well
            client.CityID = 1;
            
            // Add client to database context
            _context.Clients.Add(client);
            
            try 
            {
                // Save changes to the database
                _context.SaveChanges();
                
                // Redirect to index
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Add error message and return to form if save fails
                ModelState.AddModelError("", "Error saving client: " + ex.InnerException?.Message ?? ex.Message);
                ViewBag.Cities = _context.Cities.ToList();
                return View(client);
            }
        }
    }
}