using PawsitiveHaven.Api.Data.Repositories;
using PawsitiveHaven.Api.Models.DTOs;
using PawsitiveHaven.Api.Models.Entities;

namespace PawsitiveHaven.Api.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);

            if (user == null)
            {
                _logger.LogWarning("Login attempt for non-existent user: {Username}", request.Username);
                return new AuthResponse(false, null, null, null, null, "Invalid username or password");
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("Login attempt for inactive user: {Username}", request.Username);
                return new AuthResponse(false, null, null, null, null, "Account is deactivated");
            }

            if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Invalid password for user: {Username}", request.Username);
                return new AuthResponse(false, null, null, null, null, "Invalid username or password");
            }

            var token = _jwtService.GenerateToken(user);
            _logger.LogInformation("User logged in successfully: {Username}", request.Username);

            return new AuthResponse(true, token, user.Id, user.Username, user.UserLevel, "Login successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user: {Username}", request.Username);
            return new AuthResponse(false, null, null, null, null, "An error occurred during login");
        }
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        try
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.Username) || request.Username.Length < 3)
            {
                return new AuthResponse(false, null, null, null, null, "Username must be at least 3 characters");
            }

            if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains('@'))
            {
                return new AuthResponse(false, null, null, null, null, "Invalid email address");
            }

            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
            {
                return new AuthResponse(false, null, null, null, null, "Password must be at least 6 characters");
            }

            // Check for existing user
            if (await _userRepository.UsernameExistsAsync(request.Username))
            {
                return new AuthResponse(false, null, null, null, null, "Username already exists");
            }

            if (await _userRepository.EmailExistsAsync(request.Email))
            {
                return new AuthResponse(false, null, null, null, null, "Email already exists");
            }

            // Create user
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = _passwordHasher.HashPassword(request.Password),
                UserLevel = "User",
                IsActive = true
            };

            await _userRepository.AddAsync(user);

            var token = _jwtService.GenerateToken(user);
            _logger.LogInformation("User registered successfully: {Username}", request.Username);

            return new AuthResponse(true, token, user.Id, user.Username, user.UserLevel, "Registration successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for user: {Username}", request.Username);
            return new AuthResponse(false, null, null, null, null, "An error occurred during registration");
        }
    }
}
