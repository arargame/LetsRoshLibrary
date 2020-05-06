using LetsRoshLibrary.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Core.Context
{
    public class MainContext : DbContext
    {
        public MainContext() : base("name=MsSqlConnectionString")
        {

        }

        //public DbSet<Hero> Hero { get; set; }
        //public DbSet<Image> Image { get; set; }
        //public DbSet<Item> Item { get; set; }
        //public DbSet<Language> Language { get; set; }
        //public DbSet<Localization> Localization { get; set; }
        //public DbSet<Log> Log { get; set; }
        //public DbSet<Skill> Skill { get; set; }

        public DbSet<BaseObject> BaseObject { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            base.OnModelCreating(modelBuilder);


            //modelBuilder.Entity<Hero>().Map(m =>
            //{
            //    m.MapInheritedProperties();
            //    m.ToTable("Hero");
            //});

            //modelBuilder.Entity<Image>().Map(m =>
            //{
            //    m.MapInheritedProperties();
            //    m.ToTable("Image");
            //});

            //modelBuilder.Entity<Item>().Map(m =>
            //{
            //    m.MapInheritedProperties();
            //    m.ToTable("Item");
            //});

            //modelBuilder.Entity<Language>().Map(m =>
            //{
            //    m.MapInheritedProperties();
            //    m.ToTable("Language");
            //});

            //modelBuilder.Entity<Localization>().Map(m =>
            //{
            //    m.MapInheritedProperties();
            //    m.ToTable("Localization");
            //});

            //modelBuilder.Entity<Log>().Map(m =>
            //{
            //    m.MapInheritedProperties();
            //    m.ToTable("Log");
            //});

            //modelBuilder.Entity<Skill>().Map(m =>
            //{
            //    m.MapInheritedProperties();
            //    m.ToTable("Skill");
            //});

            //modelBuilder.Entity<Item>()
            //    .HasMany(i => i.Localizations)
            //    .WithRequired()
            //    .HasForeignKey(l => l.BaseObjectId);


            modelBuilder.Entity<Character>().ToTable("Character");

            modelBuilder.Entity<Hero>().ToTable("Hero");

            modelBuilder.Entity<Image>().ToTable("Image");

            modelBuilder.Entity<Item>().ToTable("Item");

            modelBuilder.Entity<Language>().ToTable("Language");

            modelBuilder.Entity<Localization>().ToTable("Localization");

            modelBuilder.Entity<Log>().ToTable("Log");

            modelBuilder.Entity<Skill>().ToTable("Skill");

            modelBuilder.Entity<Class1>().ToTable("Class1");

            modelBuilder.Entity<Item>()
                .Property(i => i.LinkParameter)
                .HasMaxLength(21);

            modelBuilder.Entity<Language>()
                .Property(i => i.Name)
                .IsUnicode();

            modelBuilder.Entity<BaseObject>()
                .HasMany(bo => bo.Localizations)
                .WithRequired()
                .HasForeignKey(l => l.BaseObjectId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Image>()
                .HasKey(i => i.Id)
                .HasRequired(i => i.BaseObject)
                .WithOptional()
                .Map(m => m.MapKey("BaseObjectId"))
                .WillCascadeOnDelete(true);

        }
    }
}
