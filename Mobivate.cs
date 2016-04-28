using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Xml.Linq;

namespace testcsharp
{
	class MainClass
	{
		public static void Main (string[] args) {
			testSend ();
		}

		private static void testSend()
		{
			// SMS Data
			string username = "USER:PASS"; // **** CHANGE THIS ****
			string routeID = "3d7c4a51-c68b-4ee4-b83b-822bf7eb293f"; 
			string orig = "test";
			string rcpt = "61417188345";
			string body = "Test Message < > # ? ! ? % // http://test.com £ ^&*()";

			// Make XML message
			XElement xmlMsg = new XElement ("message");
			xmlMsg.Add (new XElement ("originator", orig));
			xmlMsg.Add (new XElement ("recipient", rcpt));
			xmlMsg.Add (new XElement ("body", body));
			xmlMsg.Add (new XElement ("routeId", routeID));
			// Get XML String
			String data = xmlMsg.ToString ();

			Console.WriteLine ("Sending " + data);	

			// Post
			string url = "http://app.mobivatebulksms.com/bulksms/xmlapi/" + username.Trim () + "/send/sms/single";
			string responseFromServer = PostAsyncNew (url, data).Result;

			Console.WriteLine (responseFromServer);		
		}

		private static async Task<string> PostAsyncNew (string uri, string data)
		{
			var httpClient = new HttpClient ();
			httpClient.DefaultRequestHeaders.Add ("User-Agent", "foobar2016");

			byte[] bytes = Encoding.Default.GetBytes (data);
			data = Encoding.GetEncoding("iso-8859-1").GetString(bytes);

			// Form Encode XML value
			var keyValues = new List<KeyValuePair<string, string>> ();
			keyValues.Add (new KeyValuePair<string, string> ("xml", data));

			// Make sure data is sent as UTF-8
			HttpContent content = new FormUrlEncodedContent (keyValues);
			content.Headers.ContentType.CharSet = "UTF-8";

			var request = new HttpRequestMessage {
				RequestUri = new Uri (uri),
				Method = HttpMethod.Post,			
				Content = content         
			};

			request.Headers.Accept.Add (new MediaTypeWithQualityHeaderValue (@"*/*"));

			var response = await httpClient.SendAsync (request);

			string responseTxt = await response.Content.ReadAsStringAsync ();
			return responseTxt;
		}

	}
}
