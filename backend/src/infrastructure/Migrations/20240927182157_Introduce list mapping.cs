using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Introducelistmapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TagTodoItemModel");

            migrationBuilder.AddColumn<int>(
                name: "ListId",
                table: "Todos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TagModelTodoItemModel",
                columns: table => new
                {
                    ItemsId = table.Column<int>(type: "integer", nullable: false),
                    TagsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagModelTodoItemModel", x => new { x.ItemsId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_TagModelTodoItemModel_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TagModelTodoItemModel_Todos_ItemsId",
                        column: x => x.ItemsId,
                        principalTable: "Todos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Todos_ListId",
                table: "Todos",
                column: "ListId");

            migrationBuilder.CreateIndex(
                name: "IX_TagModelTodoItemModel_TagsId",
                table: "TagModelTodoItemModel",
                column: "TagsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Todos_todo_lists_ListId",
                table: "Todos",
                column: "ListId",
                principalTable: "todo_lists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Todos_todo_lists_ListId",
                table: "Todos");

            migrationBuilder.DropTable(
                name: "TagModelTodoItemModel");

            migrationBuilder.DropIndex(
                name: "IX_Todos_ListId",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "ListId",
                table: "Todos");

            migrationBuilder.CreateTable(
                name: "TagTodoItemModel",
                columns: table => new
                {
                    ItemsId = table.Column<int>(type: "integer", nullable: false),
                    TagsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagTodoItemModel", x => new { x.ItemsId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_TagTodoItemModel_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TagTodoItemModel_Todos_ItemsId",
                        column: x => x.ItemsId,
                        principalTable: "Todos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TagTodoItemModel_TagsId",
                table: "TagTodoItemModel",
                column: "TagsId");
        }
    }
}
