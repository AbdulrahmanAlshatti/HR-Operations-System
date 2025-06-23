using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR_Operations_System.Migrations
{
    /// <inheritdoc />
    public partial class EmpAllowTypo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EndtDate",
                table: "EmployeeAllows",
                newName: "EndDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "EmployeeAllows",
                newName: "EndtDate");
        }
    }
}
