using Isopoh.Cryptography.Argon2;
using Isopoh.Cryptography.SecureArray;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Fair_Recruitment_Web_Result.Handlers;
using Fair_Recruitment_Web_Result.Data;
using static Fair_Recruitment_Web_Result.Models.Database;

namespace Fair_Recruitment_Web_Result.Auth
{
    public class LoginOutput
    {
        public string msg { get; set; }
        public LoginResult? loginResult { get; set; }
    }
    public class LoginResult
    {
        public string userName { get; set; }
        public string email { get; set; }
        public string token { get; set; }
        public string expires { get; set; }
        public bool success { get; set; }
        public int role { get; set; }
    }
    public class CustomAuthentication : AuthenticationStateProvider
    {
        private readonly IConfiguration _configuration;
        private readonly DbHandler _dbHandler;
        public CustomAuthentication(IConfiguration config, DbHandler dbHandler)
        {
            _configuration = config;
            _dbHandler = dbHandler;
        }
        public async Task<LoginOutput> Auth420(string id, string pass, HttpContext context)
        {
            LoginOutput loginOutput = new LoginOutput();

            var user = await _dbHandler.GetUserByEmailOrUserId(id);

            if (user == null)
            {
                loginOutput.msg = "User not found in database";
                return loginOutput;
            }

            if (pass != user.password)
            {
                loginOutput.msg = $"User password mismatch, typed {pass}, actual {user.password}";
                return loginOutput;
            }

            //string secret = _configuration["Auth:Jwt:Secret"];
            //string passwordHash = await Argon2Hashing(user.password, secret);

            //if (String.IsNullOrWhiteSpace(secret) || String.IsNullOrWhiteSpace(passwordHash))
            //{
            //    return null;
            //}

            //bool verifyPassword = await VerifyPassword(pass, passwordHash);

            //if (!verifyPassword)
            //{
            //    return null;
            //}

            if (string.IsNullOrWhiteSpace(user.user_id))
            {
                user.user_id = user.email;
            }

            Claim[]? claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.user_id),
                new Claim(JwtRegisteredClaimNames.Sub, user.name),
                new Claim(JwtRegisteredClaimNames.Email, user.email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            LoginResult? loginResult = await CreateJwt(user, claims);
            loginOutput.loginResult = loginResult;

            if (loginResult == null)
            {
                loginOutput.msg = "Jwt token creation error";
                return loginOutput;
            }

            try
            {
                await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme)), new AuthenticationProperties
                {
                    IsPersistent = true, // Customize as needed
                    AllowRefresh = true,
                    ExpiresUtc = DateTime.Now.AddHours(48) // Set the cookie expiration time
                });

                return await Task.FromResult(loginOutput);
            }
            catch (Exception ex)
            {
                LogHandler.WriteErrorLog(ex);
                loginOutput.msg = ex.Message.ToString();
                return loginOutput;
            }
        }
        private async Task<bool> VerifyPassword(string password, string passwordHash)
        {
            bool result = false;
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] secretBytes = Encoding.UTF8.GetBytes(_configuration["Auth:Jwt:Secret"]);

            var configOfPasswordToVerify = new Argon2Config { Password = passwordBytes, Threads = 1, Secret = secretBytes };
            SecureArray<byte> hashB = null;

            try
            {
                if (configOfPasswordToVerify.DecodeString(passwordHash, out hashB) && hashB != null)
                {
                    var argon2ToVerify = new Argon2(configOfPasswordToVerify);
                    using (var hashToVerify = argon2ToVerify.Hash())
                    {
                        if (Argon2.FixedTimeEquals(hashB, hashToVerify))
                        {
                            result = true;
                        }
                    }
                }
            }
            finally
            {
                hashB?.Dispose();
            }

            return await Task.FromResult(result);
        }
        public async Task<LoginResult?> CreateJwt(User user, Claim[] claims)
        {
            //Create a new JwtToken with JwtHelper and add our claim
            JwtSecurityToken? generatedToken = await GetJwtToken(user_name: user.name, email: user.email, signingKey: _configuration["Auth:Jwt:SigningKey"], issuer: _configuration["Auth:Jwt:Issuer"], audience: _configuration["Auth:Jwt:Audience"], expiration: TimeSpan.FromHours(48), claims: claims);
            string token = new JwtSecurityTokenHandler().WriteToken(generatedToken);

            //Authenticate
            bool isAuthenticated = await Authenticate(token);

            //Return the JwtToken to API client
            LoginResult loginResult = new LoginResult();

            if (isAuthenticated)
            {
                loginResult.userName = user.name;
                loginResult.email = user.email;
                loginResult.token = token;
                loginResult.expires = generatedToken.ValidTo.ToString();
                loginResult.success = isAuthenticated;
                loginResult.role = user.role;
                return loginResult;
            }

            return null;
        }
        private async Task<JwtSecurityToken> GetJwtToken(string user_name, string email, string signingKey, string issuer, string audience, TimeSpan expiration, Claim[] claims)
        {
            SymmetricSecurityKey? key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
            SigningCredentials? creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                expires: DateTime.Now.Add(expiration),
                claims: claims,
                signingCredentials: creds
            );
        }

        private async Task<bool> Authenticate(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            token = token.Substring(token.IndexOf(';') + 1);
            var userId = await ValidateJwtToken(token);
            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }

            JwtSecurityToken? tokenParsed = ParseToken(token);
            if (tokenParsed != null && tokenParsed.ValidTo >= DateTime.UtcNow)
            {
                List<Claim> claims = tokenParsed.Claims.ToList();
                //var jti = tokenS.Claims.First(claim => claim.Type == "jti").Value;

                ClaimsIdentity? identity = new ClaimsIdentity(claims, "Jwt");
                ClaimsPrincipal? principal = new ClaimsPrincipal(identity);
                AuthenticationState state = new AuthenticationState(principal);
                NotifyAuthenticationStateChanged(Task.FromResult(state));
                return true;
            }
            else
            {
                return false;
            }
        }
        private async Task<string?> ValidateJwtToken(string token)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[]? key = Encoding.ASCII.GetBytes(_configuration["Auth:Jwt:SigningKey"]);
            if (key == null)
            {
                return null;
            }

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Auth:Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Auth:Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var accountId = jwtToken.Claims.First().Value;
                return await Task.FromResult(accountId);
            }
            catch (Exception ex)
            {
                LogHandler.WriteErrorLog(ex);
                return null;
            }
        }
        private JwtSecurityToken? ParseToken(string token)
        {
            JwtSecurityTokenHandler? handler = new JwtSecurityTokenHandler();
            SecurityToken? jsonToken = handler.ReadToken(token);
            JwtSecurityToken? tokenParsed = jsonToken as JwtSecurityToken;
            return tokenParsed;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            throw new NotImplementedException();
        }

        private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();
        public static async Task<string> Argon2Hashing(string password, string secret)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] secretBytes = Encoding.UTF8.GetBytes(secret);
            //byte[] associatedDataBytes = Encoding.UTF8.GetBytes(_config["Security:AssociatedData"]);
            byte[] salt = new byte[16];

            Rng.GetBytes(salt);

            var config = new Argon2Config
            {
                Type = Argon2Type.DataIndependentAddressing,
                Version = Argon2Version.Nineteen,
                TimeCost = 10,
                MemoryCost = 32768,
                Lanes = 4,
                Threads = 1,
                Password = passwordBytes,
                Salt = salt,
                Secret = secretBytes,
                //AssociatedData = associatedDataBytes,
                HashLength = 20
            };

            var argon2A = new Argon2(config);

            string hashString;

            using (SecureArray<byte> hashA = argon2A.Hash())
            {
                hashString = config.EncodeString(hashA.Buffer);
            }

            return await Task.FromResult(hashString);
        }
    }
}
