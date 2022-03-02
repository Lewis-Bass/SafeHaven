using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebDataLayer.Models;

namespace WebLibraries.OptIn
{
    public class OptInProcessing
    {

        #region Opt In - Out Processing

        /// <summary>
        /// Setup the opt in structure
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public OptOutData LoadData(string userID)
        {
            var retval = new OptOutData();
            using (var dbms = new WebDataLayer.DBMS.WebdatalayerContext())
            {
                var license = dbms.License.FirstOrDefault(r => r.Email == userID);
                if (license != null)
                {
                    var values = dbms.OptIn.Where(r => r.LicenseId == license.LicenseId).ToArray();
                    retval.IsBIOSTracked = GetOptOutValue(values, OptOutData.BIOS);
                    retval.IsCPUTracked = GetOptOutValue(values, OptOutData.CPU);
                    retval.IsDRIVETracked = GetOptOutValue(values, OptOutData.DRIVE);
                    retval.IsMBTracked = GetOptOutValue(values, OptOutData.MB);
                    retval.IsNETTracked = GetOptOutValue(values, OptOutData.NET);
                    retval.IsOSTracked = GetOptOutValue(values, OptOutData.OS);
                }
            }
            return retval;
        }

        /// <summary>
        /// Save the Opt In data changes
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userID"></param>
        public void SaveData(OptOutData model, string userID)
        {
            using (var dbms = new WebDataLayer.DBMS.WebdatalayerContext())
            {
                var license = dbms.License.FirstOrDefault(r => r.Email == userID);
                if (license != null)
                {
                    foreach (var value in dbms.OptIn.Where(r => r.LicenseId == license.LicenseId))
                    {
                        value.IsCollected = UpdateNewValue(model, value.OptionName);
                        if (!value.IsCollected.GetValueOrDefault(true))
                        {
                            // delete the data
                            DeleteData(value, license.LicenseId);
                        }

                    }
                    dbms.SaveChanges();
                }

            }
        }

        private bool GetOptOutValue(WebDataLayer.DBMS.OptIn[] values, string optionName)
        {
            var rec = values.FirstOrDefault(r => r.OptionName == optionName);
            return rec != null ? rec.IsCollected.GetValueOrDefault(true) : true;
        }

        private bool UpdateNewValue(OptOutData model, string OptionName)
        {
            switch (OptionName)
            {
                case OptOutData.BIOS:
                    return model.IsBIOSTracked;
                case OptOutData.CPU:
                    return model.IsCPUTracked;
                case OptOutData.DRIVE:
                    return model.IsDRIVETracked;
                case OptOutData.MB:
                    return model.IsMBTracked;
                case OptOutData.NET:
                    return model.IsNETTracked;
                case OptOutData.OS:
                    return model.IsOSTracked;
                default:
                    return true;
            }
        }

        private void DeleteData(WebDataLayer.DBMS.OptIn value, string licenseID)
        {
            using (var dbms = new WebDataLayer.DBMS.WebdatalayerContext())
            {
                foreach (var device in dbms.Device.Include(r => r.DeviceInformation).Where(r => r.LicenseId == licenseID)) {
                    dbms.DeviceInformation.RemoveRange(device.DeviceInformation.Where(r=>r.InformationType == value.OptionName));
                }
                dbms.SaveChanges();
            }
        }

        #endregion

        #region View Data Collected

        public InfoDetails ViewData(string userID)
        {
            var retval = new InfoDetails();
            retval.Devices = new List<InfoDevice>();
            using (var dbms = new WebDataLayer.DBMS.WebdatalayerContext())
            {
                var license = dbms.License.FirstOrDefault(r => r.Email == userID);
                if (license != null)
                {
                    foreach (var device in dbms.Device.Include(r => r.DeviceInformation).Where(r => r.LicenseId == license.LicenseId).OrderBy(r => r.Name))
                    {
                        var infoDevice = new InfoDevice
                        {
                            DeviceID = device.DeviceId,
                            DeviceName = device.Name
                        };
                        infoDevice.Types = new List<InfoTypes>();
                        foreach (var type in device.DeviceInformation.Select(r => r.InformationType).Distinct().OrderBy(r => r))
                        {
                            var infoType = new InfoTypes
                            {
                                InformationType = type,
                                Details = device.DeviceInformation
                                    .Where(r => r.InformationType == type)
                                    .OrderBy(r => r.InformationCategory)
                                    .Select(r => new InfoDetail
                                    {
                                        InformationCategory = r.InformationCategory,
                                        InformationName = r.InformationName,
                                        InformationValue = r.InformationValue
                                    }).ToList()
                            };
                            infoDevice.Types.Add(infoType);
                        }
                        retval.Devices.Add(infoDevice);
                    }
                }
            }
            return retval;
        }

        #endregion
    }
}
