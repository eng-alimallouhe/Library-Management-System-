using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class alterforeignkeyinshipmentstable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shipment_Addresses_AddressId",
                table: "Shipment");

            migrationBuilder.DropForeignKey(
                name: "FK_Shipment_Orders_OrderId",
                table: "Shipment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Shipment",
                table: "Shipment");

            migrationBuilder.RenameTable(
                name: "Shipment",
                newName: "Shipments");

            migrationBuilder.RenameIndex(
                name: "IX_Shipment_OrderId",
                table: "Shipments",
                newName: "IX_Shipments_OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Shipment_AddressId",
                table: "Shipments",
                newName: "IX_Shipments_AddressId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Shipments",
                table: "Shipments",
                column: "ShipmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shipments_Addresses_AddressId",
                table: "Shipments",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "AddressId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Shipments_BaseOrders_OrderId",
                table: "Shipments",
                column: "OrderId",
                principalTable: "BaseOrders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shipments_Addresses_AddressId",
                table: "Shipments");

            migrationBuilder.DropForeignKey(
                name: "FK_Shipments_BaseOrders_OrderId",
                table: "Shipments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Shipments",
                table: "Shipments");

            migrationBuilder.RenameTable(
                name: "Shipments",
                newName: "Shipment");

            migrationBuilder.RenameIndex(
                name: "IX_Shipments_OrderId",
                table: "Shipment",
                newName: "IX_Shipment_OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Shipments_AddressId",
                table: "Shipment",
                newName: "IX_Shipment_AddressId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Shipment",
                table: "Shipment",
                column: "ShipmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shipment_Addresses_AddressId",
                table: "Shipment",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "AddressId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Shipment_Orders_OrderId",
                table: "Shipment",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
