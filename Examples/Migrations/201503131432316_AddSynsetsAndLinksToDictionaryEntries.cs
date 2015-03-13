namespace Examples.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSynsetsAndLinksToDictionaryEntries : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Synsets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Definition = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SynsetDictionaryEntries",
                c => new
                    {
                        Synset_Id = c.Int(nullable: false),
                        DictionaryEntry_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Synset_Id, t.DictionaryEntry_Id })
                .ForeignKey("dbo.Synsets", t => t.Synset_Id, cascadeDelete: true)
                .ForeignKey("dbo.DictionaryEntries", t => t.DictionaryEntry_Id, cascadeDelete: true)
                .Index(t => t.Synset_Id)
                .Index(t => t.DictionaryEntry_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SynsetDictionaryEntries", "DictionaryEntry_Id", "dbo.DictionaryEntries");
            DropForeignKey("dbo.SynsetDictionaryEntries", "Synset_Id", "dbo.Synsets");
            DropIndex("dbo.SynsetDictionaryEntries", new[] { "DictionaryEntry_Id" });
            DropIndex("dbo.SynsetDictionaryEntries", new[] { "Synset_Id" });
            DropTable("dbo.SynsetDictionaryEntries");
            DropTable("dbo.Synsets");
        }
    }
}
