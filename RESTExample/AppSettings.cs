using System;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace RESTExample
{
	// AppSettings is a class that will read the AppSettings.json file and then give access
	// to the settings for things like Username, Password, BaseURI, SiteID
	public class AppSettings
	{
		public string Username { get; set; }
		public string Password { get; set; }
		public string BaseURI { get; set; }
		public string SiteID { get; set; }
		public static AppSettings ReadFromJsonFile(string path)
		{
			IConfigurationRoot Configuration;

			var builder = new ConfigurationBuilder()
			 .SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile(path);

			Configuration = builder.Build();
			return Configuration.Get<AppSettings>();
		}
    }
}