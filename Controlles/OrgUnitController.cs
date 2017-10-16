using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cascade.Licensing.Domain.Abstract;
using Cascade.Licensing.WebUI.Models;
using Cascade.Licensing.Domain;

namespace Cascade.Licensing.WebUI.Controllers
{
    public class OrgUnitController : BaseController
    {
        //public OrgUnitController(IRepository repository) : base(repository) { }

        //
        // GET: /OrgUnit/
        [Authorize(Roles = "Admin, ShowOrgUnitPage")]
        [Cascade.Licensing.WebUI.Infrastructure.CustomSecurityFilter]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetOrgUnits()
        {

            var org_units = repository.OrgUnit.GetNotDeletedItems().Select(j => new { org_unit = j, Count_peoples = j.Peoples.Where(i => i.IsDeleted != true).Count() }).ToList();

            var lic_act_types = repository.LicensedActivityType.GetNotDeletedItems().Select(j => new { lic_act_type = j, org_units = j.OrgUnits.ToList()}).ToList();

            List<OrgUnitViewModel> result = new List<OrgUnitViewModel>();

            foreach (var org_u in org_units)
            {
                OrgUnitViewModel now_ou = new OrgUnitViewModel() { EMail=org_u.org_unit.EMail, Fax = org_u.org_unit.Fax, Tel = org_u.org_unit.Tel, HasPeoples = org_u.Count_peoples>0, Id = org_u.org_unit.Id, Name = org_u.org_unit.Name, Address = org_u.org_unit.Address, INN = org_u.org_unit.INN, KPP = org_u.org_unit.KPP, OGRN = org_u.org_unit.OGRN, PostalCode = org_u.org_unit.PostalCode, LicenseActivityTypes = new List<LATForOrgUnitViewModel>(), TagsTable = new ValueTagTableViewModel(org_u.org_unit.Id, EntitiesForTag.OrgUnit, repository) };
                foreach (var l in lic_act_types)
                {
                    now_ou.LicenseActivityTypes.Add(new LATForOrgUnitViewModel() { Id = l.lic_act_type.Id, Name = l.lic_act_type.FullName, IsSelected = l.org_units.FirstOrDefault(j => j.Id == org_u.org_unit.Id) != null });
                }
                result.Add(now_ou);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetTemplateNewOrgUnit()
        {

            var lic_act_types = repository.LicensedActivityType.GetNotDeletedItems().ToList();

            OrgUnitViewModel result = new OrgUnitViewModel() { 
                IsError = false,
                HasPeoples = false,
                Address = "",
                Name = "Новая",
                Id = 0,
                INN = "",
                Tel ="",
                Fax ="",
                EMail="",
                KPP = "",
                LicenseActivityTypes = new List<LATForOrgUnitViewModel>(),
                OGRN = "",
                PostalCode = "", TagsTable = new ValueTagTableViewModel(0, EntitiesForTag.OrgUnit, repository) };

            foreach (var l in lic_act_types)
            {
                result.LicenseActivityTypes.Add(new LATForOrgUnitViewModel() { Id = l.Id, Name = l.FullName, IsSelected = false });
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveOrgUnit(OrgUnitViewModel org_unit)
        {
            var lic_act_types = repository.LicensedActivityType.GetNotDeletedItems().ToList();

            if (org_unit.Name == null || org_unit.Name == "")
            {
                org_unit.IsError = true;
                return Json(org_unit, JsonRequestBehavior.AllowGet);
            }

            if (org_unit.Id > 0)
            {
                OrgUnit ou = repository.OrgUnit.GetNotDeletedItems().FirstOrDefault(j => j.Id == org_unit.Id);
                try
                {
                    if (ou == null)
                    {
                        org_unit.IsError = true;
                        return Json(org_unit, JsonRequestBehavior.AllowGet);
                    }



                    var now_lic_act_types = ou.LicensedActivityTypes.Where(j => j.IsDeleted != true).ToList();

                    foreach (var l in org_unit.LicenseActivityTypes)
                    {
                        var now_lat = lic_act_types.First(j => j.Id == l.Id);
                        if (l.IsSelected)
                        {
                            if (now_lic_act_types.FirstOrDefault(j => j.Id == l.Id) == null)
                            {
                                ou.LinkLicensedActivityType(now_lat, repository.logHelper);
                            }
                        }
                        else
                        {
                            if (now_lic_act_types.FirstOrDefault(j => j.Id == l.Id) == null)
                            {
                                ou.UnLinkLicensedActivityType(now_lat, repository.logHelper);
                            }
                        }
                    }

                    var peoples_in_org_unit = ou.Peoples.ToList();

                    var first_new_license_Activity_type = org_unit.LicenseActivityTypes.First();

                    foreach (var p in peoples_in_org_unit)
                    {
                        if (org_unit.LicenseActivityTypes.FirstOrDefault(j => j.Id == p.LicensedActivityTypeId) == null)
                        {
                            p.LicensedActivityTypeId = first_new_license_Activity_type.Id;
                        }
                    }

                    OrgUnit update_org_unit = new OrgUnit() {
                        Id = org_unit.Id,
                        Address = org_unit.Address,
                        INN = org_unit.INN,
                        KPP = org_unit.KPP,
                        Name = org_unit.Name,
                        OGRN = org_unit.OGRN,
                        PostalCode = org_unit.PostalCode,
                        EMail = org_unit.EMail,
                        Fax = org_unit.Fax,
                        Tel = org_unit.Tel
                    };
                    repository.OrgUnit.Add(update_org_unit);

                    org_unit.TagsTable.SaveTableTag(repository);
                    
                    repository.SaveChanges();

                    
                }
                catch (Exception exp)
                {
                    org_unit.IsError = true;
                    return Json(org_unit, JsonRequestBehavior.AllowGet);
                }


            }
            else
            {
                try
                {


                    OrgUnit update_org_unit = new OrgUnit() { Id = org_unit.Id, Address = org_unit.Address, INN = org_unit.INN, KPP = org_unit.KPP, Name = org_unit.Name, OGRN = org_unit.OGRN, PostalCode = org_unit.PostalCode };
                    repository.OrgUnit.Add(update_org_unit);

                    foreach (var l in org_unit.LicenseActivityTypes)
                    {
                        if (l.IsSelected)
                        {
                            var now_lat = lic_act_types.First(j => j.Id == l.Id);
                            update_org_unit.LinkLicensedActivityType(now_lat, repository.logHelper);
                        }
                    }

                    org_unit.TagsTable.SaveTableTag(repository);

                    repository.SaveChanges();
                    org_unit.Id = update_org_unit.Id;

                    
                }
                catch (Exception exp)
                {
                    org_unit.IsError = true;
                    return Json(org_unit, JsonRequestBehavior.AllowGet);
                }
            }


            org_unit.IsError = false;
            return Json(org_unit, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult DeleteOrgUnit(int id_org_unit)
        {
            var ou = repository.OrgUnit.GetNotDeletedItems().FirstOrDefault(j => j.Id == id_org_unit);
            if (ou != null&&ou.Peoples.Where(j=>j.IsDeleted!=true).Count()==0)
            {
                try
                {
                    repository.OrgUnit.Remove(ou);
                    repository.SaveChanges();

                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                catch (Exception exp)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }


	}
}