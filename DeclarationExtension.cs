using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cascade.Licensing.Domain.Concrete;
using Cascade.Licensing.Domain.Abstract;

namespace Cascade.Licensing.Domain
{
    public static class DeclarationExtension
    {
        public static bool HasReferences(this Declaration declaration)
        {
            return declaration.ActionsWithDeclaration.FirstOrDefault()!=null||
                declaration.UnitsInDeclaration.FirstOrDefault()!=null||
                declaration.Revisions.FirstOrDefault()!=null||
                declaration.Documents.FirstOrDefault()!=null||
                declaration.PaymentOrders.FirstOrDefault()!=null;
        }

        public static void SetNewNom(this Declaration declaration, int NewNom, int NewNomYear, LogHelper LogHelper)
        {
            bool NeedRecalculateAllNom = false;
            if (declaration.RegNom != NewNom)
            {
                NeedRecalculateAllNom = true;
                LogHelper.Log(UpdateRecord.Declaration, declaration.Id, "Рег. номер (общий)", declaration.RegNom.ToString(), NewNom.ToString());
                declaration.RegNom = NewNom;
            }
            if (declaration.RegNomYear != NewNomYear)
            {
                NeedRecalculateAllNom = true;
                LogHelper.Log(UpdateRecord.Declaration, declaration.Id, "Рег. номер (в году)", declaration.RegNomYear.ToString(), NewNomYear.ToString());
                declaration.RegNomYear = NewNomYear;
            }
            if (NeedRecalculateAllNom)
            {
                declaration.RegNomAll = NewNomYear.ToString() + "/" + NewNom.ToString("D6");
            }
        }

        public static void LinkUnitInDeclaration(this Declaration declaration, UnitInDeclaration unit_in_declaration, LogHelper LogHelper)
        {
            declaration.UnitsInDeclaration.Add(unit_in_declaration);
            LogHelper.Log(AddLink.Declaration_UnitInDeclaration, declaration.Id, unit_in_declaration.Id, new SerializedEFObject(unit_in_declaration).StrValue);
        }

        public static void LinkActionWithDeclaration(this Declaration declaration, ActionWithDeclaration action_with_declaration, LogHelper LogHelper)
        {
            declaration.ActionsWithDeclaration.Add(action_with_declaration);
            LogHelper.Log(AddLink.Declaration_ActionWithDeclaration, declaration.Id, action_with_declaration.Id, new SerializedEFObject(action_with_declaration).StrValue);
        }

        public static void LinkRevision(this Declaration declaration, Revision revision, LogHelper LogHelper)
        {
            declaration.Revisions.Add(revision);
            LogHelper.Log(AddLink2.Declaration_Revision, declaration.Id, revision.Id, new SerializedEFObject(revision).StrValue);
        }

        public static void LinkDocument(this Declaration declaration, Document document, LogHelper LogHelper)
        {
            declaration.Documents.Add(document);
            LogHelper.Log(AddLink.Declaration_Document, declaration.Id, document.Id, new SerializedEFObject(document).StrValue);
        }

        public static void LinkPaymentOrder(this Declaration declaration, PaymentOrder payment_order, LogHelper LogHelper)
        {
            declaration.PaymentOrders.Add(payment_order);
            LogHelper.Log(AddLink3.Declaration_PaymentOrder, declaration.Id, payment_order.Id, new SerializedEFObject(payment_order).StrValue);
        }

        public static void AttachReasonForRenewList(this Declaration declaration, List<ReasonForRenew> newReasons, LogHelper LogHelper)
        {
            foreach (var rfrNew in newReasons.Where(x => !declaration.ReasonForRenews.Contains(x)).ToList())
            {
                rfrNew.LinkDeclaration(declaration, LogHelper);
            }
            var rfrToDeleteList = declaration.ReasonForRenews.Where(x => !newReasons.Contains(x)).ToList();
            foreach (var rfr in rfrToDeleteList)
            {
                declaration.ReasonForRenews.Remove(rfr);
            }
        }

    }
}
