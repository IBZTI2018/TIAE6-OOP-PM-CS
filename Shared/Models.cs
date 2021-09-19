using ProtoBuf;
using Shared.Structures;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Shared.Models
{
    public interface IBaseModel
    {
        public int id { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime modifiedAt { get; set; }
    }

    [ProtoContract]
    [ProtoInclude(10, typeof(Person))]
    [ProtoInclude(20, typeof(Municipality))]
    [ProtoInclude(30, typeof(Street))]
    [ProtoInclude(40, typeof(Rule))]
    [ProtoInclude(50, typeof(TaxDeclaration))]
    [ProtoInclude(60, typeof(TaxDeclarationAttribute))]
    [ProtoInclude(70, typeof(TaxDeclarationEntry))]
    public abstract class BaseModel : IBaseModel
    {
        [Key]
        [ProtoMember(1)]
        public int id { get; set; }

        [DataType(DataType.DateTime)]
        [ProtoMember(2)]
        public DateTime createdAt { get; set; }

        [DataType(DataType.DateTime)]
        [ProtoMember(3)]
        public DateTime modifiedAt { get; set; }
    }

    [ProtoContract]
    public class Person : BaseModel
    {
        [Required]
        [ProtoMember(1)]
        public string firstName { get; set; }
        [Required]
        [ProtoMember(2)]
        public string lastName { get; set; }
        [ProtoMember(3)]
        public int streetId { get; set; }
        [ForeignKey("streetId")]
        [Required]
        [ProtoMember(4)]
        public Street street { get; set; }
        [Required]
        [ProtoMember(5)]
        public string streetNumber { get; set; }

        public string fullName
        {
            get
            {
                return this.firstName + ' ' + this.lastName;
            }
        }
    }

    [ProtoContract]
    public class Municipality : BaseModel
    {
        [ProtoMember(1)]
        public string name { get; set; }
    }

    [ProtoContract]
    public class Street : BaseModel
    {
        [Required]
        [ProtoMember(1)]
        public string name { get; set; }
        [Required]
        [ProtoMember(2)]
        public int postalCode { get; set; }
        [ProtoMember(3)]
        public int municipalityId { get; set; }
        [ForeignKey("municipalityId")]
        [Required]
        [ProtoMember(4)]
        public Municipality municipality { get; set; }
    }

    [ProtoContract]
    [ProtoInclude(100, typeof(InferenceRule))]
    [ProtoInclude(200, typeof(EvaluationRule))]
    public abstract class Rule : BaseModel
    {
        [Required]
        [ProtoMember(1)]
        public string rule { get; set; }

        [ProtoMember(2)]
        public int? parentId { get; set; }
        [ForeignKey("parentId")]
        [ProtoMember(3)]
        public Rule parent { get; set; }
        [Required]
        [ProtoMember(4)]
        public string condition { get; set; }
        [Required]
        [ProtoMember(5)]
        public string transformation { get; set; }
        [Required]
        [ProtoMember(6)]
        public bool active { get; set; }
    }
    [ProtoContract]
    public class InferenceRule : Rule { }
    [ProtoContract]
    public class EvaluationRule : Rule { }
    [ProtoContract]
    public class TaxDeclaration : BaseModel
    {
        public TaxDeclaration()
        {
            Entries = new HashSet<TaxDeclarationEntry>();
        }

        [ProtoMember(1)]
        public int personId { get; set; }
        [ForeignKey("personId")]
        [Required]
        [ProtoMember(2)]
        public Person person { get; set; }
        [Required]
        [ProtoMember(3)]
        public int year { get; set; }
        [Required]
        [ProtoMember(4)]
        public bool isApproved { get; set; }
        [Required]
        [ProtoMember(5)]
        public bool isSent { get; set; }

        [InverseProperty("taxDeclaration")]
        public virtual ICollection<TaxDeclarationEntry> Entries { get; set; }

        [NotMapped]
        [ProtoMember(6)]
        public bool isInferred { get; set; }
        [NotMapped]
        [ProtoMember(7)]
        public bool isCalculated { get; set; }
        [NotMapped]
        [ProtoMember(8)]
        public decimal Income { get; set; }
        [NotMapped]
        [ProtoMember(9)]
        public decimal Deductions { get; set; }
        [NotMapped]
        [ProtoMember(10)]
        public decimal TaxDue { get; set; }
        [NotMapped]
        [ProtoMember(11)]
        public decimal Capital { get; set; }
        [NotMapped]
        [ProtoMember(12)]
        public bool Suspicious { get; set; }

        public bool getIsInferred()
        {
            if (this.Entries.Count > 0)
            {
                bool localIsInferred = false;
                foreach (var entry in this.Entries)
                {
                    if (entry.attribute.name == "Inferred")
                    {
                        localIsInferred = entry.value == 1;
                    }
                }
                return localIsInferred;
            }
            return false;
        }
        public bool getIsCalculated()
        {
            if (this.Entries.Count > 0)
            {
                bool localIsCalculated = false;
                foreach (var entry in this.Entries)
                {
                    if (entry.attribute.name == "Calculated")
                    {
                        localIsCalculated = entry.value == 1;
                    }
                }
                return localIsCalculated;
            }
            return false;
        }
    }

    [ProtoContract]
    public class TaxDeclarationAttribute : BaseModel
    {
        [Required]
        [ProtoMember(1)]
        public string name { get; set; }
    }

    [ProtoContract]
    public class TaxDeclarationEntry : BaseModel
    {

        [ProtoMember(1)]
        public int taxDeclarationId { get; set; }
        [ForeignKey("taxDeclarationId")]
        [Required]
        [ProtoMember(2)]
        public TaxDeclaration taxDeclaration { get; set; }
        [ProtoMember(3)]
        public int taxDeclarationAttributeId { get; set; }
        [ForeignKey("taxDeclarationAttributeId")]
        [Required]
        [ProtoMember(4)]
        public TaxDeclarationAttribute attribute { get; set; }
        [Required]
        [ProtoMember(5)]
        public decimal value { get; set; }
        [ProtoMember(6)]
        public int? createdByRuleId { get; set; }
        [ForeignKey("createdByRuleId")]
        [ProtoMember(7)]
        public Rule createdByRule { get; set; }
    }
}