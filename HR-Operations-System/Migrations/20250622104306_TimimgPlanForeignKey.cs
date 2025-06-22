using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR_Operations_System.Migrations
{
    /// <inheritdoc />
    public partial class TimimgPlanForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAllows_TimingCode",
                table: "EmployeeAllows",
                column: "TimingCode");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeAllows_TimingPlans_TimingCode",
                table: "EmployeeAllows",
                column: "TimingCode",
                principalTable: "TimingPlans",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeAllows_TimingPlans_TimingCode",
                table: "EmployeeAllows");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeAllows_TimingCode",
                table: "EmployeeAllows");
        }
    }
}
