using Ext_Dynamics_API.Configuration.Models;
using Ext_Dynamics_API.DataAccess;
using Ext_Dynamics_API.Models;
using Ext_Dynamics_API.ResponseModels;
using Ext_Dynamics_API.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scrypt;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Ext_Dynamics_API.Enums;
using System.Text.RegularExpressions;

namespace Ext_Dynamics_API.Controllers
{
    [Route("api/UserManagementController")]
    [ApiController]
    public class UserManagementController : ControllerBase
    {
        private readonly ExtensibleDbContext _dbCtx;
        private readonly TokenManager _tokenManager;
        private readonly ScryptEncoder _scryptHasher; // Using the SCRYPT hashing algorithm 
        private readonly SystemConfig _config;

        public UserManagementController(ExtensibleDbContext dbCtx)
        {
            _dbCtx = dbCtx;
            _config = SystemConfig.LoadConfig();
            _tokenManager = new TokenManager(_dbCtx, _config);
            _scryptHasher = new ScryptEncoder();
        }

        [HttpGet]
        [Authorize]
        [Route("ViewUserProfile")]
        public ActionResult ViewUserProfile()
        {
            var encodedToken = _tokenManager.ReadAndValidateToken(Request.Headers[_config.authHeader]);
            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken decodedToken;

            var objResponse = new ObjectResponse<UserProfile>();

            try
            {
                decodedToken = handler.ReadJwtToken(encodedToken);
            }
            catch (ArgumentException)
            {
                objResponse.Message = "User Token Is Not Valid";
                return new UnauthorizedObjectResult(objResponse);
            }

            var userId = _tokenManager.GetUserIdFromToken(decodedToken);

            if(userId == -1)
            {
                objResponse.Message = "User Not Found";
                return new NotFoundObjectResult(objResponse);
            }

            var user = _dbCtx.UserAccounts.Where(x => x.Id == userId).FirstOrDefault();

            if (user == null)
            {
                objResponse.Message = "User Not Found";
                return new NotFoundObjectResult(objResponse);
            }

            objResponse.Message = $"Retreived User Profile for: {user.AppUserName}";
            objResponse.Value = new UserProfile { 
                AppUserName = user.AppUserName,
                UserType = user.UserType
            };

            return new OkObjectResult(objResponse);

        }

        [HttpPost]
        [Route("RegisterUser")]
        public ActionResult RegisterUser([FromBody] ApplicationUserAccount userAccount)
        {

            // App user name must be at least 3 characters long and must be alphanumeric
            if (!string.IsNullOrWhiteSpace(userAccount.AppUserName))
            {
                if(userAccount.AppUserName.Length < 3 || !Regex.IsMatch(userAccount.AppUserName, "^[a-zA-Z0-9]+$"))
                {
                    return new BadRequestObjectResult("Username cannot be empty and must contain two alphanumeric characters");
                }
            }
            else
            {
                return new BadRequestObjectResult("Username cannot be empty and must contain two alphanumeric characters");
            }

            // Password must be at least 8 characters long
            if(!string.IsNullOrWhiteSpace(userAccount.UserPassword))
            {
                if(userAccount.UserPassword.Length < 8)
                {
                    return new BadRequestObjectResult("Password must be 8 characters long");
                }
            }
            else
            {
                return new BadRequestObjectResult("Password must be 8 characters long");
            }

            // Usernames must be also unique and they are also alternate keys in the database
            var existingUser = _dbCtx.UserAccounts.Where(x => x.AppUserName.Equals(userAccount.AppUserName)).FirstOrDefault();
            if(existingUser != null)
            {
                return new BadRequestObjectResult($"Username {userAccount.AppUserName} is already taken");
            }

            var newUser = new ApplicationUserAccount()
            {
                AppUserName = userAccount.AppUserName,
                UserPassword = _scryptHasher.Encode(userAccount.UserPassword),
                UserType = UserType.Instructor
            };
            _dbCtx.UserAccounts.Add(newUser);

            try
            {
                _dbCtx.SaveChanges();
            }
            catch(Exception)
            {
                // Rollback the addition of a new user
                _dbCtx.UserAccounts.Remove(newUser);
                return new NotFoundObjectResult("Failed to Create New User");
            }

            var token = _tokenManager.IssueToken(newUser);
            if (token.Equals(""))
            {
                return new NotFoundObjectResult("Failed to Generate Token");
            }

            return new OkObjectResult($"Successfully Registered new User: {userAccount.AppUserName}");
        }

        [HttpDelete]
        [Route("DeleteUser")]
        [Authorize]
        public ActionResult DeleteUser()
        {
            var encodedToken = _tokenManager.ReadAndValidateToken(Request.Headers[_config.authHeader]);
            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken decodedToken;

            try
            {
                decodedToken = handler.ReadJwtToken(encodedToken);
            }
            catch(ArgumentException)
            {
                return new UnauthorizedObjectResult("User Token Is Not Valid");
            }

            if (_tokenManager.IsTokenValid(encodedToken))
            {
                var userId = _tokenManager.GetUserIdFromToken(decodedToken);
                _tokenManager.DeleteUserTokens(userId);
                var user = _dbCtx.UserAccounts.Where(x => x.Id == userId).FirstOrDefault();
                if(user != null)
                {
                    _dbCtx.UserAccounts.Remove(user);
                    try
                    {
                        _dbCtx.SaveChanges();
                        return new OkObjectResult($"User Account {user.AppUserName} was successfully deleted!");
                    }
                    catch (Exception)
                    {
                        // Rollback user deleteion
                        _dbCtx.UserAccounts.Add(user);
                        return new NotFoundObjectResult("Failed to Delete User Profile");
                    }
                }

                return new NotFoundObjectResult("Failed to find User");
            }

            return new UnauthorizedObjectResult("User Token Is Not Valid");
        }

        [HttpDelete]
        [Route("DeleteUser/{id}")]
        [Authorize]
        public ActionResult DeleteUserById([FromRoute] int userId)
        {
            var encodedToken = _tokenManager.ReadAndValidateToken(Request.Headers[_config.authHeader]);
            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken decodedToken;

            try
            {
                decodedToken = handler.ReadJwtToken(encodedToken);
            }
            catch (ArgumentException)
            {
                return new UnauthorizedObjectResult("User Token Is Not Valid");
            }

            // A user must be an admin to delete other users
            if (_tokenManager.GetUserTypeFromToken(decodedToken) != UserType.System_Admin)
            {
                return new UnauthorizedObjectResult("You are not Authorized to Perform this Function, You must be a System Admin");
            }

            var user = _dbCtx.UserAccounts.Where(x => x.Id == userId).FirstOrDefault();
            if (user != null)
            {
                _dbCtx.UserAccounts.Remove(user);
                try
                {
                    _dbCtx.SaveChanges();
                    return new OkObjectResult($"User Account {user.AppUserName} was successfully deleted!");
                }
                catch (Exception)
                {
                    // Rollback user deleteion
                    _dbCtx.UserAccounts.Add(user);
                    return new NotFoundObjectResult("Failed to Delete User Profile");
                }
            }

            return new NotFoundObjectResult("Failed to find User");
        }
    }
}
