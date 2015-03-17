namespace Examples.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCascadeOnDeleteConstraintsForSynsetDictionaryEntryTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SynsetDictionaryEntries", "DictionaryEntry_Id", "dbo.DictionaryEntries");
            DropForeignKey("dbo.SynsetDictionaryEntries", "Synset_Id", "dbo.Synsets");
            DropIndex("dbo.SynsetDictionaryEntries", new[] { "DictionaryEntry_Id" });
            DropIndex("dbo.SynsetDictionaryEntries", new[] { "Synset_Id" });
            AlterColumn("dbo.SynsetDictionaryEntries", "DictionaryEntry_Id", c => c.Int(nullable: false));
            AlterColumn("dbo.SynsetDictionaryEntries", "Synset_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.SynsetDictionaryEntries", "Synset_Id");
            CreateIndex("dbo.SynsetDictionaryEntries", "DictionaryEntry_Id");
            AddForeignKey("dbo.SynsetDictionaryEntries", "DictionaryEntry_Id", "dbo.DictionaryEntries", "Id", cascadeDelete: true);
            AddForeignKey("dbo.SynsetDictionaryEntries", "Synset_Id", "dbo.Synsets", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SynsetDictionaryEntries", "Synset_Id", "dbo.Synsets");
            DropForeignKey("dbo.SynsetDictionaryEntries", "DictionaryEntry_Id", "dbo.DictionaryEntries");
            DropIndex("dbo.SynsetDictionaryEntries", new[] { "DictionaryEntry_Id" });
            DropIndex("dbo.SynsetDictionaryEntries", new[] { "Synset_Id" });
            AlterColumn("dbo.SynsetDictionaryEntries", "Synset_Id", c => c.Int());
            AlterColumn("dbo.SynsetDictionaryEntries", "DictionaryEntry_Id", c => c.Int());
            CreateIndex("dbo.SynsetDictionaryEntries", "Synset_Id");
            CreateIndex("dbo.SynsetDictionaryEntries", "DictionaryEntry_Id");
            AddForeignKey("dbo.SynsetDictionaryEntries", "Synset_Id", "dbo.Synsets", "Id");
            AddForeignKey("dbo.SynsetDictionaryEntries", "DictionaryEntry_Id", "dbo.DictionaryEntries", "Id");
        }
    }
}
