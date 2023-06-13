using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Services.Core.IService
{
    public interface ISMSService
    {
        Task<string> VerifyPhone(string phoneNumber);
        Task<IdentityResult> ConfirmPhone(string VerificationCode, string phoneNumber);
    }
}
