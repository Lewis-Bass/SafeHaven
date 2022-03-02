using CommonLibraries.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebDataLayer.Models;

namespace WebLibraries.License
{
    public class LicenseValidation
    {
        ApplicationUser _user;
        public LicenseValidation(ApplicationUser user)
        {
            _user = user;
        }

        public UserLicense GetLicense()
        {
            UserLicense userLicense = null;
            using (var dbms = new WebDataLayer.DBMS.WebdatalayerContext())
            {
                // get license associated with the user
                var license = dbms.License.FirstOrDefault(r => r.Email == _user.Email);
                if (license != null)
                {
                    userLicense = new UserLicense
                    {
                        LicenseId = license.LicenseId,
                        Email = license.Email,
                        ExpirationDate = license.ExpirationDate.GetValueOrDefault(DateTime.MaxValue),
                        LicenseName = license.Name,
                        PhoneNumber = license.PhoneNumber
                    };
                    List<UserDevice> userDevices = new List<UserDevice>();
                    foreach (var device in dbms.Device.Include(r => r.DeviceArk).Where(r => r.LicenseId == license.LicenseId))
                    {
                        var userDevice = new UserDevice
                        {
                            DeviceId = device.DeviceId,
                            DeviceName = device.Name,
                            DisabledDate = device.DisabledDate.GetValueOrDefault(DateTime.MaxValue)
                        };
                        userDevice.Arks = device.DeviceArk
                            .Select(r => new UserArk
                            {
                                ArkId = r.ArkId,
                                ArkName = r.ArkName,
                                DisabledDate = r.DisabledDate.GetValueOrDefault(DateTime.MaxValue)
                            })
                            .ToArray();
                        userDevices.Add(userDevice);
                    }
                    userLicense.Devices = userDevices.ToArray();
                }
            }
            return userLicense;
        }

        public DateTime IsLicenseExpired(string deviceId, string arkId)
        {
            using (var dbms = new WebDataLayer.DBMS.WebdatalayerContext())
            {
                // get license associated with the user
                var license = dbms.License.Include(r => r.Device).FirstOrDefault(r => r.Email == _user.Email);
                if (license != null)
                {
                    // validate that the user owns the device 
                    var device = license.Device.FirstOrDefault(r => r.DeviceId == deviceId);
                    if (device != null)
                    {
                        // validate that the user owns the ark
                        if (device.IsDeviceExpired)
                        {
                            return device.DisabledDate.GetValueOrDefault(DateTime.MinValue);
                        }

                        var ark = dbms.DeviceArk.FirstOrDefault(r => r.DeviceId == deviceId && r.ArkId == arkId);
                        if (ark != null)
                        {
                            if (ark.IsDisabled)
                            {
                                return ark.DisabledDate.GetValueOrDefault(DateTime.MinValue);
                            }
                            // return the new expiration date
                            return DateTime.Now.AddDays(30);
                        }
                    }
                }
            }
            // license is considered invalid
            return DateTime.MinValue;
        }
    }
}
