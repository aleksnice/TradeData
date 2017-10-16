using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cascade.Licensing.Domain.Abstract
{
    public interface IResolution
    {
        IQueryable<Resolution> Items { get; }

        IQueryable<Resolution> GetNotDeletedItems();

        IQueryable<Resolution> GetNotDraftItems();
        Resolution Add(LicensedActivityType licensed_activity_type, Resolution item);

        void Remove(Resolution item);

        void JustRemove(Resolution item);

        int GenerateResolutionNom();
    }
}
