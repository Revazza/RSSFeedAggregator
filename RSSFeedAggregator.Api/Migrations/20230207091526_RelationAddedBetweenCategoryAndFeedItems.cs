using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RSSFeedAggregator.Api.Migrations
{
    /// <inheritdoc />
    public partial class RelationAddedBetweenCategoryAndFeedItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryEntity_FeedItems_FeedItemEntityId",
                table: "CategoryEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryEntity",
                table: "CategoryEntity");

            migrationBuilder.DropIndex(
                name: "IX_CategoryEntity_FeedItemEntityId",
                table: "CategoryEntity");

            migrationBuilder.DropColumn(
                name: "FeedItemEntityId",
                table: "CategoryEntity");

            migrationBuilder.RenameTable(
                name: "CategoryEntity",
                newName: "Categories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CategoryFeedItems",
                columns: table => new
                {
                    CategoriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeedItemsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryFeedItems", x => new { x.CategoriesId, x.FeedItemsId });
                    table.ForeignKey(
                        name: "FK_CategoryFeedItems_Categories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryFeedItems_FeedItems_FeedItemsId",
                        column: x => x.FeedItemsId,
                        principalTable: "FeedItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryFeedItems_FeedItemsId",
                table: "CategoryFeedItems",
                column: "FeedItemsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryFeedItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "CategoryEntity");

            migrationBuilder.AddColumn<Guid>(
                name: "FeedItemEntityId",
                table: "CategoryEntity",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryEntity",
                table: "CategoryEntity",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryEntity_FeedItemEntityId",
                table: "CategoryEntity",
                column: "FeedItemEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryEntity_FeedItems_FeedItemEntityId",
                table: "CategoryEntity",
                column: "FeedItemEntityId",
                principalTable: "FeedItems",
                principalColumn: "Id");
        }
    }
}
