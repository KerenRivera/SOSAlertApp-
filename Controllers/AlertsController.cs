using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using SOS_Alert_App.Data;
using Microsoft.EntityFrameworkCore;
using SOS_Alert_App.Models;
using System.Threading.Tasks;

namespace SOS_Alert_App.Controllers
{
    public class AlertsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AlertsController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            var contacts = _context.Contacts.ToList();
            return View(contacts);
        }

        [HttpPost]
        public async Task<IActionResult> SendAlert(int contactId, string? location = null)
        {
            var contact = _context.Contacts.Find(contactId);
            if (contact != null)
            {
                try
                {
                    await SendEmailAlert(contact, location);
                    TempData["Success"] = $"Alerta enviada a {contact.Name} con exito via EMAIL!.";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error enviando email: {ex.Message}";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task SendEmailAlert(Contact contact, string? location)
        {
            var emailConfig = _configuration.GetSection("Email");

            var smtpClient = new SmtpClient(emailConfig["SmtpServer"])
            {
                Port = int.Parse(emailConfig["SmtpPort"] ?? "587"),
                Credentials = new NetworkCredential(emailConfig["FromEmail"], emailConfig["FromPassword"]),
                EnableSsl = bool.Parse(emailConfig["EnableSsl"] ?? "true"),
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(emailConfig["FromEmail"], emailConfig["FromName"]),
                Subject = "¡ALERTA DE EMERGENCIA - Necesito ayuda inmediata!",
                Body = CreateEmailBody(contact, location),
                IsBodyHtml = true,
            };

            mailMessage.To.Add(new MailAddress(contact.Email, contact.Name));
            await smtpClient.SendMailAsync(mailMessage);
        }

        private string CreateEmailBody(Contact contact, string? location)
        {
            return $@"
                <html>
                <head>
                  <style>
                    body {{ font-family: Arial, sans-serif; }}
                    .alert-header {{ background-color: #dc3545; color: white; padding: 20px; text-align: center; }}
                    .alert-content {{ padding: 20px; }}
                    .location {{ background-color: #f8f9fa; padding: 10px; border-left: 4px solid #dc3545; }}
                  </style>
               </head>
               <body>
                <div class='alert-header'>
                    <h1>🚨 EMERGENCY ALERT 🚨</h1>
                </div>
                <div class='alert-content'>
                    <h2>Hello {contact.Name},</h2>
                    <p><strong>An emergency situation has been detected and requires immediate assistance!</strong></p>
                    
                    <div class='location'>
                        <h3>📍 Location:</h3>
                        <p>{location}</p>
                    </div>
                    
                    <p><strong>🕐 Time:</strong> {DateTime.Now:dddd, MMMM dd, yyyy 'at' HH:mm:ss}</p>
                    
                    <hr>
                    <p><strong>Please respond immediately!</strong></p>
                    <p>If this is a real emergency, contact local emergency services at <strong>911</strong></p>
                    
                    <p><em>This message was sent automatically by the SOS Alert System.</em></p>
                </div>
               </body>
               </html>
            ";
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
