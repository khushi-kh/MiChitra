using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using MiChitra.Models;
using MiChitra.Services;
using System.IdentityModel.Tokens.Jwt;

namespace MiChitra.Tests;

[TestFixture]
public class JwtServiceTests
{
    private JwtService _service = null!;

    private static IConfiguration BuildConfig(string key = "supersecretkey1234567890abcdefgh", string issuer = "TestIssuer", string audience = "TestAudience", int expiry = 60)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = key,
                ["Jwt:Issuer"] = issuer,
                ["Jwt:Audience"] = audience,
                ["Jwt:ExpiryMinutes"] = expiry.ToString()
            })
            .Build();
    }

    private static User MakeUser(int id = 1, string username = "testuser", string email = "test@example.com", UserRole role = UserRole.User)
        => new User { UserId = id, Username = username, Email = email, Role = role, FName = "Test", LName = "User", PasswordHash = "hash" };

    [SetUp]
    public void SetUp()
    {
        // Match the setting in Program.cs so claim types aren't remapped
        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
        _service = new JwtService(BuildConfig(), NullLogger<JwtService>.Instance);
    }

    // --- GenerateToken ---

    [Test]
    public void GenerateToken_ValidUser_ReturnsNonEmptyToken()
    {
        var token = _service.GenerateToken(MakeUser());
        Assert.That(token, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public void GenerateToken_TokenIsValidJwtFormat()
    {
        var token = _service.GenerateToken(MakeUser());
        // JWT has 3 dot-separated parts
        Assert.That(token.Split('.').Length, Is.EqualTo(3));
    }

    // --- ValidateToken ---

    [Test]
    public void ValidateToken_ValidToken_ReturnsPrincipal()
    {
        var token = _service.GenerateToken(MakeUser());
        var principal = _service.ValidateToken(token);
        Assert.That(principal, Is.Not.Null);
    }

    [Test]
    public void ValidateToken_InvalidToken_ReturnsNull()
    {
        var principal = _service.ValidateToken("not.a.valid.token");
        Assert.That(principal, Is.Null);
    }

    [Test]
    public void ValidateToken_TamperedToken_ReturnsNull()
    {
        var token = _service.GenerateToken(MakeUser());
        var tampered = token[..^5] + "XXXXX";
        var principal = _service.ValidateToken(tampered);
        Assert.That(principal, Is.Null);
    }

    // --- GetUserIdFromToken ---

    [Test]
    public void GetUserIdFromToken_ValidToken_ReturnsCorrectUserId()
    {
        var user = MakeUser(id: 42);
        var token = _service.GenerateToken(user);

        var userId = _service.GetUserIdFromToken(token);

        Assert.That(userId, Is.EqualTo(42));
    }

    [Test]
    public void GetUserIdFromToken_InvalidToken_ReturnsNull()
    {
        var userId = _service.GetUserIdFromToken("invalid.token.here");
        Assert.That(userId, Is.Null);
    }

    // --- GetUserRoleFromToken ---

    [Test]
    public void GetUserRoleFromToken_ValidToken_ReturnsCorrectRole()
    {
        var user = MakeUser(role: UserRole.Admin);
        var token = _service.GenerateToken(user);

        var role = _service.GetUserRoleFromToken(token);

        Assert.That(role, Is.EqualTo(UserRole.Admin));
    }

    [Test]
    public void GetUserRoleFromToken_InvalidToken_ReturnsNull()
    {
        var role = _service.GetUserRoleFromToken("bad.token");
        Assert.That(role, Is.Null);
    }

    // --- Constructor validation ---

    [Test]
    public void Constructor_ShortKey_ThrowsInvalidOperationException()
    {
        var config = BuildConfig(key: "short");
        Assert.Throws<InvalidOperationException>(() => new JwtService(config, NullLogger<JwtService>.Instance));
    }
}
