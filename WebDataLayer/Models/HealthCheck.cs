using System;
using System.Collections.Generic;
using System.Text;

namespace WebDataLayer.Models
{
    /// <summary>
    /// Data Class used to display the health of their system
    /// </summary>
    public class HealthCheck
    {
        public List<MachineHealth> Machines { get; set; } = new List<MachineHealth>();
    }

    /// <summary>
    /// Contains a list of all the users machines
    /// </summary>
    public class MachineHealth { 
        public string MachineName { get; set; }
        public List<SpaceHealth> AvailableDiskSpace { get; set; }
        public List<VaultHealth> Vaults { get; set; }
    }

    /// <summary>
    /// Vaults that are installed on a specific machine
    /// </summary>
    public class VaultHealth
    {
        public string VaultName { get; set; }
        //public string DiskLocation { get; set; }
        //public List<SpaceHealth> VaultSize { get; set; }
    }

    public class SpaceHealth
    {
        public string Drive { get; set; }
        public DateTime ValueDate { get; set; }
        public string ValueStr { get; set; }

        public long Value
        {
            get
            {
                if (long.TryParse(ValueStr, out long value))
                {
                    return value;
                }
                return 0;
            }
        }
        public decimal PctOfMax { get; set; }
    }
}
