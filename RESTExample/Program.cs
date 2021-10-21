using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json.Linq;


namespace RESTExample
{
    class Program
    {
        static void Main(string[] args)
        {
			// lets create an instance of the application
            Program myProgram = new Program();

			// For CustID = C300, lets change the name, addr1, and addr2 of the customer
			myProgram.RunIt("C300", "Ted Danson", "Test Addr1", "TestAddr2");
        }

        public void RunIt(string custID, string name, string addr1, string addr2)
        {

			// NOTE: All the configuration info (username, password, etc.) is coming from the AppSettings.json file

			var httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Authorization = APIAuthHeader;
			httpClient.DefaultRequestHeaders.Add("SiteID", Config.SiteID);

			// we are going to use Newtonsoft to create an object very simply.
			// we are just going to update the Name, Addr1, and Addr2 of the customer
			var customerObject = new JObject(
				new JProperty("myCustomer",
					new JObject(
						new JProperty("Name", $"{name}"),
						new JProperty("Addr1", $"{addr1}"),
						new JProperty("Addr2", $"{addr2}")
						)
			));

			// build the uri.  the base of the URI resides in the AppSettings.json.  But since we are going to update
			// the customer, we will then look at the financial/accountsReceivable/customer endpoint
			// also, since we are doing an update (PATCH), we will pass the custID as well to note which customer we want to update.
			string uri = $"{Config.BaseURI}financial/accountsReceivable/customer/{custID}";

			// lets convert the customerObject JObject to a string
			var data = new StringContent(customerObject.ToString(), Encoding.UTF8, "application/json");

			// lets create a response for a PATCH to the endpoint and pass it the data
			var response = httpClient.PatchAsync(uri, data).Result;

			// now that the call has been made, we can then get what was passed back and write it to the console
			var content = JObject.Parse(response.Content.ReadAsStringAsync().Result);
			Console.WriteLine($"Results Returned:\n{content}");
        }

		// This will return the Authentication header value
        private AuthenticationHeaderValue APIAuthHeader
		{
			get
			{
				return new AuthenticationHeaderValue("Basic", BasicAuthString);
			}
		}

		// Catalina's REST API uses basic authentication.  This creates the Authentication String
		private string _basicAuthString = "";
		public string BasicAuthString
		{
			get
			{
				if (string.IsNullOrEmpty(_basicAuthString))
				{
					var bytes = Encoding.UTF8.GetBytes($"{Config.Username}:{Config.Password}");
					_basicAuthString = Convert.ToBase64String(bytes);
				}
				return _basicAuthString;
			}
		}

		// this allows us to read from the AppSettings.json file to get the settings so that we dont have to hardcode
		// things like username, password, URI, etc.
		private AppSettings _config = null;
		private AppSettings Config
		{
			get
			{
				if(_config == null)
				{
					_config = AppSettings.ReadFromJsonFile("appsettings.json");
				}
				return _config;
			}
		}
    }
}
