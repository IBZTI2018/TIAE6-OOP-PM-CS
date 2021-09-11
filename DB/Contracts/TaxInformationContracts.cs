using Microsoft.EntityFrameworkCore;
using System.Linq;
using Shared.Contracts;
using Shared.Structures;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Models;

namespace DB.Contracts
{
    public class TaxInformationService : ITaxInformationService
    {
        YearlyTaxData buildTaxData(ICollection<TaxDeclarationEntry> entries)
        {
            YearlyTaxData taxData = new YearlyTaxData();
            List<TaxDeclarationAttribute> allAttributes;
            using (var ctx = new TIAE6Context())
            {
                allAttributes = ctx.taxDeclarationAttributes.ToList();
            }

            foreach (var entry in entries)
            {
                taxData.id = entry.taxDeclarationId;

                if (entry.attribute == allAttributes.First(x => x.name == "Income"))
                {
                    taxData.income = entry.value;
                }

                if (entry.attribute == allAttributes.First(x => x.name == "Deductions"))
                {
                    taxData.deductions = entry.value;
                }

                if (entry.attribute == allAttributes.First(x => x.name == "TaxDue"))
                {
                    taxData.taxdue = entry.value;
                }

                if (entry.attribute == allAttributes.First(x => x.name == "Inferred"))
                {
                    taxData.inferred = entry.value == 1;
                }

                if (entry.attribute == allAttributes.First(x => x.name == "Calculated"))
                {
                    taxData.calculated = entry.value == 1;
                }

                if (entry.attribute == allAttributes.First(x => x.name == "Suspicious"))
                {
                    taxData.flagged = entry.value == 1;
                }
            }
            return taxData;
        }
        TaxInformation buildTaxInformation(TaxDeclaration thisYearDeclaration, TaxDeclaration? lastYearDeclaration)
        {
            TaxInformation info = new TaxInformation();
            info.id = thisYearDeclaration.id;
            info.thisYear = buildTaxData(thisYearDeclaration.Entries);
            if (lastYearDeclaration != null)
            {
                info.lastYear = buildTaxData(lastYearDeclaration.Entries);
            }
            return info;
        }

        TaxInformationResponse buildTaxInformationResponse(bool checkInferred = false, bool checkCalculated = false)
        {
            TaxDeclaration thisYearDeclaration;
            using (var ctx = new TIAE6Context())
            {
                thisYearDeclaration = ctx.taxDeclarations
                    .Include("Entries")
                    .FirstOrDefault(x => x.isApproved == false || x.isSent == false);
            }

            TaxInformation returnObj = new TaxInformation();

            if (thisYearDeclaration != null)
            {
                TaxDeclaration lastDeclaration;
                using (var ctx = new TIAE6Context())
                {
                    lastDeclaration = ctx.taxDeclarations.Include("Entries").FirstOrDefault(x => x.year == thisYearDeclaration.year - 1);
                }

                var newTaxInformation = buildTaxInformation(thisYearDeclaration, lastDeclaration);

                bool addWork = true;

                if (checkInferred)
                {
                    if (newTaxInformation.thisYear.inferred == true)
                    {
                        addWork = false;
                    }
                }

                if (checkCalculated)
                {
                    if (newTaxInformation.thisYear.calculated == true)
                    {
                        addWork = false;
                    }
                }

                if (addWork)
                {
                    returnObj = newTaxInformation;
                }
            }

            return new TaxInformationResponse { taxInformation = returnObj };
        }

        public ValueTask<TaxInformationResponse> getNonCalculatedWork(EmptyRequest request)
        {
            return new ValueTask<TaxInformationResponse>(buildTaxInformationResponse(checkCalculated: true));
        }

        public ValueTask<TaxInformationResponse> getNonInferredWork(EmptyRequest request)
        {
            return new ValueTask<TaxInformationResponse>(buildTaxInformationResponse(checkInferred: true));
        }

        public ValueTask<BoolResponse> putTaxData(YearlyTaxDataRequest request)
        {
            TaxDeclaration declaration;
            List<TaxDeclarationAttribute> allAttributes;
            YearlyTaxData taxData = request.taxData;
            using (var ctx = new TIAE6Context())
            {
                declaration = ctx.taxDeclarations.Include("Entries").FirstOrDefault(x => x.id == request.taxData.id);
                allAttributes = ctx.taxDeclarationAttributes.ToList();

                foreach (var entry in declaration.Entries)
                {
                    if (entry.attribute == allAttributes.First(x => x.name == "Income"))
                    {
                        entry.value = taxData.income;
                    }

                    if (entry.attribute == allAttributes.First(x => x.name == "Deductions"))
                    {
                        entry.value = taxData.deductions;
                    }

                    if (entry.attribute == allAttributes.First(x => x.name == "TaxDue"))
                    {
                        entry.value = taxData.taxdue;
                    }

                    if (entry.attribute == allAttributes.First(x => x.name == "Inferred"))
                    {
                        entry.value = taxData.inferred ? 1 : 0;
                    }

                    if (entry.attribute == allAttributes.First(x => x.name == "Calculated"))
                    {
                        entry.value = taxData.calculated ? 1 : 0;
                    }

                    if (entry.attribute == allAttributes.First(x => x.name == "Suspicious"))
                    {
                        entry.value = taxData.flagged ? 1 : 0;
                    }
                }

                declaration.isSent = true;
                ctx.SaveChanges();
            }

            return new ValueTask<BoolResponse>(new BoolResponse { success = true });
        }
    }
}
