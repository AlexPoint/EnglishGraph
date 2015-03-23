namespace Examples.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRelationshipsBetweenDictionaryEntries : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DictionaryEntryRelationships",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.Byte(nullable: false),
                        Source_Id = c.Int(nullable: false),
                        Target_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DictionaryEntries", t => t.Source_Id, cascadeDelete: true)
                .ForeignKey("dbo.DictionaryEntries", t => t.Target_Id)
                .Index(t => t.Source_Id)
                .Index(t => t.Target_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DictionaryEntryRelationships", "Target_Id", "dbo.DictionaryEntries");
            DropForeignKey("dbo.DictionaryEntryRelationships", "Source_Id", "dbo.DictionaryEntries");
            DropIndex("dbo.DictionaryEntryRelationships", new[] { "Target_Id" });
            DropIndex("dbo.DictionaryEntryRelationships", new[] { "Source_Id" });
            DropTable("dbo.DictionaryEntryRelationships");
        }
    }
}
