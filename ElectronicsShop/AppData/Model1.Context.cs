﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ElectronicsShop.AppData
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ElectronicsShopEntities : DbContext
    {
        public ElectronicsShopEntities()
            : base("name=ElectronicsShopEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Brands> Brands { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Country> Country { get; set; }
        public virtual DbSet<Korzina> Korzina { get; set; }
        public virtual DbSet<Orders> Orders { get; set; }
        public virtual DbSet<OrdersPodr> OrdersPodr { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<Sale> Sale { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<Users> Users { get; set; }
    }
}
