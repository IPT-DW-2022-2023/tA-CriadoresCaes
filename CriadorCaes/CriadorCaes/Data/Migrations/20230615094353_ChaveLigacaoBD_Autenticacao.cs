using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CriadorCaes.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChaveLigacaoBD_Autenticacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Criadores",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Criadores");
        }
    }
}
