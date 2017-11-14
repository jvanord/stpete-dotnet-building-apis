using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace ApiDemo
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// Define Swagger Document(s) for Help Files
			services.AddSwaggerGen(setup => setup.SwaggerDoc("v2017.1", new Info
			{
				Title = "Demo API",
				Version = "2017.1",
				Description = "A Practical Demonstration of Web API in ASP.NET Core",
				Contact = new Contact { Name = "Jay VanOrd", Email = "jvanord@indasysllc.com", Url = "http://www.indasysllc.com" }
			}));

			// RespectBrowserAcceptHeader and AddXmlSerializerFormatters are required for content negotiotiation
			services.AddMvc(options => options.RespectBrowserAcceptHeader = true)
				.AddXmlSerializerFormatters();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			// Enable Swagger-Generated Help Files
			// NOTE: the version part of the path matches the document name registered above.
			app.UseSwagger();
			app.UseSwaggerUI(setup => setup.SwaggerEndpoint("/swagger/v2017.1/swagger.json", "Demo API ver. 2017.1"));

			// UseMvc() if you don't want to register default routes.
			app.UseMvcWithDefaultRoute();

			// If you want to use static files, e.g. index.html
			app.UseStaticFiles();
		}
	}
}
