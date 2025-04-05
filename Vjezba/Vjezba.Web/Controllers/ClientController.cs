using Microsoft.AspNetCore.Mvc;
using Vjezba.Web.Mock;
using Vjezba.Web.Models;
using System.Linq;
using System;

namespace Vjezba.Web.Controllers
{
    public class ClientController : Controller
    {
        [HttpGet]
        public IActionResult Index(string query)
        {
            var allClients = MockClientRepository.Instance.All().ToList();
            
            if (!string.IsNullOrWhiteSpace(query))
            {
                allClients = allClients
                    .Where(c => 
                        (c.FirstName ?? "").Contains(query, StringComparison.OrdinalIgnoreCase) ||
                        (c.LastName ?? "").Contains(query, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                
                ViewBag.CurrentQuery = query;

                ViewBag.ActiveTab = "tab1";
            }
            
            return View(allClients);
        }

        // POST: Client/Index
        [HttpPost]
        public IActionResult Index(string queryName, string queryAddress)
        {
            var allClients = MockClientRepository.Instance.All().ToList();
            
            if (!string.IsNullOrWhiteSpace(queryName))
            {
                allClients = allClients
                    .Where(c => 
                        (c.FirstName ?? "").Contains(queryName, StringComparison.OrdinalIgnoreCase) ||
                        (c.LastName ?? "").Contains(queryName, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                
                ViewBag.CurrentQueryName = queryName;
            }
            
            if (!string.IsNullOrWhiteSpace(queryAddress))
            {
                allClients = allClients
                    .Where(c => (c.Address ?? "").Contains(queryAddress, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                
                ViewBag.CurrentQueryAddress = queryAddress;
            }
            
            ViewBag.ActiveTab = "tab2";
            
            return View(allClients);
        }

        // POST: Client/AdvancedSearch
        [HttpPost]
        public IActionResult AdvancedSearch(ClientFilterModel model)
        {
            var allClients = MockClientRepository.Instance.All().ToList();
            
            if (model != null)
            {
                if (!string.IsNullOrWhiteSpace(model.ClientName))
                {
                    allClients = allClients
                        .Where(c => 
                            (c.FirstName ?? "").Contains(model.ClientName, StringComparison.OrdinalIgnoreCase) ||
                            (c.LastName ?? "").Contains(model.ClientName, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
                
                if (!string.IsNullOrWhiteSpace(model.Address))
                {
                    allClients = allClients
                        .Where(c => (c.Address ?? "").Contains(model.Address, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
                
                if (!string.IsNullOrWhiteSpace(model.Email))
                {
                    allClients = allClients
                        .Where(c => (c.Email ?? "").Contains(model.Email, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
                
                if (!string.IsNullOrWhiteSpace(model.City))
                {
                    allClients = allClients
                        .Where(c => c.City != null && (c.City.Name ?? "").Contains(model.City, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
                
                ViewBag.AdvancedFilter = model;
            }
            
            ViewBag.ActiveTab = "tab3";
            
            return View("Index", allClients);
        }

        // GET: Client/Details/{id}
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                ViewBag.ErrorMessage = "ID nije specificiran.";
                return View();
            }

            var client = MockClientRepository.Instance.FindByID(id.Value);

            if (client == null)
            {
                ViewBag.ErrorMessage = $"Klijent s ID-om {id} ne postoji.";
                return View();
            }

            return View(client);
        }
    }
}