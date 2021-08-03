namespace DB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class baseERM : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Rules",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        rule = c.String(nullable: false),
                        order = c.Int(nullable: false),
                        createdAt = c.DateTime(nullable: false),
                        modifiedAt = c.DateTime(nullable: false),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Municipalities",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        createdAt = c.DateTime(nullable: false),
                        modifiedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.People",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        firstName = c.String(nullable: false),
                        lastName = c.String(nullable: false),
                        streetNumber = c.String(nullable: false),
                        createdAt = c.DateTime(nullable: false),
                        modifiedAt = c.DateTime(nullable: false),
                        street_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Streets", t => t.street_id, cascadeDelete: true)
                .Index(t => t.street_id);
            
            CreateTable(
                "dbo.Streets",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false),
                        postalCode = c.Int(nullable: false),
                        createdAt = c.DateTime(nullable: false),
                        modifiedAt = c.DateTime(nullable: false),
                        municipality_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Municipalities", t => t.municipality_id, cascadeDelete: true)
                .Index(t => t.municipality_id);
            
            CreateTable(
                "dbo.TaxDeclarationAttributes",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false),
                        createdAt = c.DateTime(nullable: false),
                        modifiedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.TaxDeclarationEntries",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        createdAt = c.DateTime(nullable: false),
                        modifiedAt = c.DateTime(nullable: false),
                        attribute_id = c.Int(nullable: false),
                        createdByRule_id = c.Int(),
                        taxDeclaration_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.TaxDeclarationAttributes", t => t.attribute_id, cascadeDelete: true)
                .ForeignKey("dbo.Rules", t => t.createdByRule_id)
                .ForeignKey("dbo.TaxDeclarations", t => t.taxDeclaration_id, cascadeDelete: true)
                .Index(t => t.attribute_id)
                .Index(t => t.createdByRule_id)
                .Index(t => t.taxDeclaration_id);
            
            CreateTable(
                "dbo.TaxDeclarations",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        year = c.Int(nullable: false),
                        isApproved = c.Boolean(nullable: false),
                        isSent = c.Boolean(nullable: false),
                        createdAt = c.DateTime(nullable: false),
                        modifiedAt = c.DateTime(nullable: false),
                        person_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.People", t => t.person_id, cascadeDelete: true)
                .Index(t => t.person_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TaxDeclarationEntries", "taxDeclaration_id", "dbo.TaxDeclarations");
            DropForeignKey("dbo.TaxDeclarations", "person_id", "dbo.People");
            DropForeignKey("dbo.TaxDeclarationEntries", "createdByRule_id", "dbo.Rules");
            DropForeignKey("dbo.TaxDeclarationEntries", "attribute_id", "dbo.TaxDeclarationAttributes");
            DropForeignKey("dbo.People", "street_id", "dbo.Streets");
            DropForeignKey("dbo.Streets", "municipality_id", "dbo.Municipalities");
            DropIndex("dbo.TaxDeclarations", new[] { "person_id" });
            DropIndex("dbo.TaxDeclarationEntries", new[] { "taxDeclaration_id" });
            DropIndex("dbo.TaxDeclarationEntries", new[] { "createdByRule_id" });
            DropIndex("dbo.TaxDeclarationEntries", new[] { "attribute_id" });
            DropIndex("dbo.Streets", new[] { "municipality_id" });
            DropIndex("dbo.People", new[] { "street_id" });
            DropTable("dbo.TaxDeclarations");
            DropTable("dbo.TaxDeclarationEntries");
            DropTable("dbo.TaxDeclarationAttributes");
            DropTable("dbo.Streets");
            DropTable("dbo.People");
            DropTable("dbo.Municipalities");
            DropTable("dbo.Rules");
        }
    }
}
