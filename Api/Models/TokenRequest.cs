using Microsoft.Build.Framework;

namespace Api.Models;

public class TokenRequest
{
    [Required] public required string grant_type { get; set; }
    [Required] public required string username { get; set; }
    [Required] public required string password { get; set; }
    public string? refresh_token { get; set; }
    public string? scope { get; set; }

    public string? client_id { get; set; }
    public string? client_secret { get; set; }
}