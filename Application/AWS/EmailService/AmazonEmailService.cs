using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.AWS
{
    public class AmazonEmailService : IAmazonEmailService
    {
        private readonly IAmazonSimpleEmailService _amazonSimpleEmailService;
        private readonly IConfiguration _config;

        public AmazonEmailService(IAmazonSimpleEmailService amazonSimpleEmailService, IConfiguration config)
        {
            _amazonSimpleEmailService = amazonSimpleEmailService;
            _config = config;
        }

        /// <summary>
        ///  Send an email by using Amazon SES.
        /// </summary>
        /// <param name="toAddresses">List of recipients.</param>
        /// <param name="bodyHtml">Body of the email in HTML.</param>
        /// <param name="bodyText">Body of the email in plain text.</param>
        /// <param name="subject">Subject line of the email.</param>
        /// <param name="senderAddress">From address.</param>
        /// <returns>The messageId of the email.</returns>
        public async Task<string> SendEmailAsync(List<string> toAddresses,
            string bodyHtml, string bodyText, string subject, string? senderAddress = null)
        {   
            if (string.IsNullOrEmpty(senderAddress))
            {
                senderAddress = _config.GetSection("AWS")["DefaultSender"];
            }
            var messageId = "";
            try
            {
                var response = await _amazonSimpleEmailService.SendEmailAsync(
                    new SendEmailRequest
                    {
                        Destination = new Destination
                        {
                            BccAddresses = new List<string>(),
                            CcAddresses = new List<string>(),
                            ToAddresses = toAddresses
                        },
                        Message = new Message
                        {
                            Body = new Body
                            {
                                Html = new Content
                                {
                                    Charset = "UTF-8",
                                    Data = bodyHtml
                                },
                                Text = new Content
                                {
                                    Charset = "UTF-8",
                                    Data = bodyText
                                }
                            },
                            Subject = new Content
                            {
                                Charset = "UTF-8",
                                Data = subject
                            }
                        },
                        Source = senderAddress
                    });
                messageId = response.MessageId;
            }
            catch (Exception ex)
            {
                Console.WriteLine("SendEmailAsync failed with exception: " + ex.Message);
                throw ex;
            }

            return messageId;
        }
    }
}
