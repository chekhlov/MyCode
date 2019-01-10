using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityOAuth2.Model;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Test;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdentityOAuth2.Server
{
	/// <summary>
	/// Расширение IdentityServer4
	/// </summary>
	public static class ServerBuilder
	{
		/// <summary>
		/// Adds clients from database.
		/// </summary>
		/// <param name="builder">The builder.</param>
		/// <param name="context">Database EF context.</param>
		/// <returns></returns>
		public static IIdentityServerBuilder AddOauth2Clients(this IIdentityServerBuilder builder)
		{
			builder.AddClientStore<ClientStore>();

			return builder;
		}


		/// <summary>
		/// Adds the in memory identity resources.
		/// </summary>
		/// <param name="builder">The builder.</param>
		/// <param name="identityResources">The identity resources.</param>
		/// <returns></returns>
		public static IIdentityServerBuilder AddOauth2Resources(this IIdentityServerBuilder builder)
		{
			builder.AddResourceStore<ResourceStore>();

			return builder;
		}


		/// <summary>
		/// Adds the in memory stores.
		/// </summary>
		/// <param name="builder">The builder.</param>
		/// <returns></returns>
		public static IIdentityServerBuilder AddOauth2PersistedGrants(this IIdentityServerBuilder builder)
		{
			builder.Services.TryAddSingleton<IPersistedGrantStore, InMemoryPersistedGrantStore>();

			return builder;
		}

		/// <summary>
		/// Adds test users.
		/// </summary>
		/// <param name="builder">The builder.</param>
		/// <param name="users">The users.</param>
		/// <returns></returns>
		public static IIdentityServerBuilder AddOauth2Users(this IIdentityServerBuilder builder)
		{
			builder.AddProfileService<UserProfileService>();
	//		builder.AddResourceOwnerValidator<TestUserResourceOwnerPasswordValidator>();

			return builder;
		}
	}
}