using CommonLibraries.Models;
using CommonLibraries.Encrypt_Decrypt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using WebDataLayer.Models;

namespace WebLibraries.License
{
    public class LicenseHelper
    {
        #region public 

        public string CreateLicenseFile(string licenseID, DateTime expirationDate)
        {
            LicenseFile lf = new LicenseFile()
            {
                License = licenseID,
                ExpirationDate = expirationDate
            };

            XmlSerializer ser = new XmlSerializer(lf.GetType());
            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb))
            {
                ser.Serialize(writer, lf);
            }
            return sb.ToString();
        }

        public DeviceLicense CreateDeviceID(DeviceInfo[] deviceInfo)
        {
            DeviceLicense deviceLicense = new DeviceLicense();
            //var sb = new StringBuilder();
            deviceLicense.Part1 = GetStatValue(deviceInfo, 
                new RequestStatistic("BIOS", "SerialNumber", string.Empty), 
                new RequestStatistic("BIOS", "Caption", string.Empty), 
                new RequestStatistic("BIOS", string.Empty, string.Empty));
            deviceLicense.Part2 = GetStatValue(deviceInfo, 
                new RequestStatistic("NET", "SettingID", string.Empty),
                new RequestStatistic("NET", "MACAddress", string.Empty),
                new RequestStatistic("NET", string.Empty, string.Empty));
            deviceLicense.Part3 = GetStatValue(deviceInfo,
                new RequestStatistic("CPU", "SerialNumber", string.Empty),
                new RequestStatistic("CPU", "ProcessorId", string.Empty),
                new RequestStatistic("CPU", string.Empty, string.Empty));
            deviceLicense.Part4 = GetStatValue(deviceInfo,
                new RequestStatistic("OS", "SerialNumber", string.Empty),
                new RequestStatistic("OS", "InstallDate", string.Empty),
                new RequestStatistic("OS", string.Empty, string.Empty));
            deviceLicense.DeviceID = EncrypDecrypt.EncryptString($"{deviceLicense.Part1}|{deviceLicense.Part2}|{deviceLicense.Part3}|{deviceLicense.Part4}");
            return deviceLicense;
        }

        public string CreateLicense(string login, string password)
        {
            // TODO: Create a real license
            string retval = $"{login}|{password}";
            retval = EncrypDecrypt.EncryptString(retval);
            return retval;
        }

        public bool VerifyLicense()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region functions

        public string GetStatValue(DeviceInfo[] deviceInfo, RequestStatistic requestProp, RequestStatistic backupProp, RequestStatistic anyProp)
        {
            var value = GetValue(deviceInfo, requestProp);

            if (value == null)
            {
                value = GetValue(deviceInfo, backupProp);
            }
            if (value == null)
            {
                value = GetValue(deviceInfo, anyProp);
            }
            if (value == null)
            {
                return Guid.NewGuid().ToString();
            }
            return value.StatisticValue;
        }

        private DeviceInfo GetValue(DeviceInfo[] deviceInfo, RequestStatistic request)
        {
            return deviceInfo.FirstOrDefault(r => (r.StatisticCategory == request.Category)
                && (r.StatisticType == request.Type || string.IsNullOrWhiteSpace(request.Type))
                && (r.StatisticName == request.Name || string.IsNullOrWhiteSpace(request.Name))
                && (!string.IsNullOrWhiteSpace(r.StatisticValue)));
        }

        #endregion

    }
}
