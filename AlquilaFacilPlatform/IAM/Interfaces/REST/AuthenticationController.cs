using System.Net.Mime;
using AlquilaFacilPlatform.IAM.Domain.Model.Commands;
using AlquilaFacilPlatform.IAM.Domain.Repositories;
using AlquilaFacilPlatform.IAM.Domain.Services;
using AlquilaFacilPlatform.IAM.Interfaces.REST.Resources;
using AlquilaFacilPlatform.IAM.Interfaces.REST.Transform;
using AlquilaFacilPlatform.Shared.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AlquilaFacilPlatform.IAM.Interfaces.REST;


[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class AuthenticationController(
    IUserCommandService userCommandService,
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork) : ControllerBase
{

    /// <summary>
    /// Sign in endpoint. Authenticates a user and returns access and refresh tokens.
    /// </summary>
    /// <param name="signInResource">The sign in resource containing email and password.</param>
    /// <returns>The authenticated user resource with access token (15 min) and refresh token (7 days)</returns>
    [HttpPost("sign-in")]
    public async Task<IActionResult> SignIn([FromBody] SignInResource signInResource)
    {
        var signInCommand = SignInCommandFromResourceAssembler.ToCommandFromResource(signInResource);
        var authenticatedUser = await userCommandService.Handle(signInCommand);
        var resource =
            AuthenticatedUserResourceFromEntityAssembler.ToResourceFromEntity(
                authenticatedUser.user,
                authenticatedUser.accessToken,
                authenticatedUser.refreshToken);
        return Ok(resource);
    }

    /// <summary>
    /// Sign up endpoint. Creates a new user account.
    /// </summary>
    /// <param name="signUpResource">The sign up resource containing user registration data.</param>
    /// <returns>A confirmation message on successful creation.</returns>
    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUp([FromBody] SignUpResource signUpResource)
    {
        var signUpCommand = SignUpCommandFromResourceAssembler.ToCommandFromResource(signUpResource);
        await userCommandService.Handle(signUpCommand);
        return Ok(new { message = "User created successfully"});
    }

    /// <summary>
    /// Refresh token endpoint. Exchanges a valid refresh token for new access and refresh tokens.
    /// </summary>
    /// <param name="refreshTokenResource">The refresh token resource containing the refresh token.</param>
    /// <returns>New access token (15 min) and refresh token (7 days), or 401 if invalid</returns>
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenResource refreshTokenResource)
    {
        var command = new RefreshTokenCommand(refreshTokenResource.RefreshToken);
        var result = await userCommandService.Handle(command);

        if (result == null)
            return Unauthorized(new { message = "Invalid or expired refresh token" });

        var resource = AuthenticatedUserResourceFromEntityAssembler.ToResourceFromEntity(
            result.Value.user,
            result.Value.accessToken,
            result.Value.refreshToken);

        return Ok(resource);
    }

    /// <summary>
    /// Logout endpoint. Revokes a specific refresh token.
    /// </summary>
    /// <param name="logoutResource">The logout resource containing the refresh token to revoke.</param>
    /// <returns>A confirmation message on successful revocation.</returns>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutResource logoutResource)
    {
        var refreshToken = await refreshTokenRepository.FindByTokenAsync(logoutResource.RefreshToken);

        if (refreshToken == null)
            return NotFound(new { message = "Token not found" });

        if (refreshToken.IsRevoked)
            return BadRequest(new { message = "Token already revoked" });

        refreshToken.Revoke();
        await unitOfWork.CompleteAsync();

        return Ok(new { message = "Logged out successfully" });
    }

    /// <summary>
    /// Revoke all tokens endpoint. Revokes all refresh tokens for a specific user.
    /// </summary>
    /// <param name="userId">The user ID whose tokens should be revoked.</param>
    /// <returns>A confirmation message on successful revocation.</returns>
    [HttpPost("revoke-all/{userId:int}")]
    public async Task<IActionResult> RevokeAllTokens(int userId)
    {
        await refreshTokenRepository.RevokeAllUserTokensAsync(userId);
        await unitOfWork.CompleteAsync();

        return Ok(new { message = "All tokens revoked successfully" });
    }
}