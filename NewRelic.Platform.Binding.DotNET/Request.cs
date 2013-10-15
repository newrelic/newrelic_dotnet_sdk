using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

using Newtonsoft.Json;

namespace NewRelic.Platform.Binding.DotNET
{
	public class Request
	{
		public const string PROD_SERVICE_URI = @"https://platform-api.newrelic.com/platform/v1/metrics";
		public const string SERVICE_URI = @"http://platform-api.staging.newrelic.com/platform/v1/metrics";
		private readonly string _licenseKey;

		public Request(string licenseKey)
		{
			_licenseKey = licenseKey;
			ServicePointManager.ServerCertificateValidationCallback = AcceptAllCertifications;
		}

		public PlatformData Data { get; set; }

		public static string SerializeData(PlatformData data)
		{
			return JsonConvert.SerializeObject(data);
		}

		protected string Send()
		{
			var serializedData = SerializeData(Data);

			var request = (HttpWebRequest) WebRequest.Create(PROD_SERVICE_URI);
			request.Timeout = 20000;
			request.Method = "POST";
			request.Headers.Add("X-License-Key", _licenseKey);
			request.ContentType = "application/json";
			request.Accept = "application/json";

			var utf8 = new UTF8Encoding();
			var bytes = utf8.GetBytes(serializedData);

			request.ContentLength = bytes.Length;

			using (var outputStream = request.GetRequestStream())
			{
				outputStream.Write(bytes, 0, (int) request.ContentLength);
				outputStream.Close();
			}

			using (var response = (HttpWebResponse) request.GetResponse())
			{
				return response.StatusCode.ToString();
			}
		}

		private static bool AcceptAllCertifications(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
		{
			return true;
		}
	}
}