namespace Examples.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPartOfSpeechToDictionaryEntry : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DictionaryEntries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Word = c.String(),
                        PartOfSpeech = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.Words");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Words",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.DictionaryEntries");
        }
    }
}
