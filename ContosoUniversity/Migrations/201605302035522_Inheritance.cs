namespace ContosoUniversity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Inheritance : DbMigration
    {
        public override void Up()
        {
            // Drop foreign keys and indexes that point to tables we're going to drop.
            DropForeignKey("school.Enrollment", "StudentID", "school.Student");
            DropIndex("school.Enrollment", new[] { "StudentID" });

            RenameTable(name: "school.Instructor", newName: "Person");
            AddColumn("school.Person", "EnrollmentDate", c => c.DateTime());
            AddColumn("school.Person", "Discriminator", c => c.String(nullable: false, maxLength: 128, defaultValue: "Instructor"));
            AlterColumn("school.Person", "HireDate", c => c.DateTime());
            AddColumn("school.Person", "OldId", c => c.Int(nullable: true));

            // Copy existing Student data into new Person table.
            Sql("INSERT INTO school.Person (LastName, FirstName, HireDate, EnrollmentDate, Discriminator, OldId) "
              + "SELECT LastName, FirstName, null AS HireDate, EnrollmentDate, 'Student' AS Discriminator, ID AS OldId " 
              + "FROM school.Student");

            // Fix up existing relationships to match new PK's
            Sql("UPDATE school.Enrollment SET StudentId = "
              + "(SELECT ID FROM school.Person WHERE OldId = Enrollment.StudentId AND Discriminator = 'Student')");

            // Remove temporary key
            DropColumn("school.Person", "OldId");

            DropTable("school.Student");

            // Re-create foreign keys and indexes pointing to new table.
            AddForeignKey("school.Enrollment", "StudentID", "school.Person", "ID", cascadeDelete: true);
            CreateIndex("school.Enrollment", "StudentID");
        }
        
        public override void Down()
        {
            CreateTable(
                "school.Student",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        LastName = c.String(nullable: false, maxLength: 50),
                        FirstName = c.String(nullable: false, maxLength: 50),
                        EnrollmentDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            AlterColumn("school.Person", "HireDate", c => c.DateTime(nullable: false));
            AlterColumn("school.Person", "FirstName", c => c.String(maxLength: 50));
            AlterColumn("school.Person", "LastName", c => c.String(maxLength: 50));
            DropColumn("school.Person", "Discriminator");
            DropColumn("school.Person", "EnrollmentDate");
            RenameTable(name: "school.Person", newName: "Instructor");
        }
    }
}
