using System;
using System.IO.Pipelines;
using System.Threading.Tasks;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace IdentityManager.Services
{
    public class MailJetEmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        public MailJetEmailSender(IConfiguration configuration)
        {
            _configuration = configuration;

       }
    
        //https://dev.mailjet.com/email/guides/send-api-V3/#send-api-v3-to-v3.1
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var apiKey = _configuration["MailJet:ApiKey"];
            var secretKey = _configuration["MailJet:SecretKey"];
            var client = new MailjetClient(apiKey, secretKey);
            var request = new MailjetRequest
                {
                    Resource = Send.Resource,
                }
                .Property(Send.FromEmail, "mikesoftware@protonmail.com")
                .Property(Send.FromName, "Mike software")
                .Property(Send.Subject, subject)
                .Property(Send.TextPart, "Dear passenger, welcome to Mailjet! May the delivery force be with you!")
                .Property(Send.HtmlPart, htmlMessage)
                .Property(Send.Recipients, new JArray {
                    new JObject {
                        {"Email", email}
                    }
                });
           // MailjetResponse response = await client.PostAsync(request);
            await client.PostAsync(request); // We don't need the response
            //if (response.IsSuccessStatusCode)
            //{
            //    Console.WriteLine(string.Format("Total: {0}, Count: {1}\n", response.GetTotal(), response.GetCount()));
            //    Console.WriteLine(response.GetData());
            //}
            //else
            //{
            //    Console.WriteLine(string.Format("StatusCode: {0}\n", response.StatusCode));
            //    Console.WriteLine(string.Format("ErrorInfo: {0}\n", response.GetErrorInfo()));
            //    Console.WriteLine(string.Format("ErrorInfo: {0}\n", response.GetErrorInfo()));
            //}
        }
    }
}
