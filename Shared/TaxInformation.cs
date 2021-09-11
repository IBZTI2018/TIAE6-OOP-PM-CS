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

        // Amount of raw, taxable income
        public decimal taxable
        {
            get
            {
                return this.income - this.deductions;
            }
        }

        [ProtoMember(5)]
        // Amount of tax due after calculation
        public decimal taxdue = 0;

        [ProtoMember(6)]
        // Whether or not data has been approved by inference system
        public bool inferred = false;

        [ProtoMember(7)]
        // Wether or not data has been calculated by tax calculator
        public bool calculated = false;

        [ProtoMember(8)]
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

        public static TaxInformation fromVariableMap(Dictionary<string, object> input)
        {
            TaxInformation t = new TaxInformation();
            if (t.lastYear == null)
            {
                t.lastYear = new YearlyTaxData();
            }

            if (t.thisYear == null)
            {
                t.thisYear = new YearlyTaxData();
            }

            t.lastYear.income = Convert.ToDecimal(input["vj_einkommen"]);
            t.lastYear.capital = Convert.ToDecimal(input["vj_vermoegen"]);
            t.lastYear.taxdue = Convert.ToDecimal(input["vj_steuersatz"]);
            t.thisYear.income = Convert.ToDecimal(input["vj_einkommen"]);
            t.thisYear.capital = Convert.ToDecimal(input["vj_vermoegen"]);
            t.thisYear.taxdue = Convert.ToDecimal(input["vj_steuersatz"]);

            t.lastYear.inferred = Convert.ToBoolean(input["vj_inferiert"]);
            t.lastYear.flagged = Convert.ToBoolean(input["vj_warnung"]);
            t.thisYear.inferred = Convert.ToBoolean(input["lj_inferiert"]);
            t.thisYear.flagged = Convert.ToBoolean(input["lj_warnung"]);

            return t;
        }

        public Dictionary<string, object> toVariableMap()
        {
            // Use defaults when this is the first tax data from a user
            if (this.lastYear == null) this.lastYear = new YearlyTaxData();

            return new Dictionary<string, object>() {
                {
                  "vj_einkommen",
                  Convert.ToDouble(this.lastYear.taxable)
                }, {
                  "vj_vermoegen",
                  Convert.ToDouble(this.lastYear.capital)
                }, {
                  "vj_steuersatz",
                  Convert.ToDouble(this.lastYear.taxdue)
                }, {
                  "lj_einkommen",
                  Convert.ToDouble(this.thisYear.taxable)
                }, {
                  "lj_vermoegen",
                  Convert.ToDouble(this.thisYear.capital)
                }, {
                  "lj_steuersatz",
                  Convert.ToDouble(this.thisYear.taxdue)
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