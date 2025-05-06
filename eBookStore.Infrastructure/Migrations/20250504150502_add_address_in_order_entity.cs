using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eBookStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_address_in_order_entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShippingAddress",
                table: "Orders",
                newName: "ShippingAddress_StreetAddress");

            migrationBuilder.AddColumn<int>(
                name: "BillingAddressId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BillingAddress_City",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "BillingAddress_Id",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BillingAddress_Name",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BillingAddress_Phone",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BillingAddress_PostCode",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BillingAddress_StreetAddress",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ShippingAddressId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress_City",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ShippingAddress_Id",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress_Name",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress_Phone",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress_PostCode",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillingAddressId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "BillingAddress_City",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "BillingAddress_Id",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "BillingAddress_Name",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "BillingAddress_Phone",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "BillingAddress_PostCode",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "BillingAddress_StreetAddress",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingAddressId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingAddress_City",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingAddress_Id",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingAddress_Name",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingAddress_Phone",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingAddress_PostCode",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "ShippingAddress_StreetAddress",
                table: "Orders",
                newName: "ShippingAddress");
        }
    }
}
