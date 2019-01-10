using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IdentityOAuth2.Model;
using IdentityOAuth2.Server;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IdentityOAuth2.Server
{

	// Любой сервис реализующий OpenID должен реализовывать выдачу конфигурации в формате JSON /.well-known/openid-configuration
	// Поэтому протестировать работу IdentityServer'а можно обратиться по адресу https://localhost:ХХХХХ/.well-known/openid-configuration
	public class ServerStartup
	{
		public IConfiguration Configuration { get; }
		public string ConnectionString { get; }

		public ServerStartup(IConfiguration configuration)
		{
			Configuration = configuration;
			ConnectionString = Configuration.GetConnectionString("DefaultConnection");
		}

		/*
		private void InitializeDatabase(IApplicationBuilder app)
		{
			using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
			{
				serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

				var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
				context.Database.Migrate();
				if (!context.Clients.Any())
				{
					foreach (var client in ServerConfig.GetClients())
					{
						context.Clients.Add(client.ToEntity());
					}
					context.SaveChanges();
				}

				if (!context.IdentityResources.Any())
				{
					foreach (var resource in ServerConfig.GetIdentityResources())
					{
						context.IdentityResources.Add(resource.ToEntity());
					}
					context.SaveChanges();
				}

				if (!context.ApiResources.Any())
				{
					foreach (var resource in ServerConfig.GetApiResources())
					{
						context.ApiResources.Add(resource.ToEntity());
					}
					context.SaveChanges();
				}
			}
		}
		*/
		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<CookiePolicyOptions>(options =>
			{
				// This lambda determines whether user consent for non-essential cookies is needed for a given request.
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

			services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(ConnectionString));
			services.AddIdentity<User, Role>().AddEntityFrameworkStores<ApplicationDbContext>();

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

			var migrationsAssembly = typeof(ServerStartup).GetTypeInfo().Assembly.GetName().Name;

			services.AddIdentityServer()
				.AddAspNetIdentity<User>()
				.AddDeveloperSigningCredential(filename: "tempkey.rsa")
				.AddOauth2Resources()
				.AddOauth2PersistedGrants()
				.AddOauth2Clients()
				.AddOauth2Users();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}

			//		InitializeDatabase(app);

			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseCookiePolicy();
			app.UseAuthentication();
			app.UseIdentityServer();

//			app.UseMvcWithDefaultRoute();

			app.UseMvc(routes => routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}"));
		}
	}
}
