using Ext_Dynamics_API.Configuration.Models;
using Ext_Dynamics_API.DataAccess;
using Ext_Dynamics_API.Models;
using Ext_Dynamics_API.Security.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Ext_Dynamics_API.Enums;

namespace Ext_Dynamics_API.Security
{
    public class TokenManager
    {
        private readonly SystemConfig _config;
        private readonly ExtensibleDbContext _dbCtx;

        public TokenManager(ExtensibleDbContext dbCtx, SystemConfig config)
        {
            _dbCtx = dbCtx;
            _config = config;
        }

        /// <summary>
        /// Issues a new JWT user token for this application
        /// </summary>
        /// <param name="user">The user to issue the token to</param>
        /// <returns>string: The issued token in its encoded format</returns>
        public string IssueToken(ApplicationUserAccount user)
        {
            var secKey = _config.tokenSecKey;
            var symSecKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secKey));
            var signingCredentials = new SigningCredentials(symSecKey, SecurityAlgorithms.HmacSha512Signature);

            var claims = new List<Claim>();

            var jwtId = Guid.NewGuid().ToString();
            var utcDate = EpochTime.GetIntDate(DateTime.Now);

            claims.Add(new Claim("iat", $"{utcDate}", ClaimValueTypes.Integer64));
            claims.Add(new Claim("jti", jwtId, ClaimValueTypes.String));
            claims.Add(new Claim("user_id", $"{user.Id}", ClaimValueTypes.Integer32));
            claims.Add(new Claim("user_name", $"{user.AppUserName}", ClaimValueTypes.String));
            claims.Add(new Claim("user_type", $"{user.UserType}", ClaimValueTypes.String));

            var token = new JwtSecurityToken(
                    issuer: _config.tokenIssuer,
                    audience: _config.tokenAudience,
                    expires: EpochTime.DateTime(EpochTime.GetIntDate(DateTime.Now.AddDays(7))),
                    signingCredentials: signingCredentials,
                    claims: claims
                );

            var tokenHandler = new JwtSecurityTokenHandler();
            var encodedToken = tokenHandler.WriteToken(token);

            // Delete expired tokens for the user (exccess tokens for each user waste space)
            DeleteInavlidTokens(user.Id);

            // If the token can't be saved in the database
            if(!SaveToken(encodedToken, user.Id, token.ValidTo))
            {
                return "";
            }

            return encodedToken;
        }

        /// <summary>
        /// Reads a token from the Authorization HTTP Header
        /// </summary>
        /// <param name="authHeader">The content in the Authorization Header</param>
        /// <returns>string: The encoded token if it is valid, otherwise returns an empty string</returns>
        public string ReadAndValidateToken(string authHeader)
        {
            var token = authHeader.Split("Bearer ")[1];
            return IsTokenValid(token) ? token : "";
        }

        /// <summary>
        /// Deletes expired user tokens for a particular user
        /// </summary>
        /// <param name="userId">The id of the user being authenticated</param>
        public void DeleteUserTokens(int userId)
        {
            var user = _dbCtx.UserAccounts.Where(x => x.Id == userId).FirstOrDefault();
            if(user != null)
            {
                var tokenEntries = _dbCtx.UserTokenEntries.Where(x => x.AppUserId == user.Id).ToList();
                _dbCtx.UserTokenEntries.RemoveRange(tokenEntries);
                _dbCtx.SaveChanges();
            }
        }

        /// <summary>
        /// Determines whether or not the token is valid in terms of having a valid format <b>and</b> whether or not it is expired
        /// </summary>
        /// <param name="encodedToken">The JWT user token in encoded format</param>
        /// <returns>bool: Whether or not the token is valid <b>and</b> not expired</returns>
        public bool IsTokenValid(string encodedToken)
        {
            return _dbCtx.UserTokenEntries.Where(x => x.EncodedToken.Equals(encodedToken) && x.ExpiryDate > DateTime.UtcNow).FirstOrDefault() != null;
        }

        /// <summary>
        /// Extracts the id of the user in this system from the supplied JWT user token
        /// </summary>
        /// <param name="token">The token in its full and <b>decoded</b> format</param>
        /// <returns>int: The id of the user in the system</returns>
        public int GetUserIdFromToken(JwtSecurityToken token)
        {
            var userClaim = token.Claims.Where(x => x.Type.Equals("user_id")).FirstOrDefault();
            int userId = -1;
            if(int.TryParse(userClaim.Value, out userId))
            {
                return userId;
            }
            return userId;
        }

        /// <summary>
        /// Gets the user type of the person logged in (e.g. Instructor or System Admin etc) 
        /// </summary>
        /// <param name="token">The token in its full and <b>decoded</b> format</param>
        /// <returns>UserType: An enumerable that specifies the type of user logged in</returns>
        public UserType GetUserTypeFromToken(JwtSecurityToken token)
        {
            var userClaim = token.Claims.Where(x => x.Type.Equals("user_type")).FirstOrDefault();
            object type;
            if (Enum.TryParse(typeof(UserType), userClaim.Value, true, out type))
            {
                return (UserType)type;
            }
            return UserType.Invalid;
        }

        private void DeleteInavlidTokens(int userId)
        {
            var user = _dbCtx.UserAccounts.Where(x => x.Id == userId).FirstOrDefault();
            if (user != null)
            {
                var tokenEntries = _dbCtx.UserTokenEntries.Where(x => 
                DateTime.UtcNow > x.ExpiryDate
                );
                _dbCtx.UserTokenEntries.RemoveRange(tokenEntries);
                _dbCtx.SaveChanges();
            }
        }

        private bool SaveToken(string token, int userId, DateTime expiry)
        {
            var newTokEntry = new UserTokenEntry()
            {
                EncodedToken = token,
                AppUserId = userId,
                ExpiryDate = expiry
            };
            _dbCtx.UserTokenEntries.Add(newTokEntry);
            try
            {
                _dbCtx.SaveChanges();
                return true;
            }
            catch (DbUpdateConcurrencyException e)
            {
                foreach (var item in e.Entries)
                {
                    if (item.Entity is UserTokenEntry)
                    {
                        var currValues = item.CurrentValues;
                        var dbValues = item.GetDatabaseValues();

                        foreach (var property in currValues.Properties)
                        {
                            var currentValue = currValues[property];
                            var dbValue = dbValues[property];
                        }

                        // Refresh the original values to bypass next concurrency check
                        item.OriginalValues.SetValues(dbValues);
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                // Rollback token entry
                _dbCtx.UserTokenEntries.Remove(newTokEntry);
                return false;
            }
        }
    }
}
