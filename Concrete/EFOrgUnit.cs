using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cascade.Licensing.Domain.Abstract;
using System.Security.Principal;

namespace Cascade.Licensing.Domain.Concrete
{
    public class EFOrgUnit:IOrgUnit
    {
        private EFDbContextContainer context;
        private People UserDB;
        LogHelper LogHelper;
        public EFOrgUnit(EFDbContextContainer context, People UserDB, LogHelper logHelper)
        {
            LogHelper = logHelper;
            this.context = context;
            this.UserDB = UserDB;
        }

        public IQueryable<OrgUnit> Items
        {
            get { return context.OrgUnitSet; }
        }

        public IQueryable<OrgUnit> GetNotDeletedItems()
        {
            return context.OrgUnitSet.Where(j => j.IsDeleted != true);
        }

        public void Add(OrgUnit item)
        {
            item.Address = item.Address == null ? "" : item.Address;
            item.INN = item.INN == null ? "" : item.INN;
            item.KPP = item.KPP == null ? "" : item.KPP;
            item.Name = item.Name == null ? "" : item.Name;
            item.OGRN = item.OGRN == null ? "" : item.OGRN;
            item.Tel = item.Tel == null ? "" : item.Tel;
            item.Fax = item.Fax == null ? "" : item.Fax;
            item.EMail = item.EMail == null ? "" : item.EMail;
            item.PostalCode = item.PostalCode == null ? "" : item.PostalCode;

            if (item.Id == 0)
            {
                item.IsDeleted = null;
                item.DateTimeCreated = DateTime.Now;
                item.CreatorName = UserDB == null ? "System" : UserDB.FIO;
                context.OrgUnitSet.Add(item);
                LogHelper.Log(TryCreateRecord2.OrgUnit, new SerializedEFObject(item));
            }
            else
            {
                OrgUnit old = context.OrgUnitSet.First(x => x.Id == item.Id);

                if (old.Address != item.Address)
                {
                    LogHelper.Log(UpdateRecord2.OrgUnit, item.Id, "Адрес", old.Address, item.Address);
                    old.Address = item.Address;
                }

                if (old.INN != item.INN)
                {
                    LogHelper.Log(UpdateRecord2.OrgUnit, item.Id, "ИНН", old.INN, item.INN);
                    old.INN = item.INN;
                }

                if (old.KPP != item.KPP)
                {
                    LogHelper.Log(UpdateRecord2.OrgUnit, item.Id, "КПП", old.KPP, item.KPP);
                    old.KPP = item.KPP;
                }

                if (old.Name != item.Name)
                {
                    LogHelper.Log(UpdateRecord2.OrgUnit, item.Id, "Название", old.Name, item.Name);
                    old.Name = item.Name;
                }

                if (old.OGRN != item.OGRN)
                {
                    LogHelper.Log(UpdateRecord2.OrgUnit, item.Id, "Огрн", old.OGRN, item.OGRN);
                    old.OGRN = item.OGRN;
                }

                if (old.PostalCode != item.PostalCode)
                {
                    LogHelper.Log(UpdateRecord2.OrgUnit, item.Id, "Почтовый индекс", old.PostalCode, item.PostalCode);
                    old.PostalCode = item.PostalCode;
                }

                if (old.Tel != item.Tel)
                {
                    LogHelper.Log(UpdateRecord2.OrgUnit, item.Id, "Телефон", old.Tel, item.Tel);
                    old.Tel = item.Tel;
                }

                if (old.Fax != item.Fax)
                {
                    LogHelper.Log(UpdateRecord2.OrgUnit, item.Id, "Факс", old.Fax, item.Fax);
                    old.Fax = item.Fax;
                }

                if (old.EMail != item.EMail)
                {
                    LogHelper.Log(UpdateRecord2.OrgUnit, item.Id, "Почта", old.EMail, item.EMail);
                    old.EMail = item.EMail;
                }

                
            }
        }

        public void Remove(OrgUnit item)
        {
            if (item.HasReferences())
            {
                item.IsDeleted = true;
                LogHelper.Log(SetDeletedRecord2.OrgUnit, item.Id);
            }
            else
            {
                JustRemove(item);
            }
        }

        public void JustRemove(OrgUnit item)
        {
            context.OrgUnitSet.Remove(item);
            LogHelper.Log(DeleteRecordFromDataBase2.OrgUnit, item.Id, new SerializedEFObject(item));
        }
    }
}
