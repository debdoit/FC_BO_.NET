using AngularAuthAPI.Models;

namespace AngularAuthAPI.Services
{
    public interface IEmailService
    {
        void SendEmail(EmailDto request);
    }
}
