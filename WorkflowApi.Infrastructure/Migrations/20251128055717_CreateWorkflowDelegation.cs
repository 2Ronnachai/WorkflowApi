using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkflowApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateWorkflowDelegation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkflowDelegations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Delegator = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Delegatee = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Scope = table.Column<int>(type: "int", nullable: false),
                    RouteId = table.Column<int>(type: "int", nullable: true),
                    DocumentType = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StepId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowDelegations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowDelegations_WorkflowRoutes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "WorkflowRoutes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowDelegations_WorkflowSteps_StepId",
                        column: x => x.StepId,
                        principalTable: "WorkflowSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowDelegatee",
                table: "WorkflowDelegations",
                column: "Delegatee");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowDelegations_RouteId",
                table: "WorkflowDelegations",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowDelegations_StepId",
                table: "WorkflowDelegations",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowDelegator_IsActive_DateRange",
                table: "WorkflowDelegations",
                columns: new[] { "Delegator", "IsActive", "StartDate", "EndDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkflowDelegations");
        }
    }
}
