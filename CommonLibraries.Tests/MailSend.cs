using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommonLibraries.MailSend;
using System;

namespace CommonLibraries.Tests
{
    [TestClass]
    public class MailSend
    {
        [TestMethod]
        public void SendOne()
        {
            try
            {
                var send = new Send()
                {
                    Body = "TEST EMAIL",
                    Subject = "TEST EMAIL"
                };
                send.SendEmail(new string[] { CommonLibraries.Properties.Resources.EmailFromAddress });
                Assert.IsTrue(true); // if i got here the test is a success            
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void SendMultiple()
        {
            try
            {
                var send = new Send()
                {
                    Body = "TEST EMAIL",
                    Subject = "TEST EMAIL"
                };
                string[] addresses = new string[]
                {
                    CommonLibraries.Properties.Resources.EmailFromAddress,
                    CommonLibraries.Properties.Resources.EmailFromAddress,
                    CommonLibraries.Properties.Resources.EmailFromAddress
                };
                send.SendEmail(new string[] { CommonLibraries.Properties.Resources.EmailFromAddress });
                Assert.IsTrue(true); // if i got here the test is a success            
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

    }
}
