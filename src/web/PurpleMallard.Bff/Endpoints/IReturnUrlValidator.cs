using System;

namespace PurpleMallard.Bff.Endpoints;

public interface IReturnUrlValidator
{
    public bool IsValidAsync(string returnUrl);
}
