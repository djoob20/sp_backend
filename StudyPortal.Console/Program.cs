using StudyPortalAPIClient;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var httpClient = new HttpClient();

        var client = new swaggerClient("https://localhost:7229/", httpClient);
        //get all courses
        var results = await client.Get2Async();
        foreach (var item in results)
        {
            Console.WriteLine(item.Title);
        }
    }
}