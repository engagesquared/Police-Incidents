// <copyright file="BaseController.cs" company="Engage Squared">
// Copyright (c) Engage Squared. All rights reserved.
// </copyright>

namespace PoliceIncidents.Controllers
{
    using System;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Base controller to handle token generation.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        /// <summary>
        /// Gets user's Azure AD object id.
        /// </summary>
        public string UserObjectId
        {
            get
            {
                var oidClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
                var claim = this.User.Claims.First(p => oidClaimType.Equals(p.Type, StringComparison.Ordinal));
                return claim.Value;
            }
        }
    }
}
