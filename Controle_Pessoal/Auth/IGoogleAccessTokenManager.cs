using Google.Apis.Auth;

namespace Controle_Pessoal.Auth;

public interface IGoogleAccessTokenManager
{
    Task<GoogleJsonWebSignature.Payload> ParseAndValidateAsync(string jwt);
}

public class GoogleAccessTokenManager : IGoogleAccessTokenManager
{
    public async Task<GoogleJsonWebSignature.Payload> ParseAndValidateAsync(string jwt)
        => await GoogleJsonWebSignature.ValidateAsync(jwt, new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = ["313667901167-d9cq0716r9ioll9uqdmf2qfa8nop0juv.apps.googleusercontent.com"]
        });
}
