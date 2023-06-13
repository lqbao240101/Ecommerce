using Ecommerce.Services.Core.IService;
using Microsoft.AspNetCore.Identity;
using Twilio;
using Twilio.Rest.Verify.V2.Service;

namespace Ecommerce.Services.Core.Catalog
{
    public class SMSService : ISMSService
    {
        public SMSService() { }
        public async Task<string> VerifyPhone(string phoneNumber)
        {
            var accountSid = "";
            var authToken = "";

            TwilioClient.Init(accountSid, authToken);

            try
            {
                var verification = await VerificationResource.CreateAsync(
                to: phoneNumber,
                channel: "sms",
                    pathServiceSid: ""
                );

                return verification.Status;
            }
            catch (Exception ex)
            {
                return $"There was an error sending the verification code, please check the phone number is correct and try again. : {ex.Message}";
            }
        }

        public async Task<IdentityResult> ConfirmPhone(string VerificationCode, string phoneNumber)
        {
            try
            {
                var verification = await VerificationCheckResource.CreateAsync(
                    to: phoneNumber,
                    code: VerificationCode,
                    pathServiceSid: ""
                );
                if (verification.Status.Equals("approved"))
                    return IdentityResult.Success;
                else
                    return IdentityResult.Failed(new IdentityError { Description = $"There was an error confirming the verification code: {verification.Status}" });
            }
            catch (Exception)
            {
                return IdentityResult.Failed(new IdentityError { Description = $"There was an error confirming the code, please check the verification code is correct and try again" });
            }
        }
    }
}