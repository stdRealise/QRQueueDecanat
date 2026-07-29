using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRQueueDecanat.DTOs;
using QRQueueDecanat.Services;
using QRQueueDecanat.Interfaces;

namespace QRQueueDecanat.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await _authService.LoginAsync(
            request, cancellationToken);
        return Ok(response);
    }
}