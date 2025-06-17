using System;
using Duende.AccessTokenManagement.OpenIdConnect;
using PurpleMallard.Bff.AccessTokenManagement;
using PurpleMallard.Bff.Endpoints;

namespace PurpleMallard.Bff.Config;

/// <summary>
/// Endpoint metadata for a remote BFF API endpoint
/// </summary>
public sealed class BffRemoteApiEndpointMetadata : IBffApiMetadata
{
    /// <summary>
    /// Required token type (if any)
    /// </summary>
    public RequiredTokenType? TokenType;

    /// <summary>
    /// Maps to UserAccessTokenParameters and included if set
    /// </summary>
    public UserTokenRequestParameters? UserTokenRequestParameters { get; set; }

    private Type _accessTokenRetriever = typeof(IAccessTokenRetriever);

    /// <summary>
    /// The type used to retrieve access tokens.
    /// </summary>
    public Type AccessTokenRetriever
    {
        get => _accessTokenRetriever;
        set
        {
            if (value.IsAssignableTo(typeof(IAccessTokenRetriever)))
            {
                _accessTokenRetriever = value;
            }
            else
            {
                throw new Exception("Attempt to assign a AccessTokenRetriever type that cannot be assigned to IAccessTokenTokenRetriever");
            }
        }
    }
}
