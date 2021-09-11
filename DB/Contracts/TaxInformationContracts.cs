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

        List<TaxInformation> buildTaxInformationList()
        {
            IQueryable<TaxDeclaration> declarations;
            using (var ctx = new TIAE6Context())
            {
                declarations = ctx.taxDeclarations
                    .Include(x => x.person)
                    .Where(x => x.isApproved == false || x.isSent == false);
            }

            var work = new List<TaxInformation>();

            foreach (var declaration in declarations)
            {
                TaxDeclaration lastDeclaration;
                using (var ctx = new TIAE6Context())
                {
                    lastDeclaration = ctx.taxDeclarations.First(x => x.year == declaration.year - 1);
                }
                work.Add(buildTaxInformation(declaration, lastDeclaration));
            }

            return work;
        }

        public ValueTask<TaxInformationListResponse> getNonCalculatedWork(EmptyRequest request)
        {
            var work = buildTaxInformationList();
            var response = new TaxInformationListResponse();
            foreach (var task in work)
            {
                response.taxInformationList.Add(task);
            }
            return new ValueTask<TaxInformationListResponse>(response);
        }

        public ValueTask<TaxInformationListResponse> getNonInferredWork(EmptyRequest request)
        {
            var work = buildTaxInformationList();
            var response = new TaxInformationListResponse();
            foreach (var task in work)
            {
                response.taxInformationList.Add(task);
            }
            return new ValueTask<TaxInformationListResponse>(response);
        }

        public ValueTask<BoolResponse> putCalculatedTaxData(YearlyTaxData request)
        {
            throw new NotImplementedException();
        }

        public ValueTask<BoolResponse> putInferredTaxData(YearlyTaxData request)
        {
            throw new NotImplementedException();
        }
    }
}
