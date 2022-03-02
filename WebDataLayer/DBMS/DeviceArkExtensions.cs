using System;
using System.Collections.Generic;
using System.Text;
using CommonLibraries.Extensions;

namespace WebDataLayer.DBMS
{
    public partial class DeviceArk
    {
        public bool IsDisabled
        {
            get
            {
                //return (DisabledDate.HasValue && DisabledDate.Value.ToDateTime() <= DateTime.Now);
                return (DisabledDate != null && DisabledDate <= DateTime.Now);
            }
        }
    }
}
