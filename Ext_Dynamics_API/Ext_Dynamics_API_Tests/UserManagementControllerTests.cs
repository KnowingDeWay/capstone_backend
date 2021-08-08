using Ext_Dynamics_API.Configuration.Models;
using Ext_Dynamics_API.Controllers;
using Ext_Dynamics_API.DataAccess;
using Ext_Dynamics_API.Enums;
using Ext_Dynamics_API.Models;
using Ext_Dynamics_API.RequestModels;
using Ext_Dynamics_API.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scrypt;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Ext_Dynamics_API_Tests
{
    public class UserManagementControllerTests : IDisposable
    {
        private readonly ExtensibleDbContext _dbCtx;
        private readonly SystemConfig _config;
        private readonly ScryptEncoder _encoder;
        private readonly List<ApplicationUserAccount> _accounts;

        public UserManagementControllerTests()
        {
            _config = SystemConfig.LoadConfig();
            DbContextOptionsBuilder<ExtensibleDbContext> optionsBuilder = new();
            optionsBuilder.UseSqlServer(_config.testDbConnString);
            _dbCtx = new ExtensibleDbContext(optionsBuilder.Options);
            _encoder = new ScryptEncoder();
            _accounts = GenerateUserList();
            SetupContext();
        }

        private void SetupContext()
        {
            var testUser = _dbCtx.UserAccounts.Where(x => x.AppUserName.Equals("Nathan")).FirstOrDefault();
            if (testUser == null)
            {
                _dbCtx.UserAccounts.AddRange(_accounts);
                _dbCtx.SaveChanges();
            }
        }

        private List<ApplicationUserAccount> GenerateUserList()
        {
            var list = new List<ApplicationUserAccount>() {
                    new ApplicationUserAccount()
                    {
                        AppUserName = "Nathan",
                        UserPassword = _encoder.Encode("password"),
                        UserType = UserType.Instructor
                    },
                    new ApplicationUserAccount()
                    {
                        AppUserName = "John",
                        UserPassword = _encoder.Encode("jklolz69"),
                        UserType = UserType.Instructor
                    },
                    new ApplicationUserAccount()
                    {
                        AppUserName = "Nolen",
                        UserPassword = _encoder.Encode("ice_queen"),
                        UserType = UserType.Instructor
                    },
                    new ApplicationUserAccount()
                    {
                        AppUserName = "Rheda",
                        UserPassword = _encoder.Encode("giantbeach"),
                        UserType = UserType.Instructor
                    },
                    new ApplicationUserAccount()
                    {
                        AppUserName = "Onyx K",
                        UserPassword = _encoder.Encode("SeoulGirl"),
                        UserType = UserType.Instructor
                    },
                    new ApplicationUserAccount()
                    {
                        AppUserName = "SysAdmin",
                        UserPassword = _encoder.Encode("AdminUser"),
                        UserType = UserType.System_Admin
                    }
            };

            return list;
        }

        [Fact]
        public void Get_User_Profile_Valid_Creds()
        {
            var controller = new AuthController(_dbCtx);
            var creds = new LoginCredentials()
            {
                Username = "Nolen",
                Password = "ice_queen"
            };
            var result = controller.LoginUser(creds) as ObjectResult;

            var token = ((AuthResponse)result.Value).ResponseToken;

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[_config.authHeader] = $"Bearer {token}";

            var controllerContext = new ControllerContext() { HttpContext = httpContext };

            var userController = new UserManagementController(_dbCtx);

            userController.ControllerContext = controllerContext;

            var userControllerResult = userController.ViewUserProfile();

            var objResult = userControllerResult as ObjectResult;

            var response = (ObjectResponse<UserProfile>)objResult.Value;

            var profile = response.Value;

            var outcome = profile != null;

            Assert.True(outcome);
        }

        [Fact]
        public void Get_User_Profile_Invalid_Creds()
        {
            var controller = new AuthController(_dbCtx);
            var creds = new LoginCredentials()
            {
                Username = "Nolen",
                Password = "icy_gurl"
            };
            var result = controller.LoginUser(creds) as ObjectResult;

            var token = ((AuthResponse)result.Value).ResponseToken;

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[_config.authHeader] = $"Bearer {token}";

            var controllerContext = new ControllerContext() { HttpContext = httpContext };

            var userController = new UserManagementController(_dbCtx);

            userController.ControllerContext = controllerContext;

            var userControllerResult = userController.ViewUserProfile();

            var objResult = userControllerResult as ObjectResult;

            var response = (ObjectResponse<UserProfile>)objResult.Value;

            var profile = response.Value;

            var outcome = profile == null;

            Assert.True(outcome);
        }

        [Fact]
        public void Register_User_Valid_Creds()
        {
            var controller = new UserManagementController(_dbCtx);
            var newUser = new ApplicationUserAccount()
            {
                AppUserName = "George",
                UserPassword = "AbsoluteLegend"
            };
            controller.RegisterUser(newUser);

            var user = _dbCtx.UserAccounts.Where(x => x.AppUserName.Equals("George")).FirstOrDefault();

            var outcome = user != null;

            // Cleanup added objects
            if(outcome)
            {
                _dbCtx.UserAccounts.Remove(user);
                _dbCtx.SaveChanges();
            }

            Assert.True(outcome);
        }

        [Fact]
        public void Register_User_Invalid_Username_By_Length()
        {
            var controller = new UserManagementController(_dbCtx);
            var newUser = new ApplicationUserAccount()
            {
                AppUserName = "Ge",
                UserPassword = "eshayyyyyz"
            };
            controller.RegisterUser(newUser);

            var user = _dbCtx.UserAccounts.Where(x => x.AppUserName.Equals("Ge")).FirstOrDefault();

            var outcome = user == null;

            // Cleanup added objects
            if (!outcome)
            {
                _dbCtx.UserAccounts.Remove(newUser);
                _dbCtx.SaveChanges();
            }

            Assert.True(outcome);
        }

        [Fact]
        public void Register_User_Invalid_Username_By_Non_Alpha_Numeric_Chars()
        {
            var controller = new UserManagementController(_dbCtx);
            var newUser = new ApplicationUserAccount()
            {
                AppUserName = "Sablique V0# Lu$",
                UserPassword = "GypsyDough"
            };
            controller.RegisterUser(newUser);

            var user = _dbCtx.UserAccounts.Where(x => x.AppUserName.Equals("Sablique V0# Lu$")).FirstOrDefault();

            var outcome = user == null;

            // Cleanup added objects
            if (!outcome)
            {
                _dbCtx.UserAccounts.Remove(newUser);
                _dbCtx.SaveChanges();
            }

            Assert.True(outcome);
        }

        [Fact]
        public void Register_User_Same_Username()
        {
            bool outcome;
            try
            {
                var controller = new UserManagementController(_dbCtx);
                var newUser = new ApplicationUserAccount()
                {
                    AppUserName = "Nathan",
                    UserPassword = "thefailwhale"
                };
                controller.RegisterUser(newUser);
                outcome = _dbCtx.UserAccounts.Where(x => x.AppUserName.Equals("Nathan")).ToList().Count == 1;

                // Cleanup added objects
                if (!outcome)
                {
                    _dbCtx.UserAccounts.Remove(newUser);
                    _dbCtx.SaveChanges();
                }
            }
            catch(ArgumentException)
            {
                outcome = false;
            }

            Assert.True(outcome);
        }

        [Fact]
        public void Register_User_Invalid_Pass()
        {
            var controller = new UserManagementController(_dbCtx);
            var newUser = new ApplicationUserAccount()
            {
                AppUserName = "Ashur",
                UserPassword = "neeta"
            };
            controller.RegisterUser(newUser);

            var user = _dbCtx.UserAccounts.Where(x => x.AppUserName.Equals("Ashur")).FirstOrDefault();

            var outcome = user == null;

            // Cleanup added objects
            if (!outcome)
            {
                _dbCtx.UserAccounts.Remove(newUser);
                _dbCtx.SaveChanges();
            }

            Assert.True(outcome);
        }

        [Fact]
        public void Delete_User_Self()
        {
            var controller = new AuthController(_dbCtx);
            var creds = new LoginCredentials()
            {
                Username = "Rheda",
                Password = "giantbeach"
            };
            var result = controller.LoginUser(creds) as ObjectResult;

            var token = ((AuthResponse)result.Value).ResponseToken;

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[_config.authHeader] = $"Bearer {token}";

            var controllerContext = new ControllerContext() { HttpContext = httpContext };

            var userController = new UserManagementController(_dbCtx);

            userController.ControllerContext = controllerContext;

            userController.DeleteUser();

            var user = _dbCtx.UserAccounts.Where(x => x.AppUserName.Equals("Rheda")).FirstOrDefault();

            var outcome = user == null;

            Assert.True(outcome);

        }

        [Fact]
        public void Delete_User_Self_Invalid()
        {
            var controller = new AuthController(_dbCtx);
            var creds = new LoginCredentials()
            {
                Username = "John",
                Password = "seymour"
            };
            var result = controller.LoginUser(creds) as ObjectResult;

            var token = ((AuthResponse)result.Value).ResponseToken;

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[_config.authHeader] = $"Bearer {token}";

            var controllerContext = new ControllerContext() { HttpContext = httpContext };

            var userController = new UserManagementController(_dbCtx);

            userController.ControllerContext = controllerContext;

            var responseResult = userController.DeleteUser();

            var user = _dbCtx.UserAccounts.Where(x => x.AppUserName.Equals("John")).FirstOrDefault();

            var objResult = responseResult as ObjectResult;

            // The operation should be unauthorized and there shouldn't be any notable side effects to the database
            var outcome = objResult.StatusCode == 401 && user != null;

            Assert.True(outcome);
        }

        [Fact]
        public void Delete_User_As_SysAdmin()
        {
            var controller = new AuthController(_dbCtx);
            var creds = new LoginCredentials()
            {
                Username = "SysAdmin",
                Password = "AdminUser"
            };
            var result = controller.LoginUser(creds) as ObjectResult;

            var token = ((AuthResponse)result.Value).ResponseToken;

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[_config.authHeader] = $"Bearer {token}";

            var controllerContext = new ControllerContext() { HttpContext = httpContext };

            var userController = new UserManagementController(_dbCtx);

            userController.ControllerContext = controllerContext;

            var userToDelete = _dbCtx.UserAccounts.Where(x => x.AppUserName.Equals("Nathan")).FirstOrDefault();

            userController.DeleteUserById(userToDelete.Id);

            var deletedUser = _dbCtx.UserAccounts.Where(x => x.Id == userToDelete.Id).FirstOrDefault();

            var outcome = deletedUser == null;

            Assert.True(outcome);
        }

        [Fact]
        public void Delete_User_As_SysAdmin_But_Invalid_Creds()
        {
            var controller = new AuthController(_dbCtx);
            var creds = new LoginCredentials()
            {
                Username = "Nolen",
                Password = "ice_queen"
            };
            var result = controller.LoginUser(creds) as ObjectResult;

            var token = ((AuthResponse)result.Value).ResponseToken;

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[_config.authHeader] = $"Bearer {token}";

            var controllerContext = new ControllerContext() { HttpContext = httpContext };

            var userController = new UserManagementController(_dbCtx);

            userController.ControllerContext = controllerContext;

            var userToDelete = _dbCtx.UserAccounts.Where(x => x.AppUserName.Equals("Nolen")).FirstOrDefault();

            userController.DeleteUserById(userToDelete.Id);

            var deletedUser = _dbCtx.UserAccounts.Where(x => x.Id == userToDelete.Id).FirstOrDefault();

            var outcome = deletedUser != null;

            Assert.True(outcome);
        }


        public void Dispose()
        {
            foreach(var user in _accounts)
            {
                var account = _dbCtx.UserAccounts.Where(x => x.AppUserName.Equals(user.AppUserName)).FirstOrDefault();
                if (account != null)
                {
                    _dbCtx.UserAccounts.Remove(account);
                }
                _dbCtx.SaveChanges();
            }
        }
    }
}
