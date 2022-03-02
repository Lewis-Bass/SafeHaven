using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSiteCustomerTestHarness
{
    public class NameSource
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        public NameSource() { }
        public NameSource(string fromString)
        {
            var split = fromString.Split(new char[] { ',' });
            if (split.Length >= 6)
            {
                FirstName = split[0];
                LastName = split[1];
                Address = split[2];
                City = split[3];
                State = split[4];
                Zip = split[5];
            }
        }
    }
}
