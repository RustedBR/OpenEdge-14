using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Content.Server.Database.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class OE14StatModifierSources : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Strength modifiers (by source)
            migrationBuilder.AddColumn<int>(
                name: "oe14_str_mod_spell",
                table: "profile",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "oe14_str_mod_spend",
                table: "profile",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "oe14_str_mod_item",
                table: "profile",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "oe14_str_mod_buff",
                table: "profile",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // Vitality modifiers (by source)
            migrationBuilder.AddColumn<int>(
                name: "oe14_vit_mod_spell",
                table: "profile",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "oe14_vit_mod_spend",
                table: "profile",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "oe14_vit_mod_item",
                table: "profile",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "oe14_vit_mod_buff",
                table: "profile",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // Dexterity modifiers (by source)
            migrationBuilder.AddColumn<int>(
                name: "oe14_dex_mod_spell",
                table: "profile",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "oe14_dex_mod_spend",
                table: "profile",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "oe14_dex_mod_item",
                table: "profile",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "oe14_dex_mod_buff",
                table: "profile",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // Intelligence modifiers (by source)
            migrationBuilder.AddColumn<int>(
                name: "oe14_int_mod_spell",
                table: "profile",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "oe14_int_mod_spend",
                table: "profile",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "oe14_int_mod_item",
                table: "profile",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "oe14_int_mod_buff",
                table: "profile",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Strength modifiers
            migrationBuilder.DropColumn(name: "oe14_str_mod_spell", table: "profile");
            migrationBuilder.DropColumn(name: "oe14_str_mod_spend", table: "profile");
            migrationBuilder.DropColumn(name: "oe14_str_mod_item", table: "profile");
            migrationBuilder.DropColumn(name: "oe14_str_mod_buff", table: "profile");

            // Vitality modifiers
            migrationBuilder.DropColumn(name: "oe14_vit_mod_spell", table: "profile");
            migrationBuilder.DropColumn(name: "oe14_vit_mod_spend", table: "profile");
            migrationBuilder.DropColumn(name: "oe14_vit_mod_item", table: "profile");
            migrationBuilder.DropColumn(name: "oe14_vit_mod_buff", table: "profile");

            // Dexterity modifiers
            migrationBuilder.DropColumn(name: "oe14_dex_mod_spell", table: "profile");
            migrationBuilder.DropColumn(name: "oe14_dex_mod_spend", table: "profile");
            migrationBuilder.DropColumn(name: "oe14_dex_mod_item", table: "profile");
            migrationBuilder.DropColumn(name: "oe14_dex_mod_buff", table: "profile");

            // Intelligence modifiers
            migrationBuilder.DropColumn(name: "oe14_int_mod_spell", table: "profile");
            migrationBuilder.DropColumn(name: "oe14_int_mod_spend", table: "profile");
            migrationBuilder.DropColumn(name: "oe14_int_mod_item", table: "profile");
            migrationBuilder.DropColumn(name: "oe14_int_mod_buff", table: "profile");
        }
    }
}
