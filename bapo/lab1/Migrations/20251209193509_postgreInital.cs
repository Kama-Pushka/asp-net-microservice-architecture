using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace lab1.Migrations
{
    /// <inheritdoc />
    public partial class postgreInital : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IpAddress",
                table: "IpRecords",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");
            
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "IpRecords");
            
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "IpRecords",
                type: "timestamp with time zone",
                nullable: false);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "IpRecords",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IpAddress",
                table: "IpRecords",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
            
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "IpRecords");
            
            migrationBuilder.AddColumn<string>(
                name: "CreatedAt",
                table: "IpRecords",
                type: "TEXT",
                nullable: false);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "IpRecords",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }
    }
}
