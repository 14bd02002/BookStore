namespace BookStore2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class authorimage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Authors", "AuthorYear", c => c.Int(nullable: false));
            AddColumn("dbo.Authors", "AuthorImageUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Authors", "AuthorImageUrl");
            DropColumn("dbo.Authors", "AuthorYear");
        }
    }
}
