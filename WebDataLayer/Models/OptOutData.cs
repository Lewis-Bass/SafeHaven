using System;
using System.Collections.Generic;
using System.Text;

namespace WebDataLayer.Models
{
    public class OptOutData
    {
        public const string BIOS = "BIOS";
        public const string CPU = "CPU";
        public const string DRIVE = "DRIVE";
        public const string MB = "MB";
        public const string NET = "NET";
        public const string OS = "OS";

        public bool IsBIOSTracked { get; set; }
        public bool IsCPUTracked { get; set; }
        public bool IsDRIVETracked { get; set; }
        public bool IsMBTracked { get; set; }
        public bool IsNETTracked { get; set; }
        public bool IsOSTracked { get; set; }

    }
}
