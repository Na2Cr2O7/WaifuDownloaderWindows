using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace WaifuDownloader
{
    // 图片信息模型
    public class ImageInfo
    {
        public string Signature { get; set; }
        public string Extension { get; set; }
        public int ImageId { get; set; }
        public int Favorites { get; set; }
        public string DominantColor { get; set; }
        public string Source { get; set; }
        public Artist0 Artist { get; set; }
        public DateTime UploadedAt { get; set; }
        public object LikedAt { get; set; }
        public bool IsNsfw { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int ByteSize { get; set; }
        public string Url { get; set; }
        public string PreviewUrl { get; set; }
        public List<Tag> Tags { get; set; }
    }

    // 标签模型
    public class Tag
    {
        public int TagId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsNsfw { get; set; }
    }
    public class Artist0
    {
        public string? ArtistId {  get; set; }
        public string? Name { get; set; }
        public string? Patreon {  get; set; }
        public string? Pixiv {  get; set; }
        public string? twitter {  get; set; }
        public string? deviant_art { get; set; }
//{"artist_id":518,"name":"冷織","patreon":null,"pixiv":"https://www.pixiv.net/users/67539403","twitter":"https://twitter.com/hiyahiyaval","deviant_art":null}
}

    // API响应模型
    public class ApiResponse
    {
        public List<ImageInfo> Images { get; set; }
    }

    public class WaifuDownloaderAPI
    {
        private readonly HttpClient _httpClient;
        private readonly string _endpoint = "https://api.waifu.im/search?is_nsfw=";
        public ApiResponse Info { get; private set; }

        public WaifuDownloaderAPI()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task<string?> GetPageAsync(bool nsfw = false)
        {
            try
            {
                var url = $"{_endpoint}{nsfw.ToString().ToLower()}";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public string? GetPageUrl(string? response)
        {
            if (string.IsNullOrEmpty(response))
                return null;

            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                Info = JsonSerializer.Deserialize<ApiResponse>(response, options)!;

                if (Info?.Images != null && Info.Images.Count > 0)
                {
                    return Info.Images[0].Url;
                }

                return null;
            }
            catch (JsonException ex)
            {
                //Console.WriteLine($"JSON解析错误: {ex.Message}");
                return null;
            }
        }

        public async Task<string?> GetAImageURLAsync(bool nsfw = false)
        {
            var pageContent = await GetPageAsync(nsfw);
            return GetPageUrl(pageContent);
        }

        public string? GetTheArtist()
        {
            
            if (Info?.Images != null && Info.Images.Count > 0)
            {
                if(Info.Images[0].Artist is null)
                {
                    return null;
                }
                return Info.Images[0].Artist.Name;
            }
            return null;
        }
        public string GetTheExtension()
        {
            return Info.Images[0].Extension ?? ".jpeg";
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }

    // 示例用法
    //class Program
    //{
    //    static async Task Main(string[] args)
    //    {
    //        using var ct = new WaifuDownloaderAPI();
    //        var imageUrl = await ct.GetAImageURLAsync();
    //        Console.WriteLine(imageUrl);
    //    }
    //}

    public class WaifuDownloader2API
    {

        public WaifuDownloader2API()
        {

        }


        public async Task<string?> GetAImageURLAsync(bool nsfw = false)
        {

            return "https://t.alcy.cc/mp";

        }

        public string? GetTheArtist()
        {
            return null;
        }
    }
}