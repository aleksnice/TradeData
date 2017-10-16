using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cascade.Licensing.Domain.Abstract;
using System.Security.Principal;
using System.Data.Entity;

namespace Cascade.Licensing.Domain.Concrete
{
    public class EFPeople : IPeople
    {
        private EFDbContextContainer context;
        private People UserDB;
        LogHelper LogHelper;
        public EFPeople(EFDbContextContainer context, People UserDB, LogHelper logHelper)
        {
            LogHelper = logHelper;
            this.context = context;
            this.UserDB = UserDB;
        }


        public DbSet<People> GetPeopleSet()
        {
             return context.PeopleSet; 
        }

        public IQueryable<People> Items
        {
            get { return context.PeopleSet; }
        }

        public IQueryable<People> GetNotDeletedItems()
        {
            return context.PeopleSet.Where(j => j.IsDeleted != true);
        }

        public IQueryable<People> GetNotDeletedItemsWithoutOrgUnit()
        {
            return GetNotDeletedItems().Where(j => j.OrgUnit == null);
        }

        public IQueryable<People> GetItemsForCompany(int id_company)
        {
            return GetNotDeletedItemsWithoutOrgUnit().Where(j => j.Company.FirstOrDefault(i => i.Id == id_company) != null);
        }

        public IQueryable<Log> GetNewLogs()
        {
           

            return context.LogSet.Where(j => !j.id_object2.HasValue && j.TypeObjectName == "People" && j.TypeAction == "UpdateRecord");
        }

        public DbSet<Log> GetLogs()
        {
            return context.LogSet;
        }

        public int GetNewLogsCount()
        {
            return context.LogSet.Where(j => !j.id_object2.HasValue && j.TypeObjectName == "People" && j.TypeAction == "UpdateRecord").Count();
        }

        public void SetLogRecordsAsViewved(List<int> ids)
        {
            foreach (var id in ids)
            {
                var log_record = context.LogSet.First(j => j.Id == id);
                log_record.id_object2 = 0;

            }
            context.SaveChanges();
        }

        public void Add(People item, OrgUnit org_unit, Company company)
        {
            if (item.job == null) { item.job = ""; }

            if (item.Tel == null) { item.Tel = ""; }

            if (item.EMail == null) { item.EMail = ""; }

            if (item.Id == 0)
            {
                item.IsDeleted = null;
                item.DateTimeCreated = DateTime.Now;
                item.DateTimeLastActivity = DateTime.Now;
                item.DateTimeSetPassword = DateTime.Now.AddDays(119);
                item.CreatorName = UserDB == null ? "System" : UserDB.FIO;
                context.PeopleSet.Add(item);

                if (org_unit != null)
                {
                    org_unit.LinkPeople(item, LogHelper);
                }

                if (company != null)
                {
                    company.LinkPeople(item, LogHelper);
                }

                LogHelper.Log(TryCreateRecord.People, new SerializedEFObject(item));
            }
            else
            {
                People old = context.PeopleSet.First(x => x.Id == item.Id);

                if (old.EMail != item.EMail)
                {
                    string old_value = old.EMail;
                    string new_value = item.EMail;
                    LogHelper.Log(UpdateRecord.People, item.Id, "Эл. почта", old_value, new_value, "Свойство 'Эл. почта' был изменен с '" + old_value + "' на '" + new_value + "'");
                    old.EMail = item.EMail;
                }

                if (old.FIO != item.FIO)
                {
                    string old_value = old.FIO;
                    string new_value = item.FIO;
                    LogHelper.Log(UpdateRecord.People, item.Id, "ФИО", old_value, new_value, "Свойство 'ФИО' был изменен с '" + old_value + "' на '" + new_value + "'");
                    old.FIO = item.FIO;
                }

                if (old.job != item.job)
                {
                    string old_value = old.job;
                    string new_value = item.job;
                    LogHelper.Log(UpdateRecord.People, item.Id, "Должность", old_value, new_value, "Свойство 'Должность' был изменен с '" + old_value + "' на '" + new_value + "'");
                    old.job = item.job;
                }

                if (old.Tel != item.Tel)
                {
                    string old_value = old.Tel;
                    string new_value = item.Tel;
                    LogHelper.Log(UpdateRecord.People, item.Id, "Телефон", old_value, new_value, "Свойство 'Телефон' был изменен с '" + old_value + "' на '" + new_value + "'");
                    old.Tel = item.Tel;
                }

                if (old.IsGeneral != item.IsGeneral)
                {
                    string old_value = old.IsGeneral.ToString();
                    string new_value = item.IsGeneral.ToString();
                    LogHelper.Log(UpdateRecord.People, item.Id, "Признак ген. директора", old_value, new_value, "Свойство 'Признак ген. директора' был изменен с '" + old_value + "' на '" + new_value + "'");
                    old.IsGeneral = item.IsGeneral;
                }

                if (old.IsChiefAccountant != item.IsChiefAccountant)
                {
                    string old_value = old.IsChiefAccountant.ToString();
                    string new_value = item.IsChiefAccountant.ToString();
                    LogHelper.Log(UpdateRecord.People, item.Id, "Признак гл. бухгалтера", old_value, new_value, "Свойство 'Признак гл. бухгалтера' был изменен с '" + old_value + "' на '" + new_value + "'");
                    old.IsChiefAccountant = item.IsChiefAccountant;
                }

                if (old.login != item.login)
                {
                    string old_value = old.login;
                    string new_value = item.login;
                    LogHelper.Log(UpdateRecord.People, item.Id, "Логин", old_value, new_value, "Свойство 'Логин' был изменен с '" + old_value + "' на '" + new_value + "'");
                    old.login = item.login;
                }

                if (old.IsDisabled != item.IsDisabled)
                {
                    string old_value = old.IsDisabled.ToString();
                    string new_value = item.IsDisabled.ToString();
                    LogHelper.Log(UpdateRecord.People, item.Id, "Флаг блокировки пользователя", old_value, new_value, "Свойство 'Флаг блокировки пользователя' был изменен с '" + old_value + "' на '" + new_value + "'");
                    old.IsDisabled = item.IsDisabled;

                    if (!item.IsDisabled)
                    {
                        old.DateTimeLastActivity = DateTime.Now;
                    }
                }

                if (old.IsTemporary != item.IsTemporary)
                {
                    string old_value = old.IsTemporary.ToString();
                    string new_value = item.IsTemporary.ToString();
                    LogHelper.Log(UpdateRecord.People, item.Id, "Флаг временной записи", old_value, new_value, "Свойство 'Флаг временной записи' был изменен с '" + old_value + "' на '" + new_value + "'");
                    old.IsTemporary = item.IsTemporary;
                }
                
                if (old.DateTimeExpired != item.DateTimeExpired)
                {
                    string old_value = UtilsClass.ToSafeStringDateTime(old.DateTimeExpired);
                    string new_value = UtilsClass.ToSafeStringDateTime(item.DateTimeExpired);
                    LogHelper.Log(UpdateRecord.People, item.Id, "Срок блокировки временной учетной записи", old_value, new_value,"Свойство 'Срок действия учетной записи' был изменен с '"+old_value+"' на '"+new_value+"'");
                    old.DateTimeExpired = item.DateTimeExpired;
                }

            }
        }

        public void SetNeedChangePasswordPeople(People p, string Reason)
        {
            if (!p.IsNeedSetPassword)
            {
                p.IsNeedSetPassword = true;
                LogHelper.Log(UpdateRecord.People, p.Id, "Флаг истечения срока действия пароля", "false", "true", Reason);
            }
        }

        public void DisablePeople(People p, string Reason)
        {
            if (!p.IsDisabled)
            {
                p.IsDisabled = true;
                LogHelper.Log(UpdateRecord.People, p.Id, "Флаг блокировки пользователя", "false", "true", Reason);
            }
        }

        public void ChangePasswordPeople(People p)
        {
            LogHelper.Log(UpdateRecord.People, p.Id, "Пароль пользователя был изменен", "", "", "Пароль пользователя был изменен");
        }

        public void CreateNewPeople(People p)
        {
            LogHelper.Log(UpdateRecord.People, p.Id, "", "", "", "Создана учетная запись");
        }

        public void LockOutPeople(People p)
        {
            LogHelper.LogSystemUpdate(UpdateRecord.People, p.Id, "Отказ в аунтификации после неуспешных попыток входа", "", "","Запись пользователя была заблокирована на 5 минут после 5 неуспешных попыток входа.");
        }

        public void Remove(People item)
        {

            //TODO после вернуть проверку на наличие связей - иначе можно многие имена заблокировать на 1 год
            //пока, чтобы можно было спокойно проверить, уберем удаление без связей

            item.IsDeleted = true;
            item.DateTimeDeleted = DateTime.Now;
            LogHelper.Log(SetDeletedRecord.People, item.Id);

            //if (item.HasReferences())
            //{
            //    item.IsDeleted = true;
            //    LogHelper.Log(SetDeletedRecord.People, item.Id);
            //}
            //else
            //{
            //    JustRemove(item);
            //}
        }

        public void JustRemove(People item)
        {
            context.PeopleSet.Remove(item);
            LogHelper.Log(DeleteRecordFromDataBase.People, item.Id, new SerializedEFObject(item));
        }
    }
}
