namespace BookStore2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bookchange : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Books", "Genre", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Books", "Genre", c => c.String());
        }
    }
}
