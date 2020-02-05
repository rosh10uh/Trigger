using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Trigger
{
    /// <summary>
    /// Main Class for Project
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Constructor for Main
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Set Startup
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>();
    }
}
