using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Bit.Core.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Device",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<byte>(nullable: false),
                    Identifier = table.Column<string>(nullable: true),
                    PushToken = table.Column<string>(nullable: true),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    RevisionDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Device", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Grant",
                columns: table => new
                {
                    Key = table.Column<string>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    SubjectId = table.Column<string>(nullable: true),
                    SessionId = table.Column<string>(nullable: true),
                    ClientId = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    ExpirationDate = table.Column<DateTime>(nullable: true),
                    ConsumedDate = table.Column<DateTime>(nullable: true),
                    Data = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grant", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Identifier = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    BusinessName = table.Column<string>(nullable: true),
                    BusinessAddress1 = table.Column<string>(nullable: true),
                    BusinessAddress2 = table.Column<string>(nullable: true),
                    BusinessAddress3 = table.Column<string>(nullable: true),
                    BusinessCountry = table.Column<string>(nullable: true),
                    BusinessTaxNumber = table.Column<string>(nullable: true),
                    BillingEmail = table.Column<string>(nullable: true),
                    Plan = table.Column<string>(nullable: true),
                    PlanType = table.Column<byte>(nullable: false),
                    Seats = table.Column<short>(nullable: true),
                    MaxCollections = table.Column<short>(nullable: true),
                    UsePolicies = table.Column<bool>(nullable: false),
                    UseSso = table.Column<bool>(nullable: false),
                    UseGroups = table.Column<bool>(nullable: false),
                    UseDirectory = table.Column<bool>(nullable: false),
                    UseEvents = table.Column<bool>(nullable: false),
                    UseTotp = table.Column<bool>(nullable: false),
                    Use2fa = table.Column<bool>(nullable: false),
                    UseApi = table.Column<bool>(nullable: false),
                    SelfHost = table.Column<bool>(nullable: false),
                    UsersGetPremium = table.Column<bool>(nullable: false),
                    Storage = table.Column<long>(nullable: true),
                    MaxStorageGb = table.Column<short>(nullable: true),
                    Gateway = table.Column<byte>(nullable: true),
                    GatewayCustomerId = table.Column<string>(nullable: true),
                    GatewaySubscriptionId = table.Column<string>(nullable: true),
                    ReferenceData = table.Column<string>(nullable: true),
                    Enabled = table.Column<bool>(nullable: false),
                    LicenseKey = table.Column<string>(nullable: true),
                    ApiKey = table.Column<string>(nullable: true),
                    TwoFactorProviders = table.Column<string>(nullable: true),
                    ExpirationDate = table.Column<DateTime>(nullable: true),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    RevisionDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organization", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    EmailVerified = table.Column<bool>(nullable: false),
                    MasterPassword = table.Column<string>(nullable: true),
                    MasterPasswordHint = table.Column<string>(nullable: true),
                    Culture = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    TwoFactorProviders = table.Column<string>(nullable: true),
                    TwoFactorRecoveryCode = table.Column<string>(nullable: true),
                    EquivalentDomains = table.Column<string>(nullable: true),
                    ExcludedGlobalEquivalentDomains = table.Column<string>(nullable: true),
                    AccountRevisionDate = table.Column<DateTime>(nullable: false),
                    Key = table.Column<string>(nullable: true),
                    PublicKey = table.Column<string>(nullable: true),
                    PrivateKey = table.Column<string>(nullable: true),
                    Premium = table.Column<bool>(nullable: false),
                    PremiumExpirationDate = table.Column<DateTime>(nullable: true),
                    RenewalReminderDate = table.Column<DateTime>(nullable: true),
                    Storage = table.Column<long>(nullable: true),
                    MaxStorageGb = table.Column<short>(nullable: true),
                    Gateway = table.Column<byte>(nullable: true),
                    GatewayCustomerId = table.Column<string>(nullable: true),
                    GatewaySubscriptionId = table.Column<string>(nullable: true),
                    ReferenceData = table.Column<string>(nullable: true),
                    LicenseKey = table.Column<string>(nullable: true),
                    ApiKey = table.Column<string>(nullable: true),
                    Kdf = table.Column<byte>(nullable: false),
                    KdfIterations = table.Column<int>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    RevisionDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Collection",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    OrganizationId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ExternalId = table.Column<string>(nullable: true),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    RevisionDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Collection_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cipher",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: true),
                    OrganizationId = table.Column<Guid>(nullable: true),
                    Type = table.Column<byte>(nullable: false),
                    Data = table.Column<string>(nullable: true),
                    Favorites = table.Column<string>(nullable: true),
                    Folders = table.Column<string>(nullable: true),
                    Attachments = table.Column<string>(nullable: true),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    RevisionDate = table.Column<DateTime>(nullable: false),
                    DeletedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cipher", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cipher_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cipher_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationUser",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    OrganizationId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Key = table.Column<string>(nullable: true),
                    Status = table.Column<byte>(nullable: false),
                    Type = table.Column<byte>(nullable: false),
                    AccessAll = table.Column<bool>(nullable: false),
                    ExternalId = table.Column<string>(nullable: true),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    RevisionDate = table.Column<DateTime>(nullable: false),
                    Permissions = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationUser_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationUser_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CollectionCipher",
                columns: table => new
                {
                    CollectionId = table.Column<Guid>(nullable: false),
                    CipherId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_CollectionCipher_Cipher_CipherId",
                        column: x => x.CipherId,
                        principalTable: "Cipher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollectionCipher_Collection_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CollectionUser",
                columns: table => new
                {
                    CollectionId = table.Column<Guid>(nullable: false),
                    OrganizationUserId = table.Column<Guid>(nullable: false),
                    ReadOnly = table.Column<bool>(nullable: false),
                    HidePasswords = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_CollectionUser_Collection_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollectionUser_OrganizationUser_OrganizationUserId",
                        column: x => x.OrganizationUserId,
                        principalTable: "OrganizationUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cipher_OrganizationId",
                table: "Cipher",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Cipher_UserId",
                table: "Cipher",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Collection_OrganizationId",
                table: "Collection",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionCipher_CipherId",
                table: "CollectionCipher",
                column: "CipherId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionCipher_CollectionId",
                table: "CollectionCipher",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionUser_CollectionId",
                table: "CollectionUser",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionUser_OrganizationUserId",
                table: "CollectionUser",
                column: "OrganizationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUser_OrganizationId",
                table: "OrganizationUser",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUser_UserId",
                table: "OrganizationUser",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CollectionCipher");

            migrationBuilder.DropTable(
                name: "CollectionUser");

            migrationBuilder.DropTable(
                name: "Device");

            migrationBuilder.DropTable(
                name: "Grant");

            migrationBuilder.DropTable(
                name: "Cipher");

            migrationBuilder.DropTable(
                name: "Collection");

            migrationBuilder.DropTable(
                name: "OrganizationUser");

            migrationBuilder.DropTable(
                name: "Organization");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
