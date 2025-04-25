using Google.Apis.Auth;
using System.Threading.Tasks;

namespace ChopperStoreAngularTest.Services
{
    public interface IGoogleAuthService
    {
        Task<GoogleJsonWebSignature.Payload> VerifyGoogleTokenAsync(string token);
    }

    public class GoogleAuthService : IGoogleAuthService
    {
        public async Task<GoogleJsonWebSignature.Payload> VerifyGoogleTokenAsync(string token)
        {
            try
            {
                // Verifica el token con Google
                var validPayload = await GoogleJsonWebSignature.ValidateAsync(token);
                return validPayload;
            }
            catch (InvalidJwtException)
            {
                return null; // Si el token no es válido
            }
        }
    }
}