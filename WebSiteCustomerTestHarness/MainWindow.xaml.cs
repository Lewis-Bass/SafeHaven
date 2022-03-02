using CommonLibraries.Microsoft;
using CommonLibraries.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WebDataLayer.DBMS;
using WebSiteCustomerTestHarness.RestHelpers;

namespace WebSiteCustomerTestHarness
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            WebSiteURL.Text = CommonLibraries.Properties.Resources.RestServer;
        }

        #region add ark or customer

        private void AddArk_Click(object sender, RoutedEventArgs e)
        {
            //validate the information
            if (string.IsNullOrEmpty(WebSiteURL.Text)
                || string.IsNullOrWhiteSpace(UserEmail.Text)
                || string.IsNullOrWhiteSpace(DeviceName.Text)
                || string.IsNullOrWhiteSpace(Password1.Text)
                || string.IsNullOrWhiteSpace(Password2.Text)
                )
            {
                MessageBox.Show("BAD INPUT");
                return;
            }
            if (Password1.Text.Trim() != Password2.Text.Trim())
            {
                MessageBox.Show("PASSWORDS DO NOT MATCH");
                return;
            }

            // get machine information
            SystemStatisticsMonitor device = new SystemStatisticsMonitor();
            var deviceInformation = device.WindowsStatistics;
            //foreach (var rec in deviceInformation.OrderBy(r => r.StatisticCategory).ThenBy(r => r.StatisticType).ThenBy(r => r.StatisticName))
            //{
            //    System.Diagnostics.Debug.WriteLine($"{rec.StatisticType}  |  {rec.StatisticCategory}  |  {rec.StatisticName}  |  {rec.StatisticValue}");
            //}
            // do we want to guarantee that this is a new install?
            if (ForceDifferent.IsChecked.Value)
            {
                foreach (var di in deviceInformation)
                {
                    // for testing the value needs to be changed - this forces it to be different
                    di.StatisticValue += DateTime.Now.Millisecond.ToString();
                }
            }
            // send the license to the web server to process            
            var uri = new Uri(WebSiteURL.Text + "LicenseRegistration/NewRegistration");
            var model = new RequestLicense
            {
                DeviceInformation = deviceInformation,
                Email = UserEmail.Text,
                DeviceName = DeviceName.Text,
                PhoneNumber = UserPhone.Text,
                Password1 = Password1.Text,
                Password2 = Password2.Text,
                ArkInformation = new ArkInfo
                {
                    ArkName = ArkName.Text
                }
            };
            var responseStr = SendToServer.Send(model, uri);
            var dataString = System.Net.WebUtility.HtmlDecode(responseStr.ToString());
            dataString = CommonLibraries.Encrypt_Decrypt.EncrypDecrypt.DecryptString(dataString);
            var response = Newtonsoft.Json.JsonConvert.DeserializeObject<GenericRestResponse>(dataString.ToString());
            if (response.ErrorList != null && response.ErrorList.Any())
            {
                StringBuilder sb = new StringBuilder();
                foreach (var str in response.ErrorList)
                {
                    sb.Append(str);
                }
                MessageBox.Show(sb.ToString());
            }
            else
            {
                MessageBox.Show(response.License);
            }
        }

        #endregion

        #region check license
        private void Check_Click(object sender, RoutedEventArgs e)
        {
            //validate the information
            if (string.IsNullOrEmpty(WebSiteURL.Text)
                || string.IsNullOrWhiteSpace(UserEmail.Text)
                || string.IsNullOrWhiteSpace(Password1.Text)
                || string.IsNullOrWhiteSpace(CheckDeviceID.Text)
                || string.IsNullOrWhiteSpace(CheckArkID.Text)
                )
            {
                MessageBox.Show("BAD INPUT");
                return;
            }

            // send the license to the web server to process            
            var uri = new Uri(WebSiteURL.Text + "LicenseCheck/Check");
            var model = new LicenseCheck
            {
                ArkId = CheckArkID.Text,
                DeviceId = CheckDeviceID.Text,
                UserInfo = new Login(UserEmail.Text, Password1.Text)
            };
            var response = SendToServer.Send(model, uri);
            CheckResults.Text = "Ark Expires on : " + response.ToString();

        }

        #endregion

        #region load up the database

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            // does the file exist?
            if (!File.Exists(LoadFileName.Text))
            {
                MessageBox.Show("ERROR FILE NAME DOES NOT EXIST");
                return;
            }
            int loadQuantity = 0;
            if (!int.TryParse(LoadTotalNumber.Text, out loadQuantity))
            {
                MessageBox.Show("ERROR QUANTITY IS NOT A NUMBER");
                return;
            }
            int loadStats = 0;
            if (!int.TryParse(LoadStatsNumber.Text, out loadStats))
            {
                MessageBox.Show("STATS IS NOT A NUMBER");
                return;
            }

            // read the csv file 
            var names = ReadCSV(LoadFileName.Text);

            for (int i = 1; i <= loadQuantity && i < names.Length; i++)
            {
                var name = names[i];

                // create the license
                // get machine information
                SystemStatisticsMonitor device = new SystemStatisticsMonitor();
                var deviceInformation = device.WindowsStatistics;
                // need to force every runs info to be different so that it does not match the previous device id.
                foreach (var di in deviceInformation)
                {
                    // for testing the value needs to be changed - this forces it to be different
                    di.StatisticValue += i.ToString();
                }

                // send the license to the web server to process
                var uri = new Uri(WebSiteURL.Text + "LicenseRegistration/NewRegistration");
                var model = new RequestLicense
                {
                    DeviceInformation = deviceInformation,
                    Email = (i==1)? "cows@manure.com": $"{name.FirstName}.{name.LastName}@{name.City}.com",
                    DeviceName = $"{name.FirstName}-{i}",
                    PhoneNumber = i.ToString(),
                    Password1 = "abcd1234",
                    Password2 = "abcd1234",
                    ArkInformation = new ArkInfo
                    {
                        ArkName = $"{name.LastName}-{i}"
                    }
                };
                var responseStr = SendToServer.Send(model, uri);
                var dataString = System.Net.WebUtility.HtmlDecode(responseStr.ToString());
                dataString = CommonLibraries.Encrypt_Decrypt.EncrypDecrypt.DecryptString(dataString);
                var response = Newtonsoft.Json.JsonConvert.DeserializeObject<GenericRestResponse>(dataString.ToString());

                // add additional stats
                using (var dbms = new WebDataLayer.DBMS.WebdatalayerContext())
                {
                    var deviceID = dbms.Device.First(r => r.LicenseId == response.License).DeviceId;
                    var diskStats = dbms.DeviceInformation.First(r => r.InformationType == "Drive" && r.InformationName == "Available" && r.InformationCategory == "C:\\" && r.DeviceId == deviceID);
                    List<DeviceInformation> di = new List<DeviceInformation>();
                    for (int s = 1; s <= loadStats; s++)
                    {
                        long diskSpace = 0;
                        if (!long.TryParse(diskStats.InformationValue, out diskSpace))
                        {
                            diskSpace = 100000000000;
                        }
                        diskSpace = diskSpace - (100000000000 * s);
                        var newStats = new DeviceInformation
                        {
                            DeviceInformationId = Guid.NewGuid(),
                            DeviceId = deviceID,
                            InformationCategory = diskStats.InformationCategory,
                            InformationName = diskStats.InformationName,
                            InformationType = diskStats.InformationType,
                            InformationValue = diskSpace.ToString(),
                            ValidDate = DateTime.Now.AddDays(s)
                        };
                        di.Add(newStats);
                        //dbms.DeviceInformation.Add(newStats);
                        //dbms.SaveChanges();
                    }
                    dbms.DeviceInformation.AddRange(di.ToArray());
                    dbms.SaveChanges();
                }

                if ((i % 10) == 0)
                {
                    System.Diagnostics.Debug.WriteLine($"Added license for {model.Email}  up to {i}");
                }
            }
        }

        private NameSource[] ReadCSV(string fileName)
        {
            // read csv file
            int cnt = 0;
            List<NameSource> values = new List<NameSource>();
            foreach (var fileLine in File.ReadAllLines(fileName))
            {
                //Console.WriteLine(fileLine);
                values.Add(new NameSource(fileLine));
                cnt++;
                if ((cnt % 100) == 0)
                {
                    Console.WriteLine($"Loaded: {cnt}");
                }
            }
            return values.ToArray();
        }

        #endregion
    }
}
