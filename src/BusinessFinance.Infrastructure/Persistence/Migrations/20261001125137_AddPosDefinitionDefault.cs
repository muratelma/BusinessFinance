using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessFinance.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPosDefinitionDefault : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Kolon önce nullable eklenir, mevcut satırlar doldurulur, sonra
            // zorunlu yapılır: `defaultValue` ile eklemek tabloda kalıcı bir
            // DEFAULT kısıtı bırakırdı ve şema modelden sessizce ayrışırdı.
            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "PosDefinitions",
                type: "bit",
                nullable: true);

            // Mevcut POS'larda ana POS seçilmemişti; "seçilmedi" bilinen bir
            // değerdir, uydurma değil. Sunucu kimseye ana POS atamaz.
            migrationBuilder.Sql("UPDATE [PosDefinitions] SET [IsDefault] = 0;");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDefault",
                table: "PosDefinitions",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "PosDefinitions");
        }
    }
}
