using System;
using System.Collections.Generic;

namespace Shared.Structures {
  class YearlyTaxData {
    // Amount of income the person entered
    public double income = 0;

    // Amount of deductions the person entered
    public double deductions = 0;

    // Amount of capital the person entered
    public double capital = 0;

    // Amount of raw, taxable income
    public double taxable
    {
      get
      {
        return this.income - this.deductions;
      }
    }

    // Amount of tax due after calculation
    public double taxdue = 0;

    // Whether or not data has been approved by inference system
    public bool inferred = false;

    // Whether or not data has been flagged as suspicious
    public bool flagged = false;
  }

  class TaxInformation {
    // Tax information for the previous year
    YearlyTaxData lastYear;

    // Tax information for the current year
    YearlyTaxData thisYear;

    public TaxInformation()
    {
      this.lastYear = new YearlyTaxData();
      this.thisYear = new YearlyTaxData();
    }

    public void fromVariableMap(Dictionary<string, object> input)
    {
      this.lastYear.income = Convert.ToDouble(input["vj_einkommen"]);
      this.lastYear.capital = Convert.ToDouble(input["vj_vermoegen"]);
      this.lastYear.taxdue = Convert.ToDouble(input["vj_steuersatz"]);
      this.thisYear.income = Convert.ToDouble(input["vj_einkommen"]);
      this.thisYear.capital = Convert.ToDouble(input["vj_vermoegen"]);
      this.thisYear.taxdue = Convert.ToDouble(input["vj_steuersatz"]);

      this.lastYear.inferred = Convert.ToBoolean(input["vj_inferiert"]);
      this.lastYear.flagged = Convert.ToBoolean(input["vj_warnung"]);
      this.thisYear.inferred = Convert.ToBoolean(input["lj_inferiert"]);
      this.thisYear.flagged = Convert.ToBoolean(input["lj_warnung"]);
    }

    public Dictionary<string, object> toVariableMap()
    {
      return new Dictionary<string, object>()
      {
        { "vj_einkommen", this.lastYear.taxable },
        { "vj_vermoegen", this.lastYear.capital },
        { "vj_steuersatz", this.lastYear.taxdue },
        { "lj_einkommen", this.thisYear.taxable },
        { "lj_vermoegen", this.thisYear.capital },
        { "lj_steuersatz", this.thisYear.taxdue },

        { "vj_inferiert", this.lastYear.inferred },
        { "vj_warnung", this.lastYear.flagged},
        { "lj_inferiert", this.thisYear.inferred },
        { "lj_warnung", this.thisYear.flagged},
      };
    }
  }
}
