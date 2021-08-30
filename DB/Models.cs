using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DB
{
    public interface IBaseModel
    {
        int id { get; set; }
        DateTime createdAt { get; set; }
        DateTime modifiedAt { get; set; }
    }

    public abstract class BaseModel : IBaseModel
    {
        [Key]
        public int id { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime createdAt { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime modifiedAt { get; set; }
    }

    public class Person : BaseModel
    {
        [Required]
        public string firstName { get; set; }
        [Required]
        public string lastName { get; set; }
        public int streetId { get; set; }
        [ForeignKey("streetId")]
        [Required]
        public Street street { get; set; }
        [Required]
        public string streetNumber { get; set; }
    }

    public class Municipality : BaseModel
    {
        public string name { get; set; }
    }

    public class Street : BaseModel
    {
        [Required]
        public string name { get; set; }
        [Required]
        public int postalCode { get; set; }
        public int municipalityId { get; set; }
        [ForeignKey("municipalityId")]
        [Required]
        public Municipality municipality { get; set; }
    }

    public abstract class Rule : BaseModel
    {
        [Required]
        public string rule { get; set; }
        [Required]
        public int order { get; set; }
    }
    public class InferenceRule : Rule { }
    public class EvaluationRule : Rule { }
    public class TaxDeclaration : BaseModel
    {
        public int personId { get; set; }
        [ForeignKey("personId")]
        [Required]
        public Person person { get; set; }
        [Required]
        public int year { get; set; }
        [Required]
        public bool isApproved { get; set; }
        [Required]
        public bool isSent { get; set; }
    }

    public class TaxDeclarationAttribute : BaseModel
    {
        [Required]
        public string name { get; set; }
    }

    public class TaxDeclarationEntry : BaseModel
    {
        public int taxDeclarationId { get; set; }
        [ForeignKey("taxDeclarationId")]
        [Required]
        public TaxDeclaration taxDeclaration { get; set; }
        public int taxDeclarationAttributeId { get; set; }
        [ForeignKey("taxDeclarationAttributeId")]
        [Required]
        public TaxDeclarationAttribute attribute { get; set; }
        [Required]
        public decimal value { get; set; }
        public int createdByRuleId { get; set; }
        [ForeignKey("createdByRuleId")]
        public Rule createdByRule { get; set; }
    }
}