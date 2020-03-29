using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BrickBundle.WebAPI
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
            services.AddSingleton<IConfiguration>(Configuration);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = Configuration.GetValue<string>("JwtIssuer"),
                            ValidAudience = Configuration.GetValue<string>("JwtAudience"),
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetValue<string>("JwtSigningKey")))
                        };
                    });

            services.AddCors(options =>
            options.AddPolicy("BrickBundlePolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            services.AddSwaggerDocument(settings =>
            {
                settings.PostProcess = document =>
                {
                    document.Info.Title = "BrickBundle API";
                };
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseOpenApi();
                app.UseSwaggerUi3();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseAuthentication();
            app.UseCors("BrickBundlePolicy");
            app.UseHttpsRedirection();
            app.UseMvc();

            if (env.IsDevelopment())
            {
                Model.BrickBundleContext.ConnectionString = Configuration.GetConnectionString("Sqlite");
                Model.BrickBundleContext.IsSqlite = true;
                Model.BrickBundleContext.EnsureDatabaseCreated();
            }
            else
            {
                Model.BrickBundleContext.ConnectionString = Configuration.GetConnectionString("SqlServer");
                Model.BrickBundleContext.IsSqlite = false;
            }

            Env.JwtIssuer = Configuration.GetValue<string>("JwtIssuer");
            Env.JwtAudience = Configuration.GetValue<string>("JwtAudience");
            Env.JwtSigningKey = Configuration.GetValue<string>("JwtSigningKey");
            
            try
            {
                ML.ObjectDetection.CreateInstance();
            }
            catch (System.Exception ex)
            {
                var log = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "errorlog.txt");
                System.IO.File.WriteAllText(log, ex.Message + System.Environment.NewLine + ex.StackTrace);
            }
        }
    }
}
