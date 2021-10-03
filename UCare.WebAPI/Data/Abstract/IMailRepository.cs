using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UCare.WebAPI.Data.Abstract
{
   public interface IMailRepository
    {
        Task SendEmailAsync(string toEmail, string subject, string content);
    }
}
