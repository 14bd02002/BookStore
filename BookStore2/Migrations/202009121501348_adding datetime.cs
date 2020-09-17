namespace BookStore2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingdatetime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Purchases", "Time", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Purchases", "Time");
        }
    }
}
