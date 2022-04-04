using Kamsyk.Reget.Model;
using Kamsyk.Reget.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Misc {
    public class Company {
        public void SetParentPgCompany() {
            CompanyRepository companyRepository = new CompanyRepository();
            List<Kamsyk.Reget.Model.Company> companies = companyRepository.GetActiveCompanies();
            foreach (var comp in companies) {
                foreach (var cg in comp.Centre_Group) {
                    foreach (var pg in cg.Purchase_Group) {
                        //int parPgId = pg.Parent_Purchase_Group.ElementAt(0).id;
                        Parent_Purchase_Group ppg = pg.Parent_Purchase_Group.ElementAt(0);
                        var pgComp = (from compDb in ppg.Company
                                      where compDb.id == comp.id
                                      select compDb).FirstOrDefault();
                        if (pgComp == null) {
                            companyRepository.AddParentPg(comp.id, ppg.id);
                            Console.WriteLine("Added " + comp.id + " - " + ppg.id);
                        }
                    }
                }
            }
        }
    }
}
