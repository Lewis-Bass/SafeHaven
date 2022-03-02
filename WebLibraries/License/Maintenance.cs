using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebLibraries.License
{
    public class Maintenance
    {

        #region device based

        /// <summary>
        /// Disable the device and all of the arks associated with it
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public bool DisableDeviceID(string deviceId)
        {
            using (var db = new WebDataLayer.DBMS.WebdatalayerContext())
            {
                var device = db.Device.FirstOrDefault(r => r.DeviceId == deviceId);
                device.DisabledDate = DateTime.Now;
                foreach (var ark in db.DeviceArk.Where(r => r.DeviceId == deviceId))
                {
                    ark.DisabledDate = DateTime.Now;
                }
                db.SaveChanges();
            }
            return true;
        }

        /// <summary>
        /// Enable the device by removing the disabled date
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public bool ActivateDeviceID(string deviceId)
        {
            using (var db = new WebDataLayer.DBMS.WebdatalayerContext())
            {
                var device = db.Device.FirstOrDefault(r => r.DeviceId == deviceId);
                device.DisabledDate = null;
                foreach (var ark in db.DeviceArk.Where(r => r.DeviceId == deviceId))
                {
                    ark.DisabledDate = null;
                }
                db.SaveChanges();
            }
            return true;
        }

        #endregion

        #region ark based

        /// <summary>
        /// Disable the device and all of the arks associated with it
        /// </summary>
        /// <param name="arkId"></param>
        /// <returns></returns>
        public bool DisableArkID(string arkId)
        {
            using (var db = new WebDataLayer.DBMS.WebdatalayerContext())
            {
                var ark = db.DeviceArk.FirstOrDefault(r => r.ArkId == arkId);
                ark.DisabledDate = DateTime.Now;
                db.SaveChanges();
            }
            return true;
        }

        /// <summary>
        /// Enable the device by removing the disabled date
        /// </summary>
        /// <param name="arkId"></param>
        /// <returns></returns>
        public bool ActivateArkID(string arkId)
        {
            using (var db = new WebDataLayer.DBMS.WebdatalayerContext())
            {
                var ark = db.DeviceArk.FirstOrDefault(r => r.ArkId == arkId);
                ark.DisabledDate = null;
                var device = db.Device.FirstOrDefault(r => r.DeviceId == ark.DeviceId);
                if (device.IsDeviceExpired)
                {
                    device.DisabledDate = null;
                }
                db.SaveChanges();
            }
            return true;
        }

        #endregion
    }
}
