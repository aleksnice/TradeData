using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cascade.Licensing.WebUI.Models
{
    public class LATForOrgUnitViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }


    public class OrgUnitViewModel
    {
        public int Id { get; set; }

        public bool HasPeoples { get; set; }

        public ValueTagTableViewModel TagsTable { get; set; }

        public string Address { get; set; }

        public string PostalCode { get; set; }

        public string Tel { get; set; }

        public string Fax { get; set; }

        public string EMail { get; set; }
        public string Name { get; set; }

        public string INN { get; set; }

        public string KPP { get; set; }

        public string OGRN { get; set; }

        public bool IsSelected { get; set; }

        public List<LATForOrgUnitViewModel> LicenseActivityTypes { get; set; }

        public bool IsError { get; set; }
    }
}