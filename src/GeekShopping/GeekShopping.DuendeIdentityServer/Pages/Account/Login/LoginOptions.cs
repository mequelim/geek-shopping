namespace GeekShopping.DuendeIdentityServer.Pages.Account.Login
{
    public static class LoginOptions
    {
        public const bool AllowLocalLogin = true;
        public const bool AllowRememberLogin = true;
        public static readonly TimeSpan RememberMeLoginDuration = TimeSpan.FromDays(30);
        public const string InvalidCredentialsErrorMessage = "Invalid username or password";
    }
}