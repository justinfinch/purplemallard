using System;
using Duende.AccessTokenManagement.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using PurpleMallard.Bff.Config;

namespace PurpleMallard.Bff.AccessTokenManagement;

/// <summary>
/// Encapsulates contextual data used to retreive an access token.
/// </summary>
public sealed record AccessTokenRetrievalContext
{
    /// <summary>
    /// The HttpContext of the incoming HTTP request that will be forwarded to
    /// the remote API.
    /// </summary>
    public required HttpContext HttpContext { get; init; }

    /// <summary>
    /// Metadata that describes the remote API.
    /// </summary>
    public required BffRemoteApiEndpointMetadata Metadata { get; init; }

    /// <summary>
    /// Additional optional per request parameters for a user access token request.
    /// </summary>
    public required UserTokenRequestParameters UserTokenRequestParameters { get; init; }


    /// <summary>
    /// The locally requested path.
    /// </summary>
    public required string LocalPath { get; init; }

    /// <summary>
    /// The remote address of the API.
    /// </summary>
    public required Uri ApiAddress { get; init; }
}
