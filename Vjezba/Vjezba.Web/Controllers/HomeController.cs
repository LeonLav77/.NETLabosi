using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Vjezba.Web.Models;

namespace Vjezba.Web.Controllers
{
    public class HomeController(
        ILogger<HomeController> _logger) 
        : Controller
    {
        public async Task<IActionResult> IndexAsync()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult FAQ(int? selected = null)
        {
            var questionsAndAnswers = new Dictionary<int, (string question, string answer)>
            {
                { 1, ("Kako mogu postaviti razvojno okruženje za ASP.NET MVC projekt?",
                    "Za razvoj ASP.NET MVC projekata preporučamo instalaciju Visual Studio 2022 (Community, Professional ili Enterprise izdanje). Nakon instalacije, odaberite 'ASP.NET and web development' radni paket. Za nove projekte koristite predložak 'ASP.NET Core Web App (Model-View-Controller)'. Za rad s bazama podataka, instalirajte i SQL Server Express ili LocalDB.") },
                
                { 2, ("Koja je razlika između ViewData, ViewBag i TempData?",
                    "ViewData je Dictionary objekt koji se koristi za prijenos podataka iz kontrolera u pogled. ViewBag je dinamički objekt baziran na ViewData, što ga čini praktičnijim za korištenje. TempData služi za prijenos podataka od jednog zahtjeva do drugog, na primjer između akcija nakon preusmjeravanja. ViewData i ViewBag su dostupni samo tijekom trenutnog zahtjeva, dok TempData može perzistirati između više zahtjeva.") },
                
                { 3, ("Kako implementirati autentikaciju u ASP.NET MVC aplikaciji?",
                    "ASP.NET Core nudi nekoliko opcija za autentikaciju. Za većinu projekata preporučljivo je koristiti ASP.NET Core Identity, koji podržava prijavu s lozinkom, višefaktorsku autentikaciju, prijavu putem vanjskih servisa poput Google ili Facebook, i uključuje bazu podataka za upravljanje korisnicima. Za jednostavnije aplikacije možete koristiti i Cookie autentikaciju, a za API projekte JWT tokene.") },
                
                { 4, ("Kako optimizirati performanse ASP.NET MVC aplikacije?",
                    "Za bolje performanse ASP.NET MVC aplikacije preporučuje se korištenje asinkronih metoda za I/O operacije, implementacija caching mehanizama, optimizacija upita prema bazi podataka, minifikacija i bundling JavaScript i CSS datoteka, te korištenje CDN za statične resurse. Također je dobra praksa izbjegavati prekomjerno korištenje ViewBag i ViewData, te umjesto toga koristiti jako tipiziranje s View modelima.") },
                
                { 5, ("Kako organizirati kompleksan projekt s više modula?",
                    "Za organizaciju kompleksnih projekata korisno je implementirati arhitekturalne obrasce poput Repository Pattern, Unit of Work, i CQRS. Projekt možete podijeliti u više slojeva: prezentacijski sloj (MVC), sloj poslovne logike, sloj pristupa podacima, i možda infrastrukturni sloj. Za veće sustave razmotrite pristup Microservices arhitekture, gdje su pojedini moduli implementirani kao zasebne aplikacije koje komuniciraju putem API-ja.") }
            };

            ViewBag.SelectedQuestion = selected;
            ViewBag.QuestionsAndAnswers = questionsAndAnswers;
            return View();
        }

        public IActionResult Contact()
        {
            ViewBag.Message = "Jednostavan način proslijeđivanja poruke iz Controller -> View.";
            //Kao rezultat se pogled /Views/Home/Contact.cshtml renderira u "pravi" HTML
            //Primjetiti - View() je poziv funkcije koja uzima cshtml template i pretvara ga u HTML
            //Zasto bas Contact.cshtml? Jer se akcija zove Contact, te prema konvenciji se "po defaultu" uzima cshtml datoteka u folderu Views/CONTROLLER_NAME/AKCIJA.cshtml


            return View();
        }

        /// <summary>
        /// Ova akcija se poziva kada na formi za kontakt kliknemo "Submit"
        /// URL ove akcije je /Home/SubmitQuery, uz POST zahtjev isključivo - ne može se napraviti GET zahtjev zbog [HttpPost] parametra
        /// </summary>
        /// <param name="formData"></param>
        /// <returns></returns>
         [HttpPost]
        public IActionResult SubmitQuery(IFormCollection form)
        {
            string ime = form["ime"];
            string prezime = form["prezime"];
            string imePrezime = $"{ime} {prezime}";
            string email = form["email"];
            string poruka = form["poruka"];
            string tipPoruke = form["tipPoruke"];
            bool newsletter = form["newsletter"].Count > 0;
            
            string newsletterText = newsletter ? "obavijestit ćemo vas" : "nećemo vas obavijestiti";
            
            string responseMessage = $"Dragi {imePrezime} ({email}) zaprimili smo vašu poruku te će vam se netko ubrzo javiti. " +
                                    $"Sadržaj vaše poruke je: [{tipPoruke}] {poruka}. " +
                                    $"Također, {newsletterText} o daljnjim promjenama preko newslettera.";
            
            return View("ContactSuccess", responseMessage);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new { RequestId = "Auditorne" });
        }
    }
}