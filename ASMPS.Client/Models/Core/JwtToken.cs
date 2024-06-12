namespace ASMPS.Client.Models.Core;

public class JwtToken
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}