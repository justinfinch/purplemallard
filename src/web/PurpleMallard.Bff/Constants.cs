using System;

namespace PurpleMallard.Bff;

public static class Constants
{
    public static class BffEndpoints
    {
        public const string Login = "/login";
        public const string User = "/user";
        public const string Logout = "/logout";
    }

    public static class RequestParameters
    {
        /// <summary>
        /// Used to prevent cookie sliding on user endpoint
        /// </summary>
        public const string SlideCookie = "slide";

        /// <summary>
        /// Used to pass a return URL to login/logout
        /// </summary>
        public const string ReturnUrl = "returnUrl";

        /// <summary>
        /// Used to pass a prompt value to login
        /// </summary>
        public const string Prompt = "prompt";
    }

    /// <summary>
    /// Custom claim types
    /// </summary>
    public static class ClaimTypes
    {
        /// <summary>
        /// Claim type for logout URL including session id
        /// </summary>
        public const string LogoutUrl = "bff:logout_url";

        /// <summary>
        /// Claim type for session expiration in seconds
        /// </summary>
        public const string SessionExpiresIn = "bff:session_expires_in";

        /// <summary>
        /// Claim type for authorize response session state value
        /// </summary>
        public const string SessionState = "bff:session_state";
    }
}
