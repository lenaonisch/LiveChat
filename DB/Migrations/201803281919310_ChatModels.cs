namespace DB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChatModels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Chats",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        StartDateTime = c.DateTime(nullable: false),
                        Duration = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        DateTime = c.DateTime(nullable: false),
                        Text = c.String(nullable: false),
                        User_ID = c.Int(),
                        Chat_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.BaseUsers", t => t.User_ID)
                .ForeignKey("dbo.Chats", t => t.Chat_ID)
                .Index(t => t.User_ID)
                .Index(t => t.Chat_ID);
            
            CreateTable(
                "dbo.BaseUsers",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        NickName = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.UserProfiles",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Avatar = c.Binary(),
                        BaseUser_ID = c.Int(),
                        Company_Name = c.String(maxLength: 128),
                        Contact_ID = c.Int(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.BaseUsers", t => t.BaseUser_ID)
                .ForeignKey("dbo.Companies", t => t.Company_Name)
                .ForeignKey("dbo.Contacts", t => t.Contact_ID)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.BaseUser_ID)
                .Index(t => t.Company_Name)
                .Index(t => t.Contact_ID)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Name);
            
            CreateTable(
                "dbo.Contacts",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Country = c.String(nullable: false, maxLength: 50),
                        Town = c.String(maxLength: 50),
                        District = c.String(maxLength: 100),
                        Street = c.String(maxLength: 100),
                        House = c.String(maxLength: 10),
                        Flat = c.String(maxLength: 10),
                        Notes = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserProfiles", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserProfiles", "Contact_ID", "dbo.Contacts");
            DropForeignKey("dbo.UserProfiles", "Company_Name", "dbo.Companies");
            DropForeignKey("dbo.UserProfiles", "BaseUser_ID", "dbo.BaseUsers");
            DropForeignKey("dbo.Messages", "Chat_ID", "dbo.Chats");
            DropForeignKey("dbo.Messages", "User_ID", "dbo.BaseUsers");
            DropIndex("dbo.UserProfiles", new[] { "User_Id" });
            DropIndex("dbo.UserProfiles", new[] { "Contact_ID" });
            DropIndex("dbo.UserProfiles", new[] { "Company_Name" });
            DropIndex("dbo.UserProfiles", new[] { "BaseUser_ID" });
            DropIndex("dbo.Messages", new[] { "Chat_ID" });
            DropIndex("dbo.Messages", new[] { "User_ID" });
            DropTable("dbo.Contacts");
            DropTable("dbo.Companies");
            DropTable("dbo.UserProfiles");
            DropTable("dbo.BaseUsers");
            DropTable("dbo.Messages");
            DropTable("dbo.Chats");
        }
    }
}
