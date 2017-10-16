using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cascade.Licensing.Domain.Abstract;
using Cascade.Licensing.WebUI.Models;
using Cascade.Licensing.Domain;
using System.Text;
using System.Configuration;
using System.Threading;
using Cascade.Licensing.Domain.Concrete;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.EntityFramework;


namespace Cascade.Licensing.WebUI.Controllers
{
    
    public class DeclarationController : BaseController
    {

        //public DeclarationController(IRepository repository) : base(repository) { }

        //
        // GET: /Declaration/
        [Authorize(Roles = "Admin, ShowDeclarationPage")]
        [Cascade.Licensing.WebUI.Infrastructure.CustomSecurityFilter]
        public ActionResult Index(string t = "null")
        {
            if (t != "null")
            {
                t = Filtrator.ConvertHexToString(Convert.ToString(t), Encoding.Default);
            }
            return View("Index", "_Layout", t);
        }

        [Authorize(Roles = "Admin, ShowDeclarationPage")]
        [Cascade.Licensing.WebUI.Infrastructure.CustomSecurityFilter]
        public ActionResult Detail(string type, int id, string tabIndex)
        {
            return View("Detail", "_Layout", "['" + type + "'," + id + ",'" + tabIndex + "']");
        }

        private DeclarationRights GetRights()
        {

            var UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

            ApplicationUser app_user = UserManager.FindByName(repository.UserDB.login);
            var roles = UserManager.GetRoles(app_user.Id);


            if (roles.Contains("Admin"))
            {
                return new DeclarationRights() { CanSetDeclarationStatusCompleted = true, CanSetDeclarationStatusInWork = true, CanSetDeclarationStatusRevision = true };
            }

            DeclarationRights res = new DeclarationRights();

            if (roles.Contains("CanSetDeclarationStatusInWork"))
            {
                res.CanSetDeclarationStatusInWork = true;
            }

            if (roles.Contains("CanSetDeclarationStatusCompleted"))
            {
                res.CanSetDeclarationStatusCompleted = true;
            }

            if (roles.Contains("CanSetDeclarationStatusRevision"))
            {
                res.CanSetDeclarationStatusRevision = true;
            }

            return res;
        }

        private DeclarationViewModel GetDeclarationViewModelById(int id)
        {
            DeclarationViewModel result = DeclarationViewModel.GetDeclarationViewModelById(repository, id);
            result.Rights = GetRights();
            return result;
        }

        private void PostCreateNewDeclarationViewModel(DeclarationViewModel dvm, TypeOfTypeOfResolution ttr)
        {
            dvm.TagsTable = new ValueTagTableViewModel(0, EntitiesForTag.Declaration, repository);
            dvm.ResolutionTypes = TypeOfResolutionForResolutionViewModel.GetTypeOfResolutionViewModelList(repository, ttr, null);
            dvm.ResolutionCommissioners = Utils.GetPeopleListByRole(repository, "ResolutionCommissioner");
            var checkedList = Utils.GetPeopleListByRole(repository, "ResolutionCommissionerDefault");
            foreach (var item in dvm.ResolutionCommissioners)
            {
                item.IsChecked = checkedList.Select(x => x.Id).Contains(item.Id);
            }
            dvm.ResolutionCommissionChairman = Utils.GetPeopleListByRole(repository, "ResolutionCommissionChairmanDefault").FirstOrDefault();
            dvm.LicenceApprover = Utils.GetPeopleListByRole(repository, "LicenceApproverDefault").FirstOrDefault();
            dvm.Rights = GetRights();
        }

        [HttpPost]
        public JsonResult GetDeclaration(string type, int id)
        {
            DeclarationViewModel result = null;
            switch (type)
            {
                case "NewForExistCompany":
                    {
                        Company c = repository.Company.GetNotDeletedItems().First(j => j.Id == id);
                        TypeOfTypeOfResolution ttr = TypeOfTypeOfResolution.Grant;
                        result = new DeclarationViewModel(new DeclarationExportFromDatabase()
                        {
                            resolution = null,
                            type_of_type_new = ttr,
                            //type_of_resolution = repository.TypeOfResolution.GetNotDeletedItems().First(j=>j.TypeOfType==ttr),
                            reason_for_renews = null,
                            refuse_basis = null,
                            license_form = null,
                            license = null,
                            TypeCreateDeclaration = "Grant",
                            declaration = null,
                            company_export = new CompanyExportFromDatabase() { addr = c.ActualAddr, addr_postal = c.PostalAddr, company = c, legal_form_type = c.LegalFormType }
                        }, repository);

                        //result.DeclarationsInPack = DeclarationViewModelForPack.GetListViewModel(repository.Declaration.GetNotDeletedItems().Where(j => j.CompanyId.HasValue && j.Status == DeclarationStatus.Consideration && j.CompanyId.Value == c.Id), 0);
                        PostCreateNewDeclarationViewModel(result, ttr);
                        break;
                    }
                case "NewForNewCompany":
                    {
                        TypeOfTypeOfResolution ttr = TypeOfTypeOfResolution.Grant;
                        result = new DeclarationViewModel(new DeclarationExportFromDatabase()
                        {
                            resolution = null,
                            type_of_type_new = ttr,
                            //type_of_resolution = repository.TypeOfResolution.GetNotDeletedItems().First(j => j.TypeOfType == ttr),
                            reason_for_renews = null,
                            refuse_basis = null,
                            license_form = null,
                            license = null,
                            TypeCreateDeclaration = "Grant",
                            declaration = null,
                            company_export = null
                        }, repository);
                        result.Company.id_addr = 1;
                        PostCreateNewDeclarationViewModel(result, ttr);
                        break;
                    }
                case "Cancel":
                    {
                        License lic = repository.License.GetNotDeletedItems().First(j => j.Id == id);
                        Company c = lic.Company;
                        TypeOfTypeOfResolution ttr = TypeOfTypeOfResolution.Discontinue;
                        result = new DeclarationViewModel(new DeclarationExportFromDatabase()
                        {
                            resolution = null,
                            type_of_type_new = ttr,
                            //type_of_resolution = repository.TypeOfResolution.GetNotDeletedItems().First(j => j.TypeOfType == ttr),
                            reason_for_renews = null,
                            refuse_basis = null,
                            license_form = null,
                            license = lic,
                            TypeCreateDeclaration = type,
                            declaration = null,
                            company_export = new CompanyExportFromDatabase() { addr = c.ActualAddr, addr_postal = c.PostalAddr, company = c, legal_form_type = c.LegalFormType }
                        }, repository);
                        //UnitType def_unit_type = repository.UnitType.GetNotDeletedItems().First();
                        //ModifyLicense def_mod_lic = repository.ModifyLicense.GetNotDeletedItems().First();
                        PostCreateNewDeclarationViewModel(result, ttr);
                        //result.DeclarationsInPack = DeclarationViewModelForPack.GetListViewModel(repository.Declaration.GetNotDeletedItems().Where(j => j.CompanyId.HasValue && j.Status == DeclarationStatus.Consideration && j.CompanyId.Value == c.Id), 0);
                        break;
                    }
                case "Edit":
                    {
                        result = GetDeclarationViewModelById(id);
                        break;
                    }
                case "ReNew":
                    {
                        result = GetDeclarationViewModelByPreviousLicense(result, type, id, TypeOfTypeOfResolution.Renewal);
                        break;
                    }
                case "Prolong":
                    {
                        result = GetDeclarationViewModelByPreviousLicense(result, type, id, TypeOfTypeOfResolution.Prolong);                     
                        break;
                    }
                default: //"ProlongReNew", "Prolong", "ReNew"
                    {

                        result = GetDeclarationViewModelByPreviousLicense(result, type, id, TypeOfTypeOfResolution.ProlongRenew);
                        break;
                    }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Метод для выбора типа действия в карточке организации
        /// </summary>
        /// <param name="result">получаем DeclarationViewModel</param>
        /// <param name="type"> тип действия</param>
        /// <param name="id">идентификатор</param>
        /// <param name="TypeOfTypeOfResolution">тип действия</param>
        private DeclarationViewModel GetDeclarationViewModelByPreviousLicense(DeclarationViewModel result, string type, int licenseId, TypeOfTypeOfResolution ttr)
        {
            License lic = repository.License.GetNotDeletedItems().First(j => j.Id == licenseId);
            Company c = lic.Company;

            result = new DeclarationViewModel(new DeclarationExportFromDatabase()
            {
                resolution = null,
                type_of_type_new = ttr,
                //type_of_resolution = repository.TypeOfResolution.GetNotDeletedItems().First(j => j.TypeOfType == ttr),
                reason_for_renews = null,
                refuse_basis = null,
                license_form = null,
                license = lic,
                TypeCreateDeclaration = type,
                declaration = null,
                company_export = new CompanyExportFromDatabase() { addr = c.ActualAddr, addr_postal = c.PostalAddr, company = c, legal_form_type = c.LegalFormType }
            }, repository);
            UnitType def_unit_type = repository.UnitType.GetNotDeletedItems().Where(x => x.LicensedActivityTypeId == lic.LicensedActivityType.Id).First();
            ModifyLicense def_mod_lic = repository.ModifyLicense.GetNotDeletedItems().Where(x => x.LicensedActivityTypeId == lic.LicensedActivityType.Id).First();
            result.UnitsEdit = UnitInDeclarationViewModel.GetListViewModelByParentUnits(repository.UnitInLicenseForm.GetItemsForLastLicenseFormByLicenseId(licenseId), def_unit_type, def_mod_lic);
            result.DeclarationsInPack = DeclarationViewModelForPack.GetListViewModel(repository.Declaration.GetNotDeletedItems().Where(j => j.CompanyId.HasValue && j.Status == DeclarationStatus.Consideration && j.CompanyId.Value == c.Id && (j.Type.HasFlag(DeclarationType.Prolong) || j.Type.HasFlag(DeclarationType.Renew))), 0);
            PostCreateNewDeclarationViewModel(result, ttr);
            return result;
        }

        [HttpPost]
        public JsonResult GetAllDeclaration()
        {
            List<DeclarationViewModel> res = DeclarationViewModel.GetListViewModel(repository, repository.Declaration.GetNotDeletedItems());
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeclarationFilter(FilterAndSortingOptions filter_template)
        {
            IQueryable<Declaration> declarations = repository.Declaration.GetNotDeletedItems();
            var f = filter_template.filters.FirstOrDefault(j => j.field == "SeeAllLicensedActivityType");
            if (f != null && f.val.Count > 0 && f.val[0] == "True")
            {
                declarations = repository.Declaration.Items.Where(j => j.IsDeleted != true);
            }

            declarations = Filtrator.FilterByTemplate<Declaration>(declarations, filter_template);



            var Declaration_vm = DeclarationViewModel.GetListViewModel(repository, declarations.Take(50));

            FilteredDeclarationListViewModel res = new FilteredDeclarationListViewModel() { declaration_list = Declaration_vm, Count = declarations.Count() };

            return Json(res, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult DeclarationFilterNextPage(FilterAndSortingOptions filter_template, int SkipCount)
        {
            IQueryable<Declaration> declarations = repository.Declaration.GetNotDeletedItems();

            var f = filter_template.filters.FirstOrDefault(j => j.field == "SeeAllLicensedActivityType");
            if (f != null && f.val.Count > 0 && f.val[0] == "True")
            {
                declarations = repository.Declaration.Items.Where(j => j.IsDeleted != true);
            }

            declarations = Filtrator.FilterByTemplate<Declaration>(declarations, filter_template);

            var Declaration_vm = DeclarationViewModel.GetListViewModel(repository, declarations.Skip(SkipCount).Take(50));

            return Json(Declaration_vm, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddDeclaration(DeclarationViewModel dec, bool return_declaration, bool sosMode)
        {
            Addr addr = dec.Company.id_addr == -1 ? null : repository.Addr.GetNotDeletedItems().First(j => j.Id == dec.Company.id_addr);
            Addr addr_postal = dec.Company.id_postal_addr == -1 ? null : repository.Addr.GetNotDeletedItems().First(j => j.Id == dec.Company.id_postal_addr);

            LegalFormType leg_f_t = repository.LegalFormType.GetNotDeletedItems().First(j => j.Id == dec.Company.LegalFormType.Id);

            Company comp = null;
            if (dec.Company.Id != 0)
            {
                comp = repository.Company.GetNotDeletedItems().First(j => j.Id == dec.Company.Id);
            }

            License lic = null;
            if (dec.LicenseId != 0)
            {
                lic = repository.License.GetNotDeletedItems().First(j => j.Id == dec.LicenseId);
            }

            LicensedActivityType license_active_type = repository.UserDB.LicensedActivityTypeContext;
            Declaration decl = GetDeclarationFromDeclarationViewModel(dec, (DeclarationStatus)Enum.Parse(typeof(DeclarationStatus), dec.Status), addr, addr_postal);
            SetDeclarationFlags(dec, decl);
            decl = repository.Declaration.Add(decl, license_active_type, leg_f_t, comp, lic);
            decl = UpdateUnitInDeclaration(dec, decl);

            Resolution res = null;
            if (!sosMode)
            {
                res = UpdateResolution(license_active_type, dec, decl, true, ResultOfResolution.None);
                if (decl.Id == 0)
                {
                    ActionWithDeclaration awd = new ActionWithDeclaration() { Id = 0, Comm = "", DateTimeAction = DateTime.Now, EndedDateTime = null, NewStatusOfDeclaration = DeclarationStatus.Draft };
                    repository.ActionWithDeclaration.Add(awd, decl);
                }
                else
                {
                    var dec_st = (DeclarationStatus)Enum.Parse(typeof(DeclarationStatus), dec.Status);
                    ActionWithDeclaration old_awd = repository.ActionWithDeclaration.GetItemsForDeclaration(decl.Id).FirstOrDefault(j => j.EndedDateTime == null);
                    if (old_awd != null && old_awd.NewStatusOfDeclaration != dec_st)
                    {
                        var dt = DateTime.Now;
                        old_awd.EndedDateTime = dt;
                        ActionWithDeclaration awd = new ActionWithDeclaration() { Id = 0, Comm = "", DateTimeAction = dt, EndedDateTime = null, NewStatusOfDeclaration = dec_st };
                        repository.ActionWithDeclaration.Add(awd, decl);
                    }
                }
            }

            UpdatePaymentOrders(dec, decl);
            if (!sosMode)
            {
                UpdateDeclarationsPackAndUpdateResolutionPaymentOrdersAll(dec, res, decl.PaymentOrdersAll);
            }
            TagsTableAndDocumentsSaveAndCommit(dec, decl);

            if (return_declaration)
            {
                DeclarationViewModel result = GetDeclarationViewModelById(decl.Id);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(decl.Id, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult SwitchDeclarationNumber(DeclarationViewModel dvm)
        {
            String errMsg = null;
            try
            {
                errMsg = repository.Declaration.SwitchNumbers(dvm.Id, dvm.RegNomAll);
                repository.SaveChanges();
                //DateTime newDt;
                //DateTime firstYearDate;
                //if (DateTime.TryParse(dvm.DateDeclaration, out newDt))
                //{
                //    firstYearDate = new DateTime(newDt.Year, 1, 1);
                //}
                ////int newNumber = 0;
                ////if (int.TryParse(dvm.RegNomAll, out newNumber))
                ////{
                ////}
                ////if (repository.Declaration.GetNotDeletedItems().FirstOrDefault(x => x.RegNom == newNumber && x.Id != dvm.Id && x.DateDeclaration >= firstYearDate) == null)
                ////{
                //Declaration updatedDec = new Declaration()
                //{
                //    Id = item.Id,
                //    CountOfYears = item.CountOfYears,
                //    DateDeclaration = newDt,
                //    GetInformationType = item.GetInformationType,
                //    GetResultType = item.GetResultType,
                //    OtherReason = item.OtherReason,
                //    MasterName = item.MasterName,
                //    Status = item.Status,
                //    Type = item.Type,
                //    ActualAddr = item.ActualAddr,
                //    ActualAddrStr = item.ActualAddrStr,
                //    AddressForDocument = item.AddressForDocument,
                //    BanksName = item.BanksName,
                //    CheckingAccount = item.CheckingAccount,
                //    Email = item.Email,
                //    Fax = item.Fax,
                //    FullNameCompany = item.FullNameCompany,
                //    INN = item.INN,
                //    KPP = item.KPP,
                //    OGRN = item.OGRN,
                //    Phone = item.Phone,
                //    ShortNameCompany = item.ShortNameCompany,
                //    Comm = item.Comm,
                //    LicenseFormNom = item.LicenseFormNom,
                //    LicenseNom = item.LicenseNom,
                //    LicenseNomOfBook = item.LicenseNomOfBook,
                //    LicenseForm = item.LicenseForm,
                //    PostalAddr = item.PostalAddr,
                //    PostalAddrStr = item.PostalAddrStr,
                //    PostalCode = item.PostalCode,
                //    ActualAddrAddon = item.ActualAddrAddon,
                //    PostalAddrAddon = item.PostalAddrAddon,
                //    FSRARActualAddr = item.FSRARActualAddr,
                //    FSRARPostalAddr = item.FSRARPostalAddr
                //};
                //repository.Declaration.Add(updatedDec, item.LicensedActivityType, item.LegalFormType, item.Company, item.License, item.ReasonForRenew);
                //repository.SaveChanges();
                //    else errMsg = String.Format("Заявление с номером {0} уже существует, пожалуйста, выберите другой номер.", newNumber);
                //}
                //else errMsg = "Номер заявления не может быть пустым";
                //}
                //else errMsg = "Дата заявления не может быть пустой";
            }
            catch (Exception e)
            {
                errMsg = Cascade.Utils.Common.GetFullExceptionMessage(e);
            }
            return Json(new
            {
                ErrMsg = errMsg
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, CanSetDeclarationStatusInWork")]
        public JsonResult SetDeclarationConsideration(DeclarationViewModel dec)
        {
            String errMsg = null;
            DeclarationViewModel result = null;
            try
            {
                // repository.SaveChanges();
                Addr addr = dec.Company.id_addr == -1 ? null : repository.Addr.GetNotDeletedItems().First(j => j.Id == dec.Company.id_addr);
                Addr addr_postal = dec.Company.id_postal_addr == -1 ? null : repository.Addr.GetNotDeletedItems().First(j => j.Id == dec.Company.id_postal_addr);

                LegalFormType leg_f_t = repository.LegalFormType.GetNotDeletedItems().First(j => j.Id == dec.Company.LegalFormType.Id);

                Company comp = null;

                if (dec.Company.Id != 0)
                {
                    comp = repository.Company.GetNotDeletedItems().First(j => j.Id == dec.Company.Id);
                }
                else
                {
                    comp = new Company()
                    {
                        ActualAddr = addr,
                        ActualAddrStr = dec.Company.Address,
                        AddressForDocument = dec.Company.AddressForDocument,
                        AddrStrFromImport = "",
                        BanksName = dec.Company.BanksName,
                        CheckingAccount = dec.Company.CheckingAccount,
                        Email = dec.Company.Email,
                        Fax = dec.Company.Fax,
                        FullName = dec.Company.FullName,
                        Name = dec.Company.CompanyName,
                        INN = dec.Company.INN,
                        KPP = dec.Company.KPP,
                        LegalAddr = null,
                        LegalAddrStr = "",
                        ORGN = dec.Company.OGRN,
                        Phone = dec.Company.Phone,
                        PostalAddr = addr_postal,
                        PostalAddrStr = dec.Company.PostalAddress,
                        PostalCode = dec.Company.PostalCode,
                        ShortName = dec.Company.ShortName,
                        Status = TCompanyState.Active,
                        FSRARActualAddr = dec.Company.FSRARAddress,
                        FSRARPostalAddr = dec.Company.FSRARPostalAddress,
                        ActualAddrAddon = dec.Company.GetAddrAddon(),
                        PostalAddrAddon = dec.Company.GetPostalAddrAddon()
                    };

                    repository.Company.Add(comp, leg_f_t);
                }

                License lic = null;

                if (dec.LicenseId != 0)
                {
                    lic = repository.License.GetNotDeletedItems().First(j => j.Id == dec.LicenseId);
                }

                LicensedActivityType license_active_type = repository.UserDB.LicensedActivityTypeContext;

                Declaration decl = GetDeclarationFromDeclarationViewModel(dec, DeclarationStatus.Consideration, addr, addr_postal);

                SetDeclarationFlags(dec, decl);

                repository.Declaration.Add(decl, license_active_type, leg_f_t, comp, lic);

                decl = UpdateUnitInDeclaration(dec, decl);
                DateTime dt = DateTime.Now;
                if (decl.Id != 0)
                {
                    ActionWithDeclaration old_awd = repository.ActionWithDeclaration.GetItemsForDeclaration(decl.Id).FirstOrDefault(j => j.EndedDateTime == null);
                    if (old_awd != null)
                    {
                        old_awd.EndedDateTime = dt;
                    }
                }
                ActionWithDeclaration awd = new ActionWithDeclaration() { Id = 0, Comm = "", DateTimeAction = dt, EndedDateTime = null, NewStatusOfDeclaration = DeclarationStatus.Consideration };
                repository.ActionWithDeclaration.Add(awd, decl);

                GenerateDeclarationNom(decl);

                Resolution res = UpdateResolution(license_active_type, dec, decl, true, ResultOfResolution.None);

                UpdatePaymentOrders(dec, decl);

                UpdateDeclarationsPackAndUpdateResolutionPaymentOrdersAll(dec, res, decl.PaymentOrdersAll);

                TagsTableAndDocumentsSaveAndCommit(dec, decl);

                CheckClonesDeclaration(decl);

                // return Json(true, JsonRequestBehavior.AllowGet);

                result = GetDeclarationViewModelById(decl.Id);
            }
            catch (Exception e)
            {
                errMsg = Cascade.Utils.Common.GetFullExceptionMessage(e);
            }
            return Json(new
            {
                ErrMsg = errMsg,
                Item = result
            }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        [Authorize(Roles = "Admin, CanSetDeclarationStatusCompleted")]
        public JsonResult SetDeclarationCancel(DeclarationViewModel dec)
        {
            String errMsg = null;
            try
            {
                Declaration decl = repository.Declaration.GetNotDeletedItems().First(j => j.Id == dec.Id);
                LicensedActivityType license_active_type = repository.UserDB.LicensedActivityTypeContext;
                var reasonIds = dec.ReasonForRenews.Where(x => x.IsChecked).Select(x => x.Id);
                decl.AttachReasonForRenewList(repository.ReasonForRenew.GetNotDeletedItems().Where(x => reasonIds.Contains(x.Id)).ToList(),repository.logHelper);

                decl.Status = DeclarationStatus.Canceled;

                decl.MasterName = dec.MasterName == null ? "" : dec.MasterName;
                decl.VakeelName = dec.VakeelName == null ? "" : dec.VakeelName;
                Resolution res = UpdateResolution(license_active_type, dec, decl, false, ResultOfResolution.Negative);

                DateTime dt = DateTime.Now;
                ActionWithDeclaration old_awd = repository.ActionWithDeclaration.GetItemsForDeclaration(decl.Id).First(j => j.EndedDateTime == null);
                old_awd.EndedDateTime = dt;

                ActionWithDeclaration awd = new ActionWithDeclaration() { Id = 0, Comm = "", DateTimeAction = dt, EndedDateTime = null, NewStatusOfDeclaration = DeclarationStatus.Canceled };
                repository.ActionWithDeclaration.Add(awd, decl);

                dec.TagsTable.SaveTableTag(repository);

                var del_files = DocumentViewModel.UpdateDocuments(dec.Documents, repository);

                //Company comp = repository.Company.GetNotDeletedItems().First(j=>j.Id == dec.Company.Id);

                //foreach (var doc in dec.Documents)
                //{
                //    Document doc_db = repository.Document.GetNotDeletedItems().First(j=>j.Id == doc.Id);
                //    comp.LinkDocument(doc_db);
                //}

                UpdateDeclarationsPackAndUpdateResolutionPaymentOrdersAllAndSetStatus(dec, res, decl.PaymentOrdersAll, DeclarationStatus.Canceled);

                repository.SaveChanges();

                //DocumentViewModel.RemoveFileDocuments(del_files);
            }
            catch (Exception e)
            {
                errMsg = Cascade.Utils.Common.GetFullExceptionMessage(e);
            }
            return Json(new
            {
                ErrMsg = errMsg,
            }, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        [Authorize(Roles = "Admin, CanSetDeclarationStatusCompleted")]
        public JsonResult SetDeclarationAgreed(DeclarationViewModel dec)
        {
            String errMsg = null;
            try
            {
                Addr addr = dec.Company.id_addr == -1 ? null : repository.Addr.GetNotDeletedItems().First(j => j.Id == dec.Company.id_addr);
                Addr addr_postal = dec.Company.id_postal_addr == -1 ? null : repository.Addr.GetNotDeletedItems().First(j => j.Id == dec.Company.id_postal_addr);
                LegalFormType leg_f_t = repository.LegalFormType.GetNotDeletedItems().First(j => j.Id == dec.Company.LegalFormType.Id);

                Company comp = repository.Company.GetNotDeletedItems().First(j => j.Id == dec.Company.Id);
                Company comp_update = new Company()
                {
                    Id = dec.Company.Id,
                    ActualAddr = addr,
                    ActualAddrStr = dec.Company.Address,
                    AddressForDocument = dec.Company.AddressForDocument,
                    AddrStrFromImport = "",
                    BanksName = dec.Company.BanksName,
                    CheckingAccount = dec.Company.CheckingAccount,
                    Email = dec.Company.Email,
                    Fax = dec.Company.Fax,
                    FullName = dec.Company.FullName,
                    Name = dec.Company.CompanyName,
                    INN = dec.Company.INN,
                    KPP = dec.Company.KPP,
                    LegalAddr = null,
                    LegalAddrStr = "",
                    ORGN = dec.Company.OGRN,
                    Phone = dec.Company.Phone,
                    PostalAddr = addr_postal,
                    PostalAddrStr = dec.Company.PostalAddress,
                    PostalCode = dec.Company.PostalCode,
                    ShortName = dec.Company.ShortName,
                    Status = TCompanyState.Active,
                    FSRARActualAddr = dec.Company.FSRARAddress,
                    FSRARPostalAddr = dec.Company.FSRARPostalAddress,
                    ActualAddrAddon = dec.Company.GetAddrAddon(),
                    PostalAddrAddon = dec.Company.GetPostalAddrAddon()
                };
                repository.Company.Add(comp_update, leg_f_t);

                
                DateTime dt_dec = Convert.ToDateTime(dec.ResolutionDate);
                DateTime dt_now = DateTime.Now;
                LicensedActivityType license_active_type = repository.UserDB.LicensedActivityTypeContext;

                License lic = null;

                LicenseForm lic_form = null;


                LicenseForm prev_lic_form = null;

                if (dec.LicenseId != 0)
                {
                    lic = repository.License.GetNotDeletedItems().First(j => j.Id == dec.LicenseId);

                    prev_lic_form = lic.LastLicenseForm;

                    if (dec.IsProlongType)
                    {
                        if (!String.IsNullOrEmpty(dec.LicenseDateExpired))
                        {
                            dt_dec = Convert.ToDateTime(dec.LicenseDateExpired).AddDays(1);
                        }
                        lic.DateExpired = dt_dec.AddYears(dec.CountYears).AddDays(-1);
                    }
                    else
                    {
                        if (dec.IsReNewType)
                        {
                            DeclarationViewModelForPack decl_from_pack_prolong = dec.DeclarationsInPack.FirstOrDefault(j => j.IsSelected && j.IsProlongType);
                            if (decl_from_pack_prolong != null)
                            {
                                lic.DateExpired = Convert.ToDateTime(decl_from_pack_prolong.DateStartLicense).AddYears(decl_from_pack_prolong.CountOfYears).AddDays(-1);
                            }
                        }
                    }

                    if (dec.IsCancelType)
                    {
                        lic.Status = LicenseStatus.Discontinued;
                    }

                    lic.LastActionWithLicense.EndedDateTime = dt_now;
                }
                else
                {
                    lic = new License() { Comment = "", DateExpired = dt_dec.AddYears(dec.CountYears).AddDays(-1), DateStart = dt_dec, Nom = dec.LicenseNumber, NomOfBook = dec.LicenseNomOfBook, Status = LicenseStatus.Active };
                    repository.License.Add(lic, license_active_type, comp);
                }

                ActionWithLicense action_with_license = new ActionWithLicense() { DateTimeAction = dt_now, EndedDateTime = null, NewStatusOfLicense = dec.IsCancelType ? LicenseStatus.Canceled : LicenseStatus.Active };

                if (dec.IsCancelType)
                {
                    action_with_license.Reason = "Прекращение лицензии по заявлению";
                }
                else
                {
                    if (dec.IsGrantType)
                    {
                        action_with_license.Reason = "Выдача новой лицензии";
                    }
                    else
                    {
                        if (dec.IsProlongType)
                        {
                            if (dec.IsReNewType)
                            {
                                action_with_license.Reason = "Переоформление и продление лицензии";
                            }
                            else
                            {
                                DeclarationViewModelForPack decl_from_pack_renew = dec.DeclarationsInPack.FirstOrDefault(j => j.IsSelected && j.IsReNewType);
                                if (decl_from_pack_renew != null)
                                {
                                    action_with_license.Reason = "Переоформление и продление лицензии";
                                }
                                else
                                {
                                    action_with_license.Reason = "Продление лицензии";
                                }
                            }
                        }
                        else
                        {
                            DeclarationViewModelForPack decl_from_pack_prolong = dec.DeclarationsInPack.FirstOrDefault(j => j.IsSelected && j.IsProlongType);
                            if (decl_from_pack_prolong != null)
                            {
                                action_with_license.Reason = "Переоформление и продление лицензии";
                            }
                            else
                            {
                                action_with_license.Reason = "Переоформление лицензии";
                            }

                        }
                    }
                }

                repository.ActionWithLicense.Add(action_with_license, lic);

                lic.LastActionWithLicense = action_with_license;

                if (!dec.IsCancelType)
                {
                    lic_form = new LicenseForm() { FSRARAddress = comp.FSRARActualAddr, ShortNameCompany = comp.ShortName, AddressCompany = comp.AddressForDocument, DateExpired = lic.DateExpired, Email = comp.Email, FullNameCompany = comp.FullName, INN = comp.INN, KPP = comp.KPP, Nom = dec.LicenseFormNom };
                    repository.LicenseForm.Add(lic_form, action_with_license);
                    lic.LastLicenseForm = lic_form;
                }

                Declaration decl = GetDeclarationFromDeclarationViewModel(dec, DeclarationStatus.Agreed, addr, addr_postal);

                decl.LicenseForm = lic_form;

                SetDeclarationFlags(dec, decl);

                repository.Declaration.Add(decl, license_active_type, leg_f_t, comp, lic);

                decl = repository.Declaration.GetNotDeletedItems().First(j => j.Id == dec.Id);


                List<UnitInDeclaration> now_units_in_dec = repository.UnitInDeclaration.GetItemsForDeclaration(decl.Id).ToList();
                List<UnitInDeclaration> now_units_in_dec_add = now_units_in_dec.Where(j => j.Type == UnitInDeclarationType.Add).ToList();
                List<UnitInDeclaration> now_units_in_dec_remove = now_units_in_dec.Where(j => j.Type == UnitInDeclarationType.Remove).ToList();

                List<UnitInDeclaration> now_units_in_dec_edit = now_units_in_dec.Where(j => j.Type == UnitInDeclarationType.Edit).ToList();

                foreach (var u in now_units_in_dec_add)
                {
                    if (dec.UnitsAdd.FirstOrDefault(j => j.Id == u.Id) == null)
                    {
                        repository.UnitInDeclaration.Remove(u);
                    }
                }

                foreach (var u in now_units_in_dec_remove)
                {
                    if (dec.UnitsRemove.FirstOrDefault(j => j.Id == u.Id) == null)
                    {
                        repository.UnitInDeclaration.Remove(u);
                    }
                }

                foreach (var u in now_units_in_dec_edit)
                {
                    if (dec.UnitsEdit.FirstOrDefault(j => j.Id == u.Id) == null)
                    {
                        repository.UnitInDeclaration.Remove(u);
                    }
                }


                DateTime dt = DateTime.Now;
                ActionWithDeclaration old_awd = repository.ActionWithDeclaration.GetItemsForDeclaration(decl.Id).First(j => j.EndedDateTime == null);
                old_awd.EndedDateTime = dt;

                ActionWithDeclaration awd = new ActionWithDeclaration() { Id = 0, Comm = "", DateTimeAction = dt, EndedDateTime = null, NewStatusOfDeclaration = DeclarationStatus.Agreed };
                repository.ActionWithDeclaration.Add(awd, decl);

                Resolution res = UpdateResolution(license_active_type, dec, decl, false, ResultOfResolution.Positive);

                res.LicenseForm = lic_form;
                res.LinkActionWithLicense(action_with_license, repository.logHelper);

                //List<UnitInDeclaration> now_units_add = repository.UnitInDeclaration.GetAddItemsForDeclaration(decl.Id).ToList();
                //foreach (var u in now_units_add)
                //{
                //    if (dec.UnitsAdd.FirstOrDefault(j => j.Id == u.Id) == null)
                //    {
                //        repository.UnitInDeclaration.Remove(u);
                //    }
                //}

                List<Unit> now_units_company = repository.Unit.GetItemsForCompany(comp_update.Id).ToList();

                List<UnitInLicenseForm> now_units_company_in_current_license_form = prev_lic_form == null ? new List<UnitInLicenseForm>() : prev_lic_form.UnitsInLicenseForm.ToList(); //repository.Unit.GetItemsForCompany(comp.Id).ToList();
                List<UnitInLicenseForm> units_for_new_license_form = new List<UnitInLicenseForm>();

                List<UnitInDeclarationViewModel> all_unit_in_declaration_for_force_update = new List<UnitInDeclarationViewModel>();

                all_unit_in_declaration_for_force_update.AddRange(dec.UnitsAdd.Where(j => j.Status != "Agreed"));
                all_unit_in_declaration_for_force_update.AddRange(dec.UnitsEdit.Where(j => j.Status != "Agreed"));
                all_unit_in_declaration_for_force_update.AddRange(dec.UnitsRemove);

                //обновляем все обособки, которые не влияют на создание новых сущностей
                foreach (var u in all_unit_in_declaration_for_force_update)
                {
                    ModifyLicense mod = repository.ModifyLicense.GetNotDeletedItems().First(j => j.Id == u.ModifyLicense.Id);
                    UnitType unit_type = repository.UnitType.GetNotDeletedItems().First(j => j.Id == u.UnitType.Id);
                    Unit unit = u.UnitId == 0 ? null : now_units_company.First(j => j.Id == u.UnitId);

                    Addr unit_addr = u.id_addr == -1 ? null : repository.Addr.GetNotDeletedItems().First(j => j.Id == u.id_addr);

                    UnitInDeclaration new_unit = new UnitInDeclaration()
                    {
                        FSRARAddr = u.FSRARAddress,
                        AddrAddon = u.GetAddrAddon(),
                        Addr = unit_addr,
                        AddrStr = u.Address,
                        AddressForDocument = u.AddressForDocument,
                        FullName = u.FullName,
                        Id = u.Id,
                        KPP = u.KPP,
                        PostalCode = u.PostalCode,
                        ShortName = u.ShortName,
                        Type = (UnitInDeclarationType)Enum.Parse(typeof(UnitInDeclarationType), u.Type),
                        Status = (UnitInDeclarationStatus)Enum.Parse(typeof(UnitInDeclarationStatus), u.Status),
                        StatusComm = u.StatusComm,
                        HasInternet = u.HasInternet
                    };
                    repository.UnitInDeclaration.Add(new_unit, decl, unit, unit_type, mod);
                }

                List<UnitInDeclarationViewModel> all_add_and_edit_unit_in_declaration = new List<UnitInDeclarationViewModel>();
                List<UnitInDeclarationViewModel> all_edit_unit_in_declaration = new List<UnitInDeclarationViewModel>();
                all_add_and_edit_unit_in_declaration.AddRange(dec.UnitsAdd.Where(j => j.Status == "Agreed"));
                all_add_and_edit_unit_in_declaration.AddRange(dec.UnitsEdit.Where(j => j.Status == "Agreed"));
                all_edit_unit_in_declaration.AddRange(dec.UnitsEdit.Where(j => j.Status == "Agreed"));
                foreach (var dec_in_pack in dec.DeclarationsInPack)
                {
                    if (dec_in_pack.IsSelected)
                    {
                        all_add_and_edit_unit_in_declaration.AddRange(dec_in_pack.UnitsAdd.Where(j => j.Status == "Agreed"));
                        all_add_and_edit_unit_in_declaration.AddRange(dec_in_pack.UnitsEdit.Where(j => j.Status == "Agreed"));
                        all_edit_unit_in_declaration.AddRange(dec_in_pack.UnitsEdit.Where(j => j.Status == "Agreed"));
                    }
                }





                if (dec.IsCancelType || dec.IsReNewType || dec.IsProlongType)
                {

                    List<UnitInDeclarationViewModel> all_remove_unit_in_declaration = new List<UnitInDeclarationViewModel>();

                    all_remove_unit_in_declaration.AddRange(dec.UnitsRemove);

                    foreach (var dec_in_pack in dec.DeclarationsInPack)
                    {
                        if (dec_in_pack.IsSelected)
                        {
                            all_remove_unit_in_declaration.AddRange(dec_in_pack.UnitsRemove);
                        }
                    }


                    if (dec.IsCancelType)
                    {
                        foreach (var u in now_units_company_in_current_license_form)
                        {
                            Unit real_unit = u.Unit;
                            if (real_unit.IsActive)
                            {
                                real_unit.IsActive = false;
                                real_unit.DateEnd = dt_now;
                            }
                        }
                    }
                    else
                    {
                        foreach (var u in now_units_company_in_current_license_form)
                        {
                            Unit real_unit = u.Unit;
                            if (all_remove_unit_in_declaration.FirstOrDefault(j => j.UnitId == real_unit.Id) != null)
                            {
                                if (real_unit.IsActive)
                                {
                                    real_unit.IsActive = false;
                                    real_unit.DateEnd = dt_now;
                                }
                            }
                            else
                            {
                                if (all_edit_unit_in_declaration.FirstOrDefault(j => j.UnitId == real_unit.Id) == null)
                                {
                                    units_for_new_license_form.Add(u);
                                }
                            }
                        }
                    }
                }

                foreach (var u in all_add_and_edit_unit_in_declaration)
                {
                    ModifyLicense mod = repository.ModifyLicense.GetNotDeletedItems().First(j => j.Id == u.ModifyLicense.Id);
                    UnitType unit_type = repository.UnitType.GetNotDeletedItems().First(j => j.Id == u.UnitType.Id);
                    Unit unit = u.UnitId == 0 ? null : now_units_company.First(j => j.Id == u.UnitId);

                    Addr unit_addr = u.id_addr == -1 ? null : repository.Addr.GetNotDeletedItems().First(j => j.Id == u.id_addr);
                    string addr_addon = u.GetAddrAddon();
                    if (!dec.IsCancelType)
                    {

                        if (unit != null)
                        {
                            unit.Addr = unit_addr;
                            unit.AddrStr = u.Address == null ? "" : u.Address;
                            unit.AddressForDocument = u.AddressForDocument == null ? "" : u.AddressForDocument;
                            unit.FullName = u.FullName == null ? "" : u.FullName;
                            unit.IsActive = true;
                            unit.KPP = u.KPP == null ? "" : u.KPP;
                            unit.PostalCode = u.PostalCode == null ? "" : u.PostalCode;
                            unit.ModifyLicense = mod;
                            unit.ShortName = u.ShortName == null ? "" : u.ShortName;
                            unit.UnitType = unit_type;
                            unit.DateEnd = lic.DateExpired;
                            unit.AddrAddon = addr_addon;
                            unit.FSRARAddr = u.FSRARAddress;
                            unit.HasInternet = u.HasInternet;
                        }
                        else
                        {
                            unit = new Unit() { FSRARAddr = u.FSRARAddress, AddrAddon = addr_addon, Addr = unit_addr, AddrStr = u.Address, AddressForDocument = u.AddressForDocument, AddrStrFromImport = "", DateBegin = dt_now, DateEnd = lic.DateExpired, FullName = u.FullName, Id = 0, IsActive = true, KPP = u.KPP, ShortName = u.ShortName, PostalCode = u.PostalCode };

                            repository.Unit.Add(comp, unit_type, mod, unit, repository.UserDB.LicensedActivityTypeContext);
                        }

                        UnitInLicenseForm unit_in_license_form = new UnitInLicenseForm() { FSRARAddress = u.FSRARAddress, AddressForDocument = u.AddressForDocument, Address = u.Address, Id = 0, KPP = u.KPP, Name = u.FullName };

                        repository.UnitInLicenseForm.Add(unit_in_license_form, lic_form, unit, unit_type);

                    }
                    UnitInDeclarationStatus st = (UnitInDeclarationStatus)Enum.Parse(typeof(UnitInDeclarationStatus), u.Status);
                    UnitInDeclaration new_unit = new UnitInDeclaration()
                    {
                        FSRARAddr = u.FSRARAddress,
                        AddrAddon = addr_addon,
                        Addr = unit_addr,
                        AddrStr = u.Address,
                        AddressForDocument = u.AddressForDocument,
                        FullName = u.FullName,
                        Id = u.Id,
                        KPP = u.KPP,
                        PostalCode = u.PostalCode,
                        ShortName = u.ShortName,
                        Type = (UnitInDeclarationType)Enum.Parse(typeof(UnitInDeclarationType), u.Type),
                        Status = st,
                        StatusComm = "",
                        HasInternet = u.HasInternet
                    };
                    repository.UnitInDeclaration.Add(new_unit, decl, unit, unit_type, mod);
                }

                if (!dec.IsGrantType && !dec.IsCancelType)
                {
                    //добавляем обособки из предыдущего бланка

                    foreach (var now_unit_in_license_form_old in units_for_new_license_form)
                    {
                        Unit now_unit = now_unit_in_license_form_old.Unit;

                        now_unit.DateEnd = lic.DateExpired;

                        UnitInLicenseForm unit_in_license_form = new UnitInLicenseForm()
                        {
                            FSRARAddress = now_unit.FSRARAddr,
                            AddressForDocument = now_unit.AddressForDocument,
                            Address = now_unit.AddrStr,
                            Id = 0,
                            KPP = now_unit.KPP,
                            Name = now_unit.FullName
                        };

                        repository.UnitInLicenseForm.Add(unit_in_license_form, lic_form, now_unit, now_unit.UnitType);
                    }
                }

                UpdatePaymentOrders(dec, decl);

                UpdateDeclarationsPackAndUpdateResolutionPaymentOrdersAllAndSetStatus(dec, res, decl.PaymentOrdersAll, DeclarationStatus.Agreed);

                dec.TagsTable.SaveTableTag(repository);

                var del_files = DocumentViewModel.UpdateDocuments(dec.Documents, repository);
                foreach (var doc in dec.Documents)
                {
                    Document doc_db = repository.Document.GetNotDeletedItems().First(j => j.Id == doc.Id);
                    comp.LinkDocument(doc_db, repository.logHelper);
                }
                repository.SaveChanges();
                DocumentViewModel.RemoveFileDocuments(del_files);
            }
            catch (Exception e)
            {
                errMsg = Cascade.Utils.Common.GetFullExceptionMessage(e);
            }
            return Json(new
            {
                ErrMsg = errMsg,
            }, JsonRequestBehavior.AllowGet);
        }

        private void SetDeclarationFlags(DeclarationViewModel dec, Declaration decl)
        {
            if (dec.IsCloneType)
            {
                decl.Type = decl.Type | DeclarationType.Clone;
            }
            if (dec.IsGrantType)
            {
                decl.Type = decl.Type | DeclarationType.Grant;
            }

            if (dec.IsCancelType)
            {
                decl.Type = decl.Type | DeclarationType.Cancel;
            }

            if (dec.IsProlongType)
            {
                decl.Type = decl.Type | DeclarationType.Prolong;
            }

            if (dec.IsReNewType)
            {
                decl.Type = decl.Type | DeclarationType.Renew;
            }

            if (dec.IsEMailGetResult)
            {
                decl.GetResultType = decl.GetResultType | GetResultType.Email;
            }

            if (dec.IsMailGetResult)
            {
                decl.GetResultType = decl.GetResultType | GetResultType.Mail;
            }

            if (dec.IsPersonGetResult)
            {
                decl.GetResultType = decl.GetResultType | GetResultType.Personal;
            }

            if (dec.IsPhoneInformation)
            {
                decl.GetInformationType = decl.GetInformationType | TypeOfGetInformation.Phone;
            }

            if (dec.IsEmailInformation)
            {
                decl.GetInformationType = decl.GetInformationType | TypeOfGetInformation.Email;
            }
        }

        private Declaration GetDeclarationFromDeclarationViewModel(DeclarationViewModel dec, DeclarationStatus status, Addr addr, Addr addr_postal)
        {
            //if (dec.ReasonForRenews == null)
            //{
            //    dec.ReasonForRenews = new List<DictionaryRecordViewModel>();
            //}
            var rfrCheckedList = dec.ReasonForRenews.Where(x => x.IsChecked).Select(x => x.Id).ToList();
            var rbCheckedList = dec.RefuseBasises.Where(x => x.IsChecked).Select(x => x.Id).ToList();
            Declaration decl = new Declaration()
            {
                MasterName = dec.MasterName,
                VakeelName = dec.VakeelName,
                ActualAddr = addr,
                ActualAddrStr = dec.Company.Address,
                AddressForDocument = dec.Company.AddressForDocument,
                PostalAddr = addr_postal,
                PostalAddrStr = dec.Company.PostalAddress,
                PostalCode = dec.Company.PostalCode,
                BanksName = dec.Company.BanksName,
                CheckingAccount = dec.Company.CheckingAccount,
                Email = dec.Company.Email,
                Fax = dec.Company.Fax,
                FullNameCompany = dec.Company.FullName,
                NameCompany = dec.Company.CompanyName,
                INN = dec.Company.INN,
                KPP = dec.Company.KPP,
                OGRN = dec.Company.OGRN,
                Phone = dec.Company.Phone,
                ShortNameCompany = dec.Company.ShortName,
                Id = dec.Id,
                CountOfYears = dec.CountYears,
                DateDeclaration = status == DeclarationStatus.Draft ? DateTime.Now : Convert.ToDateTime(dec.DateTimeDeclaration),
                ReasonForRenews = repository.ReasonForRenew.GetNotDeletedItems().Where(x => rfrCheckedList.Contains(x.Id)).ToList(),
                
                OtherReason = dec.OtherReason,
                Comm = dec.Comm,
                Status = status,
                LicenseFormNom = dec.LicenseFormNom,
                LicenseNom = dec.LicenseNumber,
                LicenseNomOfBook = dec.LicenseNomOfBook,
                FSRARActualAddr = dec.Company.FSRARAddress,
                FSRARPostalAddr = dec.Company.FSRARPostalAddress,
                ActualAddrAddon = dec.Company.GetAddrAddon(),
                PostalAddrAddon = dec.Company.GetPostalAddrAddon(),
                SmevId = dec.SmevId
            };

            return decl;
        }

        private Declaration UpdateUnitInDeclaration(DeclarationViewModel dec, Declaration d)
        {
            Declaration decl = d;
            if (dec.Id != 0)
            {

                decl = repository.Declaration.GetNotDeletedItems().First(j => j.Id == dec.Id);


                List<UnitInDeclaration> now_units = repository.UnitInDeclaration.GetItemsForDeclaration(decl.Id).ToList();
                List<UnitInDeclaration> now_units_add = now_units.Where(j => j.Type == UnitInDeclarationType.Add).ToList();
                List<UnitInDeclaration> now_units_remove = now_units.Where(j => j.Type == UnitInDeclarationType.Remove).ToList();
                List<UnitInDeclaration> now_units_edit = now_units.Where(j => j.Type == UnitInDeclarationType.Edit).ToList();
                foreach (var u in now_units_add)
                {
                    if (dec.UnitsAdd.FirstOrDefault(j => j.Id == u.Id) == null)
                    {
                        repository.UnitInDeclaration.Remove(u);
                    }
                }

                foreach (var u in now_units_remove)
                {
                    if (dec.UnitsRemove.FirstOrDefault(j => j.Id == u.Id) == null)
                    {
                        repository.UnitInDeclaration.Remove(u);
                    }
                }

                foreach (var u in now_units_edit)
                {
                    if (dec.UnitsEdit.FirstOrDefault(j => j.Id == u.Id) == null)
                    {
                        repository.UnitInDeclaration.Remove(u);
                    }
                }
            }


            UpdateUnitTypeInDeclaration(dec.UnitsAdd, UnitInDeclarationType.Add, decl);
            UpdateUnitTypeInDeclaration(dec.UnitsRemove, UnitInDeclarationType.Remove, decl);
            UpdateUnitTypeInDeclaration(dec.UnitsEdit, UnitInDeclarationType.Edit, decl);

            foreach (var dec_in_pack in dec.DeclarationsInPack)
            {
                if (dec_in_pack.IsSelected)
                {
                    foreach (var unit_add in dec_in_pack.UnitsAdd)
                    {
                        UnitInDeclaration uid = repository.UnitInDeclaration.GetNotDeletedItems().FirstOrDefault(j => j.Id == unit_add.Id);
                        if (uid != null)
                        {

                            uid.Status = (UnitInDeclarationStatus)Enum.Parse(typeof(UnitInDeclarationStatus), unit_add.Status);
                            uid.StatusComm = uid.Status == UnitInDeclarationStatus.Agreed ? "" : Utils.NormalizeString(unit_add.StatusComm);
                        }
                    }
                    foreach (var unit_edit in dec_in_pack.UnitsEdit)
                    {
                        UnitInDeclaration uid = repository.UnitInDeclaration.GetNotDeletedItems().FirstOrDefault(j => j.Id == unit_edit.Id);
                        if (uid != null)
                        {
                            uid.Status = (UnitInDeclarationStatus)Enum.Parse(typeof(UnitInDeclarationStatus), unit_edit.Status);
                            uid.StatusComm = uid.Status == UnitInDeclarationStatus.Agreed ? "" : Utils.NormalizeString(unit_edit.StatusComm);
                        }
                    }
                    //Вроде как подтверждения для удаления не требуется
                    //foreach (var unit_remove in dec_in_pack.UnitsRemove)
                    //{
                    //    UnitInDeclaration uid = repository.UnitInDeclaration.GetNotDeletedItems().FirstOrDefault(j => j.Id == unit_remove.Id);
                    //    if (uid != null)
                    //    {
                    //        uid.Status = (UnitInDeclarationStatus)Enum.Parse(typeof(UnitInDeclarationStatus), unit_remove.Status);
                    //        uid.StatusComm = unit_remove.StatusComm;
                    //    }
                    //}
                }
            }


            return decl;
        }

        private void UpdateUnitTypeInDeclaration(List<UnitInDeclarationViewModel> units_vm, UnitInDeclarationType type_unit_in_declaration, Declaration decl)
        {
            foreach (var u in units_vm)
            {
                ModifyLicense mod = repository.ModifyLicense.GetNotDeletedItems().First(j => j.Id == u.ModifyLicense.Id);
                UnitType unit_type = repository.UnitType.GetNotDeletedItems().First(j => j.Id == u.UnitType.Id);
                Unit unit = u.UnitId == 0 ? null : repository.Unit.GetNotDeletedItems().First(j => j.Id == u.UnitId);
                Addr unit_addr = u.id_addr == -1 ? null : repository.Addr.GetNotDeletedItems().First(j => j.Id == u.id_addr);
                string addr_addon = Utils.NormalizeString(u.AddressHouse) + "|" + Utils.NormalizeString(u.AddressBuildNom) + "|" + Utils.NormalizeString(u.AddressLitera) + "|" + Utils.NormalizeString(u.AddressRoomNom);
                UnitInDeclarationStatus st = (UnitInDeclarationStatus)Enum.Parse(typeof(UnitInDeclarationStatus), u.Status);
                UnitInDeclaration new_unit = new UnitInDeclaration()
                {
                    AddrAddon = addr_addon,
                    FSRARAddr = u.FSRARAddress,
                    Addr = unit_addr,
                    AddrStr = u.Address,
                    AddressForDocument = u.AddressForDocument,
                    FullName = u.FullName,
                    Id = u.Id,
                    KPP = u.KPP,
                    PostalCode = u.PostalCode,
                    ShortName = u.ShortName,
                    Type = type_unit_in_declaration,
                    Status = st,
                    StatusComm = st == UnitInDeclarationStatus.Agreed ? "" : u.StatusComm,
                    HasInternet = u.HasInternet
                };

                repository.UnitInDeclaration.Add(new_unit, decl, unit, unit_type, mod);
            }
        }

        private void UpdatePaymentOrders(DeclarationViewModel dec, Declaration d)
        {
            var not_deleted_payment_orders = dec.PaymentOrders.Where(j => !j.IsDeleted).ToList();
            if (d.Id != 0)
            {
                List<PaymentOrder> now_payment_orders = repository.PaymentOrder.GetItemsForDeclaration(d.Id).ToList();
                foreach (var po in now_payment_orders)
                {
                    if (not_deleted_payment_orders.FirstOrDefault(j => j.Id == po.Id) == null)
                    {
                        repository.PaymentOrder.Remove(po);
                    }
                }
            }
            var payment_order_all_arr = (from p in not_deleted_payment_orders select p.Nom + " от " + Convert.ToDateTime(p.Date).ToShortDateString() + " (" + p.Summ.ToString() + ")");
            string payment_order_all = string.Join(" ; ", payment_order_all_arr);
            d.PaymentOrdersAll = payment_order_all;
            foreach (var po in not_deleted_payment_orders)
            {

                PaymentOrder new_payment_order = new PaymentOrder() { Comm = po.Comm, Date = Convert.ToDateTime(po.Date).Date, Id = po.Id, Nom = po.Nom, Summ = po.Summ };
                repository.PaymentOrder.Add(d, new_payment_order);
            }
        }

        private void TagsTableAndDocumentsSaveAndCommit(DeclarationViewModel dec, Declaration decl)
        {
            if (decl.Id > 0)
            {
                dec.TagsTable.SaveTableTag(repository);
                var del_files = DocumentViewModel.UpdateDocuments(dec.Documents, repository);
                repository.SaveChanges();
                DocumentViewModel.RemoveFileDocuments(del_files);
            }
            else
            {
                repository.SaveChanges();
                if (dec.TagsTable != null)
                {
                    dec.TagsTable.Id_entity = decl.Id;
                    dec.TagsTable.SaveTableTag(repository);
                    repository.SaveChanges();
                }
            }
        }

        private void GenerateDeclarationNom(Declaration decl)
        {
            int reg_nom = 1;
            int reg_nom_year = 1;

            var now_dec = repository.Declaration.Items.Where(j => j.IsDeleted != true && j.Status != DeclarationStatus.Draft).OrderByDescending(j => j.RegNom).Take(1).Select(j => new { id = j.Id, nom = j.RegNom, nom_year = j.RegNomYear, dt = j.DateDeclaration }).ToList();
            if (now_dec.Count > 0)
            {
                var now_dec_last = now_dec.First();
                reg_nom = now_dec_last.nom + 1;

                if (now_dec_last.dt.Year >= DateTime.Now.Year)
                {
                    reg_nom_year = now_dec_last.nom_year + 1;
                }
            }
            decl.SetNewNom(reg_nom, reg_nom_year, repository.logHelper);
        }

        private void CheckClonesDeclaration(Declaration decl)
        {
            Thread.Sleep(100);
            var clones = repository.Declaration.Items.Where(j => j.Id != decl.Id && j.RegNom == decl.RegNom).OrderBy(j => j.DateDeclaration).ToList();
            if (clones.Count > 0)
            {
                int i = 0;
                while (i < clones.Count && decl.DateDeclaration > clones[i].DateDeclaration)
                {
                    i++;
                }
                if (i > 0)
                {
                    repository.logHelper.Log(Information.ChangeDeclarationNomByCollision, String.Format("У заявления с id = {0} был скорректирван рег номер на {1}, количество клонов при проверке: {2}", decl.Id, i, clones.Count));
                    int reg_nom = decl.RegNom + i;
                    int reg_nom_year = decl.RegNomYear + i;
                    decl.SetNewNom(reg_nom, reg_nom_year, repository.logHelper);
                    repository.SaveChanges();
                }
            }
        }

        private Resolution UpdateResolution(LicensedActivityType license_activity_type, DeclarationViewModel dec, Declaration decl, bool is_draft, ResultOfResolution res_of_res)
        {
            Resolution res = new Resolution()
            {
                Comm = dec.ResolutionComm,
                RefuseComment = dec.RefuseComm,
                Date = Convert.ToDateTime(dec.ResolutionDate),
                Id = dec.ResolutionId,
                IsDraft = is_draft,
                RegNom = is_draft ? 0 : repository.Resolution.GenerateResolutionNom(),
                CreatorName = repository.UserDB != null ? repository.UserDB.FIO : "System",
                CompanyINNAll = dec.Company.INN,
                CompanyShortNameAll = dec.Company.ShortName,
                LicenseFormNomAll = dec.LicenseFormNom,
                LicenseNomAll = dec.LicenseNumber,
                LicenseNomOfBookAll = dec.LicenseNomOfBook,
                LicenceApproverPeopleId = dec.LicenceApprover != null && dec.LicenceApprover.Id != 0 ? dec.LicenceApprover.Id : (int?)null,
                ResolutionCommissionChairmanPeopleId = dec.ResolutionCommissionChairman != null && dec.ResolutionCommissionChairman.Id != 0 ? dec.ResolutionCommissionChairman.Id : (int?)null,
                TypeOfType = (TypeOfTypeOfResolution)Enum.Parse(typeof(TypeOfTypeOfResolution), dec.TypeOfType),
                Result = res_of_res
            };
            //dec.ResolutionComm
            res = repository.Resolution.Add(license_activity_type, res);

            res.LinkDeclaration(decl, repository.logHelper);

            List<TypeOfResolution> all_types_of_resolution = repository.TypeOfResolution.Items.ToList();
            List<People> all_commissioners = repository.People.GetNotDeletedItems().ToList();
            //--------
            List<RefuseBasis> all_basises = repository.RefuseBasis.GetNotDeletedItems().ToList();

            if (res.Id == 0)
            {
                foreach (var tor in dec.ResolutionTypes)
                {
                    if(res.TypeOfType == TypeOfTypeOfResolution.ProlongRenew)
                    {
                        if (tor.Name == "Продление") tor.IsChecked = true;
                        if (tor.Name == "Переоформление") tor.IsChecked = true;
                    }

                    if (res.TypeOfType == TypeOfTypeOfResolution.Renewal)
                    {
                        if (tor.Name == "Переоформление") tor.IsChecked = true;
                    }


                    if (res.TypeOfType == TypeOfTypeOfResolution.Prolong)
                    {
                        if (tor.Name == "Продление") tor.IsChecked = true;
                    }

                    if (tor.IsChecked)
                    {
                        TypeOfResolution tor_db = all_types_of_resolution.FirstOrDefault(j => j.Id == tor.Id);
                        if (tor_db != null)
                        {
                            tor_db.LinkResolution(res, repository.logHelper);
                        }
                    }
                }
                foreach (var com in dec.ResolutionCommissioners)
                {
                    if (com.IsChecked)
                    {
                        People commissioner = repository.People.GetNotDeletedItems().FirstOrDefault(j => j.Id == com.Id);
                        if (commissioner != null)
                        {
                            res.LinkResolutionCommissioner(commissioner, repository.logHelper);
                        }
                    }
                }
                //------------------------------------------------------------------
                foreach (var rb in dec.RefuseBasises)
                {
                    if (rb.IsChecked)
                    {
                        RefuseBasis basis= repository.RefuseBasis.GetNotDeletedItems().FirstOrDefault(j => j.Id == rb.Id);
                        if (basis != null)
                        {
                            res.LinkRefuseBasis(basis, repository.logHelper);
                        }
                    }
                }
            }
            else
            {
                List<TypeOfResolution> now_type_of_resolution_in_db = res.TypeOfResolutions.ToList();
                foreach (var tor in dec.ResolutionTypes)
                {
                    if (tor.IsChecked)
                    {
                        TypeOfResolution tor_db = all_types_of_resolution.FirstOrDefault(j => j.Id == tor.Id);
                        if (tor_db != null)
                        {
                            tor_db.LinkResolution(res, repository.logHelper);
                        }
                    }
                    else
                    {
                        TypeOfResolution tor_db_for_remove = now_type_of_resolution_in_db.FirstOrDefault(j => j.Id == tor.Id);
                        if (tor_db_for_remove != null)
                        {
                            tor_db_for_remove.UnLinkResolution(res, repository.logHelper);
                        }
                    }
                }

                List<People> dbResolutionCommissioners = res.ResolutionCommissioners.ToList();
                foreach (var com in dec.ResolutionCommissioners)
                {
                    if (com.IsChecked)
                    {
                        People commissioner = all_commissioners.FirstOrDefault(j => j.Id == com.Id);
                        if (commissioner != null)
                        {
                            res.LinkResolutionCommissioner(commissioner, repository.logHelper);
                        }
                    }
                    else
                    {
                        People commissioner = dbResolutionCommissioners.FirstOrDefault(j => j.Id == com.Id);
                        if (commissioner != null)
                        {
                            res.UnlinkResolutionCommissioner(commissioner, repository.logHelper);
                        }
                    }
                }
                //------------------------------------------------------------
                List<RefuseBasis> dbRefuseBasises = res.RefuseBasises.ToList();
                foreach (var rb in dec.RefuseBasises)
                {
                    if (rb.IsChecked)
                    {
                        RefuseBasis basis = all_basises.FirstOrDefault(j => j.Id == rb.Id);
                        if (basis != null)
                        {
                            res.LinkRefuseBasis(basis, repository.logHelper);
                        }
                    }
                    else
                    {
                        RefuseBasis basis = dbRefuseBasises.FirstOrDefault(j => j.Id == rb.Id);
                        if (basis != null)
                        {
                            res.UnlinkRefuseBasises(basis, repository.logHelper);
                        }
                    }
                }
            }

            res.TypeOfResolutionsAll = String.Join("; ", dec.ResolutionTypes.Where(j => j.IsChecked).Select(j => j.Name));

            return res;
        }

        private void UpdateDeclarationsPackAndUpdateResolutionPaymentOrdersAllAndSetStatus(DeclarationViewModel dec, Resolution res, string NowDeclarationPaymentOrdersAll, DeclarationStatus status)
        {
            List<string> payment_orders_all_list = new List<string>();

            if (NowDeclarationPaymentOrdersAll != null && NowDeclarationPaymentOrdersAll != "")
            {
                payment_orders_all_list.Add(NowDeclarationPaymentOrdersAll);
            }

            foreach (var declaration in dec.DeclarationsInPack)
            {
                Declaration decl_db = repository.Declaration.GetNotDeletedItems().First(j => j.Id == declaration.Id);
                if (declaration.IsSelected)
                {
                    if (decl_db.Resolution != res)
                    {
                        res.LinkDeclaration(decl_db, repository.logHelper);
                    }
                    if (decl_db.PaymentOrdersAll != "")
                    {
                        payment_orders_all_list.Add(decl_db.PaymentOrdersAll);
                    }
                    repository.logHelper.Log(UpdateRecord.Declaration, decl_db.Id, "Status", decl_db.Status.ToString(), status.ToString());
                    decl_db.Status = status;
                    DateTime dt = DateTime.Now;
                    ActionWithDeclaration old_awd = decl_db.ActionsWithDeclaration.First(j => j.EndedDateTime == null);
                    old_awd.EndedDateTime = dt;

                    ActionWithDeclaration awd = new ActionWithDeclaration() { Id = 0, Comm = "Заявление закрыто пакетом с заявлением № " + dec.RegNomAll, DateTimeAction = dt, EndedDateTime = null, NewStatusOfDeclaration = status };
                    repository.ActionWithDeclaration.Add(awd, decl_db);
                }
                else
                {
                    if (decl_db.Resolution == res)
                    {
                        //создаем новое призрачное решение
                        Resolution new_res = new Resolution()
                        {
                            Comm = "",
                            RefuseComment = "",
                            Date = DateTime.Now,
                            Id = 0,
                            IsDraft = true,
                            PaymentOrdersAll = decl_db.PaymentOrdersAll,
                            RegNomAll = "",
                            LicenseNomOfBookAll = "",
                            CompanyShortNameAll = "",
                            CompanyINNAll = "",
                            LicenseFormNomAll = "",
                            LicenseNomAll = "",
                            TypeOfResolutionsAll = ""
                        };
                        repository.Resolution.Add(repository.UserDB.LicensedActivityTypeContext, new_res);
                        new_res.LinkDeclaration(decl_db, repository.logHelper);
                    }
                }
            }

            res.PaymentOrdersAll = String.Join("; ", payment_orders_all_list);
        }


        private void UpdateDeclarationsPackAndUpdateResolutionPaymentOrdersAll(DeclarationViewModel dec, Resolution res, string NowDeclarationPaymentOrdersAll)
        {
            List<string> payment_orders_all_list = new List<string>();

            if (!String.IsNullOrEmpty(NowDeclarationPaymentOrdersAll))
            {
                payment_orders_all_list.Add(NowDeclarationPaymentOrdersAll);
            }

            foreach (var declaration in dec.DeclarationsInPack)
            {
                Declaration decl_db = repository.Declaration.GetNotDeletedItems().First(j => j.Id == declaration.Id);
                if (declaration.IsSelected)
                {
                    if (decl_db.Resolution != res)
                    {
                        res.LinkDeclaration(decl_db, repository.logHelper);
                    }
                    if (decl_db.PaymentOrdersAll != "")
                    {
                        payment_orders_all_list.Add(decl_db.PaymentOrdersAll);
                    }
                }
                else
                {
                    if (decl_db.Resolution == res)
                    {
                        //создаем новое призрачное решение
                        Resolution new_res = new Resolution()
                        {
                            Comm = "",
                            RefuseComment = "",
                            Date = DateTime.Now,
                            Id = 0,
                            IsDraft = true,
                            PaymentOrdersAll = decl_db.PaymentOrdersAll,
                            RegNomAll = "",
                            LicenseNomOfBookAll = "",
                            CompanyShortNameAll = "",
                            CompanyINNAll = "",
                            LicenseFormNomAll = "",
                            LicenseNomAll = "",
                            TypeOfResolutionsAll = ""
                        };
                        repository.Resolution.Add(repository.UserDB.LicensedActivityTypeContext, new_res);
                        new_res.LinkDeclaration(decl_db, repository.logHelper);
                    }
                }
            }

            res.PaymentOrdersAll = String.Join("; ", payment_orders_all_list);
        }


        [HttpPost]
        public JsonResult RemoveDeclarationDraft(int id)
        {
            Declaration decl = repository.Declaration.GetNotDeletedItems().FirstOrDefault(j => j.Id == id);
            if (decl == null)
            {
                return Json("Данный черновик заявления отсутствует в базе данных: возможно его кто-то уже удалил", JsonRequestBehavior.AllowGet);
            }
            if (decl.Status != DeclarationStatus.Draft)
            {
                return Json("Нельзя удалить этот черновик: его уже запустили в работу", JsonRequestBehavior.AllowGet);
            }
            repository.Declaration.Remove(decl);

            foreach (var unit_in_decl in decl.UnitsInDeclaration.ToList())
            {
                repository.UnitInDeclaration.Remove(unit_in_decl);
            }

            foreach (var revision in decl.Revisions.ToList())
            {
                foreach (var unit_in_revision in revision.UnitsInRevision.ToList())
                {
                    repository.UnitInRevision.Remove(unit_in_revision);
                }
                repository.Revision.Remove(revision);

            }
            //УТОЧНИТЬ!!!
            foreach (var po in decl.PaymentOrders.ToList())
            {
                repository.PaymentOrder.Remove(po);
            }

            foreach (var document in decl.Documents.ToList())
            {
                repository.Document.Remove(document);
            }

            try
            {
                repository.SaveChanges();
                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json("Ошибка сохранения в базу данных", JsonRequestBehavior.AllowGet);
            }
        }
    }
}