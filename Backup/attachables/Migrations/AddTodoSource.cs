using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;

namespace ninlabs.attachables.Migrations
{
    public partial class AddTodoSource : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reminders", "SourcePath", c => c.String());
            AddColumn("dbo.Reminders", "LineStart", c => c.Int());
        }

        public override void Down()
        {
            DropColumn("dbo.Reminders", "SourcePath");
            DropColumn("dbo.Reminders", "LineStart");
        }
    } 

}
