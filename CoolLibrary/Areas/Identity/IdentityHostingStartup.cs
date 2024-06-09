using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(CoolLibrary.Areas.Identity.IdentityHostingStartup))]
namespace CoolLibrary.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}