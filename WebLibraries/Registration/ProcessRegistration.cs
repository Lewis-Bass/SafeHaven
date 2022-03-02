using CommonLibraries.MailSend;
using CommonLibraries.Models;
using CommonLibraries.Encrypt_Decrypt;
using System;
using System.Collections.Generic;
using System.Text;
using CommonLibraries.License;
using System.Linq;
using WebDataLayer.Models;
using WebDataLayer.DBMS;
using Microsoft.AspNetCore.Identity;

using CommonLibraries.Extensions;

namespace WebLibraries.Registration
{
    public class ProcessRegistration
    {
        const string _CustomerRoleName = "Customer";
        private UserManager<ApplicationUser> _userManager;

        public ProcessRegistration(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// create the license
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public GenericRestResponse RegisterDevice(RequestLicense request)
        {
            GenericRestResponse resp = new GenericRestResponse();
            try
            {
                // check the values
                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    resp = new CommonLibraries.Models.GenericRestResponse()
                    {
                        ResponseString = new string[] { "Invalid Email - Please try again" }
                    };
                    return resp;
                }
                if (string.IsNullOrWhiteSpace(request.Password1) || string.IsNullOrWhiteSpace(request.Password2))
                {
                    resp = new CommonLibraries.Models.GenericRestResponse()
                    {
                        ResponseString = new string[] { "Invalid Password - Please try again" }
                    };
                    return resp;
                }

                if (request.Password1 != request.Password2)
                {
                    resp = new CommonLibraries.Models.GenericRestResponse()
                    {
                        ResponseString = new string[] { "Unequal Password - Please try again" }
                    };
                    return resp;
                }

                // create or find the user
                var user = CreateOrUpdateUser(request.Email, request.PhoneNumber, request.Password1, request.Password2);
                if (user == null)
                {
                    // could not add user
                    resp = new CommonLibraries.Models.GenericRestResponse()
                    {
                        ResponseString = new string[] { "Invalid User - Please try again" }
                    };
                    return resp;
                }

                // create license
                License.LicenseHelper licenseHelper = new License.LicenseHelper();
                var licenseRec = CreateOrFindLicense(request, licenseHelper);
                if (licenseRec == null)
                {
                    resp = new CommonLibraries.Models.GenericRestResponse()
                    {
                        ResponseString = new string[] { "License Creation Error - Please try again" }
                    };
                    return resp;
                }

                // device
                var deviceRec = CreateOrFindDevice(request, licenseHelper, licenseRec);

                // ark
                var arkRec = CreateOrFindArk(request.ArkInformation, deviceRec.DeviceId);

                // license file
                string licenseFile = string.Empty;
                licenseFile = licenseHelper.CreateLicenseFile(licenseRec.LicenseId, DateTime.Now.AddDays(30));
                resp.Device = deviceRec.DeviceId;
                resp.License = licenseRec.LicenseId;

                // send email
                Send send = new Send()
                {
                    Subject = "Product Name Software License",
                    Body = "Attached is your software license"
                };
                send.Attachments.Add("License", Encoding.ASCII.GetBytes(licenseFile));
                send.SendEmail(new string[] { request.Email });

                resp.ResponseString = new string[] { "Email Has been Sent " };

            }
            catch (Exception ex)
            {
                resp = new CommonLibraries.Models.GenericRestResponse()
                {
                    ResponseString = new string[] { "Error occured - please try again later " }
                };
            }
            return resp;
        }

        private WebDataLayer.DBMS.DeviceArk CreateOrFindArk(ArkInfo arkInformation, string deviceId)
        {
            WebDataLayer.DBMS.DeviceArk deviceArk = null;

            using (var dbms = new WebDataLayer.DBMS.WebdatalayerContext())
            {
                // does it already exist?
                if (!string.IsNullOrWhiteSpace(arkInformation.ArkID) || !string.IsNullOrWhiteSpace(arkInformation.ArkName))
                {
                    deviceArk = dbms.DeviceArk.FirstOrDefault(r => (r.ArkId == arkInformation.ArkID || r.ArkName == arkInformation.ArkName)
                        && r.DeviceId == deviceId);
                }
                if (deviceArk == null)
                {
                    deviceArk = new WebDataLayer.DBMS.DeviceArk()
                    {
                        ArkId = string.IsNullOrWhiteSpace(arkInformation.ArkID) ? Guid.NewGuid().ToString() : arkInformation.ArkID,
                        DeviceId = deviceId,
                        ArkName = arkInformation.ArkName

                    };
                    dbms.DeviceArk.Add(deviceArk);
                    dbms.SaveChanges();
                }
            }
            return deviceArk;
        }

        private WebDataLayer.DBMS.Device CreateOrFindDevice(RequestLicense request, License.LicenseHelper licenseHelper, WebDataLayer.DBMS.License licenseRec)
        {
            WebDataLayer.DBMS.Device deviceRec = null;
            using (var dbms = new WebDataLayer.DBMS.WebdatalayerContext())
            {
                var deviceLicense = licenseHelper.CreateDeviceID(request.DeviceInformation);
                string deviceID = deviceLicense.DeviceID;
                deviceRec = dbms.Device.FirstOrDefault(r => r.DeviceId == deviceID);
                if (deviceRec == null)
                {

                    deviceRec = new WebDataLayer.DBMS.Device()
                    {
                        DeviceId = deviceID,
                        CreatedDate = DateTime.UtcNow,
                        LicenseId = licenseRec.LicenseId,
                        Part1 = deviceLicense.Part1,
                        Part2 = deviceLicense.Part2,
                        Part3 = deviceLicense.Part3,
                        Part4 = deviceLicense.Part4,
                        Part5 = deviceLicense.Part5,
                        Part6 = deviceLicense.Part6,
                        Name = request.DeviceName
                    };
                    dbms.Device.Add(deviceRec);
                    dbms.SaveChanges();
                }

                foreach (var info in request.DeviceInformation)
                {
                    var infoRec = new WebDataLayer.DBMS.DeviceInformation()
                    {
                        DeviceId = deviceRec.DeviceId,
                        DeviceInformationId = Guid.NewGuid(),
                        InformationName = info.StatisticName,
                        InformationValue = info.StatisticValue,
                        InformationCategory = info.StatisticCategory,
                        InformationType = info.StatisticType,
                        ValidDate = DateTime.UtcNow
                    };
                    dbms.DeviceInformation.Add(infoRec);
                }
                dbms.SaveChanges();
            }
            return deviceRec;
        }

        private WebDataLayer.DBMS.License CreateOrFindLicense(RequestLicense request, License.LicenseHelper licenseHelper)
        {
            WebDataLayer.DBMS.License licenseRec = null;
            // record information in dbms
            using (var dbms = new WebDataLayer.DBMS.WebdatalayerContext())
            {
                // does the license already exist?
                licenseRec = dbms.License.FirstOrDefault(r => r.Email == request.Email);
                if (licenseRec == null)
                {
                    // Add
                    licenseRec = new WebDataLayer.DBMS.License()
                    {
                        Email = request.Email,
                        EmailSent = true,
                        ExpirationDate = DateTime.UtcNow.AddDays(30),
                        LicenseId = licenseHelper.CreateLicense(request.Email, request.Password1),
                        Name = request.UserName,
                        MaxArksSupported = 0,
                        MaxDevicesSupported = 3,
                        MaxGbAllArk = 0,
                        MaxGbFilesSingleArk = 0,
                        MaxGbSingleArk = 0,
                        MaxViolations = 1,
                        PhoneNumber = request.PhoneNumber,
                        Password = EncrypDecrypt.EncryptString(request.Password1)
                    };
                    dbms.License.Add(licenseRec);
                    // create the opt_in records
                    foreach (var opt in WebDataLayer.DBMS.Constants.OptInOptions)
                    {
                        var optIn = new WebDataLayer.DBMS.OptIn
                        {
                            LicenseId = licenseRec.LicenseId,
                            IsCollected = true,
                            OptionName = opt,
                            OptInId = Guid.NewGuid()
                        };
                        dbms.OptIn.Add(optIn);
                    }
                }
                else
                {
                    // Update!
                    licenseRec.PhoneNumber = request.PhoneNumber;
                    licenseRec.Name = request.UserName;
                }
                dbms.SaveChanges();
            }
            return licenseRec;
        }

        private ApplicationUser CreateOrUpdateUser(string email, string phoneNumber, string password1, string password2)
        {
            if (password1 != password2)
            {
                throw new Exception("Passwords do not match");
            }

            var user = _userManager.FindByNameAsync(email).GetAwaiter().GetResult();
            if (user == null)
            {
                user = new ApplicationUser { UserName = email, Email = email, PhoneNumber = phoneNumber };
                var result = _userManager.CreateAsync(user, password1).GetAwaiter().GetResult();
                if (result != IdentityResult.Success)
                {
                    throw new Exception("Failed to add user");
                }
            }

            return user;
        }
    }
}
