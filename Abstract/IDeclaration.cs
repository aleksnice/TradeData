using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cascade.Licensing.Domain.Abstract
{
    public interface IDeclaration
    {
        IQueryable<Declaration> Items { get; }

        IQueryable<Declaration> GetNotDeletedItems();

        String SwitchNumbers(int itemId, String newRegNumAll);

        Declaration Add(Declaration item, LicensedActivityType license_activity_type, LegalFormType legal_form_type, Company company, License license);

        void Remove(Declaration item);

        void JustRemove(Declaration item);
    }
}
