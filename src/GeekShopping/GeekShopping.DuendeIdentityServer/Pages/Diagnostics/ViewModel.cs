using Microsoft.AspNetCore.Authentication;
using System.Buffers.Text;
using System.Text.Json;
using System.Text;

namespace GeekShopping.DuendeIdentityServer.Pages.Diagnostics
{
    public class ViewModel
    {
        public ViewModel(AuthenticateResult result)
        {
            AuthenticateResult = result;

            if(result?.Properties?.Items.TryGetValue("client_list", out string? encoded) is true)
            {
                if(encoded is not null)
                {
                    byte[] bytes = Base64Url.DecodeFromChars(encoded);
                    string value = Encoding.UTF8.GetString(bytes);

                    Clients = JsonSerializer.Deserialize<string[]>(value) ?? Enumerable.Empty<string>();

                    return;
                }
            }

            Clients = [];
        }

        public AuthenticateResult AuthenticateResult { get; }
        public IEnumerable<string> Clients { get; }
    }
}