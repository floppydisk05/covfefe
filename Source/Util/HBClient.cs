using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class HasteBinClient {
    private static readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    static HasteBinClient() {
        _httpClient = new HttpClient();
    }

    public HasteBinClient(string baseUrl) {
        _baseUrl = baseUrl;
    }

    public async Task<HasteBinResult> Post(string content) {
        var fullUrl = _baseUrl;
        if (!fullUrl.EndsWith("/")) fullUrl += "/";
        var postUrl = $"{fullUrl}documents";

        var request = new HttpRequestMessage(HttpMethod.Post, new Uri(postUrl));
        request.Content = new StringContent(content);
        var result = await _httpClient.SendAsync(request);

        if (result.IsSuccessStatusCode) {
            var json = await result.Content.ReadAsStringAsync();
            var hasteBinResult = JsonConvert.DeserializeObject<HasteBinResult>(json);

            if (hasteBinResult?.Key != null) {
                hasteBinResult.FullUrl = $"{fullUrl}{hasteBinResult.Key}";
                hasteBinResult.IsSuccess = true;
                hasteBinResult.StatusCode = 200;
                return hasteBinResult;
            }
        }

        return new HasteBinResult {
            FullUrl = fullUrl,
            IsSuccess = false,
            StatusCode = (int)result.StatusCode
        };
    }
}

// Define other methods and classes here
public class HasteBinResult {
    public string Key { get; set; }
    public string FullUrl { get; set; }
    public bool IsSuccess { get; set; }
    public int StatusCode { get; set; }
}
