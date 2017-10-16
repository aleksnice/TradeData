using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cascade.Licensing.Domain.Concrete;
using Cascade.Licensing.Domain.Abstract;

namespace Cascade.Licensing.Domain
{
    public static class CompanyExtension
    {
        public static bool HasReferences(this Company company)
        {
            return
                company.Units.FirstOrDefault() != null ||
                company.Revisions.FirstOrDefault() != null ||
                company.Declarations.FirstOrDefault() != null ||
                company.Licensee.FirstOrDefault() != null ||
                company.Peoples.FirstOrDefault() != null||
                company.ConstituentDocuments.FirstOrDefault() !=null;
        }


        public static void LinkUnit(this Company company, Unit unit, LogHelper LogHelper)
        {
            company.Units.Add(unit);
            LogHelper.Log(AddLink.Company_Unit, company.Id, unit.Id, new SerializedEFObject(unit).StrValue);
        }

        public static void LinkLicense(this Company company, License license, LogHelper LogHelper)
        {
            company.Licensee.Add(license);
            LogHelper.Log(AddLink.Company_License, company.Id, license.Id, new SerializedEFObject(license).StrValue);
        }

        public static void LinkDeclaration(this Company company, Declaration declaration, LogHelper LogHelper)
        {
            company.Declarations.Add(declaration);
            LogHelper.Log(AddLink.Company_Declaration, company.Id, declaration.Id, new SerializedEFObject(declaration).StrValue);
        }

        

        public static void LinkRevision(this Company company, Revision revision, LogHelper LogHelper)
        {
            company.Revisions.Add(revision);
            LogHelper.Log(AddLink2.Company_Revision, company.Id, revision.Id, new SerializedEFObject(revision).StrValue);
        }

        public static void LinkPeople(this Company company, People people, LogHelper LogHelper)
        {
            company.Peoples.Add(people);
            LogHelper.Log(AddLink3.Company_People, company.Id, people.Id, new SerializedEFObject(people).StrValue);
        }

        public static void LinkDocument(this Company company, Document document, LogHelper LogHelper)
        {
            company.ConstituentDocuments.Add(document);
            LogHelper.Log(AddLink.Company_Document, company.Id, document.Id, new SerializedEFObject(document).StrValue);
        }
    }
}
