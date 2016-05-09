namespace ContosoUniversity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StudentComplete : DbMigration
    {
        public override void Up()
        {
            AlterColumn("school.Student", "LastName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("school.Student", "FirstName", c => c.String(nullable: false, maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("school.Student", "FirstName", c => c.String(maxLength: 50));
            AlterColumn("school.Student", "LastName", c => c.String(maxLength: 50));
        }
    }
}
