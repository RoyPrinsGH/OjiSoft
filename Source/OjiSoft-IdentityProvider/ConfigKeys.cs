namespace OjiSoft.IdentityProvider
{
    public static class ConfigKeys
    {
        public const string SystemAccountUserName = "SystemAccount:UserName";
        public const string SystemAccountPassword = "SystemAccount:Password";

        public const string RateLimit = "RateLimit";
        public const string AnonymousResourceModificationRateLimiterName = "AnonymousResourceModification";
        public const string AuthenticatedResourceModificationRateLimiterName = "AuthenticatedResourceModification";
        public const string GeneralRateLimiterName = "General";
        public const string AuthenticatedRateLimiterName = "Authenticated";

        public const string PreRegisteredApplications = "PreRegisteredApplications";
    }
}
