using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace IssueTracker.IntegrationTests.Library.Common
{
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string AuthenticationScheme = "Test";
        private readonly List<Claim> _additionalClaims;

        public TestAuthHandler(IOptionsMonitor<TestAuthHandlerOptions> options,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            _additionalClaims = options.CurrentValue.AdditionalClaims;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "Test user")};
            if (_additionalClaims.Count > 0)
            {
                claims.AddRange(_additionalClaims);
            }
            var identity = new ClaimsIdentity(claims, AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, AuthenticationScheme);

            var result = AuthenticateResult.Success(ticket);

            return Task.FromResult(result);
        }
    }

    public class TestAuthHandlerOptions : AuthenticationSchemeOptions
    {
        public List<Claim> AdditionalClaims { get; set; } = new List<Claim>();
    }
}
