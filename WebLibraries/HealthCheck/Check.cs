using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebDataLayer.Models;

namespace WebLibraries.HealthCheck
{
    public class Check
    {
        public WebDataLayer.Models.HealthCheck GetHealthForUser(string email)
        {
            var retval = new WebDataLayer.Models.HealthCheck();

            using (var dbms = new WebDataLayer.DBMS.WebdatalayerContext())
            {
                foreach (var license in dbms.License.Include(r => r.Device).Where(r => r.Email == email))
                {
                    foreach (var dev in license.Device)
                    {
                        var mHealth = new WebDataLayer.Models.MachineHealth();
                        mHealth.Vaults = new List<VaultHealth>();
                        mHealth.MachineName = dev.Name;
                        mHealth.Vaults = dbms.DeviceArk
                            .Where(r => r.DeviceId == dev.DeviceId)
                            .Select(r => new VaultHealth { VaultName = r.ArkName })
                            .ToList();
                        mHealth.AvailableDiskSpace = dbms.DeviceInformation
                            .Where(r => r.DeviceId == dev.DeviceId && r.InformationType == "Drive" && r.InformationName == "Available" && r.ValidDate > DateTime.MinValue)
                            .Take(500)
                            .Select(r => new SpaceHealth
                            {
                                Drive = r.InformationCategory,
                                ValueStr = r.InformationValue,
                                ValueDate = r.ValidDate.Value
                            })
                            .ToList();
                        // calculate the prorated value...
                        foreach (var drive in mHealth.AvailableDiskSpace.Select(r => r.Drive).Distinct())
                        {
                            long maxSpace = mHealth.AvailableDiskSpace.Where(r => r.Drive == drive).Max(r => r.Value);
                            foreach (var space in mHealth.AvailableDiskSpace.Where(r => r.Drive == drive))
                            {
                                space.PctOfMax = (decimal)(space.Value * 100 / maxSpace);
                            }
                        }
                        retval.Machines.Add(mHealth);
                    }
                }
            }
            return retval;
        }
    }
}
