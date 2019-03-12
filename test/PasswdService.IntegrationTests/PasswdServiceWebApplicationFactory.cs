using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace PasswdService
{
    public sealed class PasswdServiceWebApplicationFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder) =>
            builder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                var configPath = Path.Combine(AppContext.BaseDirectory, "Configuration");
                config.AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "PasswdService:GroupFilePath", configPath },
                    { "PasswdService:PasswordFilePath", configPath },
                });
            });
    }
}
