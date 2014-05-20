using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;

namespace ninlabs.attachables.Migrations
{
    public partial class AddNotificationType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reminders", "NotificationType", c => c.Int());
        }

        public override void Down()
        {
            DropColumn("dbo.Reminders", "NotificationType");
        }
    } 

}
