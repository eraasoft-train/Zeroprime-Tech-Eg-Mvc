using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Zeroprime.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanyProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Mission = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Vision = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Industry = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ContactEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    FoundingYear = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContactInquiries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Company = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Subject = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Message = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactInquiries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    Category = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    KeyFeatures = table.Column<string>(type: "text", nullable: false),
                    TargetLocations = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiteSettings",
                columns: table => new
                {
                    Key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteSettings", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "SocialMediaLinks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Platform = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Icon = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialMediaLinks", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "CompanyProfiles",
                columns: new[] { "Id", "ContactEmail", "CreatedAt", "Description", "FoundingYear", "Industry", "Location", "Mission", "Name", "UpdatedAt", "Vision" },
                values: new object[] { 1, "zeroprime.tech@gmail.com", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "ZeroPrime Technologies is an Egyptian startup founded in 2017, specializing in smart sustainable cities infrastructure. The company concentrates on autonomous waste management solutions as part of its broader smart city vision.", 2017, "Mechanical or Industrial Engineering / Smart Cities", "Cairo, Egypt", "To design and implement smart city infrastructure that creates the urban environment we all want to live in, aligned with sustainable development goals.", "ZeroPrime Technologies", null, "Sustainable cities powered by autonomous waste management, where recycling is accessible, rewarding and seamless for every citizen." });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "CreatedAt", "Description", "DisplayOrder", "ImageUrl", "IsActive", "KeyFeatures", "Name", "TargetLocations", "UpdatedAt" },
                values: new object[] { 1, "Reverse Vending Machine", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "CanBank is a reverse vending machine (RVM) that motivates people to recycle by exchanging used beverage containers, such as aluminum cans and plastic bottles, for instant rewards such as mobile phone credit or promotional codes. Users can also choose to donate their rewards to charitable organizations. For B2B partners, CanBank doubles as a highly visible promotional platform. Machines are designed for deployment in universities, fitness clubs, shopping malls, supermarkets and other high-traffic public spaces. Collected materials are subsequently recycled, closing the sustainability loop. CanBank represents the first component of a comprehensive waste collection system being developed by ZeroPrime Technologies.", 1, "/images/canbank.svg", true, "[\"Instant mobile phone credit rewards\",\"Promotional codes\",\"Optional charity donations\",\"B2B promotional platform\",\"Accepts aluminum cans and plastic bottles\"]", "CanBank", "[\"Universities\",\"Fitness clubs\",\"Shopping malls\",\"Supermarkets\",\"Public spaces\"]", null });

            migrationBuilder.InsertData(
                table: "SiteSettings",
                columns: new[] { "Key", "Description", "UpdatedAt", "Value" },
                values: new object[,]
                {
                    { "ContactEmail", "Primary contact email shown on website", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "zeroprime.tech@gmail.com" },
                    { "HeroSubtitle", "Hero section subtitle", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Autonomous waste management solutions that reward recycling and power a sustainable future." },
                    { "HeroTitle", "Hero section main heading", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Building Smarter, Cleaner Cities" },
                    { "SiteDescription", "Meta description used for SEO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Smart Sustainable Cities Infrastructure" },
                    { "SiteName", "Website name displayed in browser tab", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "ZeroPrime Technologies" }
                });

            migrationBuilder.InsertData(
                table: "SocialMediaLinks",
                columns: new[] { "Id", "CreatedAt", "DisplayOrder", "Icon", "IsActive", "Platform", "UpdatedAt", "Url" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "bi-linkedin", true, "LinkedIn", null, "https://www.linkedin.com/company/zeroprime-technologies" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "bi-instagram", true, "Instagram", null, "https://www.instagram.com/zeroprime.tech/" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "bi-facebook", true, "Facebook", null, "https://www.facebook.com/zeroprime.tech" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProfiles_ContactEmail",
                table: "CompanyProfiles",
                column: "ContactEmail");

            migrationBuilder.CreateIndex(
                name: "IX_ContactInquiries_Email",
                table: "ContactInquiries",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_ContactInquiries_Status",
                table: "ContactInquiries",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ContactInquiries_SubmittedAt",
                table: "ContactInquiries",
                column: "SubmittedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Category",
                table: "Products",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Products_DisplayOrder",
                table: "Products",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Products_IsActive",
                table: "Products",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_SiteSettings_Key",
                table: "SiteSettings",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SocialMediaLinks_DisplayOrder",
                table: "SocialMediaLinks",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_SocialMediaLinks_IsActive",
                table: "SocialMediaLinks",
                column: "IsActive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyProfiles");

            migrationBuilder.DropTable(
                name: "ContactInquiries");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "SiteSettings");

            migrationBuilder.DropTable(
                name: "SocialMediaLinks");
        }
    }
}
