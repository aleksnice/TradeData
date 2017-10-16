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
    public class LicenseController : BaseController
    {

        //public LicenseController(IRepository repository) : base(repository) { }
        //
        // GET: /License/
        [Authorize(Roles = "Admin, ShowLicensePage")]
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
        public JsonResult LicenseFilter(FilterAndSortingOptions filter_template)
        {
            IQueryable<License> licenses = repository.License.GetNotDeletedItems();

            licenses = Filtrator.FilterByTemplate<License>(licenses, filter_template);

            var licenses_vm_list = LicenseViewModel.GetListViewModel(licenses.Take(60));
            FilteredLicenseListViewModel result = new FilteredLicenseListViewModel() { Count = licenses.Count(), license_list = licenses_vm_list };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult LicenseFilterNextPage(FilterAndSortingOptions filter_template, int SkipCount)
        {
            IQueryable<License> licenses = repository.License.GetNotDeletedItems();

            licenses = Filtrator.FilterByTemplate<License>(licenses, filter_template);

            var licenses_vm_list = LicenseViewModel.GetListViewModel(licenses.Skip(SkipCount).Take(60));
            return Json(licenses_vm_list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetLicenseByCompany(int id_company)
        {
           var license_list = repository.License.GetItemsByCompany(id_company);
           var result = LicenseViewModelForCompany.GetListViewModel(license_list);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
	}
}