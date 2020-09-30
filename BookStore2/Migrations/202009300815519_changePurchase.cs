namespace BookStore2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changePurchase : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Purchases", "Book_Id", "dbo.Books");
            DropForeignKey("dbo.Purchases", "User_Id", "dbo.Users");
            DropIndex("dbo.Purchases", new[] { "Book_Id" });
            DropIndex("dbo.Purchases", new[] { "User_Id" });
            RenameColumn(table: "dbo.Purchases", name: "Book_Id", newName: "BookId");
            RenameColumn(table: "dbo.Purchases", name: "User_Id", newName: "UserId");
            AlterColumn("dbo.Purchases", "BookId", c => c.Int(nullable: true));
            AlterColumn("dbo.Purchases", "UserId", c => c.Int(nullable: true));
            CreateIndex("dbo.Purchases", "BookId");
            CreateIndex("dbo.Purchases", "UserId");
            AddForeignKey("dbo.Purchases", "BookId", "dbo.Books", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Purchases", "UserId", "dbo.Users", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Purchases", "UserId", "dbo.Users");
            DropForeignKey("dbo.Purchases", "BookId", "dbo.Books");
            DropIndex("dbo.Purchases", new[] { "UserId" });
            DropIndex("dbo.Purchases", new[] { "BookId" });
            AlterColumn("dbo.Purchases", "UserId", c => c.Int());
            AlterColumn("dbo.Purchases", "BookId", c => c.Int());
            RenameColumn(table: "dbo.Purchases", name: "UserId", newName: "User_Id");
            RenameColumn(table: "dbo.Purchases", name: "BookId", newName: "Book_Id");
            CreateIndex("dbo.Purchases", "User_Id");
            CreateIndex("dbo.Purchases", "Book_Id");
            AddForeignKey("dbo.Purchases", "User_Id", "dbo.Users", "Id");
            AddForeignKey("dbo.Purchases", "Book_Id", "dbo.Books", "Id");
        }
    }
}
