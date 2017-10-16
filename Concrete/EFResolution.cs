using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cascade.Licensing.Domain.Abstract;
using System.Security.Principal;

namespace Cascade.Licensing.Domain.Concrete
{
    public class EFResolution:IResolution
    {
        private EFDbContextContainer context;
        private People UserDB;
        LogHelper LogHelper;
        public EFResolution(EFDbContextContainer context, People UserDB, LogHelper logHelper)
        {
            LogHelper = logHelper;
            this.context = context;
            this.UserDB = UserDB;
        }


        public IQueryable<Resolution> Items
        {
            get { return context.ResolutionSet; }
        }

        public IQueryable<Resolution> GetNotDeletedItems()
        {
            return context.ResolutionSet.Where(j => j.IsDeleted != true && j.LicensedActivityTypeId == UserDB.LicensedActivityTypeId);
        }

        public IQueryable<Resolution> GetNotDraftItems()
        {
            return GetNotDeletedItems().Where(j => !j.IsDraft);
        }

        public Resolution Add(LicensedActivityType licensed_activity_type, Resolution item)
        {
            item.Comm = item.Comm == null ? "" : item.Comm;
            item.RefuseComment = item.RefuseComment == null ? "" : item.RefuseComment;
            item.RegNomAll = item.RegNomAll == null ? "" : item.RegNomAll;
            if (item.LicenseNomOfBookAll == null) { item.LicenseNomOfBookAll = ""; }
            if (item.LicenseNomAll == null) { item.LicenseNomAll = ""; }
            if (item.LicenseFormNomAll == null) { item.LicenseFormNomAll = ""; }
            if (item.CompanyShortNameAll == null) { item.CompanyShortNameAll = ""; }
            if (item.CompanyINNAll == null) { item.CompanyINNAll = ""; }
            if (item.PaymentOrdersAll == null) { item.PaymentOrdersAll = ""; }
            if (item.TypeOfResolutionsAll == null) { item.TypeOfResolutionsAll = ""; }
            if (item.CreatorName == null) { item.CreatorName = ""; }
            if (item.Id == 0)
            {
                item.IsDeleted = null;
                item.DateTimeCreated = DateTime.Now;
                item.CreatorName = UserDB == null ? "System" : UserDB.FIO;
                context.ResolutionSet.Add(item);
                licensed_activity_type.LinkResolution(item, LogHelper);

                if (item.RegNom != 0)
                {
                    item.RegNomAll = item.RegNom.ToString("D5");
                }
                //type_of_resolution.LinkResolution(item);
                LogHelper.Log(TryCreateRecord2.Resolution, new SerializedEFObject(item));
                
                return item;
            }
            else
            {
                Resolution old = context.ResolutionSet.First(x => x.Id == item.Id);

                old.LicenseNomOfBookAll = item.LicenseNomOfBookAll;
                old.LicenseNomAll = item.LicenseNomAll;
                old.LicenseFormNomAll = item.LicenseFormNomAll;
                old.CompanyShortNameAll = item.CompanyShortNameAll;
                old.CompanyINNAll = item.CompanyINNAll;
                old.TypeOfResolutionsAll = item.TypeOfResolutionsAll;
                old.RefuseComment = item.RefuseComment;
                if (old.Result != item.Result)
                {
                    LogHelper.Log(UpdateRecord2.Resolution, item.Id, "Результат решения", old.Result.ToString(), item.Result.ToString());
                    old.Result = item.Result;
                }

                if (old.CreatorName != item.CreatorName)
                {
                    LogHelper.Log(UpdateRecord2.Resolution, item.Id, "Сотрудник", old.CreatorName, item.CreatorName);
                    old.CreatorName = item.CreatorName;
                }

                if (old.Comm != item.Comm)
                {
                    LogHelper.Log(UpdateRecord2.Resolution, item.Id, "Комментарий", old.Comm, item.Comm);
                    old.Comm = item.Comm;
                }

                if (old.Date != item.Date)
                {
                    LogHelper.Log(UpdateRecord2.Resolution, item.Id, "Дата", old.Date.ToString(), item.Date.ToString());
                    old.Date = item.Date;
                }

                if (old.IsDraft != item.IsDraft)
                {
                    LogHelper.Log(UpdateRecord2.Resolution, item.Id, "Признак черновика", old.IsDraft.ToString(), item.IsDraft.ToString());
                    old.IsDraft = item.IsDraft;
                }

                if (old.RegNom != item.RegNom)
                {
                    LogHelper.Log(UpdateRecord2.Resolution, item.Id, "Рег. номер", old.RegNom.ToString(), item.RegNom.ToString());
                    old.RegNom = item.RegNom;
                    old.RegNomAll = item.RegNom.ToString("D5");
                }

                if (old.LicenceApproverPeopleId != item.LicenceApproverPeopleId)
                {
                    LogHelper.Log(UpdateRecord2.Resolution, item.Id, "Визирование лицензии", old.LicenceApproverPeopleId.ToSafeString(), item.LicenceApproverPeopleId.ToSafeString());
                    old.LicenceApproverPeopleId = item.LicenceApproverPeopleId;
                }

                if (old.ResolutionCommissionChairmanPeopleId != item.ResolutionCommissionChairmanPeopleId)
                {
                    LogHelper.Log(UpdateRecord2.Resolution, item.Id, "Председатель комиссии по решениям", old.ResolutionCommissionChairmanPeopleId.ToSafeString(), item.ResolutionCommissionChairmanPeopleId.ToSafeString());
                    old.ResolutionCommissionChairmanPeopleId = item.ResolutionCommissionChairmanPeopleId;
                }

                return old;
            }
        }

        public void Remove(Resolution item)
        {
            if (item.HasReferences())
            {
                item.IsDeleted = true;
                LogHelper.Log(SetDeletedRecord2.Resolution, item.Id);
            }
            else
            {
                JustRemove(item);
            }
        }

        public void JustRemove(Resolution item)
        {
            context.ResolutionSet.Remove(item);
            LogHelper.Log(DeleteRecordFromDataBase2.Resolution, item.Id, new SerializedEFObject(item));
        }

        public int GenerateResolutionNom()
        {
            int reg_nom = 1;
            DateTime dt = new DateTime(DateTime.Now.Year, 1, 1);
            var now_res = context.ResolutionSet.Where(j => j.IsDeleted != true && !j.IsDraft && j.Date>dt).OrderByDescending(j => j.RegNom).Take(1).Select(j => new { id = j.Id, nom = j.RegNom }).ToList();
            if (now_res.Count > 0)
            {
                var now_res_last = now_res.First();
                reg_nom = now_res_last.nom + 1;
            }
            return reg_nom;
        }
    }
}
