namespace BookStore2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deletedmodelUserBOokCollection : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Books", "UserBookCollection_Id", "dbo.UserBookCollections");
            DropForeignKey("dbo.UserBookCollections", "User_Id", "dbo.Users");
            DropIndex("dbo.Books", new[] { "UserBookCollection_Id" });
            DropIndex("dbo.UserBookCollections", new[] { "User_Id" });
            DropColumn("dbo.Books", "UserBookCollection_Id");
            DropTable("dbo.UserBookCollections");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.UserBookCollections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Books", "UserBookCollection_Id", c => c.Int());
            CreateIndex("dbo.UserBookCollections", "User_Id");
            CreateIndex("dbo.Books", "UserBookCollection_Id");
            AddForeignKey("dbo.UserBookCollections", "User_Id", "dbo.Users", "Id");
            AddForeignKey("dbo.Books", "UserBookCollection_Id", "dbo.UserBookCollections", "Id");
        }
    }
}
