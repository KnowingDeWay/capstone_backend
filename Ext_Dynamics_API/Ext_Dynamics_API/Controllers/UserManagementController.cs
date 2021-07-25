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

        [HttpPost]
        [Route("RegisterUser")]
        public ActionResult RegisterUser([FromBody] ApplicationUserAccount userAccount)
        {
            var objResponse = new ObjectResponse();

            // App user name must be at least 3 characters long and must be alphanumeric
            if (!string.IsNullOrWhiteSpace(userAccount.AppUserName))
            {
                if(userAccount.AppUserName.Length < 3 || !Regex.IsMatch(userAccount.AppUserName, "^[a-zA-Z0-9]+$"))
                {
                    objResponse.Message = "Username cannot be empty and must contain two alphanumeric characters";
                    return new BadRequestObjectResult(objResponse);
                }
            }
            else
            {
                objResponse.Message = "Username cannot be empty and must contain two alphanumeric characters";
                return new BadRequestObjectResult(objResponse);
            }

            // Password must be at least 8 characters long
            if(!string.IsNullOrWhiteSpace(userAccount.UserPassword))
            {
                if(userAccount.UserPassword.Length < 8)
                {
                    objResponse.Message = "Password must be 8 characters long";
                    return new BadRequestObjectResult(objResponse);
                }
            }
            else
            {
                objResponse.Message = "Password must be 8 characters long";
                return new BadRequestObjectResult(objResponse);
            }

            // Usernames must be also unique and they are also alternate keys in the database
            var existingUser = _dbCtx.UserAccounts.Where(x => x.AppUserName.Equals(userAccount.AppUserName)).FirstOrDefault();
            if(existingUser != null)
            {
                objResponse.Message = $"Username {userAccount.AppUserName} is already taken";
                return new BadRequestObjectResult(objResponse);
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
                objResponse.Message = "Failed to Create New User";
                // Rollback the addition of a new user
                _dbCtx.UserAccounts.Remove(newUser);
                return new NotFoundObjectResult(objResponse);
            }

            var token = _tokenManager.IssueToken(newUser);
            if (token.Equals(""))
            {
                objResponse.Message = "Failed to Generate Token";
                return new NotFoundObjectResult(objResponse);
            }

            objResponse.Message = $"Successfully Registered new User: {userAccount.AppUserName}";
            return new OkObjectResult(objResponse);
        }

        [HttpDelete]
        [Route("DeleteUser")]
        [Authorize]
        public ActionResult DeleteUser()
        {
            var encodedToken = _tokenManager.ReadToken(Request.Headers[_config.authHeader]);
            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken decodedToken;

            var objResponse = new ObjectResponse();
            try
            {
                decodedToken = handler.ReadJwtToken(encodedToken);
            }
            catch(ArgumentException)
            {
                objResponse.Message = "User Token Is Not Valid";
                return new UnauthorizedObjectResult(objResponse);
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
                        objResponse.Message = $"User Account {user.AppUserName} was successfully deleted!";
                        return new OkObjectResult(objResponse);
                    }
                    catch (Exception)
                    {
                        // Rollback user deleteion
                        _dbCtx.UserAccounts.Add(user);
                        objResponse.Message = "Failed to Delete User Profile";
                        return new NotFoundObjectResult(objResponse);
                    }
                }
                objResponse.Message = "Failed to find User";
                return new NotFoundObjectResult(objResponse);
            }
            objResponse.Message = "User Token Is Not Valid";
            return new UnauthorizedObjectResult(objResponse);
        }

        [HttpDelete]
        [Route("DeleteUser/{id}")]
        [Authorize]
        public ActionResult DeleteUserById([FromRoute] int userId)
        {
            var encodedToken = _tokenManager.ReadToken(Request.Headers[_config.authHeader]);
            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken decodedToken;

            var objResponse = new ObjectResponse();
            try
            {
                decodedToken = handler.ReadJwtToken(encodedToken);
            }
            catch (ArgumentException)
            {
                objResponse.Message = "User Token Is Not Valid";
                return new UnauthorizedObjectResult(objResponse);
            }

            // A user must be an admin to delete other users
            if (_tokenManager.GetUserTypeFromToken(decodedToken) != UserType.System_Admin)
            {
                objResponse.Message = "You are not Authorized to Perform this Function, You must be a System Admin";
                return new UnauthorizedObjectResult(objResponse);
            }

            var user = _dbCtx.UserAccounts.Where(x => x.Id == userId).FirstOrDefault();
            if (user != null)
            {
                _dbCtx.UserAccounts.Remove(user);
                try
                {
                    _dbCtx.SaveChanges();
                    objResponse.Message = $"User Account {user.AppUserName} was successfully deleted!";
                    return new OkObjectResult(objResponse);
                }
                catch (Exception)
                {
                    // Rollback user deleteion
                    _dbCtx.UserAccounts.Add(user);
                    objResponse.Message = "Failed to Delete User Profile";
                    return new NotFoundObjectResult(objResponse);
                }
            }
            objResponse.Message = "Failed to find User";
            return new NotFoundObjectResult(objResponse);
        }
    }
}
