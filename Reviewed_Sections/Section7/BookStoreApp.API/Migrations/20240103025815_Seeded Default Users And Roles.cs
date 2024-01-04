using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookStoreApp.API.Migrations
{
    /// <inheritdoc />
    public partial class SeededDefaultUsersAndRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "70b4f51c-f566-4ea9-aa48-612759bbafec", null, "User", "USER" },
                    { "78a9a3b6-9647-49a1-b953-49bd82a175e6", null, "Administrator", "ADMINISTRATOR" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "10851584-7191-4dee-a2b5-305ae30a9777", 0, "e3d27776-8d83-49c0-b66b-70a477725363", "user@bookstore.com", false, "System", "User", false, null, "USER@BOOKSTORE.COM", "USER@BOOKSTORE.COM", "AQAAAAIAAYagAAAAEJpyG39i6E2xg9yzhTvmvl7KtANaE/nnCIkCXBXrrACbgfZBr1Xmq82xli293yCGhg==", null, false, "d0cc246b-a64c-400d-a032-27499b229ef9", false, "user@bookstore.com" },
                    { "8383f68f-764c-4427-92b7-6e98902f5f4e", 0, "3248b9b1-e9b5-44c8-9c38-316c2b383b88", "admin@bookstore.com", false, "System", "Admin", false, null, "ADMIN@BOOKSTORE.COM", "ADMIN@BOOKSTORE.COM", "AQAAAAIAAYagAAAAEJeyiRmZjD9n1etV8Tkp5nHH4HgzwqtcW+Hpgs/aWmmWy7ALerIkc2l+i7BueuYbPw==", null, false, "8b6488d7-4df3-40f4-b477-73e36faf9d0f", false, "admin@bookstore.com" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "78a9a3b6-9647-49a1-b953-49bd82a175e6", "10851584-7191-4dee-a2b5-305ae30a9777" },
                    { "70b4f51c-f566-4ea9-aa48-612759bbafec", "8383f68f-764c-4427-92b7-6e98902f5f4e" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "78a9a3b6-9647-49a1-b953-49bd82a175e6", "10851584-7191-4dee-a2b5-305ae30a9777" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "70b4f51c-f566-4ea9-aa48-612759bbafec", "8383f68f-764c-4427-92b7-6e98902f5f4e" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "70b4f51c-f566-4ea9-aa48-612759bbafec");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "78a9a3b6-9647-49a1-b953-49bd82a175e6");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "10851584-7191-4dee-a2b5-305ae30a9777");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8383f68f-764c-4427-92b7-6e98902f5f4e");
        }
    }
}
