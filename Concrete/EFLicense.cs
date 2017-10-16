using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cascade.Licensing.Domain.Abstract;
using System.Security.Principal;

namespace Cascade.Licensing.Domain.Concrete
{
    public class EFLicense:ILicense
    {
        private EFDbContextContainer context;
        private People UserDB;
        LogHelper LogHelper;
        public EFLicense(EFDbContextContainer context, People UserDB, LogHelper logHelper)
        {
            LogHelper = logHelper;
            this.context = context;
            this.UserDB = UserDB;
        }
        public IQueryable<License> Items
        {
            get { return context.LicenseSet; }
        }

        public IQueryable<License> GetNotDeletedItems()
        {
            return context.LicenseSet.Where(j => j.IsDeleted != true && j.LicensedActivityType.Id == UserDB.LicensedActivityTypeId);
        }

        public IQueryable<License> GetItemsByCompany(int id_company)
        {
            return GetNotDeletedItems().Where(j => j.Company.Id == id_company).OrderByDescending(j=>j.DateStart);
        }

        public void Add(License item, LicensedActivityType licensed_activity_type, Company company)
        {
            if (item.Id == 0)
            {
                item.IsArchive = false;
                item.IsDeleted = null;
                item.DateTimeCreated = DateTime.Now;
                item.CreatorName = UserDB == null ? "System" : UserDB.FIO;
                if (item.Comment == null)
                {
                    item.Comment = "";
                }
                context.LicenseSet.Add(item);

                licensed_activity_type.LinkLicense(item, LogHelper);
                company.LinkLicense(item, LogHelper);

                LogHelper.Log(TryCreateRecord.License, new SerializedEFObject(item));
            }
            else
            {
                License old = context.LicenseSet.First(x => x.Id == item.Id);

                if (old.Comment != item.Comment)
                {
                    LogHelper.Log(UpdateRecord.License, item.Id, "Комментарий", old.Comment, item.Comment);
                    old.Comment = item.Comment;
                }

                if (old.IsArchive != item.IsArchive)
                {
                    LogHelper.Log(UpdateRecord.License, item.Id, "Признак архива", old.IsArchive.ToString(), item.IsArchive.ToString());
                    old.IsArchive = item.IsArchive;
                }

                if (old.DateExpired != item.DateExpired)
                {
                    LogHelper.Log(UpdateRecord.License, item.Id, "Дата окончания", old.DateExpired.ToString(), item.DateExpired.ToString());
                    old.DateExpired = item.DateExpired;
                }

                if (old.DateStart != item.DateStart)
                {
                    LogHelper.Log(UpdateRecord.License, item.Id, "Дата начала", old.DateStart.ToString(), item.DateStart.ToString());
                    old.DateStart = item.DateStart;
                }

                if (old.Nom != item.Nom)
                {
                    LogHelper.Log(UpdateRecord.License, item.Id, "Номер лицензии", old.Nom, item.Nom);
                    old.Nom = item.Nom;
                }

                if (old.NomOfBook != item.NomOfBook)
                {
                    LogHelper.Log(UpdateRecord.License, item.Id, "Номер дела", old.NomOfBook, item.NomOfBook);
                    old.NomOfBook = item.NomOfBook;
                }

                if (old.Status != item.Status)
                {
                    LogHelper.Log(UpdateRecord.License, item.Id, "Статус", old.Status.ToString(), item.Status.ToString());
                    old.Status = item.Status;
                }
            }
        }

        public void Remove(License item)
        {
            if (item.HasReferences())
            {
                item.IsDeleted = true;
                LogHelper.Log(SetDeletedRecord.License, item.Id);
            }
            else
            {
                JustRemove(item);
            }
        }

        public void JustRemove(License item)
        {
            context.LicenseSet.Remove(item);
            LogHelper.Log(DeleteRecordFromDataBase.License, item.Id, new SerializedEFObject(item));
        }
    }
}
