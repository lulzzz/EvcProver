﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Prover.Core.Migrations;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Instruments;
using Prover.Core.Migrations;
using System.Data.Entity.Migrations;

namespace Prover.Core.Storage
{

    public class ProverContext : DbContext
    {
        public ProverContext()
            : base(@"name=ConnectionString")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ProverContext, Configuration>());
            Database.Initialize(false);
            ((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized += ObjectContext_ObjectMaterialized;
            Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        }

        protected void ObjectContext_ObjectMaterialized(object sender, ObjectMaterializedEventArgs e)
        {
            (e.Entity as BaseEntity)?.OnInitializing();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<VerificationTest> VerificationTests { get; set; }
        public DbSet<VolumeTest> VolumeTests { get; set; }
        public DbSet<TemperatureTest> TemperatureTests { get; set; }
        public DbSet<PressureTest> PressureTests { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<Instrument> Instruments { get; set; }

    }
}
