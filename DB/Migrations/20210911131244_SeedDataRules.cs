using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace DB.Migrations
{
    public partial class SeedDataRules : Migration
    {
        private DateTime fakeDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified);

        protected override void Up(MigrationBuilder migrationBuilder)
        {
              migrationBuilder.InsertData(
              table: "Rule",
              columns: new[] { "id", "parentId", "Discriminator", "rule", "createdAt", "modifiedAt", "condition", "transformation" },
              values: new object[,]
              {
                  { 1, null, "InferenceRule", "Inferenz-Root", this.fakeDate, this.fakeDate, "true", "" },
                  { 2, 1, "InferenceRule", "Fall 5", this.fakeDate, this.fakeDate, "lj_vermoegen == null || lj_vermoegen == 0", "lj_vermoegen = lj_einkommen * 0.75" },

                  { 3, null, "EvaluationRule", "Evaluation-Root", this.fakeDate, this.fakeDate, "true", "" },
                  { 4, 3, "EvaluationRule", "Fall 4", this.fakeDate, this.fakeDate, "vj_einkommen > (1.5* lj_einkommen) && vj_einkommen > 100000", "lj_warnung = true" },
                  { 5, 3, "EvaluationRule", "Knoten", this.fakeDate, this.fakeDate, "true", "" },
                  { 6, 5, "EvaluationRule", "VM/EK < 1", this.fakeDate, this.fakeDate, "(lj_vermoegen / lj_einkommen) < 1", "" },
                  { 7, 6, "EvaluationRule", "Fall 1", this.fakeDate, this.fakeDate, "lj_einkommen < 80000", "lj_steuersatz = lj_einkommen * 0.25" },
                  { 8, 6, "EvaluationRule", "Fall 3", this.fakeDate, this.fakeDate, "lj_einkommen > 100000", "lj_warnung = true"},
                  { 9, 5, "EvaluationRule", "VM/EK > 1", this.fakeDate, this.fakeDate, "(lj_vermoegen / lj_einkommen) > 1", "" },
                  { 10, 9, "EvaluationRule", "Fall 2", this.fakeDate, this.fakeDate, "lj_einkommen > 100000", "lj_steuersatz = lj_einkommen * 0.35" }
              });
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
