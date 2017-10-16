using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cascade.Licensing.Domain.Abstract;
using Cascade.Licensing.WebUI.Models;
using Cascade.Licensing.Domain;
using System.Text;

namespace Cascade.Licensing.WebUI.Controllers
{
    
    public class CompanyController : BaseController
    {

        //public CompanyController(IRepository repository) : base(repository) { }
        //
        // GET: /Company/
        [Authorize(Roles = "Admin, ShowCompanyPage")]
        [Cascade.Licensing.WebUI.Infrastructure.CustomSecurityFilter]
        public ActionResult Index(string t = "null")
        {
            if (t != "null")
            {
                t = Filtrator.ConvertHexToString(Convert.ToString(t), Encoding.Default);
            }
            return View("Index", "_Layout", t);
        }


        [HttpPost]
        public JsonResult getCompanesForMainPage(string val)
        {
            List<CompanySearchResult> res = new List<CompanySearchResult>();

            res.AddRange(repository.Company.GetNotDeletedItems().Where(j => j.ShortName.Contains(val)|| j.INN.Contains(val) || j.ORGN.Contains(val) ).ToList().Select(j => new CompanySearchResult() {CompanyId = j.Id, name = "ИНН: " + j.INN +" ОГРН: "+j.ORGN+" Сокр: " + j.ShortName, url = "/Company/Index?t=" + Filtrator.ConvertStringToHex("{filters:[{field:'INN',val:[" + j.INN + "]}]}", Encoding.Default) }).ToList());
            res.AddRange(repository.Company.GetNotDeletedItems().Where(j => j.FullName.Contains(val) && !j.ShortName.Contains(val)).ToList().Select(j => new CompanySearchResult() { CompanyId = j.Id, name = "ИНН: " + j.INN + " Название: " + j.FullName, url = "/Company/Index?t=" + Filtrator.ConvertStringToHex("{filters:[{field:'INN',val:[" + j.INN + "]}]}", Encoding.Default) }).ToList());
            res.AddRange(repository.License.GetNotDeletedItems().Where(j => j.Nom.Contains(val)).ToList().Select(j => new CompanySearchResult() { CompanyId = j.Company.Id, name = "ИНН: " + j.Company.INN + " № лиц.: " + j.Nom + " Сокр.: " + j.Company.ShortName, url = "/Company/Index?t=" + Filtrator.ConvertStringToHex("{filters:[{field:'INN',val:[" + j.Company.INN + "]}]}", Encoding.Default) }).ToList());
            


            return Json(res.Take(40));
        }

        [HttpPost]
        public JsonResult CompanyFilter(FilterAndSortingOptions filter_template)
        {
            IQueryable<Company> companies = repository.Company.GetNotDeletedItems();

            companies = Filtrator.FilterByTemplate<Company>(companies, filter_template);

            var companies_vm_list = CompanyViewModel.GetListViewModel(companies.Take(60));
            FilteredCompanyListViewModel result = new FilteredCompanyListViewModel() { Count = companies.Count(), company_list = companies_vm_list };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetCompanyById(int Id)
        {
            if (Id > 0)
            {
                Company c = repository.Company.GetNotDeletedItems().First(j => j.Id == Id);
                CompanyViewModel result = new CompanyViewModel(new CompanyExportFromDatabase() { company = c, legal_form_type = c.LegalFormType, addr = c.ActualAddr });
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new CompanyViewModel(), JsonRequestBehavior.AllowGet); 
            }
        }

        [HttpPost]
        public JsonResult CheckINN(string inn)
        {
            Company comp = repository.Company.GetNotDeletedItems().FirstOrDefault(j => j.INN == inn);
            string res ="";
            if (comp != null)
            {
                res = "/Company/Index?t=" + Filtrator.ConvertStringToHex("{filters:[{field:'INN',val:["+inn+"]}]}", Encoding.Default);
            }

           return Json(res, JsonRequestBehavior.AllowGet);
            
        }

        [HttpPost]
        public JsonResult AddCompany(CompanyViewModel CompanyVM)
        {
            //Addr addr = CompanyVM.id_addr == -1 ? null : repository.Addr.GetNotDeletedItems().First(j => j.Id == CompanyVM.id_addr);
            //Company comp = new Company() { Id = CompanyVM.Id, ActualAddrStr = CompanyVM.Address, Email = CompanyVM.Email, Fax = CompanyVM.Fax == null ? "" : CompanyVM.Fax, FullName = CompanyVM.FullName, INN = CompanyVM.INN, IsModifiedLegalAddrStr = true, IsModifiedPostalAddrStr = true, IsModifiredActualAddrStr = CompanyVM.IsModifiedAddress, KPP = CompanyVM.KPP, LegalAddrStr = "", ORGN = CompanyVM.OGRN == null ? "" : CompanyVM.OGRN, Phone = CompanyVM.Phone == null ? "" : CompanyVM.Phone, PostalAddrStr = "", ShortName = CompanyVM.ShortName, Status = TCompanyState.Active, ActualAddr = addr };
            //LegalFormType leg_f_t = repository.LegalFormType.GetNotDeletedItems().First(j=>j.Id == CompanyVM.LegalFormType.Id);
            //repository.Company.Add(comp, leg_f_t);
            //repository.SaveChanges();
            //var new_company = repository.Company.GetNotDeletedItems().First(j => j.Id == comp.Id);
            //CompanyViewModel result = new CompanyViewModel(new CompanyExportFromDatabase() { company = new_company, legal_form_type = new_company.LegalFormType, addr = new_company.ActualAddr});

            return Json(true, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult CompanyFilterNextPage(FilterAndSortingOptions filter_template, int SkipCount)
        {
            IQueryable<Company> companies = repository.Company.GetNotDeletedItems();

            companies = Filtrator.FilterByTemplate<Company>(companies,filter_template);

            var companies_vm_list = CompanyViewModel.GetListViewModel(companies.Skip(SkipCount).Take(60));
            return Json(companies_vm_list, JsonRequestBehavior.AllowGet);
        }
	}
}