using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NLog.Web;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetsChess_MatchmakingService
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
			try
			{
				logger.Debug($"starting application '{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}'");
				CreateHostBuilder(args).Build().Run();
			}
			catch (Exception exception)
			{
				logger.Error(exception, $"Stopped '{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}' because of an exception");
				throw;
			}
			finally
			{
				NLog.LogManager.Shutdown();
			}
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
			.ConfigureAppConfiguration((hostingContext, config) =>
			{
				var env = hostingContext.HostingEnvironment;
				Console.WriteLine($"the environment is now: {env.EnvironmentName}");

				//TODO: hij pakt deze niet goed in kubernetes?
				config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json",
								optional: true, reloadOnChange: true);
			})
			.ConfigureWebHostDefaults(webBuilder =>
			{
				webBuilder.UseNLog().UseStartup<Startup>();
			}).ConfigureLogging(logging =>
			{
				logging.ClearProviders();
				logging.SetMinimumLevel(LogLevel.Trace);
			});
	}
}
