using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR_Operations_System.Migrations
{
    /// <inheritdoc />
    public partial class EmployeeForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAllows_EmpId",
                table: "EmployeeAllows",
                column: "EmpId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeAllows_Employees_EmpId",
                table: "EmployeeAllows",
                column: "EmpId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeAllows_Employees_EmpId",
                table: "EmployeeAllows");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeAllows_EmpId",
                table: "EmployeeAllows");
        }
    }
}
