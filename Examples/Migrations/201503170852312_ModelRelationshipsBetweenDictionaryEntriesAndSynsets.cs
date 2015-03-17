namespace Examples.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModelRelationshipsBetweenDictionaryEntriesAndSynsets : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SynsetDictionaryEntries", "Synset_Id", "dbo.Synsets");
            DropForeignKey("dbo.SynsetDictionaryEntries", "DictionaryEntry_Id", "dbo.DictionaryEntries");
            DropIndex("dbo.SynsetDictionaryEntries", new[] { "Synset_Id" });
            DropIndex("dbo.SynsetDictionaryEntries", new[] { "DictionaryEntry_Id" });
            DropPrimaryKey("dbo.SynsetDictionaryEntries");
            AddColumn("dbo.SynsetDictionaryEntries", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.SynsetDictionaryEntries", "Synset_Id", c => c.Int());
            AlterColumn("dbo.SynsetDictionaryEntries", "DictionaryEntry_Id", c => c.Int());
            AddPrimaryKey("dbo.SynsetDictionaryEntries", "Id");
            CreateIndex("dbo.SynsetDictionaryEntries", "DictionaryEntry_Id");
            CreateIndex("dbo.SynsetDictionaryEntries", "Synset_Id");
            AddForeignKey("dbo.SynsetDictionaryEntries", "Synset_Id", "dbo.Synsets", "Id");
            AddForeignKey("dbo.SynsetDictionaryEntries", "DictionaryEntry_Id", "dbo.DictionaryEntries", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SynsetDictionaryEntries", "DictionaryEntry_Id", "dbo.DictionaryEntries");
            DropForeignKey("dbo.SynsetDictionaryEntries", "Synset_Id", "dbo.Synsets");
            DropIndex("dbo.SynsetDictionaryEntries", new[] { "Synset_Id" });
            DropIndex("dbo.SynsetDictionaryEntries", new[] { "DictionaryEntry_Id" });
            DropPrimaryKey("dbo.SynsetDictionaryEntries");
            AlterColumn("dbo.SynsetDictionaryEntries", "DictionaryEntry_Id", c => c.Int(nullable: false));
            AlterColumn("dbo.SynsetDictionaryEntries", "Synset_Id", c => c.Int(nullable: false));
            DropColumn("dbo.SynsetDictionaryEntries", "Id");
            AddPrimaryKey("dbo.SynsetDictionaryEntries", new[] { "Synset_Id", "DictionaryEntry_Id" });
            CreateIndex("dbo.SynsetDictionaryEntries", "DictionaryEntry_Id");
            CreateIndex("dbo.SynsetDictionaryEntries", "Synset_Id");
            AddForeignKey("dbo.SynsetDictionaryEntries", "DictionaryEntry_Id", "dbo.DictionaryEntries", "Id", cascadeDelete: true);
            AddForeignKey("dbo.SynsetDictionaryEntries", "Synset_Id", "dbo.Synsets", "Id", cascadeDelete: true);
        }
    }
}
