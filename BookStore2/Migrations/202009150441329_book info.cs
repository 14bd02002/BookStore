namespace BookStore2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bookinfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Books", "Year", c => c.Int(nullable: false));
            AddColumn("dbo.Books", "Genre", c => c.String());
            AddColumn("dbo.Books", "BoughtBooks", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Books", "BoughtBooks");
            DropColumn("dbo.Books", "Genre");
            DropColumn("dbo.Books", "Year");
        }
    }
}
