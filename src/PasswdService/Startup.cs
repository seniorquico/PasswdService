using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using PasswdService.Options;
using PasswdService.Services;
using Swashbuckle.AspNetCore.Swagger;

namespace PasswdService
{
    /// <summary>Configures a web application to host the Passwd Service API server.</summary>
    public sealed class Startup
    {
        /// <summary>The application configuration.</summary>
        private readonly IConfiguration configuration;

        /// <summary>Initializes a new instance of the <see cref="Startup"/> class.</summary>
        /// <param name="configuration">The application configuration.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="configuration"/> is <c>null</c>.</exception>
        public Startup(IConfiguration configuration) =>
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        /// <summary>Called by the runtime to configure the web application.</summary>
        /// <param name="app">The web application builder.</param>
        /// <param name="env">The web hosting environment.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="app"/> is <c>null</c>.</exception>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (env == null)
            {
                throw new ArgumentNullException(nameof(env));
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHostFiltering();
            app.UseForwardedHeaders();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Passwd Service v1");
            });
            app.Use(HandleCustomExceptions);
            app.UseMvc();
        }

        /// <summary>Called by the runtime to configure the dependency injection container.</summary>
        /// <param name="services">The dependency injection container.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="services"/> is <c>null</c>.</exception>
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ServiceOptions>(this.configuration.GetSection("PasswdService"));

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options =>
                {
                    // Make documents a bit prettier.
                    options.SerializerSettings.Converters.Add(new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy() });
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(
                    "v1",
                    new Info
                    {
                        Contact = new Contact
                        {
                            Email = "kyledodson@gmail.com",
                            Name = "Kyle Dodson",
                        },
                        Description = "An example API",
                        License = new License
                        {
                            Name = "MIT License",
                            Url = "https://opensource.org/licenses/MIT",
                        },
                        Title = "Passwd Service",
                        Version = "v1",
                    });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

                options.DescribeAllEnumsAsStrings();
                options.DescribeStringEnumsInCamelCase();
            });

            // Add background service to watch and parse `/etc/group` file.
            services.AddSingleton<IGroupFileProvider, GroupFileProvider>();
            services.AddHostedService<GroupFileWatcherService>();

            // Add background service to watch and parse `/etc/passwd` file.
            services.AddSingleton<IPasswordFileProvider, PasswordFileProvider>();
            services.AddHostedService<PasswordFileWatcherService>();

            // Add store to share the parsed `/etc/group` and `/etc/passwd` files with other application components (e.g.
            // controllers).
            services.AddSingleton<IStore, Store>();
            services.AddSingleton<IGroupStore>(s => s.GetService<IStore>());
            services.AddSingleton<IUserStore>(s => s.GetService<IStore>());
        }

        private static async Task HandleCustomExceptions(HttpContext context, Func<Task> next)
        {
            try
            {
                await next.Invoke();
            }
            catch (StoreException)
            {
                // We can't do anything if the response has already started.
                if (context.Response.HasStarted)
                {
                    throw;
                }

                // Create a JSON serializer.
                var options = context.RequestServices.GetService<IOptions<MvcJsonOptions>>();
                var serializerSettings = options?.Value?.SerializerSettings;
                var serializer = serializerSettings != null
                    ? JsonSerializer.Create(serializerSettings)
                    : new JsonSerializer
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    };

                // Return JSON serialized `ProblemDetails`.
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.Headers[HeaderNames.CacheControl] = "no-cache";
                context.Response.Headers[HeaderNames.Pragma] = "no-cache";
                context.Response.Headers[HeaderNames.Expires] = "-1";
                context.Response.Headers.Remove(HeaderNames.ETag);
                using (var writer = new HttpResponseStreamWriter(context.Response.Body, Encoding.UTF8))
                using (var jsonWriter = new JsonTextWriter(writer))
                {
                    jsonWriter.CloseOutput = false;
                    jsonWriter.AutoCompleteOnClose = false;

                    serializer.Serialize(jsonWriter, new ProblemDetails
                    {
                        Instance = "https://www.kyledodson.com/store-error",
                        Status = 500,
                        Title = "The requested information is currently unavailable.",
                    });
                }
            }
        }
    }
}
