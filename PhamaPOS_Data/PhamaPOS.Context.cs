﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PhamaPOS_Data
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class PhamaPOSEntities : DbContext
    {
        public PhamaPOSEntities()
            : base("name=PhamaPOSEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<item> items { get; set; }
        public DbSet<sale> sales { get; set; }
        public DbSet<saleBatch> saleBatches { get; set; }
        public DbSet<stock> stocks { get; set; }
        public DbSet<unit> units { get; set; }
        public DbSet<user> users { get; set; }
    }
}