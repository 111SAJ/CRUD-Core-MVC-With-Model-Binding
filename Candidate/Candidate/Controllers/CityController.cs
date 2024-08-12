using Candidate.Data;
using Candidate.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Candidate.Controllers
{
    public class CityController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CityController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // City/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.States = new SelectList(_context.State.ToList(), "StateId", "StateName");
            return View();
        }

        [HttpPost]
        public IActionResult Create(City city)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            foreach (var error in errors)
            {
                Console.WriteLine(error); // Inspect or log each error
            }
            if (ModelState.IsValid)
            {
                //check if City already exist
                var existCity = _context.City.FirstOrDefault(c => c.CityName == city.CityName);
                if (existCity != null)
                {
                    ModelState.AddModelError("CityName", "City already exist");
                    ViewBag.States = new SelectList(_context.State.ToList(), "StateId", "StateName");
                    return View(existCity);
                }

                _context.City.Add(city);
                _context.SaveChanges();

                return RedirectToAction("Index", "Home");
            }

            ViewBag.States = new SelectList(_context.State.ToList(), "StateId", "StateName");
            return View(city);
        }
    }
}
