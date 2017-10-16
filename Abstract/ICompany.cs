using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cascade.Licensing.Domain.Abstract
{
    public interface ICompany
    {
        IQueryable<Company> Items { get; }

        IQueryable<Company> GetNotDeletedItems();


        void Add(Company item, LegalFormType legal_form_type);

        void Remove(Company item);

        void JustRemove(Company item);
    }
}
