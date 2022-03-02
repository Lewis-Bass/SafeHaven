using CommonLibraries.Encrypt_Decrypt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace WebSiteCustomerTestHarness.RestHelpers
{
    public class SendToServer
    {
        public static Object Send(Object data, Uri destination)
        {
            string dataString = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            dataString = EncrypDecrypt.EncryptString(dataString);
            dataString = System.Net.WebUtility.HtmlEncode(dataString);

            //using (var httpClient = new System.Net.Http.HttpClient())
            //{
            //    httpClient.DefaultRequestHeaders.Add("encryptedJson", dataString);                

            //    var response = httpClient.GetStringAsync(destination).Result;

            //    response = System.Net.WebUtility.HtmlDecode(response);
            //    response = EncrypDecrypt.DecryptString(response);
            //    Console.Out.WriteLine(response);
            //    Object retval = Newtonsoft.Json.JsonConvert.DeserializeObject(response);

            //    return retval;
            //}

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(destination);
            httpWebRequest.ContentType = "application/json";
            //httpWebRequest.ContentType = "text/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {              
                streamWriter.Write(dataString);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                return result;
            }

        }

    }
}
