using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserService.Interfaces;

namespace UserService.Services
{
	public class JwtProvider : IJwtProvider
	{
		/// <summary>
		/// Creates a token using the User class with a UserName property
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="user"></param>
		/// <returns></returns>
		public string CreateToken<T>(T user) where T : class
		{

			List<Claim> claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, user.GetType().GetProperty("Username")?.GetValue(user, null) as string)
			};

			var secretPath = Environment.GetEnvironmentVariable("secretPath");
			if (string.IsNullOrEmpty(secretPath) || !System.IO.File.Exists(secretPath))
			{
				Console.WriteLine("Secret path is not set or file does not exist");
			}
			var secretValue = System.IO.File.ReadAllText(secretPath!);
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretValue!));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

			var token = new JwtSecurityToken(
				claims: claims,
				expires: DateTime.Now.AddHours(5),
				signingCredentials: creds,
				issuer: "http://UserService"
				);

			var jwt = new JwtSecurityTokenHandler().WriteToken(token);
			return jwt;

		}


		public bool VerifyToken(string jwt)
		{
			var secretPath = Environment.GetEnvironmentVariable("secretPath");
			if (string.IsNullOrEmpty(secretPath) || !System.IO.File.Exists(secretPath))
			{
				Console.WriteLine("Secret path is not set or file does not exist");
			}

			var secretValue = System.IO.File.ReadAllText(secretPath!);
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretValue!));

			var tokenHandler = new JwtSecurityTokenHandler();
			var validationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = false,
				ValidateLifetime = true,          // Ensure the token has not expired
				ValidateIssuerSigningKey = true,  // Validate the signing key
				IssuerSigningKey = key, // Provide the shared secret key for validation
				ValidIssuers = new[] { "http://loginapi" }  // valid issuer, should be hidden of course
			};

			try
			{
				// Validate the token and return the principal (claims)
				var principal = tokenHandler.ValidateToken(jwt, validationParameters, out SecurityToken validatedToken);

				// You can access token properties like claims and expiration here
				var jwtToken = (JwtSecurityToken)validatedToken;
				return true;
			}
			catch (SecurityTokenException ex)
			{
				// Token validation failed
				Console.WriteLine($"Token validation failed: {ex.Message}");
				return false;
			}
		}
	}
}
