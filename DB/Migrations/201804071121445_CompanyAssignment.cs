namespace DB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompanyAssignment : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserProfiles", "Company_Name", "dbo.Companies");
            DropForeignKey("dbo.Chats", "Company_ID", "dbo.Companies");
            DropForeignKey("dbo.BaseUsers", "Company_ID", "dbo.Companies");
            DropIndex("dbo.UserProfiles", new[] { "Company_Name" });
            DropPrimaryKey("dbo.Companies");
            AddColumn("dbo.Chats", "Company_ID", c => c.Int());
            AddColumn("dbo.BaseUsers", "Company_ID", c => c.Int());
            AddColumn("dbo.Companies", "ID", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.BaseUsers", "NickName", c => c.String());
            AlterColumn("dbo.Companies", "Name", c => c.String(maxLength: 150));
            AddPrimaryKey("dbo.Companies", "ID");
            CreateIndex("dbo.Chats", "Company_ID");
            CreateIndex("dbo.Companies", "Name", unique: true);
            CreateIndex("dbo.BaseUsers", "Company_ID");
            AddForeignKey("dbo.Chats", "Company_ID", "dbo.Companies", "ID");
            AddForeignKey("dbo.BaseUsers", "Company_ID", "dbo.Companies", "ID");
            DropColumn("dbo.UserProfiles", "Company_Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserProfiles", "Company_Name", c => c.String(maxLength: 128));
            DropForeignKey("dbo.BaseUsers", "Company_ID", "dbo.Companies");
            DropForeignKey("dbo.Chats", "Company_ID", "dbo.Companies");
            DropIndex("dbo.BaseUsers", new[] { "Company_ID" });
            DropIndex("dbo.Companies", new[] { "Name" });
            DropIndex("dbo.Chats", new[] { "Company_ID" });
            DropPrimaryKey("dbo.Companies");
            AlterColumn("dbo.Companies", "Name", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.BaseUsers", "NickName", c => c.Int(nullable: false));
            DropColumn("dbo.Companies", "ID");
            DropColumn("dbo.BaseUsers", "Company_ID");
            DropColumn("dbo.Chats", "Company_ID");
            AddPrimaryKey("dbo.Companies", "Name");
            CreateIndex("dbo.UserProfiles", "Company_Name");
            AddForeignKey("dbo.BaseUsers", "Company_ID", "dbo.Companies", "ID");
            AddForeignKey("dbo.Chats", "Company_ID", "dbo.Companies", "ID");
            AddForeignKey("dbo.UserProfiles", "Company_Name", "dbo.Companies", "Name");
        }
    }
}
