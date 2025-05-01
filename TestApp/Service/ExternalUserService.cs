using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;
using TestApp.Configuration;
using TestApp.DTO;
using TestApp.Interfaces;

namespace TestApp.Service
{
    public class ExternalUserService:IExternalUserService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _apiKey;
        private readonly IMemoryCache _cache;
        public ExternalUserService(HttpClient httpClient, IOptions<ApiConfiguration> config, IMemoryCache memoryCache)
        {
            _httpClient = httpClient;
            _baseUrl = config.Value.BaseUrl.TrimEnd('/');
            _apiKey = config.Value.ApiKey;
            _cache = memoryCache;

            if (!_httpClient.DefaultRequestHeaders.Contains("x-api-key"))
            {
                _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
            }
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            try
            {
                string cacheKey = $"User_{userId}";
                if (_cache.TryGetValue(cacheKey, out User cachedUser))
                    return cachedUser;

                var response = await _httpClient.GetAsync($"{_baseUrl}/users/{userId}");
                if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new UserNotFoundException(userId);

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<SingleUserResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (result?.Data != null)
                {
                    _cache.Set(cacheKey, result.Data, TimeSpan.FromMinutes(5)); 
                }
                return result?.Data;
            }
            catch (UserNotFoundException)
            {
                throw; 
            }
            catch (HttpRequestException ex)
            {
                throw new ApplicationException("Network error occurred.", ex);
            }
            catch (JsonException ex)
            {
                throw new ApplicationException("Deserialization failed.", ex);
            }
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {

            string cacheKey = "AllUsers";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<User> cachedUsers))
                return cachedUsers;


            var users = new List<User>();
            int currentPage = 1;
            int totalPages;

            try
            {
                do
                {
                    var response = await _httpClient.GetAsync($"{_baseUrl}/users?page={currentPage}");
                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();
                    var pageResult = JsonSerializer.Deserialize<UserDataResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (pageResult?.Data != null)
                        users.AddRange(pageResult.Data);

                    totalPages = pageResult?.Total_Pages ?? 0;
                    currentPage++;
                } while (currentPage <= totalPages);

                _cache.Set(cacheKey, users, TimeSpan.FromMinutes(10));
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to retrieve all users.", ex);
            }
            return users;
        }
    }
}
