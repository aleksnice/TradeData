using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cascade.Licensing.Domain.Abstract
{
    public interface IOrgUnit
    {
        IQueryable<OrgUnit> Items { get; }

        IQueryable<OrgUnit> GetNotDeletedItems();


        void Add(OrgUnit item);

        void Remove(OrgUnit item);

        void JustRemove(OrgUnit item);
    }
}
