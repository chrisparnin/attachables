using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ninlabs.attachables.Storage
{
    class RemindersInitializer : CreateOrMigrateDatabaseInitializer<RemindersContext, ninlabs.attachables.Migrations.Configuration>
    {
        public RemindersInitializer()
        {
        }
        protected override void Seed(RemindersContext context)
        {
            //context.Database.ExecuteSqlCommand("CREATE UNIQUE INDEX Name ON Stations (Name)");
            //context.Database.ExecuteSqlCommand("CREATE UNIQUE INDEX Name ON Sequences (Name)");
            //context.Database.ExecuteSqlCommand("CREATE UNIQUE INDEX StationPartNumber ON StationPartNumbers (StationId,PartNumberId)");
        }
    }
}
