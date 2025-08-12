using Microsoft.AspNetCore.Mvc;
using SOS_Alert_App.Data;
using Microsoft.EntityFrameworkCore;
using SOS_Alert_App.Models;

namespace SOS_Alert_App.Controllers
{
    public class AlertsController : Controller
    {
        private readonly AppDbContext _context;

        public AlertsController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var contacts = _context.Contacts.ToList();
            return View(contacts);
        }

        [HttpPost]
        public IActionResult SendAlert(int contactId, string? location = null)
        {
            var contact = _context.Contacts.Find(contactId);
            if (contact == null)
            { 
                return NotFound("Contacto no encontrado.");
            }

            var message = "¡Alerta SOS! Estoy en una situación de emergencia. Mi ubicación es: " + (location);
            if (string.IsNullOrEmpty(location))
            {
                message += $" Location: {location}";
            }

            var alert = new AlertLog
            {
                ContactId = contactId,
                Message = message,
                SentAt = DateTime.Now
            };

            _context.AlertLogs.Add(alert);
            _context.SaveChanges();

            TempData["Success"] = $"Alert sent to {contact.Name} successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult History()
        {
            var logs = _context.AlertLogs
                .Include(a => a.Contact)
                .OrderByDescending(a => a.SentAt)
                .ToList();
            return View(logs);
        }

    }
}
