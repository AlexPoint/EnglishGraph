using EnglishGraph.Models;

namespace Examples.Classes
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class EnglishGraphContext : DbContext
    {
        // Your context has been configured to use a 'EnglishGraphContext' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'Examples.Classes.EnglishGraphContext' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'EnglishGraphContext' 
        // connection string in the application configuration file.
        public EnglishGraphContext(): base("EnglishGraphContext"){ }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<DictionaryEntry> DictionaryEntries { get; set; }
        public virtual DbSet<Synset> Synsets { get; set; }
        public virtual DbSet<SynsetDictionaryEntry> SynsetsAndDictionaryEntries { get; set; }
        public virtual DbSet<DictionaryEntryRelationship> DictionaryEntryRelationships { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            
            // DictionaryEntry
            modelBuilder.Entity<DictionaryEntry>()
                .HasMany(e => e.Synsets)
                .WithRequired(sde => sde.DictionaryEntry)
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<DictionaryEntry>()
                .HasMany(de => de.DerivedRelationships)
                .WithRequired(rel => rel.Source)
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<DictionaryEntry>()
                .HasMany(de => de.StemmedFromRelationships)
                .WithRequired(rel => rel.Target)
                //cannot add cascade delete condition here because of circular references
                .WillCascadeOnDelete(false);

            // Synsets
            modelBuilder.Entity<Synset>()
                .HasMany(e => e.DictionaryEntries)
                .WithRequired(sde => sde.Synset)
                .WillCascadeOnDelete(true);


        }
    }
}