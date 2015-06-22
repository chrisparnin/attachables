using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace ninlabs.attachables.Storage
{
    class CreateOrMigrateDatabaseInitializer<TContext, TConfiguration> : CreateDatabaseIfNotExists<TContext>, IDatabaseInitializer<TContext>
        where TContext : DbContext
        where TConfiguration : DbMigrationsConfiguration<TContext>, new()
    {
        private readonly DbMigrationsConfiguration _configuration;
        public CreateOrMigrateDatabaseInitializer()
        {
            _configuration = new TConfiguration();
        }
        public CreateOrMigrateDatabaseInitializer(string connection)
        {
            Contract.Requires(!string.IsNullOrEmpty(connection), "connection");

            _configuration = new TConfiguration
            {
                TargetDatabase = new DbConnectionInfo(connection)
            };
        }
        void IDatabaseInitializer<TContext>.InitializeDatabase(TContext context)
        {
            Contract.Requires(context != null, "context");

            if (context.Database.Exists())
            {
                if (!context.Database.CompatibleWithModel(throwIfNoMetadata: false))
                {
                    var migrator = new DbMigrator(_configuration);
                    migrator.Update();
                }
            }
            else
            {
                context.Database.Create();
                Seed(context);
                context.SaveChanges();
            }


        }
        protected virtual void Seed(TContext context)
        {
        }
    }
}
