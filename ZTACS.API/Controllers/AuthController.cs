using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ZTACS.Shared.Models;
[AllowAnonymus]
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public AuthController(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    [HttpPost("get-token")]
    public async Task<IActionResult> GetToken([FromBody] KeycloakTokenRequest request)
    {
        var tokenEndpoint = $"{_config["Auth0:Domain"]}/protocol/openid-connect/token";

        var parameters = new Dictionary<string, string>
        {
            { "grant_type", "password" },
            { "client_id", _config["Auth0:ClientId"] },
            { "client_secret", _config["Auth0:ClientSecret"] },
            { "username", request.Username },
            { "password", request.Password }
        };

        var content = new FormUrlEncodedContent(parameters);

        var response = await _httpClient.PostAsync(tokenEndpoint, content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return BadRequest(error);
        }

        var responseString = await response.Content.ReadAsStringAsync();
        return Ok(JsonDocument.Parse(responseString));
    }
}