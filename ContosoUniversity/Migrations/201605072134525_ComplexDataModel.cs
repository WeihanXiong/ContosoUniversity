namespace ContosoUniversity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ComplexDataModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "school.Department",
                c => new
                    {
                        DepartmentID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        Budget = c.Decimal(nullable: false, storeType: "money"),
                        StartDate = c.DateTime(nullable: false),
                        InstructorID = c.Int(),
                    })
                .PrimaryKey(t => t.DepartmentID)
                .ForeignKey("school.Instructor", t => t.InstructorID)
                .Index(t => t.InstructorID);
            
            CreateTable(
                "school.Instructor",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        LastName = c.String(maxLength: 50),
                        FirstName = c.String(maxLength: 50),
                        HireDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "school.OfficeAssignment",
                c => new
                    {
                        InstructorID = c.Int(nullable: false),
                        Location = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.InstructorID)
                .ForeignKey("school.Instructor", t => t.InstructorID)
                .Index(t => t.InstructorID);
            
            CreateTable(
                "school.CourseInstructor",
                c => new
                    {
                        CourseID = c.Int(nullable: false),
                        InstructorID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.CourseID, t.InstructorID })
                .ForeignKey("school.Course", t => t.CourseID, cascadeDelete: true)
                .ForeignKey("school.Instructor", t => t.InstructorID, cascadeDelete: true)
                .Index(t => t.CourseID)
                .Index(t => t.InstructorID);

            // Create  a department for course to point to.
            Sql("INSERT INTO dbo.Department (Name, Budget, StartDate) VALUES ('Temp', 0.00, GETDATE())");
            //  default value for FK points to department created above.
            AddColumn("dbo.Course", "DepartmentID", c => c.Int(nullable: false, defaultValue: 1));
            //AddColumn("dbo.Course", "DepartmentID", c => c.Int(nullable: false));
            AlterColumn("school.Course", "Title", c => c.String(maxLength: 50));
            CreateIndex("school.Course", "DepartmentID");
            AddForeignKey("school.Course", "DepartmentID", "school.Department", "DepartmentID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("school.CourseInstructor", "InstructorID", "school.Instructor");
            DropForeignKey("school.CourseInstructor", "CourseID", "school.Course");
            DropForeignKey("school.Course", "DepartmentID", "school.Department");
            DropForeignKey("school.Department", "InstructorID", "school.Instructor");
            DropForeignKey("school.OfficeAssignment", "InstructorID", "school.Instructor");
            DropIndex("school.CourseInstructor", new[] { "InstructorID" });
            DropIndex("school.CourseInstructor", new[] { "CourseID" });
            DropIndex("school.OfficeAssignment", new[] { "InstructorID" });
            DropIndex("school.Department", new[] { "InstructorID" });
            DropIndex("school.Course", new[] { "DepartmentID" });
            AlterColumn("school.Course", "Title", c => c.String());
            DropColumn("school.Course", "DepartmentID");
            DropTable("school.CourseInstructor");
            DropTable("school.OfficeAssignment");
            DropTable("school.Instructor");
            DropTable("school.Department");
        }
    }
}
