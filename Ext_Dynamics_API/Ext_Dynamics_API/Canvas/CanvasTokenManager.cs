using Ext_Dynamics_API.DataAccess;
using Ext_Dynamics_API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Canvas
{
    public class CanvasTokenManager
    {
        private readonly ExtensibleDbContext _dbCtx;

        public CanvasTokenManager(ExtensibleDbContext dbCtx)
        {
            _dbCtx = dbCtx;
        }

        /// <summary>
        /// Updates the details of a Canvas PAT
        /// </summary>
        /// <param name="personalAccessToken">The PAT to update</param>
        /// <returns>Boolean: Whether or not the update operation was successful</returns>
        public bool UpdateCanvasPat(CanvasPersonalAccessToken personalAccessToken)
        {
            var token = _dbCtx.PersonalAccessTokens.Where(x => x.Id == personalAccessToken.Id).FirstOrDefault();

            if(token == null)
            {
                return false;
            }

            var oldToken = new CanvasPersonalAccessToken()
            {
                TokenName = token.TokenName,
                AccessToken = token.AccessToken
            };

            token.TokenName = personalAccessToken.TokenName;
            token.AccessToken = personalAccessToken.AccessToken;

            try
            {
                _dbCtx.SaveChanges();
                return true;
            }
            catch (DbUpdateConcurrencyException e)
            {
                foreach (var item in e.Entries)
                {
                    if (item.Entity is CanvasPersonalAccessToken)
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
                // Rollback changes to access token
                token.TokenName = oldToken.TokenName;
                token.AccessToken = oldToken.AccessToken;
                return false;
            }
        }

        /// <summary>
        /// Activates a specific PAT to be used with Canvas for a particular user
        /// </summary>
        /// <param name="patId">The id of the PAT</param>
        /// <param name="userId">The id of the user</param>
        /// <returns></returns>
        public bool ActivateToken(int patId, int userId)
        {
            var pat = _dbCtx.PersonalAccessTokens.Where(x => x.Id == patId).FirstOrDefault();

            if(pat == null)
            {
               return false;
            }

            pat.TokenActive = true;

            var lastActiveTokens = _dbCtx.PersonalAccessTokens.
                Where(x => x.AppUserId == userId && x.TokenActive && x.Id != patId).ToList();

            foreach(var lastToken in lastActiveTokens)
            {
                lastToken.TokenActive = false;
            }

            try
            {
               _dbCtx.SaveChanges();
                return true;
            }
            catch(DbUpdateConcurrencyException e)
            {
                foreach (var item in e.Entries)
                {
                    if (item.Entity is CanvasPersonalAccessToken)
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
        }

        /// <summary>
        /// Gets the active PAT for a user
        /// </summary>
        /// <param name="userId">The id of the user</param>
        /// <returns>String: The API Key of the PAT</returns>
        public string GetActiveAccessToken(int userId)
        {
            var activeToken = _dbCtx.PersonalAccessTokens.Where(x => x.AppUserId == userId && x.TokenActive).FirstOrDefault();

            if (activeToken != null)
            {
                return activeToken.AccessToken;
            }
            return null;
        }
    }
}
