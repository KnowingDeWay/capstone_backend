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
    }
}
