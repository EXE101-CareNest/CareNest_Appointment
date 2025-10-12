using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareNest_Appointment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerPhoneToAppointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerPhone",
                table: "Appointments",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerPhone",
                table: "Appointments");
        }
    }
}
