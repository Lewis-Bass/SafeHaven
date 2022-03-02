using CommonLibraries.MailSend;
using System;
using System.Collections.Generic;
using System.Text;
using WebDataLayer.Models;

namespace WebLibraries.Contact
{
    public class ContactUs
    {
        public string ProcessContact(string accountName, ContactSecureData contact)
        {
            if (string.IsNullOrWhiteSpace(contact.Question))
            {
                return "Please fill out the form properly";
            }
            // send email
            Send send = new Send()
            {
                Subject = "Customer Question",
                Body = contact.Question,
                FromEmailAddress = accountName,
            };          
            send.SendEmail(new string[] { CommonLibraries.Properties.Resources.CustomerSupportEmail });
            return "Your question has been sent. You will receive an email reply";
        }
    }
}

