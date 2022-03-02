using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CommonLibraries.Models
{
    public class LicenseFile
    {
        [XmlAttribute()]
        public string License { get; set; }

        [XmlAttribute()]
        public DateTime ExpirationDate { get; set; }
    }
}
