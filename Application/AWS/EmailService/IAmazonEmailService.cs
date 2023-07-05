using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.AWS
{
    public interface IAmazonEmailService
    {
        Task<string> SendEmailAsync(List<string> toAddresses,
            List<string> ccAddresses, List<string> bccAddresses,
            string bodyHtml, string bodyText, string subject, string senderAddress);
    }
}
