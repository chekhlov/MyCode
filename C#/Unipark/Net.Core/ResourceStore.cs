using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityOAuth2.Model;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace IdentityOAuth2.Server
{
	public class ResourceStore : IResourceStore
	{
		private readonly ApplicationDbContext  Context;

		private readonly IEnumerable<IdentityResource> _identityResources;
		private readonly IEnumerable<ApiResource> _apiResources;

		/// <summary>
		/// Initializes a new instance of the <see cref="InMemoryResourcesStore" /> class.
		/// </summary>
		public ResourceStore(ApplicationDbContext context)
		{
			Context = context;

			_apiResources =  new List<ApiResource>
			{
				new ApiResource("ileu_auth_api", "Unipark.Security.IdentityOAuth2.Api"),
				new ApiResource("ileu_admin_module", "Unipark.Admin.Module"),
				new ApiResource("ileu_user_module", "Unipark.User.Module")
			};

			_identityResources = new List<IdentityResource>
			{
				new IdentityResources.OpenId(),
				new IdentityResources.Profile(),
			};
		}

		/// <summary>
		/// Gets all resources.
		/// </summary>
		/// <returns></returns>
		public Task<Resources> GetAllResourcesAsync()
		{
			var result = new Resources(_identityResources, _apiResources);
			return Task.FromResult(result);
		}

		/// <summary>
		/// Finds the API resource by name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public Task<ApiResource> FindApiResourceAsync(string name)
		{
			return Task.FromResult(_apiResources.FirstOrDefault(x => x.Name == name));

		}

		/// <summary>
		/// Finds the identity resources by scope.
		/// </summary>
		/// <param name="names">The names.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentNullException">names</exception>
		public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> names)
		{
			if (names == null) throw new ArgumentNullException(nameof(names));

			return Task.FromResult(_identityResources.Where(x => names.Contains(x.Name)));
		}

		/// <summary>
		/// Finds the API resources by scope.
		/// </summary>
		/// <param name="names">The names.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentNullException">names</exception>
		public Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> names)
		{
			return Task.FromResult(_apiResources.Where(x => x.Scopes.Any(s => names.Contains(s.Name))));
		}
	}
}
