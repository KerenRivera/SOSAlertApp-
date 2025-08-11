using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SOS_Alert_App.Data;
using SOS_Alert_App.Models;


namespace SOS_Alert_App.Controllers
{
    public class ContactsController : Controller
    {
        private readonly AppDbContext _context;

        public ContactsController(AppDbContext context)
        {
            _context = context;
        }

        //GET: /Contacts
        public IActionResult Index()
        {
            var contacts = _context.Contacts
                .Include(c => c.AlertLogs)
                .ToListAsync();

            return View(contacts);
        }

        //GET: /Contacts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Contacts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contact contact)
        {
            if (ModelState.IsValid)
            {
                _context.Contacts.Add(contact);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(contact);
        }
    }
}
