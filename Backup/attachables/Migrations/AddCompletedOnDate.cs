using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;

namespace ninlabs.attachables.Migrations
{
    public partial class AddCompletedOnDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reminders", "CompletedOn", c => c.DateTime(nullable:true));
        }

        public override void Down()
        {
            DropColumn("dbo.Reminders", "CompletedOn");
        }
    } 

}
