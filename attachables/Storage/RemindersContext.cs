using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ninlabs.attachables.Models;

namespace ninlabs.attachables.Storage
{
    public class RemindersContext : DbContext
    {
        public DbSet<ReminderContract> Reminders { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReminderContract>().HasKey(p => p.Id);
            modelBuilder.Entity<ReminderContract>().Property(p => p.ReminderMessage).HasMaxLength(1000);
        }

        public static void ConfigureDatabase(string path)
        {
            Database.SetInitializer<RemindersContext>(new RemindersInitializer());
            //Database.SetInitializer<RemindersContext>(new DropCreateDatabaseIfModelChanges<RemindersContext>());
            Database.DefaultConnectionFactory = new SqlCeConnectionFactory("System.Data.SqlServerCe.4.0",
                path, "");
        }

    }
}
