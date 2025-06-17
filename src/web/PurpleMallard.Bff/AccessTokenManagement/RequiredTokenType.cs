namespace PurpleMallard.Bff.AccessTokenManagement;

/// <summary>
/// Expresses required token type
/// </summary>
public enum RequiredTokenType
{
    /// <summary>
    /// No token required
    /// </summary>
    None,

    /// <summary>
    /// User token
    /// </summary>
    User,

    /// <summary>
    /// Client token
    /// </summary>
    Client,

    /// <summary>
    /// If logged the User token will be sent. If not logged in, then a client credentials token will be sent. 
    /// </summary>
    UserOrClient,

    /// <summary>
    /// If logged the User token will be sent. If not logged in, then no token will be sent. 
    /// </summary>
    UserOrNone
}
