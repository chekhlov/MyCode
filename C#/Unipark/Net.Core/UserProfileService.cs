using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityOAuth2.Model;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Test;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdentityOAuth2.Server
{
	public class UserProfileService : IProfileService
	{
		protected readonly ILogger Logger;

		/// <summary>
		/// The users
		/// </summary>
		protected readonly ApplicationDbContext Context;

		/// <summary>
		/// Initializes a new instance of the <see cref="TestUserProfileService"/> class.
		/// </summary>
		/// <param name="users">The users.</param>
		/// <param name="logger">The logger.</param>
		public UserProfileService(ApplicationDbContext context, ILogger<TestUserProfileService> logger)
		{
			Context = context;
			Logger = logger;

			if (Context.Users.Any())
				return;

			var admin = new User()
			{
				Id = Guid.Parse("00000000-1111-0000-1111-000000000000"),
				UserName = "admin",
				NormalizedUserName = "Администратор",
				Email = "admin@ileu.biz",
				PasswordHash = "admin12345".Sha512()
			};

			admin.UserRoles.Add(Role.defRoles[(int) Role.DefRolesType.superuser]);

			if (!Context.Roles.Any())
			{
				foreach (var role in Role.defRoles)
					Context.Roles.Add(role);

				Context.SaveChanges();
			}

		}

		/// <summary>
		/// This method is called whenever claims about the user are requested (e.g. during token creation or via the userinfo endpoint)
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		public virtual Task GetProfileDataAsync(ProfileDataRequestContext context)
		{
//			context.LogProfileRequest(Logger);

			if (context.RequestedClaimTypes.Any())
			{
				var user = Context.Users
					.Include(x => x.Claims)
					.FirstOrDefault(x => x.Id.ToString() == context.Subject.GetSubjectId());
 
				if (user != null)
					context.AddRequestedClaims(user.Claims.Select(x => x.ToClaim()).AsEnumerable());
			}

//			context.LogIssuedClaims(Logger);

			return Task.CompletedTask;
		}

		/// <summary>
		/// This method gets called whenever identity server needs to determine if the user is valid or active (e.g. if the user's account has been deactivated since they logged in).
		/// (e.g. during token issuance or validation).
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		public virtual Task IsActiveAsync(IsActiveContext context)
		{
			//			Logger.LogDebug("IsActive called from: {caller}", context.Caller);
			var user = Context.Users
				.FirstOrDefault(x => x.Id.ToString() == context.Subject.GetSubjectId());

			context.IsActive = user?.Enabled == true;

			return Task.CompletedTask;
		}
	}
}
