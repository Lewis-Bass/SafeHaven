﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace WebDataLayer.DBMS
{
    public partial class OptIn
    {
        public Guid OptInId { get; set; }
        public string OptionName { get; set; }
        public bool? IsCollected { get; set; }
        public string LicenseId { get; set; }

        public virtual License License { get; set; }
    }
}