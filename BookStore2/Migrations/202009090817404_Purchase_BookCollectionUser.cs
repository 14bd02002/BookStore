namespace BookStore2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Purchase_BookCollectionUser : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Purchases",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PurchaseCode = c.String(),
                        Book_Id = c.Int(),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Books", t => t.Book_Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.Book_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.UserBookCollections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
            AddColumn("dbo.Books", "UserBookCollection_Id", c => c.Int());
            CreateIndex("dbo.Books", "UserBookCollection_Id");
            AddForeignKey("dbo.Books", "UserBookCollection_Id", "dbo.UserBookCollections", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserBookCollections", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Books", "UserBookCollection_Id", "dbo.UserBookCollections");
            DropForeignKey("dbo.Purchases", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Purchases", "Book_Id", "dbo.Books");
            DropIndex("dbo.UserBookCollections", new[] { "User_Id" });
            DropIndex("dbo.Purchases", new[] { "User_Id" });
            DropIndex("dbo.Purchases", new[] { "Book_Id" });
            DropIndex("dbo.Books", new[] { "UserBookCollection_Id" });
            DropColumn("dbo.Books", "UserBookCollection_Id");
            DropTable("dbo.UserBookCollections");
            DropTable("dbo.Purchases");
        }
    }
}
