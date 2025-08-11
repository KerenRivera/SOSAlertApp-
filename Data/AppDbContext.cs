using Microsoft.EntityFrameworkCore;
using SOS_Alert_App.Models;

namespace SOS_Alert_App.Data
{
    public class AppDbContext :DbContext
    {
        public DbSet<Contact> Contacts { get; set; } 
        public DbSet<AlertLog> AlertLogs { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
    }
}
