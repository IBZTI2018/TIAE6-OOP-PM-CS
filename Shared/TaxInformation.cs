using ProtoBuf;
using System;
using System.Collections.Generic;

namespace Shared.Structures
{
    [ProtoContract]
    public class YearlyTaxData
    {
        [ProtoMember(1)]
        public int id;

        [ProtoMember(2)]
        // Amount of income the person entered
        public decimal income = 0;

        [ProtoMember(3)]
        // Amount of deductions the person entered
        public decimal deductions = 0;

        [ProtoMember(4)]
        // Amount of capital the person entered
        public decimal capital = 0;

        [ProtoMember(5)]
        // Amount of raw, taxable income
        public decimal taxable
        {
            get
            {
                return this.income - this.deductions;
            }
        }

        [ProtoMember(6)]
        // Amount of tax due after calculation
        public decimal taxdue = 0;

        [ProtoMember(7)]
        // Whether or not data has been approved by inference system
        public bool inferred = false;

        [ProtoMember(8)]
        // Wether or not data has been calculated by tax calculator
        public bool calculated = false;

        [ProtoMember(9)]
        // Whether or not data has been flagged as suspicious
        public bool flagged = false;
    }

    [ProtoContract]
    public class TaxInformation
    {
        [ProtoMember(1)]
        public int id;

        [ProtoMember(2)]
        // Tax information for the previous year
        public YearlyTaxData lastYear;

        [ProtoMember(3)]
        // Tax information for the current year
        public YearlyTaxData thisYear;

        public void fromVariableMap(Dictionary<string, object> input)
        {
            if (this.lastYear == null)
            {
                this.lastYear = new YearlyTaxData();
            }

            if (this.thisYear == null)
            {
                this.thisYear = new YearlyTaxData();
            }

            this.lastYear.income = Convert.ToDecimal(input["vj_einkommen"]);
            this.lastYear.capital = Convert.ToDecimal(input["vj_vermoegen"]);
            this.lastYear.taxdue = Convert.ToDecimal(input["vj_steuersatz"]);
            this.thisYear.income = Convert.ToDecimal(input["vj_einkommen"]);
            this.thisYear.capital = Convert.ToDecimal(input["vj_vermoegen"]);
            this.thisYear.taxdue = Convert.ToDecimal(input["vj_steuersatz"]);

            this.lastYear.inferred = Convert.ToBoolean(input["vj_inferiert"]);
            this.lastYear.flagged = Convert.ToBoolean(input["vj_warnung"]);
            this.thisYear.inferred = Convert.ToBoolean(input["lj_inferiert"]);
            this.thisYear.flagged = Convert.ToBoolean(input["lj_warnung"]);
        }

        public Dictionary<string, object> toVariableMap()
        {
            return new Dictionary<string, object>() {
                {
                  "vj_einkommen",
                  this.lastYear.taxable
                }, {
                  "vj_vermoegen",
                  this.lastYear.capital
                }, {
                  "vj_steuersatz",
                  this.lastYear.taxdue
                }, {
                  "lj_einkommen",
                  this.thisYear.taxable
                }, {
                  "lj_vermoegen",
                  this.thisYear.capital
                }, {
                  "lj_steuersatz",
                  this.thisYear.taxdue
                },

                {
                  "vj_inferiert",
                  this.lastYear.inferred
                }, {
                  "vj_warnung",
                  this.lastYear.flagged
                }, {
                  "lj_inferiert",
                  this.thisYear.inferred
                }, {
                  "lj_warnung",
                  this.thisYear.flagged
                },
              };
        }
    }
}