using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QRQueueDecanat.Authentication;
using QRQueueDecanat.Data;
using QRQueueDecanat.DTOs;
using QRQueueDecanat.Entities;
using QRQueueDecanat.Exceptions;
using QRQueueDecanat.Interfaces;

namespace QRQueueDecanat.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher<AppUser> _passwordHasher;
    private readonly JwtTokenService _jwtTokenService;

    public AuthService(ApplicationDbContext context,
        IPasswordHasher<AppUser> passwordHasher,
        JwtTokenService jwtTokenService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var username = request.Username;
        var user = await _context.Users
            .Include(user => user.Role)
            .SingleOrDefaultAsync(user =>
                user.Username == username && user.IsActive,
                cancellationToken);
        if (user is null)
        {
            throw new InvalidCredentialsException(
                "Invalid login or password.");
        }
        var passwordResult = _passwordHasher.VerifyHashedPassword(
            user, user.PasswordHash, request.Password);
        if (passwordResult == PasswordVerificationResult.Failed)
        {
            throw new InvalidCredentialsException(
                "Invalid login or password.");
        }
        var token = _jwtTokenService.CreateToken(
            user.Id, user.Role.Name);
        return new LoginResponse(
            token.AccessToken,
            token.ExpiresAtUtc,
            new AuthenticatedUserResponse(
                user.Id,
                user.FullName,
                user.Role.Name));
    }
}