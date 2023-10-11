using MediDate.Models;
using MediDate.Models.Queries;

namespace MediDate.Services
{
    public interface IEmailService
    {
        void SendMail(Correo request);
    }
}
