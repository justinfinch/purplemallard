using System;
using Duende.AccessTokenManagement;

namespace PurpleMallard.Bff.AccessTokenManagement;

/// <summary>
/// Represents a bearer token result obtained during access token retrieval.
/// </summary>
public sealed record BearerTokenResult : AccessTokenResult
{
    /// <summary>
    /// The access token.
    /// </summary>
    public required AccessToken AccessToken { get; init; }

}
