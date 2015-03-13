namespace Examples.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RedefinePartOfSpeechCodes : DbMigration
    {
        public override void Up()
        {
            Sql("UPDATE DictionaryEntries SET PartOfSpeech = PartOfSpeech * 10");
        }
        
        public override void Down()
        {
            Sql("UPDATE DictionaryEntries SET PartOfSpeech = PartOfSpeech / 10");
        }
    }
}
