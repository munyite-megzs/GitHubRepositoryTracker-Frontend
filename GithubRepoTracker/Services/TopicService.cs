﻿using GithubRepoTracker.Interfaces;
using GithubRepoTracker.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Web;

namespace GithubRepoTracker.Services
{
    public class TopicService : TopicInterface
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        private readonly string BaseUrl;
        private readonly ApiAuthInterface _apiAuthInterface;

        public TopicService(HttpClient client, IConfiguration configuration, ApiAuthInterface apiAuthInterface)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _configuration = configuration;
            _apiAuthInterface = apiAuthInterface;

            BaseUrl = _configuration.GetValue<string>("ApiBaseUrl");
            
        }
        

        public async Task<IEnumerable<Topic>> GetAllTopics()
        {
            var Token = await _apiAuthInterface.GetAccessTokenAsync();
            var topics = new List<Topic>();

            _client.BaseAddress = new Uri(BaseUrl);
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

            var pageNumber = 1;
            var pageSize = 20;

            while (true)
            {
                var builder = new UriBuilder(BaseUrl + _configuration.GetValue<string>("ApiEndpoints:GetAllTopicsEndpoint"));

                var query = HttpUtility.ParseQueryString(builder.Query);
                query["pageNumber"] = pageNumber.ToString();
                query["pageSize"] = pageSize.ToString();
                builder.Query = query.ToString();
                var uri = builder.ToString();

                try
                {
                    var response = await _client.GetAsync(uri);

                    if (!response.IsSuccessStatusCode)
                    {
                        // handle error response
                        break;
                    }

                    var content = await response.Content.ReadAsStringAsync();

                    var paginatedTopics = JsonConvert.DeserializeObject<PaginatedTopics>(content);

                    if (!paginatedTopics.data.Any())
                    {
                        // no more topics available
                        break;
                    }

                    topics.AddRange(paginatedTopics.data);

                    pageNumber++;
                }
                catch (Exception ex)
                {
                    // handle exception
                    Console.WriteLine(ex.Message);
                    break;
                }
            }

            return topics;
        }


    }
}
