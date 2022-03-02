using System;
using System.Collections.Generic;
using System.Text;
using CommonLibraries.Extensions;

namespace WebDataLayer.DBMS
{
    public partial class Device
    {
        public bool IsDeviceExpired
        {
            get
            {
                //return (DisabledDate.HasValue && DisabledDate.Value.ToDateTime <= DateTime.Now);
                return (DisabledDate.HasValue && DisabledDate.Value <= DateTime.Now);
            }
        }
    }
}
