using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityOAuth2.Model;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.EntityFrameworkCore;
using static IdentityServer4.IdentityServerConstants;
using static IdentityServer4.Models.IdentityResources;
using Scope = IdentityOAuth2.Model.Scope;

namespace IdentityOAuth2.Server
{
	// Возвращает список клиентских приложений с которыми работает IS4
	public class ClientStore : IClientStore
	{
		private readonly ApplicationDbContext Context;

		/// <param name="context">Контекст БД EF6.</param>
		public ClientStore(ApplicationDbContext context)
		{
			Context = context;

			if (Context.Applications.Any())
				return;

			// Создаем значения по умолчанию
			var application = new Application()
			{
				Id = Guid.Parse("91B40E82-507E-4641-B00A-33A194116F1C"),
				Enabled = true,
				Name = "Ileu.Security.IdentityOAuth2.Client",
				DisplayName = "Ileu.Security.IdentityOAuth2.Client",
				Description = "Приложение Ileu v.2"
			};

			application.SecretKeys.Add(new SecretKey(application, "Ileu_secret_key_1234567890".Sha256()));

			application.Scopes = new List<Scope>()
			{
				new Scope(application, IdentityServerConstants.StandardScopes.OpenId),
				new Scope(application, IdentityServerConstants.StandardScopes.Profile),
				new Scope(application, "ileu_auth_api")
			};

			Context.Applications.Add(application);
			Context.SaveChanges();
		}

		// реализация метода IClientStore получающий клиента по Id
		public Task<Client> FindClientByIdAsync(string clientId)
		{
			var app = Context.Applications
				.Include(x => x.SecretKeys)
				.Include(x => x.Scopes)
				.FirstOrDefault(x => x.Id.ToString() == clientId);

			if (app == null)
				throw new Exception($"Зарегистрированное приложение с Id = '{clientId}' не найдено");

			var appScopes = app.Scopes.Select(x => x.Name).ToList();
			if (!appScopes.Any()) appScopes = new List<string>() { StandardScopes.OpenId };

			var client = new Client()
			{
				ClientId = app.Id.ToString(),
				Enabled = app.Enabled,
				ClientName = app.Name,
				ClientSecrets = app.SecretKeys.Select(x => new Secret(x.Value, x.Description, x.Expiration)).ToList(),

				AllowedGrantTypes = app.AllowedGrantTypes.Split(' '),
				AllowedScopes = appScopes
			};

			return Task.FromResult(client);
		}
	}

}
