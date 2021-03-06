using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Piranha;
//using Piranha.AspNetCore.Identity.SQLite;
using Piranha.AspNetCore.Identity.Postgress;

namespace WebSite
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(config =>
            {
                config.ModelBinderProviders.Insert(0, new Piranha.Manager.Binders.AbstractModelBinderProvider());
            });
            services.AddPiranha();
            services.AddPiranhaApplication();
            services.AddPiranhaFileStorage();
            services.AddPiranhaImageSharp();

            var connection = "host=127.0.0.1;Database=savehaven;Username=postgres;Password=2conjoKids";
            services.AddPiranhaEF(options =>
                options.UseNpgsql(connection));
            services.AddPiranhaIdentityWithSeed<IdentityPostgressDb>(options =>
                options.UseNpgsql(connection));
            services.AddPiranhaManager();

            services.AddMemoryCache();
            services.AddPiranhaMemoryCache();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApi api)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            App.Init(api);

            // Configure cache level
            //App.CacheLevel = Piranha.Cache.CacheLevel.Full;
            App.CacheLevel = Piranha.Cache.CacheLevel.None;

            // Custom components
            App.Blocks.Register<Models.Blocks.SeparatorBlock>();

            // Build content types
            var pageTypeBuilder = new Piranha.AttributeBuilder.PageTypeBuilder(api)
                .AddType(typeof(Models.BlogArchive))
                .AddType(typeof(Models.StandardPage))
                .AddType(typeof(Models.TeaserPage))
                .AddType(typeof(Models.CustomerSecure))
                //.AddType(typeof(Models.PurchaseSecure))
                //.AddType(typeof(Models.VaultSecure))
                .Build()
                .DeleteOrphans();
            var postTypeBuilder = new Piranha.AttributeBuilder.PostTypeBuilder(api)
                .AddType(typeof(Models.BlogPost))
                .Build()
                .DeleteOrphans();

            // Register middleware
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UsePiranha();
            app.UsePiranhaManager();
            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "areaRoute",
                    template: "{area:exists}/{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });

                routes.MapRoute(
                    name: "default",
                    template: "{controller=home}/{action=index}/{id?}");
            });

            Seed.RunAsync(api).GetAwaiter().GetResult();
        }
    }
}
