using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkflowApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitailCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkflowRoutes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RouteName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowRoutes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RouteId = table.Column<int>(type: "int", nullable: false),
                    SequenceNo = table.Column<int>(type: "int", nullable: false),
                    StepName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ParallelGroupId = table.Column<int>(type: "int", nullable: true),
                    ExecutionMode = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    CompletionRule = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    AllowReturn = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ReturnBehavior = table.Column<int>(type: "int", nullable: false, defaultValue: 3),
                    ReturnStepId = table.Column<int>(type: "int", nullable: true),
                    IsFinalStep = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    NextStepCondition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowSteps_WorkflowRoutes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "WorkflowRoutes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkflowSteps_WorkflowSteps_ReturnStepId",
                        column: x => x.ReturnStepId,
                        principalTable: "WorkflowSteps",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkflowStepAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StepId = table.Column<int>(type: "int", nullable: false),
                    AssignmentType = table.Column<int>(type: "int", nullable: false),
                    PositionId = table.Column<int>(type: "int", nullable: true),
                    OUResolutionMode = table.Column<int>(type: "int", nullable: false, defaultValue: 2),
                    OrganizationalUnitId = table.Column<int>(type: "int", nullable: true),
                    NId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowStepAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowStepAssignments_WorkflowSteps_StepId",
                        column: x => x.StepId,
                        principalTable: "WorkflowSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowRoutes_DocumentType",
                table: "WorkflowRoutes",
                column: "DocumentType");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowRoutes_IsActive",
                table: "WorkflowRoutes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowRoutes_IsDeleted",
                table: "WorkflowRoutes",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowRoutes_RouteName_DocumentType",
                table: "WorkflowRoutes",
                columns: new[] { "RouteName", "DocumentType" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStepAssignments_AssignmentType",
                table: "WorkflowStepAssignments",
                column: "AssignmentType");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStepAssignments_NId",
                table: "WorkflowStepAssignments",
                column: "NId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStepAssignments_PositionId",
                table: "WorkflowStepAssignments",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStepAssignments_StepId",
                table: "WorkflowStepAssignments",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowSteps_IsFinalStep",
                table: "WorkflowSteps",
                column: "IsFinalStep");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowSteps_ParallelGroupId",
                table: "WorkflowSteps",
                column: "ParallelGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowSteps_ReturnStepId",
                table: "WorkflowSteps",
                column: "ReturnStepId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowSteps_RouteId",
                table: "WorkflowSteps",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowSteps_RouteId_SequenceNo",
                table: "WorkflowSteps",
                columns: new[] { "RouteId", "SequenceNo" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkflowStepAssignments");

            migrationBuilder.DropTable(
                name: "WorkflowSteps");

            migrationBuilder.DropTable(
                name: "WorkflowRoutes");
        }
    }
}
