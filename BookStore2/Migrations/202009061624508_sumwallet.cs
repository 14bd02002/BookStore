namespace BookStore2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sumwallet : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Books", "BookPrice", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "Wallet", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "Wallet");
            DropColumn("dbo.Books", "BookPrice");
        }
    }
}
