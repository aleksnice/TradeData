using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cascade.Licensing.Domain.Abstract;
using System.Security.Principal;

namespace Cascade.Licensing.Domain.Concrete
{
    public class EFCompany: ICompany
    {
        private EFDbContextContainer context;
        private People UserDB;
        LogHelper LogHelper;
        public EFCompany(EFDbContextContainer context, People UserDB, LogHelper logHelper)
        {
            LogHelper = logHelper;
            this.context = context;
            this.UserDB = UserDB;
        }


        public IQueryable<Company> Items
        {
            get { return context.CompanySet; }
        }

        public IQueryable<Company> GetNotDeletedItems()
        {
            return context.CompanySet.Where(j => j.IsDeleted != true);
        }


        public void Add(Company item, LegalFormType legal_form_type)
        {
            // Временно разрешаем ничего не вводить
            if (item.BanksName == null) { item.BanksName = ""; }
            if (item.CheckingAccount == null) { item.CheckingAccount = ""; }
            if (item.PostalCode == null) { item.PostalCode = ""; }
            if (item.Fax == null) { item.Fax = ""; }
            if (item.Phone == null) { item.Phone = ""; }
            if (item.Email == null) { item.Email = ""; }
            if (item.ORGN == null) { item.ORGN = ""; }
            if (item.KPP == null) { item.KPP = ""; }
            if (item.FullName == null) { item.FullName = ""; }
            if (item.ShortName == null) { item.ShortName = ""; }
            if (item.Name == null) { item.Name = ""; }
            if (item.AddressForDocument == null) { item.AddressForDocument = ""; }
            if (item.ActualAddrStr == null) { item.ActualAddrStr = ""; }
            if (item.PostalAddrStr == null) { item.PostalAddrStr = ""; }

            if (item.LegalAddrStr == null) { item.LegalAddrStr = ""; }
            if (item.LegalAddrAddon == null) { item.LegalAddrAddon = "|||"; }
            if (item.FSRARLegalAddr == null) { item.FSRARLegalAddr = ""; }
            if (item.ActualAddrAddon == null) { item.ActualAddrAddon = "|||"; }
            if (item.FSRARActualAddr == null) { item.FSRARActualAddr = ""; }
            if (item.PostalAddrAddon == null) { item.PostalAddrAddon = "|||"; }
            if (item.FSRARPostalAddr == null) { item.FSRARPostalAddr = ""; }

            if (item.Id == 0)
            {
                if (item.AddrStrFromImport == null)
                {
                    item.AddrStrFromImport = "";
                }

                item.IsDeleted = null;
                item.DateTimeCreated = DateTime.Now;
                item.CreatorName = UserDB == null ? "System" : UserDB.FIO;
                context.CompanySet.Add(item);
                legal_form_type.LinkCompany(item, LogHelper);
                LogHelper.Log(TryCreateRecord.Company, new SerializedEFObject(item));
            }
            else
            {
                Company old = context.CompanySet.First(x => x.Id == item.Id);
                if (old.Email != item.Email)
                {
                    LogHelper.Log(UpdateRecord.Company, item.Id, "Эл. почта", old.Email, item.Email);
                    old.Email = item.Email;
                }

                if (old.Fax != item.Fax)
                {
                    LogHelper.Log(UpdateRecord.Company, item.Id, "Факс", old.Fax, item.Fax);
                    old.Fax = item.Fax;
                }

                if (old.FullName != item.FullName)
                {
                    LogHelper.Log(UpdateRecord.Company, item.Id, "Полное название", old.FullName, item.FullName);
                    old.FullName = item.FullName;
                }

                if (old.Name != item.Name)
                {
                    LogHelper.Log(UpdateRecord.Company, item.Id, "Hазвание", old.Name, item.Name);
                    old.Name = item.Name;
                }


                if (old.INN != item.INN)
                {
                    LogHelper.Log(UpdateRecord.Company, item.Id, "ИНН", old.INN, item.INN);
                    old.INN = item.INN;
                }

                
                if (old.KPP != item.KPP)
                {
                    LogHelper.Log(UpdateRecord.Company, item.Id, "КПП", old.KPP, item.KPP);
                    old.KPP = item.KPP;
                }

                if (old.ORGN != item.ORGN)
                {
                    LogHelper.Log(UpdateRecord.Company, item.Id, "ОГРН", old.ORGN, item.ORGN);
                    old.ORGN = item.ORGN;
                }

                if (old.Phone != item.Phone)
                {
                    LogHelper.Log(UpdateRecord.Company, item.Id, "Телефон", old.Phone, item.Phone);
                    old.Phone = item.Phone;
                }

                if (old.LegalAddrStr != item.LegalAddrStr)
                {
                    LogHelper.Log(UpdateRecord.Company, item.Id, "Юридический адрес", old.LegalAddrStr, item.LegalAddrStr);
                    old.LegalAddrStr = item.LegalAddrStr;
                }

                if (old.PostalAddrStr != item.PostalAddrStr)
                {
                    LogHelper.Log(UpdateRecord.Company, item.Id, "Почтовый адрес", old.PostalAddrStr, item.PostalAddrStr);
                    old.PostalAddrStr = item.PostalAddrStr;
                }

                if (old.ActualAddrStr != item.ActualAddrStr)
                {
                    LogHelper.Log(UpdateRecord.Company, item.Id, "Фактический адрес", old.ActualAddrStr, item.ActualAddrStr);
                    old.ActualAddrStr = item.ActualAddrStr;
                }


                if (old.AddressForDocument != item.AddressForDocument)
                {
                    LogHelper.Log(UpdateRecord.Company, item.Id, "Адрес для документов", old.AddressForDocument, item.AddressForDocument);
                    old.AddressForDocument = item.AddressForDocument;
                }

                if (item.AddrStrFromImport !=null && old.AddrStrFromImport != item.AddrStrFromImport)
                {
                    LogHelper.Log(UpdateRecord.Company, item.Id, "Адрес импорта", old.AddrStrFromImport, item.AddrStrFromImport);
                    old.AddrStrFromImport = item.AddrStrFromImport;
                }

                if (old.ActualAddr != item.ActualAddr)
                {
                    LogHelper.Log(UpdateRecord.Company, item.Id, "Привязка Юр. адреса к новому объекту", "", "");
                    old.ActualAddr = item.ActualAddr;
                }

                if (old.ShortName != item.ShortName)
                {
                    LogHelper.Log(UpdateRecord.Company, item.Id, "Сокращенное название", old.ShortName, item.ShortName);
                    old.ShortName = item.ShortName;
                }

                if (old.Status != item.Status)
                {
                    LogHelper.Log(UpdateRecord.Company, item.Id, "Статус", old.Status.ToString(), item.Status.ToString());
                    old.Status = item.Status;
                }

                if (old.BanksName != item.BanksName)
                {
                    LogHelper.Log(UpdateRecord.Company, item.Id, "Название банка", old.BanksName, item.BanksName);
                    old.BanksName = item.BanksName;
                }

                if (old.CheckingAccount != item.CheckingAccount)
                {
                    LogHelper.Log(UpdateRecord.Company, item.Id, "Номер р/с", old.CheckingAccount, item.CheckingAccount);
                    old.CheckingAccount = item.CheckingAccount;
                }

                if (old.PostalCode != item.PostalCode)
                {
                    LogHelper.Log(UpdateRecord.Company, item.Id, "Почтовый индекс", old.PostalCode, item.PostalCode);
                    old.PostalCode = item.PostalCode;
                }

                if (old.LegalFormType != legal_form_type)
                {
                    legal_form_type.LinkCompany(old, LogHelper);
                }

                if (old.LegalAddrAddon != item.LegalAddrAddon)
                {
                    old.LegalAddrAddon = item.LegalAddrAddon;
                }

                if (old.ActualAddrAddon != item.ActualAddrAddon)
                {
                    old.ActualAddrAddon = item.ActualAddrAddon;
                }

                if (old.PostalAddrAddon != item.PostalAddrAddon)
                {
                    old.PostalAddrAddon = item.PostalAddrAddon;
                }

                if (old.FSRARLegalAddr != item.FSRARLegalAddr)
                {
                    LogHelper.Log(UpdateRecord.Company, item.Id, "Юр. адрес - ФСРАР", old.FSRARLegalAddr, item.FSRARLegalAddr);
                    old.FSRARLegalAddr = item.FSRARLegalAddr;
                }

                if (old.FSRARActualAddr != item.FSRARActualAddr)
                {
                    LogHelper.Log(UpdateRecord.Company, item.Id, "Фак. адрес - ФСРАР", old.FSRARActualAddr, item.FSRARActualAddr);
                    old.FSRARActualAddr = item.FSRARActualAddr;
                }

                if (old.FSRARPostalAddr != item.FSRARPostalAddr)
                {
                    LogHelper.Log(UpdateRecord.Company, item.Id, "Поч. адрес - ФСРАР", old.FSRARPostalAddr, item.FSRARPostalAddr);
                    old.FSRARPostalAddr = item.FSRARPostalAddr;
                }

            }
        }

        public void Remove(Company item)
        {
            if (item.HasReferences())
            {
                item.IsDeleted = true;
                LogHelper.Log(SetDeletedRecord.Company, item.Id);
            }
            else
            {
                JustRemove(item);
            }
        }

        public void JustRemove(Company item)
        {
            context.CompanySet.Remove(item);
            LogHelper.Log(DeleteRecordFromDataBase.Company, item.Id, new SerializedEFObject(item));
        }
    }
}
