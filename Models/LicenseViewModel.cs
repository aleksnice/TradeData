using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cascade.Licensing.Domain;


namespace Cascade.Licensing.WebUI.Models
{
    public class FilteredLicenseListViewModel
    {
        public int Count { get; set; }

        public List<LicenseViewModel> license_list { get; set; }


    }

    public class LicenseViewModel
    {
        public int Id { get; set; }
        public string LicenseNom { get; set; }

        public string CompanyShortName { get; set; }

        public int CompanyId { get; set; }

        public string CompanyINN { get; set; }

        public string CompanyKPP { get; set; }

        public string CompanyOGRN { get; set; }

        public string CompanyAddress { get; set; }

        public string CompanyEmail { get; set; }

        public string LicenseDateStart { get; set; }

        public string LicenseDateExpired { get; set; }

        public string LicenseStatusRus { get; set; }

        public string LastActionDate { get; set; }

        public string LastActionReason { get; set; }

        public string LastLicenseFormNom { get; set; }

        public string NomOfBook { get; set; }

        public LicenseViewModel()
        {
 
        }

        public LicenseViewModel(LicenseExportFromDatabase l_exp)
        {
            License l = l_exp.license;

            Id = l.Id;
            LicenseNom = l.Nom;
            NomOfBook = l.NomOfBook;
            LicenseDateStart = l.DateStart.ToShortDateString();
            LicenseDateExpired = l.DateExpired.ToShortDateString();

            LicenseStatusRus = LicenseViewModelForCompany.GetRusLicenseStatus(l.Status);

            Company c = l_exp.company;

            CompanyId = c.Id;
            CompanyShortName = c.ShortName;
            CompanyINN = c.INN;
            CompanyKPP = c.KPP;
            CompanyOGRN = c.ORGN;
            CompanyAddress = c.ActualAddrStr;
            CompanyEmail = c.Email;
            ActionWithLicense a = l_exp.last_action;
            if (a != null)
            {
                LastActionDate = a.DateTimeAction.ToShortDateString();

                LastActionReason = a.Reason;
            }
            else
            {
                LastActionDate = "";

                LastActionReason = "";
            }
            LicenseForm lf = l_exp.last_license_form;
            if (lf != null)
            {
                LastLicenseFormNom = lf.Nom;
            }
            else
            {
                LastLicenseFormNom = "";
            }
        }

        public static List<LicenseViewModel> GetListViewModel(IQueryable<License> license_list)
        {
            List<LicenseViewModel> res = new List<LicenseViewModel>();

            foreach (var l in license_list.Select(j => new LicenseExportFromDatabase() { license = j, company = j.Company, last_action = j.LastActionWithLicense, last_license_form = j.LastLicenseForm }).ToList())
            {
                res.Add(new LicenseViewModel(l));

            }
            return res;
        }
    }

    public class LicenseExportFromDatabase
    {
        public License license { get; set; }

        public Company company { get; set; }

        public ActionWithLicense last_action { get; set; }

        public LicenseForm last_license_form { get; set; }
    }


    public class UnitInLicenseFormViewModelShort
    {
        public int Id { get; set; }
        public string Address { get; set; }
    }

    public class ActionWithLicenseWithLicenseFormViewModel
    {
        public int IdAction { get; set; }
        public string Reason { get; set; }

        public string DateAction { get; set; }

        public bool IsSelected { get; set; }

        public bool HasResolution { get; set; }

        public string ResolutionNom { get; set; }

        public int ResolutionId { get; set; }

        public bool HasLicenseForm { get; set; }

        public int IdLicenseForm { get; set; }

        public string Nom { get; set; }

        public string CompanyName { get; set; }

        public string CompanyAddress { get; set; }

        public string Email { get; set; }

        public string DateExpired { get; set; }


        public List<UnitInLicenseFormViewModelShort> Units { get; set; }

        public ActionWithLicenseWithLicenseFormViewModel()
        {
            Units = new List<UnitInLicenseFormViewModelShort>();
            IsSelected = false;
            HasLicenseForm = false;
        }

        public ActionWithLicenseWithLicenseFormViewModel(ActionWithLicense awl)
        {
            IdAction = awl.Id;
            Reason = awl.Reason;
            DateAction = awl.DateTimeAction.ToShortDateString();
            IsSelected = false;

            HasResolution = awl.Resolution != null;

            if (HasResolution)
            {
                ResolutionId = awl.Resolution.Id;
                ResolutionNom = awl.Resolution.RegNomAll;
            }

            HasLicenseForm = awl.LicenseForm!=null;

            if (HasLicenseForm)
            {
                LicenseForm lf = awl.LicenseForm;
                IdLicenseForm = lf.Id;
                Nom = lf.Nom;
                CompanyName = lf.FullNameCompany;
                CompanyAddress = lf.AddressCompany;
                Email = lf.Email;
                DateExpired = lf.DateExpired.ToShortDateString();

                Units = new List<UnitInLicenseFormViewModelShort>();
                foreach (var u in lf.UnitsInLicenseForm.OrderBy(j => j.Address))
                {
                    Units.Add(new UnitInLicenseFormViewModelShort() { Id = u.Id, Address = u.Address });
                }
            }
            else
            {
                Nom = "";
                CompanyName = "";
                CompanyAddress ="";
                Email = "";
                DateExpired = "";
                Units = new List<UnitInLicenseFormViewModelShort>();
            }

            


        }

        public static List<ActionWithLicenseWithLicenseFormViewModel> GetListViewModel(IQueryable<ActionWithLicense> actions_list)
        {
            List<ActionWithLicenseWithLicenseFormViewModel> res = new List<ActionWithLicenseWithLicenseFormViewModel>();

            foreach (var a in actions_list)
            {
                res.Add(new ActionWithLicenseWithLicenseFormViewModel(a));

            }
            return res;
        }
    }

    public class LicenseViewModelForCompany
    {
        public int Id {get;set;}
        public string Nom { get; set; }

        public string NomOfBook { get; set; }
        public string DateStart { get; set; }
        public string DateExpired { get; set; }

        public string Status { get; set; }

        public string StatusRus { get; set; }

        public string DateCreatedStr { get; set; }

        public string Creator { get; set; }

        public bool IsChanged { get; set; }

        public bool IsSelected { get; set; }

        public List<ActionWithLicenseWithLicenseFormViewModel> Actions {get;set;}

        public LicenseViewModelForCompany()
        {
            Actions = new List<ActionWithLicenseWithLicenseFormViewModel>();

            IsChanged = false;
            IsSelected = false;
        }

        public static string GetRusLicenseStatus(LicenseStatus ls)
        {
            string res = "";
            switch (ls)
            {
                case LicenseStatus.Active:
                    {
                        res = "Действующая"; 
                        break;
                    }
                case LicenseStatus.Canceled:
                    {
                        res = "Аннулирована";
                        break;
                    }
                case LicenseStatus.Discontinued:
                    {
                        res = "Прекращена";
                        break;
                    }
                case LicenseStatus.Expired:
                    {
                        res = "Истекла";
                        break;
                    }
                case LicenseStatus.Suspended:
                    {
                        res = "Приостановлена";
                        break;
                    }
            }
            return res;
        }

        public LicenseViewModelForCompany(License l)
        {
            Id = l.Id;
            Nom = l.Nom;
            DateStart = l.DateStart.ToShortDateString();
            DateExpired = l.DateExpired.ToShortDateString();
            Status = l.Status.ToString();
            StatusRus = GetRusLicenseStatus(l.Status);
            IsChanged = false;
            IsSelected = false;
            NomOfBook = l.NomOfBook;

            Actions = ActionWithLicenseWithLicenseFormViewModel.GetListViewModel(l.ActionsWithLicense.Where(j => j.IsDeleted != true).OrderByDescending(j => j.DateTimeAction).AsQueryable());

        }

        public static List<LicenseViewModelForCompany> GetListViewModel(IQueryable<License> license_list)
        {
            List<LicenseViewModelForCompany> res = new List<LicenseViewModelForCompany>();

            foreach (var l in license_list.ToList())
            {
                res.Add(new LicenseViewModelForCompany(l));

            }
            return res;
        }
    }
}