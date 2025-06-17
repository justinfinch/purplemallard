using System;

namespace PurpleMallard.Bff.AccessTokenManagement;

/// <summary>
/// Represents an error that occurred during the retrieval of an access token.
/// </summary>
public record AccessTokenRetrievalError : AccessTokenResult
{
    public required string Error { get; init; }

    public string? ErrorDescription { get; init; }
}
