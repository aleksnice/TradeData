using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cascade.Licensing.Domain.Abstract
{
    public interface ILicense
    {
        IQueryable<License> Items { get; }

        IQueryable<License> GetNotDeletedItems();
       
        IQueryable<License> GetItemsByCompany(int id_company);
        void Add(License item, LicensedActivityType licensed_activity_type, Company company);

        void Remove(License item);

        void JustRemove(License item);
    }
}
