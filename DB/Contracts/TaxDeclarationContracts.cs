using Microsoft.EntityFrameworkCore;
using Shared.Contracts;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DB.Contracts
{
    public class TaxDeclarationService : ITaxDeclarationService
    {
        public ValueTask<BoolResponse> createNewTaxDeclaration(NewTaxDeclarationRequest request)
        {
            using (var ctx = new TIAE6Context())
            {
                using (var txn = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        TaxDeclaration td = new TaxDeclaration();
                        td.personId = request.personId;
                        td.year = request.year;
                        ctx.taxDeclarations.Add(td);
                        ctx.SaveChanges();

                        TaxDeclarationEntry income = new TaxDeclarationEntry();
                        income.taxDeclarationId = td.id;
                        income.attribute = ctx.taxDeclarationAttributes.Where(x => x.name == "Income").FirstOrDefault();
                        income.value = request.income;
                        ctx.taxDeclarationEntries.Add(income);

                        TaxDeclarationEntry deductions = new TaxDeclarationEntry();
                        deductions.taxDeclarationId = td.id;
                        deductions.attribute = ctx.taxDeclarationAttributes.Where(x => x.name == "Deductions").FirstOrDefault();
                        deductions.value = request.deductions;
                        ctx.taxDeclarationEntries.Add(deductions);

                        TaxDeclarationEntry inferred = new TaxDeclarationEntry();
                        inferred.taxDeclarationId = td.id;
                        inferred.attribute = ctx.taxDeclarationAttributes.Where(x => x.name == "Inferred").FirstOrDefault();
                        inferred.value = 0;
                        ctx.taxDeclarationEntries.Add(inferred);

                        TaxDeclarationEntry calculated = new TaxDeclarationEntry();
                        calculated.taxDeclarationId = td.id;
                        calculated.attribute = ctx.taxDeclarationAttributes.Where(x => x.name == "Calculated").FirstOrDefault();
                        calculated.value = 0;
                        ctx.taxDeclarationEntries.Add(calculated);

                        TaxDeclarationEntry suspicious = new TaxDeclarationEntry();
                        suspicious.taxDeclarationId = td.id;
                        suspicious.attribute = ctx.taxDeclarationAttributes.Where(x => x.name == "Suspicious").FirstOrDefault();
                        suspicious.value = 0;
                        ctx.taxDeclarationEntries.Add(suspicious);

                        TaxDeclarationEntry capital = new TaxDeclarationEntry();
                        suspicious.taxDeclarationId = td.id;
                        suspicious.attribute = ctx.taxDeclarationAttributes.Where(x => x.name == "Capital").FirstOrDefault();
                        suspicious.value = 0;
                        ctx.taxDeclarationEntries.Add(capital);

                        ctx.SaveChanges();
                        txn.Commit();
                        return new ValueTask<BoolResponse>(new BoolResponse { success = true });
                    }
                    catch (Exception ex)
                    {
                        txn?.Rollback();
                    }
                }
                return new ValueTask<BoolResponse>(new BoolResponse { success = false });
            }
        }

        public ValueTask<TaxDeclarationListResponse> getAllTaxDeclarations(EmptyRequest request)
        {
            using (var ctx = new TIAE6Context())
            {
                List<TaxDeclaration> tdList = ctx.taxDeclarations
                                         .Include(x => x.person)
                                         .Include(x => x.Entries)
                                         .ThenInclude(x => x.attribute)
                                         .ToList();

                for (int i = 0; i < tdList.Count; i++)
                {
                    tdList[i].isInferred = tdList[i].getIsInferred();
                    tdList[i].isCalculated = tdList[i].getIsCalculated();
                    tdList[i].Income = ctx.taxDeclarationEntries.Single(x => x.taxDeclarationAttributeId == 1 && x.taxDeclarationId == tdList[i].id).value;
                    tdList[i].Deductions = ctx.taxDeclarationEntries.Single(x => x.taxDeclarationAttributeId == 2 && x.taxDeclarationId == tdList[i].id).value;
                    tdList[i].TaxDue = ctx.taxDeclarationEntries.Single(x => x.taxDeclarationAttributeId == 3 && x.taxDeclarationId == tdList[i].id).value;
                    tdList[i].Capital = ctx.taxDeclarationEntries.Single(x => x.taxDeclarationAttributeId == 7 && x.taxDeclarationId == tdList[i].id).value;
                    tdList[i].Suspicious = ctx.taxDeclarationEntries.Single(x => x.taxDeclarationAttributeId == 6 && x.taxDeclarationId == tdList[i].id).value == 1;
                }

                TaxDeclarationListResponse response = new TaxDeclarationListResponse { 
                    declarationList = tdList
                };
                return new ValueTask<TaxDeclarationListResponse>(response);
            }
        }
    }
}
