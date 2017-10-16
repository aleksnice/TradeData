using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cascade.Licensing.Domain;

namespace Cascade.Licensing.WebUI.Models
{

    public class CompanySearchResult
    {
        public string name { get; set; }

        public int CompanyId { get; set; }

        public string url { get; set; }
    }


    public class FilteredCompanyListViewModel
    {
        public int Count { get; set; }

        public List<CompanyViewModel> company_list { get; set; }

       
    }

    public class DictionaryRecordViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public bool IsChecked { get; set; }
        public string CodeName { get; set; }

    }


    public class CompanyViewModel
    {
        public int Id { get; set; }

        public string CompanyName { get; set; }

        public string FullName { get; set; }

        public string ShortName { get; set; }

        public string INN { get; set; }

        public string KPP { get; set; }


        public DictionaryRecordViewModel LegalFormType { get; set; }


        public string Email { get; set; }

        public string Address { get; set; }

        public string AddressHouse { get; set; }

        public string AddressBuildNom { get; set; }

        public string AddressLitera { get; set; }

        public string AddressRoomNom { get; set; }

        public string FSRARAddress { get; set; }
        public string AddressForDocument { get; set; }

        public int id_addr { get; set; }

        public string PostalAddress { get; set; }

        public string PostalAddressHouse { get; set; }

        public string PostalAddressBuildNom { get; set; }

        public string PostalAddressLitera { get; set; }

        public string PostalAddressRoomNom { get; set; }

        public string FSRARPostalAddress { get; set; }
        public int id_postal_addr { get; set; }

        public string PostalCode { get; set; }

        public string OGRN { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }

        public string BanksName { get; set; }

        public string CheckingAccount { get; set; }

        public string DateCreatedStr { get; set; }

        public string Creator { get; set; }

        public bool IsChanged { get; set; }

        public bool IsSelected { get; set; }


        public CompanyViewModel()
        {
            LegalFormType = new DictionaryRecordViewModel() { Id=1, Name = "ООО", FullName = "Общество с ограниченной ответственностью"};

            id_addr = -1;

            id_postal_addr = -1;

            IsChanged = false;
            IsSelected = false;

            AddressBuildNom = "";
            AddressHouse = "";
            AddressLitera = "";
            AddressRoomNom = "";
            FSRARAddress = "";

            PostalAddressBuildNom = "";
            PostalAddressHouse = "";
            PostalAddressLitera = "";
            PostalAddressRoomNom = "";
            FSRARPostalAddress = "";



            AddressForDocument = "";
            PostalCode = "";
        }

        public static char[] AddressAddonSep = new char[] { '|'};
        

        public CompanyViewModel(CompanyExportFromDatabase cc)
        {
            Company c = cc.company;
            Id = c.Id;
            FullName = c.FullName;
            CompanyName = c.Name;
            ShortName = c.ShortName;
            INN = c.INN;
            KPP = c.KPP;
            LegalFormType = new DictionaryRecordViewModel() { Name = cc.legal_form_type.ShortName, Id = cc.legal_form_type.Id, FullName = cc.legal_form_type.FullName };

            OGRN = c.ORGN;

            id_addr = cc.addr == null ? -1 : cc.addr.Id;

            id_postal_addr = cc.addr_postal == null ? -1 : cc.addr_postal.Id;


            Email = c.Email;
            Address = c.ActualAddrStr;
            AddressForDocument = c.AddressForDocument;
            PostalAddress = c.PostalAddrStr;
            PostalCode = c.PostalCode;
            DateCreatedStr = c.DateTimeCreated.ToString();
            Creator = c.CreatorName;

            Phone = c.Phone;

            CheckingAccount = c.CheckingAccount;
            BanksName = c.BanksName;

            Fax = c.Fax;

            IsChanged = false;
            IsSelected = false;

            var act_addr_arr = c.ActualAddrAddon.Split(AddressAddonSep);

            AddressHouse = act_addr_arr[0];
            AddressBuildNom = act_addr_arr[1];
            AddressLitera = act_addr_arr[2];
            AddressRoomNom = act_addr_arr[3];
            FSRARAddress = c.FSRARActualAddr;

            var post_addr_arr = c.PostalAddrAddon.Split(AddressAddonSep);

            PostalAddressHouse = post_addr_arr[0];
            PostalAddressBuildNom = post_addr_arr[1];
            PostalAddressLitera = post_addr_arr[2];
            PostalAddressRoomNom = post_addr_arr[3];
            FSRARPostalAddress = c.FSRARPostalAddr;
        }

        

        public static List<CompanyViewModel> GetListViewModel(IQueryable<Company> company_list)
        {
            List<CompanyViewModel> res = new List<CompanyViewModel>();

            foreach (var c in company_list.Select(j => new CompanyExportFromDatabase() { company = j, legal_form_type = j.LegalFormType, addr = j.ActualAddr, addr_postal = j.PostalAddr}).ToList())
            {
                res.Add(new CompanyViewModel(c));

            }
            return res;
        }

    }

    public class CompanyExportFromDatabase
    {
        public Company company { get; set; }

        public LegalFormType legal_form_type { get; set; }

        public Addr addr { get; set; }

        public Addr addr_postal { get; set; }
    }
}