using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityOAuth2.Model;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace IdentityOAuth2.Server
{
	public class ServerConfig
	{
		public static IEnumerable<ApiResource> GetApiResources()
		{
			return new List<ApiResource>
				{
					 new ApiResource("ileu_auth_api", "Unipark.Security.IdentityOAuth2.Api")
				};
		}

		public static IEnumerable<IdentityResource> GetIdentityResources()
		{
			return new List<IdentityResource>
				{
					 new IdentityResources.OpenId(),
					 new IdentityResources.Profile(),
				};
		}


		// Возвращает список клиентских приложений с которыми работает IS4
		public static IEnumerable<Client> GetClients()
		{
			return new List<Client>
				{
                // Hybrid Flow = OpenId Connect + OAuth
                // To use both Identity and Access Tokens
                new Client
					 {
						  ClientId = "ileu_auth_client",
						  ClientName = "Ileu.Security.IdentityOAuth2.Client",
						 
						
						  ClientSecrets = { new IdentityServer4.Models.Secret("Ileu_secret_key_91B40E82-507E-4641-B00A-33A194116F1C".Sha256()) },

						  AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
						  AllowOfflineAccess = true,
						  RequireConsent = false,

						  RedirectUris = { "http://localhost:5002/signin-oidc" },
						  PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },

						  AllowedScopes =
						  {
								IdentityServerConstants.StandardScopes.OpenId,
								IdentityServerConstants.StandardScopes.Profile,
								"ileu_auth_api"
						  },
					 },
                // Resource Owner Password Flow
                new Client
					 {
						  ClientId = "ileu_auth_client_ro",
						  ClientName = "Ileu.Security.IdentityOAuth2.Client.RO",
						  ClientSecrets = { new IdentityServer4.Models.Secret("secret".Sha256()) },

						  AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

						  AllowedScopes =
						  {
								"ileu_auth_api"
						  },
					 }
				};
		}

		public static List<TestUser> GetUsers()
		{
			return new List<TestUser>
				{
					 new TestUser
					 {
						  SubjectId = "1",
						  Username = "james",
						  Password = "password",
						  Claims = new List<Claim>
						  {
								new Claim("name", "James Bond"),
								new Claim("website", "https://james.com")
						  }
					 },
					 new TestUser
					 {
						  SubjectId = "2",
						  Username = "spectre",
						  Password = "password",
						  Claims = new List<Claim>
						  {
								new Claim("name", "Spectre"),
								new Claim("website", "https://spectre.com")
						  }
					 }
				};
		}

	}
}
