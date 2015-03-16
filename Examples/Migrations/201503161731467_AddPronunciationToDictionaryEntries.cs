namespace Examples.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPronunciationToDictionaryEntries : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DictionaryEntries", "Pronunciation", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DictionaryEntries", "Pronunciation");
        }
    }
}
