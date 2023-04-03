using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.Application.Common.Interfaces
{
    public interface IEmailPreparationService
    {
        public string GetConfirmationEmailBody(string confirmationLink);
        public string GetResetPasswordEmailBody(string resetLink);
    }
}
