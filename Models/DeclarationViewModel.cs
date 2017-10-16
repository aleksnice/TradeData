using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Cascade.Licensing.Domain;
using Cascade.Licensing.Domain.Abstract;
using Cascade.Licensing.Domain.Concrete;

namespace Cascade.Licensing.WebUI.Models
{

    public class DeclarationViewModelForPack
    {
        public int Id { get; set; }
        public string RegNom { get; set; }
        public string DateDeclaration { get; set; }
        public string DateStartLicense { get; set; }
        //public string DateExpiredLicense { get; set; }
        public int CountOfYears { get; set; }
        public string ActionDeclarationRus { get; set; }
        public string CreatorName { get; set; }
        public string Comm { get; set; }

        public bool IsSelected { get; set; }

        public bool IsGrantType { get; set; }
        public bool IsProlongType { get; set; }
        public bool IsReNewType { get; set; }
        public bool IsCancelType { get; set; }
        public bool IsClone { get; set; }

        public List<UnitInDeclarationViewModel> UnitsAdd { get; set; }
        public List<UnitInDeclarationViewModel> UnitsRemove { get; set; }
        public List<UnitInDeclarationViewModel> UnitsEdit { get; set; }

        public DeclarationViewModelForPack()
        {
            UnitsAdd = new List<UnitInDeclarationViewModel>();
            UnitsEdit = new List<UnitInDeclarationViewModel>();
            UnitsRemove = new List<UnitInDeclarationViewModel>();
        }

        public DeclarationViewModelForPack(Declaration dec, int ResolutionId)
        {
            Id = dec.Id;
            RegNom = dec.RegNomAll;
            DateDeclaration = dec.DateDeclaration.ToString();
            DateStartLicense = (dec.LicenseBaseDateStart.HasValue ? dec.LicenseBaseDateStart.Value : dec.Resolution.Date).ToString();
            //DateExpiredLicense = dec.LicenseBaseDateExpired?.ToString();
            CountOfYears = dec.CountOfYears;
            CreatorName = dec.CreatorName;
            Comm = dec.Comm;
            IsSelected = dec.ResolutionId.HasValue ? dec.ResolutionId.Value == ResolutionId : false;

            //if (IsSelected)
            //{
            List<UnitInDeclaration> all_unit_in_declaration = dec.UnitsInDeclaration.ToList();
            UnitsAdd = UnitInDeclarationViewModel.GetListViewModel(all_unit_in_declaration.Where(j => j.Type == UnitInDeclarationType.Add).AsQueryable());
            UnitsRemove = UnitInDeclarationViewModel.GetListViewModel(all_unit_in_declaration.Where(j => j.Type == UnitInDeclarationType.Remove).AsQueryable());
            UnitsEdit = UnitInDeclarationViewModel.GetListViewModel(all_unit_in_declaration.Where(j => j.Type == UnitInDeclarationType.Edit).AsQueryable());
            //}

            IsGrantType = dec.Type.HasFlag(DeclarationType.Grant);
            IsProlongType = dec.Type.HasFlag(DeclarationType.Prolong);
            IsReNewType = dec.Type.HasFlag(DeclarationType.Renew);
            IsCancelType = dec.Type.HasFlag(DeclarationType.Cancel);
            IsClone = dec.Type.HasFlag(DeclarationType.Clone);
            ActionDeclarationRus = DeclarationViewModel.GetRusDeclarationType(dec.Type);
        }

        public static List<DeclarationViewModelForPack> GetListViewModel(IQueryable<Declaration> declaration_list, int ResolutionId)
        {
            List<DeclarationViewModelForPack> res = new List<DeclarationViewModelForPack>();

            foreach (var d in declaration_list.ToList())
            {
                //, legal_form_type = j.Company.LegalFormType, addr = j.Company.ActualAddr
                res.Add(new DeclarationViewModelForPack(d, ResolutionId));

            }
            return res;
        }
    }

    public class FilteredDeclarationListViewModel
    {
        public int Count { get; set; }

        public List<DeclarationViewModel> declaration_list { get; set; }


    }

    public class DeclarationRights 
    {
        public bool CanSetDeclarationStatusInWork { get; set; }
        public bool CanSetDeclarationStatusRevision { get; set; }
        public bool CanSetDeclarationStatusCompleted { get; set; }
    }

    public class DeclarationViewModel
    {
        public int Id { get; set; }
        public String SmevId { get; set; }
        public CompanyViewModel Company { get; set; }
        public string CompanyNameNow { get; set; }
        public string CompanyFullNameNow { get; set; }
        public string CompanyShortNameNow { get; set; }
        public string LicensedActivityTypeName { get; set; }
        public int LicensedActivityTypeId { get; set; }

        public DeclarationRights Rights { get; set; }
        public List<DeclarationViewModelForPack> DeclarationsInPack { get; set; }
        public ValueTagTableViewModel TagsTable { get; set; }
        public List<ActionWithDeclarationViewModelForDeclaration> ActionsWithDeclaration { get; set; }
        public List<DocumentViewModel> Documents { get; set; }

        public List<PaymentOrderViewModelForDeclaration> PaymentOrders { get; set; }
        public List<UnitInDeclarationViewModel> UnitsAdd { get; set; }
        public List<UnitInDeclarationViewModel> UnitsRemove { get; set; }
        public List<UnitInDeclarationViewModel> UnitsEdit { get; set; }
        public List<UnitInDeclarationViewModel> UnitsCanceled { get; set; }
        public List<RevisionViewModel> Revisions { get; set; }

        public string TypeCreateDeclaration { get; set; }
        public string RegNomAll { get; set; }
        public string OtherReason { get; set; }
        public string DateDeclaration { get; set; }
        public string TimeDeclaration { get; set; }
        public string DateTimeDeclaration { get; set; }

        public string AllTypeDeclarationRus { get; set; }
        public bool IsGrantType { get; set; }
        public bool IsProlongType { get; set; }
        public bool IsReNewType { get; set; }
        public bool IsCancelType { get; set; }
        public bool IsCloneType { get; set; }

        public string DateCreatedStr { get; set; }
        public string Creator { get; set; }

        public string Status { get; set; }
        public string StatusRus { get; set; }
        public bool IsClosed { get; set; }
        public int CountYears { get; set; }
        public string ActionDeclarationRus { get; set; }

        public int LicenseId { get; set; }
        public string LicenseNumber { get; set; }
        public string LicenseDateExpired { get; set; }
        public string LicenseDateStart { get; set; }
        public string LicenseNomOfBook { get; set; }

        public string MasterName { get; set; }
        public string VakeelName { get; set; }
        public int LicenseFormId { get; set; }
        public string LicenseFormNom { get; set; }

        public bool IsPhoneInformation { get; set; }
        public bool IsEmailInformation { get; set; }
        public bool IsMailGetResult { get; set; }
        public bool IsEMailGetResult { get; set; }
        public bool IsPersonGetResult { get; set; }

        public List<DictionaryRecordViewModel> ReasonForRenews { get; set; }
        public string PaymentOrdersAll { get; set; }

        public int ResolutionId { get; set; }
        public string ResolutionNom { get; set; }
        public string ResolutionDate { get; set; }
        public string ResolutionComm { get; set; }
        public string RefuseComm { get; set; }

        public DictionaryRecordViewModel LicenceApprover { get; set; }

        public List<DictionaryRecordViewModel> RefuseBasises { get; set; }

        public DictionaryRecordViewModel ResolutionCommissionChairman { get; set; }
        public List<DictionaryRecordViewModel> ResolutionCommissioners { get; set; }

        public List<TypeOfResolutionForResolutionViewModel> ResolutionTypes { get; set; }
        public string ResolutionTypesAll { get; set; }
        public string TypeOfType { get; set; }
        public string Comm { get; set; }

        public DeclarationViewModel()
        {
            UnitsAdd = new List<UnitInDeclarationViewModel>();
            UnitsRemove = new List<UnitInDeclarationViewModel>();
            UnitsEdit = new List<UnitInDeclarationViewModel>();
            UnitsCanceled = new List<UnitInDeclarationViewModel>();
            Documents = new List<DocumentViewModel>();
            PaymentOrders = new List<PaymentOrderViewModelForDeclaration>();
            ActionsWithDeclaration = new List<ActionWithDeclarationViewModelForDeclaration>();
            DeclarationsInPack = new List<DeclarationViewModelForPack>();
            ResolutionTypes = new List<TypeOfResolutionForResolutionViewModel>();
            RefuseBasises = new List<DictionaryRecordViewModel>();
            ReasonForRenews = new List<DictionaryRecordViewModel>();
        }

        private static String GetXmlNodeNullableValue(XElement node, String nodeName, Boolean requied = true)
        {
            if (node.Element(nodeName) != null)
            {
                return node.Element(nodeName).Value;
            }
            else
            {
                if (requied)
                {
                    throw new Exception(String.Format("Обязательное поле {0} не заполнено.", nodeName));
                }
                return "";
            }
        }

        private static int GetXmlNodeIntValue(XElement node, String nodeName, Boolean requied = true)
        {
            String value = GetXmlNodeNullableValue(node, nodeName, false);
            int result = 0;
            if (!String.IsNullOrEmpty(value) && !int.TryParse(value, out result))
            {
                throw new Exception(String.Format("Неверное значение поля {0}: {1}", nodeName, value));
            }
            return result;
        }

        private static Double GetXmlNodeDoubleValue(XElement node, String nodeName, Boolean requied = true)
        {
            String value = GetXmlNodeNullableValue(node, nodeName, false);
            Double result = 0;
            if (!String.IsNullOrEmpty(value) && !Double.TryParse(value, out result))
            {
                throw new Exception(String.Format("Неверное значение поля {0}: {1}", nodeName, value));
            }
            return result;
        }

        private static Boolean GetXmlNodeBooleanValue(XElement node, String nodeName)
        {
            String value = GetXmlNodeNullableValue(node, nodeName, false);
            Boolean result = false;
            if (!String.IsNullOrEmpty(value) && !Boolean.TryParse(value, out result))
            {
                throw new Exception(String.Format("Неверное значение поля {0}: {1}", nodeName, value));
            }
            return result;
        }

        //public static DeclarationViewModel GetDeclarationViewModelFromXML(IRepository repository, XElement xDec)
        //{
        //    DeclarationViewModel result = new DeclarationViewModel();
        //    result.SmevId = GetXmlNodeNullableValue(xDec, "SmevId");
        //    Declaration decDB = repository.Declaration.Items.FirstOrDefault(x => x.IsDeleted != true && x.SmevId == result.SmevId);
        //    if (decDB != null)
        //    {
        //        if (decDB.Status == DeclarationStatus.Draft)
        //        {
        //            result = GetDeclarationViewModelById(repository, decDB.Id);
        //        }
        //        else
        //        {
        //            throw new Exception(String.Format("Заявление с кодом {0} уже поступило в обработку в учреждении и не может быть изменено.", result.SmevId));
        //        }
        //    }

        //    result.Status = "Draft";

        //    foreach (var item in xDec.Elements("DeclarationTypes"))
        //    {
        //        result.IsGrantType = item.Value == "Выдача";
        //        result.IsProlongType = item.Value == "Продление";
        //        result.IsReNewType = item.Value == "Переоформление";
        //        result.IsCancelType = item.Value == "Прекращение";
        //        result.IsCloneType = item.Value == "Дубликат";
        //    }

        //    result.Comm = GetXmlNodeNullableValue(xDec, "Comment", false);
        //    result.CountYears = GetXmlNodeIntValue(xDec, "Years", result.IsProlongType);
        //    result.IsPhoneInformation = GetXmlNodeBooleanValue(xDec, "IsPhoneInformation");
        //    result.IsEmailInformation = GetXmlNodeBooleanValue(xDec, "IsEmailInformation");
        //    result.IsMailGetResult = GetXmlNodeBooleanValue(xDec, "IsMailGetResult");
        //    result.IsEMailGetResult = GetXmlNodeBooleanValue(xDec, "IsEMailGetResult");
        //    result.IsPersonGetResult = GetXmlNodeBooleanValue(xDec, "IsPersonGetResult");

        //    result.OtherReason = GetXmlNodeNullableValue(xDec, "CustomReasonForRenew", false);
        //    result.ReasonForRenew = new DictionaryRecordViewModel() { Id = 0 };
        //    foreach (var item in xDec.Elements("ReasonsForRenew"))
        //    {
        //        //TODO: Причины переоформления, реализовать мультипричинность из списка
        //    }

        //    result.MasterName = GetXmlNodeNullableValue(xDec, "Applicant", false);

        //    var xComp = xDec.Element("Company");
        //    var xLic = xComp.Element("License");

        //    String dictTypeStr = GetXmlNodeNullableValue(xLic, "LicensedActivityType");
        //    LicensedActivityType lat = repository.LicensedActivityType.Items.FirstOrDefault(x => x.IsDeleted != true && x.ShortName == dictTypeStr);
        //    if (lat != null)
        //    {
        //        result.LicensedActivityTypeId = lat.Id;
        //        result.LicensedActivityTypeName = lat.ShortName;
        //    }
        //    else
        //    {
        //        throw new Exception(String.Format("Тип лицензируемой деятельности задан неверно: {0}", dictTypeStr));
        //    }

        //    if (decDB == null)
        //    {
        //        result.Company = new CompanyViewModel() { Id = 0 };
        //    }

        //    result.Company.ShortName = GetXmlNodeNullableValue(xComp, "ShortName");
        //    result.Company.FullName = GetXmlNodeNullableValue(xComp, "FullName");
        //    result.Company.INN = GetXmlNodeNullableValue(xComp, "INN");
        //    result.Company.KPP = GetXmlNodeNullableValue(xComp, "KPP", false);
        //    result.Company.OGRN = GetXmlNodeNullableValue(xComp, "OGRN", false);
        //    result.Company.Email = GetXmlNodeNullableValue(xComp, "Email", false);
        //    result.Company.Phone = GetXmlNodeNullableValue(xComp, "Phone", false);
        //    result.Company.Fax = GetXmlNodeNullableValue(xComp, "Fax", false);
        //    result.Company.BanksName = GetXmlNodeNullableValue(xComp, "BankName", false);
        //    result.Company.CheckingAccount = GetXmlNodeNullableValue(xComp, "BankAccount", false);

        //    Company companyDB = repository.Company.Items.FirstOrDefault(x => x.INN == result.Company.INN);

        //    String licenseNumber = GetXmlNodeNullableValue(xLic, "LicenseNumber", false);
        //    License activeLicense = null;
        //    if (companyDB != null)
        //    {
        //        activeLicense = companyDB.Licensee.FirstOrDefault(x => x.Status == LicenseStatus.Active && x.LicensedActivityType == lat);
        //    }
        //    if (!result.IsGrantType)
        //    {
        //        if (activeLicense == null)
        //        {
        //            throw new Exception("Действующая лицензия компании по лицензируемому типу деятельности не найдена. Вы можете претендовать только на выдачу новой лицензии.");
        //        }
        //        else if (activeLicense.Nom != licenseNumber)
        //        {
        //            throw new Exception(String.Format("Номер действующей лицензии компании по лицензируемому типу деятельности {0} не соответствует номеру в заявлении: {1}", activeLicense.Nom, licenseNumber));
        //        }
        //        result.LicenseNumber = activeLicense.Nom;
        //        result.LicenseNomOfBook = activeLicense.NomOfBook;
        //        result.LicenseId = activeLicense.Id;
        //    }
        //    else
        //    {
        //        TypeOfTypeOfResolution ttr = TypeOfTypeOfResolution.Grant;
        //        result.TypeOfType = ttr.ToString();
        //        //TODO: инициализация result.ResolutionTypes
        //        //var resolutionTypesAll = TypeOfResolutionForResolutionViewModel.GetTypeOfResolutionViewModelList(repository, ttr, null);
        //        //foreach (var item in resolutionTypesAll)
        //        //{
        //        //    result.ResolutionTypes.Add(item);
        //        //}

        //        if (activeLicense != null)
        //        {
        //            throw new Exception("Действующая лицензия компании по лицензируемому типу деятельности уже существует. Вы можете претендовать только на продление/переоформление/прекращение действия действующей лицензии.");
        //        }
        //    }

        //    if (decDB != null)
        //    {
        //        if (decDB.INN != result.Company.INN)
        //        {
        //            throw new Exception(String.Format("ИНН компании не может быть изменен при редактировании заявления: {0}", decDB.INN));
        //        }
        //        if (decDB.LicenseNom != licenseNumber)
        //        {
        //            throw new Exception(String.Format("Номер лицензии не может быть изменен при редактировании заявления: {0}", decDB.LicenseNom));
        //        }
        //    }

        //    dictTypeStr = GetXmlNodeNullableValue(xComp, "LegalFormType");
        //    LegalFormType lft = repository.LegalFormType.Items.FirstOrDefault(x => x.IsDeleted != true && x.ShortName == dictTypeStr);
        //    if (lft != null)
        //    {
        //        result.Company.LegalFormType = new DictionaryRecordViewModel() { Name = lft.ShortName, Id = lft.Id };
        //    }
        //    else
        //    {
        //        throw new Exception(String.Format("ОПФ задана неверно: {0}", dictTypeStr));
        //    }

        //    //TODO: Addr
        //    var xAddr = xComp.Element("Addr");
        //    result.Company.Address = GetXmlNodeNullableValue(xAddr, "Address");
        //    result.Company.AddressForDocument = GetXmlNodeNullableValue(xAddr, "AddressForDocument", false);

        //    //TODO: AddrPostal
        //    xAddr = xComp.Element("AddrPostal");
        //    result.Company.PostalAddress = GetXmlNodeNullableValue(xAddr, "Address");

        //    foreach (var item in xDec.Descendants("Unit"))
        //    {
        //        UnitInDeclarationViewModel unitVM = null;
        //        int sysId = GetXmlNodeIntValue(item, "SystemId", false);
        //        String relationKind = GetXmlNodeNullableValue(item, "RelationKind");
        //        Boolean isDeleted = GetXmlNodeBooleanValue(item, "IsDeleted");
        //        List<UnitInDeclarationViewModel> unitList = null;
        //        if (relationKind == "Add")
        //        {
        //            unitList = result.UnitsAdd;
        //        }
        //        else if (relationKind == "Edit")
        //        {
        //            unitList = result.UnitsEdit;
        //        }
        //        else if (relationKind == "Remove")
        //        {
        //            unitList = result.UnitsRemove;
        //        }
        //        if (sysId > 0 && decDB != null)
        //        {
        //            unitVM = unitList.FirstOrDefault(x => x.Id == sysId);
        //        }

        //        if (unitVM == null)
        //        {
        //            if (!isDeleted)
        //            {
        //                unitVM = new UnitInDeclarationViewModel();
        //                unitList.Add(unitVM);
        //            }
        //        }
        //        else
        //        {
        //            if (isDeleted)
        //            {
        //                unitList.Remove(unitVM);
        //            }
        //        }

        //        if (!isDeleted)
        //        {
        //            unitVM.ShortName = GetXmlNodeNullableValue(item, "ShortName");
        //            unitVM.FullName = GetXmlNodeNullableValue(item, "FullName");
        //            unitVM.KPP = GetXmlNodeNullableValue(item, "KPP", false);
        //            unitVM.Status = "None";

        //            dictTypeStr = GetXmlNodeNullableValue(item, "UnitType");
        //            UnitType unitType = repository.UnitType.Items.FirstOrDefault(x => x.IsDeleted != true && x.ShortName == dictTypeStr);
        //            if (unitType != null)
        //            {
        //                unitVM.UnitType = new DictionaryRecordViewModel() { Name = unitType.ShortName, Id = unitType.Id };
        //            }
        //            else
        //            {
        //                throw new Exception(String.Format("Тип подразделения задан неверно: {0}", dictTypeStr));
        //            }

        //            dictTypeStr = GetXmlNodeNullableValue(item, "ModifyLicense");
        //            ModifyLicense ModifyLicense = repository.ModifyLicense.Items.FirstOrDefault(x => x.IsDeleted != true && x.ShortName == dictTypeStr);
        //            if (unitType != null)
        //            {
        //                unitVM.ModifyLicense = new DictionaryRecordViewModel() { Name = unitType.ShortName, Id = unitType.Id };
        //            }
        //            else
        //            {
        //                throw new Exception(String.Format("Вид лицензируемой продукции задан неверно: {0}", dictTypeStr));
        //            }

        //            //TODO: Addr
        //            xAddr = item.Element("Addr");
        //            unitVM.Address = GetXmlNodeNullableValue(xAddr, "Address");
        //            unitVM.AddressForDocument = GetXmlNodeNullableValue(xAddr, "AddressForDocument", false);
        //        }
        //    }

        //    foreach (var item in xDec.Descendants("PaymentOrder"))
        //    {
        //        PaymentOrderViewModelForDeclaration po = null;
        //        int sysId = GetXmlNodeIntValue(item, "SystemId", false);
        //        if (sysId > 0 && decDB != null)
        //        {
        //            po = result.PaymentOrders.FirstOrDefault(x => x.Id == sysId);
        //        }
        //        if (po == null)
        //        {
        //            po = new PaymentOrderViewModelForDeclaration();
        //        }
        //        po.Nom = GetXmlNodeNullableValue(item, "Number");
        //        po.Date = GetXmlNodeNullableValue(item, "Date");
        //        po.Summ = GetXmlNodeDoubleValue(item, "Sum", true);
        //        po.Comm = GetXmlNodeNullableValue(item, "Comment", false);
        //        po.IsDeleted = item.Element("IsDeleted").Value == "Y";
        //        result.PaymentOrders.Add(po);
        //        //TODO: Загружать сканы PaymentOrder (BlobData)
        //    }
        //    foreach (var item in xDec.Descendants("Attachment"))
        //    {
        //        //TODO: Загружать сканы Attachment (BlobData)
        //    }
        //    return result;
        //}

        public DeclarationViewModel(DeclarationExportFromDatabase dec_exp, IRepository repository)
        {
            if (dec_exp.TypeCreateDeclaration == "List")
            {
                if (dec_exp.company_export != null)
                {
                    CompanyFullNameNow = dec_exp.company_export.company.FullName;
                    CompanyShortNameNow = dec_exp.company_export.company.ShortName;
                    CompanyNameNow = dec_exp.company_export.company.Name;
                }
                LicensedActivityTypeName = dec_exp.licensed_activity_type.ShortName;
                LicensedActivityTypeId = dec_exp.licensed_activity_type.Id;
            }
            TypeCreateDeclaration = dec_exp.TypeCreateDeclaration;
            TagsTable = null;
            if (dec_exp.license != null)
            {
                LicenseId = dec_exp.license.Id;
                LicenseNumber = dec_exp.license.Nom;
                LicenseNomOfBook = dec_exp.license.NomOfBook;
                LicenseDateStart = (dec_exp.declaration != null && dec_exp.declaration.LicenseBaseDateStart.HasValue ? dec_exp.declaration.LicenseBaseDateStart.Value : dec_exp.license.DateStart).ToShortDateString();
                LicenseDateExpired = (dec_exp.declaration != null && dec_exp.declaration.LicenseBaseDateExpired.HasValue ? dec_exp.declaration.LicenseBaseDateExpired.Value : dec_exp.license.DateExpired).ToShortDateString();
            }
            else
            {
                LicenseId = 0;

                LicenseDateStart = "";
                LicenseDateExpired = "";
                if (dec_exp.declaration == null)
                {
                    LicenseNumber = "";
                    LicenseNomOfBook = "";
                }
                else
                {
                    LicenseNumber = dec_exp.declaration.LicenseNom;
                    LicenseNomOfBook = dec_exp.declaration.LicenseNomOfBook;
                }
            }

            //ResolutionType = new DictionaryRecordViewModel() { Id = dec_exp.type_of_resolution.Id, CodeName = dec_exp.type_of_resolution.CodeName, Name = dec_exp.type_of_resolution.ShortName };

            //repository.ReasonForRenew.GetNotDeletedItems().ToList()
            ReasonForRenews = repository.ReasonForRenew.GetNotDeletedItems().Select(x => new DictionaryRecordViewModel() { Id = x.Id, CodeName = x.CodeName, Name = x.ShortName }).ToList();
            if (dec_exp.reason_for_renews != null && dec_exp.reason_for_renews.Count > 0)
            {
                foreach (var item in dec_exp.reason_for_renews)
                {
                    ReasonForRenews.FirstOrDefault(x => x.Id == item.Id).IsChecked = true;
                }
            }

            RefuseBasises = repository.RefuseBasis.GetNotDeletedItems().Select(x => new DictionaryRecordViewModel() { Id = x.Id, CodeName = x.CodeName, Name = x.ShortName }).ToList();
            if (dec_exp.refuse_basis != null && dec_exp.refuse_basis.Count > 0)
            {
                foreach (var item in dec_exp.refuse_basis)
                {
                    RefuseBasises.FirstOrDefault(x => x.Id == item.Id).IsChecked = true;
                }
            }

            if (dec_exp.license_form != null)
            {
                LicenseFormNom = dec_exp.license_form.Nom;
                LicenseFormId = dec_exp.license_form.Id;
            }
            else
            {
                if (dec_exp.declaration == null)
                {
                    LicenseFormNom = "";
                }
                else
                {
                    LicenseFormNom = dec_exp.declaration.LicenseFormNom;
                }
                LicenseFormId = 0;
            }

            if (dec_exp.declaration == null)
            {
                Comm = "";

                ResolutionId = 0;
                ResolutionComm = "";
                RefuseComm = "";
                ResolutionNom = "";
                ResolutionDate = DateTime.Now.ToShortDateString();
                TypeOfType = dec_exp.type_of_type_new.ToString();

                MasterName = "";
                VakeelName = "";
                IsPhoneInformation = true;
                IsPersonGetResult = true;

                UnitsAdd = new List<UnitInDeclarationViewModel>();
                UnitsRemove = new List<UnitInDeclarationViewModel>();
                UnitsEdit = new List<UnitInDeclarationViewModel>();
                UnitsCanceled = new List<UnitInDeclarationViewModel>();
                Status = DeclarationStatus.Draft.ToString();
                Id = 0;
                RegNomAll = "";
                OtherReason = "";
                DateDeclaration = DateTime.Now.ToShortDateString();

                DateTimeDeclaration = DateTime.Now.ToString();

                TimeDeclaration = DateTime.Now.ToShortTimeString();

                IsGrantType = TypeCreateDeclaration == "Grant";
                IsProlongType = TypeCreateDeclaration.Contains("Prolong");
                IsReNewType = TypeCreateDeclaration.Contains("ReNew");
                IsCancelType = TypeCreateDeclaration == "Cancel";
                IsCloneType = TypeCreateDeclaration == "Clone";
                CountYears = 0;

                if (dec_exp.company_export != null)
                {
                    Company = new CompanyViewModel(dec_exp.company_export);
                }
                else
                {
                    Company = new CompanyViewModel();
                }

                StatusRus = "Новый черновик";
            }
            else
            {
                Company = new CompanyViewModel();
                Declaration d = dec_exp.declaration;
                SmevId = d.SmevId;

                PaymentOrdersAll = d.PaymentOrdersAll;

                Comm = d.Comm;

                IsMailGetResult = d.GetResultType.HasFlag(GetResultType.Mail);
                IsEMailGetResult = d.GetResultType.HasFlag(GetResultType.Email);
                IsPersonGetResult = d.GetResultType.HasFlag(GetResultType.Personal);

                IsPhoneInformation = d.GetInformationType.HasFlag(TypeOfGetInformation.Phone);
                IsEmailInformation = d.GetInformationType.HasFlag(TypeOfGetInformation.Email);

                Status = d.Status.ToString();
                UnitsAdd = new List<UnitInDeclarationViewModel>();
                UnitsRemove = new List<UnitInDeclarationViewModel>();
                UnitsEdit = new List<UnitInDeclarationViewModel>();
                UnitsCanceled = new List<UnitInDeclarationViewModel>();
                if (dec_exp.company_export != null)
                {
                    Company.Id = dec_exp.company_export.company.Id;
                }
                else
                {
                    Company.Id = 0;
                }

                Resolution resolution = dec_exp.resolution;
                if (resolution == null)
                {
                    ResolutionId = 0;
                    ResolutionComm = "";
                    RefuseComm = "";
                    ResolutionDate = DateTime.Now.ToShortDateString();
                    TypeOfType = dec_exp.type_of_type_new.ToString();
                    ResolutionTypesAll = "";

                }
                else
                {
                    TypeOfType = resolution.TypeOfType.ToString();
                    ResolutionId = resolution.Id;
                    ResolutionComm = resolution.Comm;
                    RefuseComm = resolution.RefuseComment;
                    ResolutionTypesAll = resolution.TypeOfResolutionsAll;
                    if (dec_exp.TypeCreateDeclaration == "List" && resolution.IsDraft)
                    {
                        ResolutionNom = "";
                        ResolutionDate = "";
                    }
                    else
                    {
                        ResolutionNom = resolution.RegNomAll;
                        //ResolutionDate = d.Status == DeclarationStatus.Agreed || d.Status == DeclarationStatus.Canceled ? resolution.Date.ToShortDateString() : DateTime.Now.ToShortDateString();
                        ResolutionDate = resolution.Date.ToShortDateString();
                    }

                    if (resolution.LicenceApprover == null)
                    {
                        LicenceApprover = new DictionaryRecordViewModel() { Id = 0, Name = "Не определено", CodeName = "PeopleNotChoosen" };
                    }
                    else
                    {
                        LicenceApprover = new DictionaryRecordViewModel() { Id = resolution.LicenceApprover.Id, Name = resolution.LicenceApprover.FIO };
                    }

                    if (resolution.ResolutionCommissionChairman == null)
                    {
                        ResolutionCommissionChairman = new DictionaryRecordViewModel() { Id = 0, Name = "Не определено", CodeName = "PeopleNotChoosen" };
                    }
                    else
                    {
                        ResolutionCommissionChairman = new DictionaryRecordViewModel() { Id = resolution.ResolutionCommissionChairman.Id, Name = resolution.ResolutionCommissionChairman.FIO };
                    }
                }
                MasterName = d.MasterName;
                VakeelName = d.VakeelName;
                StatusRus = DeclarationViewModel.GetRusDeclarationStatus(d.Status);
                IsClosed = d.Status == DeclarationStatus.Agreed || d.Status == DeclarationStatus.Canceled;
                LicenseNomOfBook = d.LicenseNomOfBook;
                OtherReason = d.OtherReason;

                Id = d.Id;
                RegNomAll = d.RegNomAll;

                DateTime dt_update = d.Status == DeclarationStatus.Draft ? DateTime.Now : d.DateDeclaration;

                if (dec_exp.TypeCreateDeclaration == "List" && d.Status == DeclarationStatus.Draft)
                {
                    DateTimeDeclaration = "Соз. " + d.DateTimeCreated.ToString();
                }
                else
                {
                    DateTimeDeclaration = dt_update.ToString();
                }

                DateDeclaration = dt_update.ToShortDateString();
                TimeDeclaration = dt_update.ToShortTimeString();

                IsGrantType = d.Type.HasFlag(DeclarationType.Grant);
                IsProlongType = d.Type.HasFlag(DeclarationType.Prolong);
                IsReNewType = d.Type.HasFlag(DeclarationType.Renew);
                IsCancelType = d.Type.HasFlag(DeclarationType.Cancel);
                IsCloneType = d.Type.HasFlag(DeclarationType.Clone);
                ActionDeclarationRus = DeclarationViewModel.GetRusDeclarationType(d.Type);
                AllTypeDeclarationRus = ActionDeclarationRus;
                if (ReasonForRenews.Where(x => x.IsChecked).Count() > 0)
                {
                    AllTypeDeclarationRus += String.Format(" ({0})", String.Join("; ", ReasonForRenews.Where(x => x.IsChecked).Select(x => x.Name)));
                }

                CountYears = d.CountOfYears;

                DateCreatedStr = d.DateTimeCreated.ToString();
                Creator = d.CreatorName;

                Company.FullName = d.FullNameCompany;
                Company.ShortName = d.ShortNameCompany;
                Company.CompanyName = d.NameCompany;
                Company.INN = d.INN;
                Company.KPP = d.KPP;
                Company.LegalFormType = new DictionaryRecordViewModel() { Name = dec_exp.legal_form_type.ShortName, Id = dec_exp.legal_form_type.Id, FullName = dec_exp.legal_form_type.FullName };
                Company.OGRN = d.OGRN;
                Company.Phone = d.Phone;
                Company.Fax = d.Fax;
                Company.CheckingAccount = d.CheckingAccount;
                Company.BanksName = d.BanksName;

                Company.id_addr = dec_exp.addr == null ? -1 : dec_exp.addr.Id;
                Company.id_postal_addr = dec_exp.postal_addr == null ? -1 : dec_exp.postal_addr.Id;
                Company.Email = d.Email;
                Company.Address = d.ActualAddrStr;
                Company.AddressForDocument = d.AddressForDocument;
                Company.PostalAddress = d.PostalAddrStr;
                Company.PostalCode = d.PostalCode;

                var act_addr_arr = d.ActualAddrAddon.Split(CompanyViewModel.AddressAddonSep);

                Company.AddressHouse = act_addr_arr[0];
                Company.AddressBuildNom = act_addr_arr[1];
                Company.AddressLitera = act_addr_arr[2];
                Company.AddressRoomNom = act_addr_arr[3];
                Company.FSRARAddress = d.FSRARActualAddr;

                var post_addr_arr = d.PostalAddrAddon.Split(CompanyViewModel.AddressAddonSep);

                Company.PostalAddressHouse = post_addr_arr[0];
                Company.PostalAddressBuildNom = post_addr_arr[1];
                Company.PostalAddressLitera = post_addr_arr[2];
                Company.PostalAddressRoomNom = post_addr_arr[3];
                Company.FSRARPostalAddress = d.FSRARPostalAddr;
            }

            Revisions = new List<RevisionViewModel>();
            PaymentOrders = new List<PaymentOrderViewModelForDeclaration>();
            ActionsWithDeclaration = new List<ActionWithDeclarationViewModelForDeclaration>();

            DeclarationsInPack = new List<DeclarationViewModelForPack>();

        }

        public static string GetRusDeclarationType(DeclarationType dt)
        {
            string res = "";
            if (dt.HasFlag(DeclarationType.Clone))
            {
                res = "Дубликат";
            }
            else
            {
                if (dt.HasFlag(DeclarationType.Grant))
                {
                    res = "Выдача";
                }
                else
                {
                    if (dt.HasFlag(DeclarationType.Cancel))
                    {
                        res = "Прекращение";
                    }
                    else
                    {
                        if (dt.HasFlag(DeclarationType.Renew))
                        {
                            if (dt.HasFlag(DeclarationType.Prolong))
                            {
                                res = "Переоформление/Продление";
                            }
                            else
                            {
                                res = "Переоформление";
                            }
                        }
                        else
                        {
                            if (dt.HasFlag(DeclarationType.Prolong))
                            {
                                res = "Продление";
                            }
                            else
                            {
                                res = "";
                            }
                        }
                    }
                }
            }
            return res;
        }

        public static string GetRusDeclarationStatus(DeclarationStatus ds)
        {
            string res = "";

            switch (ds)
            {
                case DeclarationStatus.Draft: { res = "Черновик"; break; }
                case DeclarationStatus.Agreed: { res = "Выполнено"; break; }
                case DeclarationStatus.Canceled: { res = "Отменено"; break; }
                case DeclarationStatus.Consideration: { res = "Зарегистрировано"; break; }
                case DeclarationStatus.StartRevision: { res = "Проверка"; break; }
            }

            return res;
        }

        public static List<DeclarationViewModel> GetListViewModel(IRepository repository, IQueryable<Declaration> declaration_list)
        {
            List<DeclarationViewModel> res = new List<DeclarationViewModel>();

            foreach (var d in declaration_list.Select(j => new DeclarationExportFromDatabase() {
                reason_for_renews = j.ReasonForRenews.ToList(),
                TypeCreateDeclaration = "List",
                resolution = j.Resolution,
                license_form = j.LicenseForm,
                license = j.License, declaration = j,
                addr = j.ActualAddr,
                postal_addr = j.PostalAddr,
                legal_form_type = j.LegalFormType,
                licensed_activity_type = j.LicensedActivityType,
                company_export = j.Company != null ? new CompanyExportFromDatabase() { company = j.Company } : null }).ToList())
            {
                //, legal_form_type = j.Company.LegalFormType, addr = j.Company.ActualAddr
                res.Add(new DeclarationViewModel(d, repository));

            }
            return res;
        }


        public static DeclarationViewModel GetDeclarationViewModelById(IRepository repository, int id)
        {
            Declaration d = repository.Declaration.Items.First(j => j.Id == id);
            // TypeOfResolution type_of_res = d.Resolution == null ? repository.TypeOfResolution.GetNotDeletedItems().First() : d.Resolution.TypeOfResolution;
            DeclarationViewModel result = new DeclarationViewModel(new DeclarationExportFromDatabase()
            {
                resolution = d.Resolution,
                //   type_of_resolution = d.Resolution.TypeOfResolution,
                reason_for_renews = d.ReasonForRenews.ToList(),
                // добавил соединения с Resolution
                refuse_basis = d.Resolution.RefuseBasises.ToList(),
                license_form = d.LicenseForm,
                license = d.License,
                TypeCreateDeclaration = "Edit",
                declaration = d,
                legal_form_type = d.LegalFormType,
                licensed_activity_type = d.LicensedActivityType,
                addr = d.ActualAddr,
                postal_addr = d.PostalAddr,
                company_export = d.Company == null ? null : new CompanyExportFromDatabase() { company = d.Company }
            }, repository);
            result.UnitsAdd = UnitInDeclarationViewModel.GetListViewModel(repository.UnitInDeclaration.GetAddItemsForDeclaration(id));
            result.UnitsRemove = UnitInDeclarationViewModel.GetListViewModel(repository.UnitInDeclaration.GetRemoveItemsForDeclaration(id));
            result.UnitsEdit = UnitInDeclarationViewModel.GetListViewModel(repository.UnitInDeclaration.GetEditItemsForDeclaration(id));
            result.Revisions = RevisionViewModel.GetListViewModel(repository.Revision.GetItemsForDeclaration(id));
            result.TagsTable = new ValueTagTableViewModel(d.Id, EntitiesForTag.Declaration, repository);
            result.Documents = DocumentViewModel.GetListViewModel(repository.Document.GetItemsForDeclaration(id));
            result.PaymentOrders = PaymentOrderViewModelForDeclaration.GetListViewModel(repository.PaymentOrder.GetItemsForDeclaration(id));
            result.ActionsWithDeclaration = ActionWithDeclarationViewModelForDeclaration.GetListViewModel(repository.ActionWithDeclaration.GetItemsForDeclaration(id));

            int res_id = d.Resolution == null ? 0 : d.ResolutionId.Value;

            if (result.IsProlongType || result.IsReNewType)
            {
                result.DeclarationsInPack = DeclarationViewModelForPack.GetListViewModel(repository.Declaration.GetNotDeletedItems().Where(j => j.Id != d.Id && j.Status == DeclarationStatus.Consideration && j.CompanyId.HasValue && j.CompanyId.Value == d.CompanyId.Value && (j.Type.HasFlag(DeclarationType.Prolong) || j.Type.HasFlag(DeclarationType.Renew))), res_id);
            }

            result.ResolutionTypes = TypeOfResolutionForResolutionViewModel.GetTypeOfResolutionViewModelList(repository, d.Resolution.TypeOfType, d.Resolution);

            result.ResolutionCommissioners = Utils.GetPeopleListByRole(repository, "ResolutionCommissioner");
            foreach (var item in result.ResolutionCommissioners)
            {
                item.IsChecked = d.Resolution.ResolutionCommissioners.Select(x => x.Id).ToList().Contains(item.Id);
            }

            //result.list_Refuse_Basis = Utils.GetPeopleListByRole(repository, "ShowRefuseBasis");
            //foreach (var item in result.list_Refuse_Basis)
            //{
            //    item.IsChecked = d.Resolution.RefuseBasises.Select(x => x.Id).ToList().Contains(item.Id);
            //}

            return result;
        }
    }

    public class DeclarationExportFromDatabase
    {
        public CompanyExportFromDatabase company_export { get; set; }
        public Resolution resolution { get; set; }
        public TypeOfTypeOfResolution type_of_type_new { get; set; }
        public Declaration declaration { get; set; }
        public Addr addr { get; set; }
        public Addr postal_addr { get; set; }
        public LicensedActivityType licensed_activity_type { get; set; }
        public LegalFormType legal_form_type { get; set; }
        public License license { get; set; }
        public LicenseForm license_form { get; set; }
        public List<ReasonForRenew> reason_for_renews { get; set; }
        public List<RefuseBasis> refuse_basis { get; set; }
        public string TypeCreateDeclaration { get; set; }
    }
}