using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cascade.Licensing.Domain.Abstract;
using System.Security.Principal;

namespace Cascade.Licensing.Domain.Concrete
{
    public class EFDeclaration:IDeclaration
    {

        private EFDbContextContainer context;
        private People UserDB;
        LogHelper LogHelper;
        public EFDeclaration(EFDbContextContainer context, People UserDB, LogHelper logHelper)
        {
            LogHelper = logHelper;
            this.context = context;
            this.UserDB = UserDB;
        }

        public IQueryable<Declaration> Items
        {
            get { return context.DeclarationSet; }
        }

        public IQueryable<Declaration> GetNotDeletedItems()
        {
            if (UserDB != null)
            {
                return context.DeclarationSet.Where(j => j.IsDeleted != true && j.LicensedActivityTypeId == UserDB.LicensedActivityTypeId);
            }
            else
            {
                return context.DeclarationSet.Where(j => j.IsDeleted != true);
            }
        }

        public String SwitchNumbers(int itemId, String newRegNumAll)
        {
            String errMsg = null;
            var curItem = context.DeclarationSet.Where(j => j.IsDeleted != true).FirstOrDefault(x => x.Id == itemId);
            if (curItem.RegNomAll != newRegNumAll)
            {
                var broItem = context.DeclarationSet.Where(j => j.IsDeleted != true).FirstOrDefault(x => x.RegNomAll == newRegNumAll);
                if (broItem != null)
                {
                    int tmpRegNom = curItem.RegNom;
                    int tmpRegNomYear = curItem.RegNomYear;
                    curItem.SetNewNom(broItem.RegNom, broItem.RegNomYear,LogHelper);
                    broItem.SetNewNom(tmpRegNom, tmpRegNomYear, LogHelper);
                }
                else errMsg = String.Format("Заявления с номером {0} не существует. Пожалуйста, выберите существующее заявление для обмена номеров.", newRegNumAll);
            }
            return errMsg;
        }

        public Declaration Add(Declaration item, LicensedActivityType license_activity_type, LegalFormType legal_form_type, Company company, License license)
        {
            if (item.ActualAddrStr == null) { item.ActualAddrStr = ""; }
            if (item.AddressForDocument == null) { item.AddressForDocument = ""; }
            if (item.BanksName == null) { item.BanksName = ""; }
            if (item.CheckingAccount == null) { item.CheckingAccount = ""; }
            if (item.Email == null) { item.Email = ""; }
            if (item.Fax == null) { item.Fax = ""; }
            if (item.FullNameCompany == null) { item.FullNameCompany = ""; }
            if (item.INN == null) { item.INN = ""; }
            if (item.KPP == null) { item.KPP = ""; }
            if (item.OGRN == null) { item.OGRN = ""; }
            if (item.OtherReason == null) { item.OtherReason = ""; }
            if (item.Phone == null) { item.Phone = ""; }
            if (item.PostalAddrStr == null) { item.PostalAddrStr = ""; }
            if (item.PostalCode == null) { item.PostalCode = ""; }
            if (item.ShortNameCompany == null) { item.ShortNameCompany = ""; }
            if (item.RegNomAll == null) { item.RegNomAll = ""; }
            if (item.MasterName == null) { item.MasterName = ""; }
            if (item.Comm == null) { item.Comm = ""; }
            if (item.LicenseFormNom == null) { item.LicenseFormNom = ""; }
            if (item.LicenseNom == null) { item.LicenseNom = ""; }
            if (item.LicenseNomOfBook == null) { item.LicenseNomOfBook = ""; }
            if (item.ActualAddrAddon == null) { item.ActualAddrAddon = ""; }
            if (item.PostalAddrAddon == null) { item.PostalAddrAddon = ""; }
            if (item.FSRARActualAddr == null) { item.FSRARActualAddr = ""; }
            if (item.FSRARPostalAddr == null) { item.FSRARPostalAddr = ""; }
            if (item.SmevId == null) { item.SmevId = ""; }
            if (item.NameCompany == null) { item.NameCompany = ""; }
            if (item.VakeelName == null) { item.VakeelName = ""; }

            if (item.Id == 0)
            {
                item.IsDeleted = null;
                item.DateTimeCreated = DateTime.Now;
                item.CreatorName = UserDB == null ? "System" : UserDB.FIO;
                context.DeclarationSet.Add(item);
                license_activity_type.LinkDeclaration(item, LogHelper);
                legal_form_type.LinkDeclaration(item, LogHelper);
                if (company != null)
                {
                    company.LinkDeclaration(item, LogHelper);
                }
                if (license != null)
                {
                    license.LinkDeclaration(item, LogHelper);
                }
                LogHelper.Log(TryCreateRecord.Declaration, new SerializedEFObject(item));
                return item;
            }
            else
            {
                Declaration old = context.DeclarationSet.First(x => x.Id == item.Id);
                if (old.CountOfYears != item.CountOfYears)
                {
                    LogHelper.Log(UpdateRecord.Declaration, item.Id, "Срок лицензии", old.CountOfYears.ToString(), item.CountOfYears.ToString());
                    old.CountOfYears = item.CountOfYears;
                }
                if (old.DateDeclaration != item.DateDeclaration)
                {
                    LogHelper.Log(UpdateRecord.Declaration, item.Id, "Дата заявления", old.DateDeclaration.ToSafeString(), item.DateDeclaration.ToSafeString());
                    old.DateDeclaration = item.DateDeclaration;
                }
                if (old.GetInformationType != item.GetInformationType)
                {
                    LogHelper.Log(UpdateRecord.Declaration, item.Id, "Способ получения информации", old.GetInformationType.ToString(), item.GetInformationType.ToString());
                    old.GetInformationType = item.GetInformationType;
                }
                if (old.GetResultType != item.GetResultType)
                {
                    LogHelper.Log(UpdateRecord.Declaration, item.Id, "Способ получения результата", old.GetResultType.ToString(), item.GetResultType.ToString());
                    old.GetResultType = item.GetResultType;
                }
                if (old.OtherReason != item.OtherReason)
                {
                    LogHelper.Log(UpdateRecord.Declaration, item.Id, "Другая причина переоформления", old.OtherReason, item.OtherReason);
                    old.OtherReason = item.OtherReason;
                }
                if (old.MasterName != item.MasterName)
                {
                    LogHelper.Log(UpdateRecord.Declaration, item.Id, "Заявитель", old.MasterName, item.MasterName);
                    old.MasterName = item.MasterName;
                }
                if (old.Status != item.Status)
                {
                    LogHelper.Log(UpdateRecord.Declaration, item.Id, "Статус", old.Status.ToString(), item.Status.ToString());
                    old.Status = item.Status;
                }
                if (old.Type != item.Type)
                {
                    LogHelper.Log(UpdateRecord.Declaration, item.Id, "Тип заявления", old.Type.ToString(), item.Type.ToString());
                    old.Type = item.Type;
                }
                if (old.ActualAddr != item.ActualAddr)
                {
                    old.ActualAddr = item.ActualAddr;
                }
                if (old.ActualAddrStr != item.ActualAddrStr)
                {
                    LogHelper.Log(UpdateRecord.Declaration, item.Id, "Фактический адрес", old.ActualAddrStr, item.ActualAddrStr);
                    old.ActualAddrStr = item.ActualAddrStr;
                }

                if (old.AddressForDocument != item.AddressForDocument)
                {
                    LogHelper.Log(UpdateRecord.Declaration, item.Id, "Адрес для документов", old.AddressForDocument, item.AddressForDocument);
                    old.AddressForDocument = item.AddressForDocument;
                }

                if (old.BanksName != item.BanksName)
                {
                    LogHelper.Log(UpdateRecord.Declaration, item.Id, "Банк", old.BanksName, item.BanksName);
                    old.BanksName = item.BanksName;
                }

                if (old.CheckingAccount != item.CheckingAccount)
                {
                    LogHelper.Log(UpdateRecord.Declaration, item.Id, "Номер р/с", old.CheckingAccount, item.CheckingAccount);
                    old.CheckingAccount = item.CheckingAccount;
                }

                if (old.Email != item.Email)
                {
                    LogHelper.Log(UpdateRecord.Declaration, item.Id, "Эл. почта", old.Email, item.Email);
                    old.Email = item.Email;
                }

                if (old.Fax != item.Fax)
                {
                    LogHelper.Log(UpdateRecord.Declaration, item.Id, "Факс", old.Fax, item.Fax);
                    old.Fax = item.Fax;
                }

                if (old.FullNameCompany != item.FullNameCompany)
                {
                    LogHelper.Log(UpdateRecord.Declaration, item.Id, "Полное название", old.FullNameCompany, item.FullNameCompany);
                    old.FullNameCompany = item.FullNameCompany;
                }

                if (old.NameCompany != item.NameCompany)
                {
                    LogHelper.Log(UpdateRecord.Declaration, item.Id, "Hазвание", old.NameCompany, item.NameCompany);
                    old.NameCompany = item.NameCompany;
                }


                if (old.VakeelName != item.VakeelName)
                {
                    LogHelper.Log(UpdateRecord.Declaration, item.Id, "Представитель", old.VakeelName, item.VakeelName);
                    old.VakeelName = item.VakeelName;
                }

                if (old.INN != item.INN)
                {
                    LogHelper.Log(UpdateRecord.Declaration, item.Id, "ИНН", old.INN, item.INN);
                    old.INN = item.INN;
                }

                if (old.KPP != item.KPP)
                {
                    LogHelper.Log(UpdateRecord.Declaration, item.Id, "КПП", old.KPP, item.KPP);
                    old.KPP = item.KPP;
                }

                if (old.OGRN != item.OGRN)
                {
                    LogHelper.Log(UpdateRecord.Declaration, item.Id, "ОГРН", old.OGRN, item.OGRN);
                    old.OGRN = item.OGRN;
                }

                if (old.Phone != item.Phone)
                {
                    LogHelper.Log(UpdateRecord.Declaration, item.Id, "Телефон", old.Phone, item.Phone);
                    old.Phone = item.Phone;
                }

                if (old.ShortNameCompany != item.ShortNameCompany)
                {
                    LogHelper.Log(UpdateRecord.Declaration, item.Id, "Сокращеное название", old.ShortNameCompany, item.ShortNameCompany);
                    old.ShortNameCompany = item.ShortNameCompany;
                }

                if (old.Comm != item.Comm)
                {
                    LogHelper.Log(UpdateRecord.Declaration, item.Id, "Комментарий к заявлению", old.Comm, item.Comm);
                    old.Comm = item.Comm;
                }

                if (old.LicenseFormNom != item.LicenseFormNom)
                {
                    LogHelper.Log(UpdateRecord.Declaration, item.Id, "Номер бланка лицензии", old.LicenseFormNom, item.LicenseFormNom);
                    old.LicenseFormNom = item.LicenseFormNom;
                }

                if (old.LicenseNom != item.LicenseNom)
                {
                    LogHelper.Log(UpdateRecord.Declaration, item.Id, "Номер лицензии", old.LicenseNom, item.LicenseNom);
                    old.LicenseNom = item.LicenseNom;
                }

                if (old.LicenseNomOfBook != item.LicenseNomOfBook)
                {
                    LogHelper.Log(UpdateRecord.Declaration, item.Id, "Номер дела", old.LicenseNomOfBook, item.LicenseNomOfBook);
                    old.LicenseNomOfBook = item.LicenseNomOfBook;
                }

                if (old.SmevId != item.SmevId)
                {
                    LogHelper.Log(UpdateRecord.Declaration, item.Id, "Код SmevId", old.SmevId, item.SmevId);
                    old.SmevId = item.SmevId;
                }

                if (old.LegalFormType != legal_form_type)
                {
                    legal_form_type.LinkDeclaration(old, LogHelper);
                }

                if (old.LicensedActivityType != license_activity_type)
                {
                    license_activity_type.LinkDeclaration(old, LogHelper);
                }

                old.AttachReasonForRenewList(item.ReasonForRenews.ToList(), LogHelper);

                if (old.Company != company)
                {
                    if (company != null)
                    {
                        company.LinkDeclaration(old, LogHelper);
                    }
                    else
                    {
                        if (old.Company != null)
                        {
                            old.Company.Declarations.Remove(old);
                        }
                    }
                }

                if (old.License != license)
                {
                    if (license != null)
                    {
                        license.LinkDeclaration(old, LogHelper);
                    }
                    else
                    {
                        if (old.License != null)
                        {
                            old.License.Declarations.Remove(old);
                        }
                    }
                }

                if (old.LicenseForm != item.LicenseForm)
                {
                    LogHelper.Log(UpdateRecord.Declaration, item.Id, "Привязка бланка лицензии", "", "");
                    old.LicenseForm = item.LicenseForm;
                }

                if (old.PostalAddr != item.PostalAddr)
                {
                    old.PostalAddr = item.PostalAddr;
                }

                if (old.PostalAddrStr != item.PostalAddrStr)
                {
                    LogHelper.Log(UpdateRecord.Declaration, item.Id, "Почтовый адрес", old.PostalAddrStr, item.PostalAddrStr);
                    old.PostalAddrStr = item.PostalAddrStr;
                }

                if (old.PostalCode != item.PostalCode)
                {
                    LogHelper.Log(UpdateRecord.Declaration, item.Id, "Почтовый индекс", old.PostalCode, item.PostalCode);
                    old.PostalCode = item.PostalCode;
                }

                if (old.ActualAddrAddon != item.ActualAddrAddon)
                {
                    old.ActualAddrAddon = item.ActualAddrAddon;
                }

                if (old.PostalAddrAddon != item.PostalAddrAddon)
                {
                    old.PostalAddrAddon = item.PostalAddrAddon;
                }

                if (old.FSRARActualAddr != item.FSRARActualAddr)
                {
                    LogHelper.Log(UpdateRecord.Declaration, item.Id, "Фак. адрес - ФСРАР", old.FSRARActualAddr, item.FSRARActualAddr);
                    old.FSRARActualAddr = item.FSRARActualAddr;
                }

                if (old.FSRARPostalAddr != item.FSRARPostalAddr)
                {
                    LogHelper.Log(UpdateRecord.Declaration, item.Id, "Поч. адрес - ФСРАР", old.FSRARPostalAddr, item.FSRARPostalAddr);
                    old.FSRARPostalAddr = item.FSRARPostalAddr;
                }
                return old;
            }
        }

        public void Remove(Declaration item)
        {
            if (item.HasReferences())
            {
                item.IsDeleted = true;
                LogHelper.Log(SetDeletedRecord.Declaration, item.Id);
            }
            else
            {
                JustRemove(item);
            }
        }

        public void JustRemove(Declaration item)
        {
            context.DeclarationSet.Remove(item);
            LogHelper.Log(DeleteRecordFromDataBase.Declaration, item.Id, new SerializedEFObject(item));
        }
    }
}
