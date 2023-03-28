using Microsoft.EntityFrameworkCore.Migrations;

namespace ArtShareServer.Migrations
{
    public partial class AddedContentType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Images_ContentId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_ContentReports_Images_ContentId",
                table: "ContentReports");

            migrationBuilder.DropForeignKey(
                name: "FK_ContentTag_Images_ContentsId",
                table: "ContentTag");

            migrationBuilder.DropForeignKey(
                name: "FK_Dislikes_Images_ContentId",
                table: "Dislikes");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Users_UserId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Images_ContentId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Images_ContentId",
                table: "Votes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Images",
                table: "Images");

            migrationBuilder.RenameTable(
                name: "Images",
                newName: "Content");

            migrationBuilder.RenameIndex(
                name: "IX_Images_UserId",
                table: "Content",
                newName: "IX_Content_UserId");

            migrationBuilder.AddColumn<int>(
                name: "ContentType",
                table: "Content",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Content",
                table: "Content",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Content_ContentId",
                table: "Comments",
                column: "ContentId",
                principalTable: "Content",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Content_Users_UserId",
                table: "Content",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContentReports_Content_ContentId",
                table: "ContentReports",
                column: "ContentId",
                principalTable: "Content",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContentTag_Content_ContentsId",
                table: "ContentTag",
                column: "ContentsId",
                principalTable: "Content",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Dislikes_Content_ContentId",
                table: "Dislikes",
                column: "ContentId",
                principalTable: "Content",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Content_ContentId",
                table: "Likes",
                column: "ContentId",
                principalTable: "Content",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Content_ContentId",
                table: "Votes",
                column: "ContentId",
                principalTable: "Content",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Content_ContentId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Content_Users_UserId",
                table: "Content");

            migrationBuilder.DropForeignKey(
                name: "FK_ContentReports_Content_ContentId",
                table: "ContentReports");

            migrationBuilder.DropForeignKey(
                name: "FK_ContentTag_Content_ContentsId",
                table: "ContentTag");

            migrationBuilder.DropForeignKey(
                name: "FK_Dislikes_Content_ContentId",
                table: "Dislikes");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Content_ContentId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Content_ContentId",
                table: "Votes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Content",
                table: "Content");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Content");

            migrationBuilder.RenameTable(
                name: "Content",
                newName: "Images");

            migrationBuilder.RenameIndex(
                name: "IX_Content_UserId",
                table: "Images",
                newName: "IX_Images_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Images",
                table: "Images",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Images_ContentId",
                table: "Comments",
                column: "ContentId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContentReports_Images_ContentId",
                table: "ContentReports",
                column: "ContentId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContentTag_Images_ContentsId",
                table: "ContentTag",
                column: "ContentsId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Dislikes_Images_ContentId",
                table: "Dislikes",
                column: "ContentId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Users_UserId",
                table: "Images",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Images_ContentId",
                table: "Likes",
                column: "ContentId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Images_ContentId",
                table: "Votes",
                column: "ContentId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
