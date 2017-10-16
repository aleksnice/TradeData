using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cascade.Licensing.Domain.Abstract
{
    public interface IPeople
    {
        IQueryable<People> Items { get; }

        IQueryable<People> GetNotDeletedItems();

        IQueryable<People> GetNotDeletedItemsWithoutOrgUnit();

        IQueryable<People> GetItemsForCompany(int id_company);

        IQueryable<Log> GetNewLogs();

        DbSet<Log> GetLogs();

        DbSet<People> GetPeopleSet();

        void SetLogRecordsAsViewved(List<int> ids);
        int GetNewLogsCount();

        void Add(People item, OrgUnit org_unit, Company company);

        void DisablePeople(People p, string Reason);

        void SetNeedChangePasswordPeople(People p, string Reason);

        void ChangePasswordPeople(People p);

        void CreateNewPeople(People p);

        void LockOutPeople(People p);
        void Remove(People item);

        void JustRemove(People item);
    }
}
