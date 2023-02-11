namespace FetcherConsole;

public static class HttpUtil
{
    public static async Task<string> FetchAsync(string url)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "MyUserAgent");
        var response = await client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Error Occoured while fetching data from: {url}");
        }

        return await response.Content.ReadAsStringAsync();

    }
}