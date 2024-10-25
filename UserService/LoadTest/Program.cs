using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;

class Endpointspam
{
	private static readonly HttpClient client = new HttpClient();

	public static async Task Main(string[] args)
	{
		int numberOfRequests = 1000; // Number of requests to send
		//should be other url this endpoint is rate limited now
		string url = "http://localhost:7000/User/login"; 

		await SendRequestsConcurrently(url, numberOfRequests);
	}

	private static async Task SendRequestsConcurrently(string url, int numberOfRequests)
	{
		var tasks = new Task[numberOfRequests];

		for (int i = 0; i < numberOfRequests; i++)
		{
			tasks[i] = SendRequestAsync(url, i);
			Thread.Sleep(10); // Optional: Add delay between requests to simulate more realistic load
		}

		await Task.WhenAll(tasks);
		Console.WriteLine("All requests completed.");
	}

	private static async Task SendRequestAsync(string url, int requestNumber)
	{
		try
		{
			var jsonData = new { Username = "string", Password = "string" };
			var jsonDatas = JsonConvert.SerializeObject(jsonData);
			StringContent content = new StringContent(jsonDatas, System.Text.Encoding.UTF8, "application/json");
			HttpResponseMessage response = await client.PostAsync(url, content);
			string result = await response.Content.ReadAsStringAsync();
			Console.WriteLine($"Request {requestNumber}: {response.StatusCode}");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Request {requestNumber} failed: {ex.Message}");
		}
	}
}
