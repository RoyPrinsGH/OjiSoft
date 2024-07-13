namespace OjiSoftPortal
{
    public static class ConfigKeys
    {
        public const string PowerUserName = "PowerUser:Name";
        public const string PowerUserPassword = "PowerUser:Password";

        public const string RateLimit = "RateLimit";
        public const string AnonymousResourceModificationRateLimiterName = "AnonymousResourceModification";
        public const string AuthenticatedResourceModificationRateLimiterName = "AuthenticatedResourceModification";
        public const string GeneralRateLimiterName = "General";
        public const string AuthenticatedRateLimiterName = "Authenticated";

        public const string PreRegisteredApplications = "PreRegisteredApplications";
    }
}
