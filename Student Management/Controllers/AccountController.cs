using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Student_Management.DTO.AccountDto;
using Student_Management.Models;
using Student_Management.Services;

namespace Student_Management.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AccountController(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            ITokenService tokenService,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _emailService = emailService;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User already exists!" });

            User user = new User()
            {
                Email = model.Email,
                Name = model.Name,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = $"User creation failed! Errors: {errors}" });
            }

            if (!await _roleManager.RoleExistsAsync(model.Role))
                await _roleManager.CreateAsync(new IdentityRole(model.Role));

            if (await _roleManager.RoleExistsAsync(model.Role))
            {
                await _userManager.AddToRoleAsync(user, model.Role);
            }

            return Ok(new { Status = "Success", Message = "User created successfully!" });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized();

            var tokens = await _tokenService.IssueTokensAsync(user);
            return Ok(tokens);
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto model)
        {
            var tokens = await _tokenService.RefreshAsync(model.RefreshToken);
            if (tokens == null)
                return Unauthorized(new { Status = "Error", Message = "Invalid or expired refresh token" });

            return Ok(tokens);
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Email))
                return BadRequest(new { Status = "Error", Message = "Email is required" });

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Ok(new { Status = "Success", Message = "If the email exists, a reset link has been sent." });

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Uri.EscapeDataString(token);
            var encodedEmail = Uri.EscapeDataString(model.Email);

            var baseUrl = _configuration["ClientApp:ResetPasswordUrl"] ?? "http://localhost:3000/reset-password";
            var resetLink = $"{baseUrl}?email={encodedEmail}&token={encodedToken}";

            var body = $@"
                <h2>Password Reset Request</h2>
                <p>To reset your password, click the link below:</p>
                <p><a href=""{resetLink}"">Reset Password</a></p>
                <p>If you did not request this, please ignore this email.</p>";

            await _emailService.SendAsync(model.Email, "Reset Your Password", body);

            return Ok(new { Status = "Success", Message = "If the email exists, a reset link has been sent." });
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) ||
                string.IsNullOrWhiteSpace(model.Token) ||
                string.IsNullOrWhiteSpace(model.NewPassword))
            {
                return BadRequest(new { Status = "Error", Message = "Email, token and new password are required" });
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest(new { Status = "Error", Message = "Invalid reset request" });

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(new { Status = "Error", Message = $"Password reset failed: {errors}" });
            }

            return Ok(new { Status = "Success", Message = "Password has been reset successfully." });
        }
    }
}
