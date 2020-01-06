using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace MyConsoleApp
{
	class Program
	{
		static void Main(string[] args)
		{
			Program myProgram = new Program();
			myProgram.RunIt(args);
		}

		private void RunIt(string[] args)
		{
			var returnValue = CustomerService.getScreenByCustID(CustomerHeader, "C300");
			if (!string.IsNullOrEmpty(returnValue.errorMessage)) { throw new Exception(returnValue.errorMessage); }
			if (string.IsNullOrEmpty(returnValue.myCustomer.CustId)) { throw new Exception("No Customer returned for CustID = C300"); }
			Console.WriteLine("Customer {0}: {1}", returnValue.myCustomer.CustId.Trim(), returnValue.myCustomer.Name.Trim());
			Console.WriteLine("Current Balance: {0:C}", returnValue.myBalances.CurrBal);
		}

		private ctDynamicsSL.CustomerMaintenance.ctDynamicsSLHeader _customerHeader = null;
		private ctDynamicsSL.CustomerMaintenance.ctDynamicsSLHeader CustomerHeader
		{
			get
			{
				_customerHeader = new ctDynamicsSL.CustomerMaintenance.ctDynamicsSLHeader
				{
					cpnyID = Settings.CpnyID,
					siteID = Settings.SiteID,
					siteKey = Settings.SiteKey,
					licenseExpiration = Settings.LicenseExpire,
					licenseKey = Settings.LicenseKey,
					licenseName = Settings.LicenseName,
					softwareName = Settings.SoftwareName
				};

				return _customerHeader;
			}
		}

		private ctDynamicsSL.CustomerMaintenance.customerMaintenanceSoapClient _customerService = null;
		public ctDynamicsSL.CustomerMaintenance.customerMaintenanceSoapClient CustomerService
		{
			get
			{
				if (_customerService == null)
				{
					var outBinding = new System.ServiceModel.BasicHttpBinding();
					System.ServiceModel.EndpointAddress serviceAddress = new System.ServiceModel.EndpointAddress(Settings.CustomerServiceEndpoint);

					_customerService = new ctDynamicsSL.CustomerMaintenance.customerMaintenanceSoapClient(outBinding, serviceAddress);
				}
				return _customerService;
			}
			set
			{
				_customerService = value;
			}
		}

		private SettingsConfig _settings = null;
		private SettingsConfig Settings
		{
			get
			{
				if (_settings == null)
				{
					var path = Path.Combine(Directory.GetCurrentDirectory(), "AppSettings.json");
					_settings = new SettingsConfig();
					IConfiguration config = new ConfigurationBuilder()
					.AddJsonFile(path, optional: true, reloadOnChange: true)
					.Build();

					config.GetSection("AppSettings").Bind(_settings);
				}
				return _settings;
			}
		}
	}
}
