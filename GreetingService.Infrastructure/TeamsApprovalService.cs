using GreetingService.Core.Entities;
using GreetingService.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace GreetingService.Infrastructure
{
    public class TeamsApprovalService : IApprovalService
    {
        private readonly ILogger<TeamsApprovalService> _logger;
        private readonly string _teamswebhookbaseurl;
        private readonly string _greetingServiceBaseUrl;
        private readonly HttpClient _httpClient;
        public TeamsApprovalService(ILogger<TeamsApprovalService> logger, IConfiguration config,IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _teamswebhookbaseurl = config["TeamsWebHookUrl"];
            _logger = logger;
            _greetingServiceBaseUrl = config["GreetingServiceBaseUrl"];
        }
        public async Task BeginUserApprovalAsync(User user)
        {
            
            var newjson = @$"{{
						""@type"": ""MessageCard"",
						""@context"": ""https://schema.org/extensions"",
						""summary"": ""Approval for new GreetingService user"",
						""sections"": [
							{{
									""title"": ""Pending Approval for admin Sushree Sadhana"",
								""activityImage"": ""https://upload.wikimedia.org/wikipedia/commons/thumb/7/7c/User_font_awesome.svg/1024px-User_font_awesome.svg.png?20160212005950"",
								""activityTitle"": ""Approve new user in GreetingService: {user.Email}"",
								""activitySubtitle"": ""{user.First_name} {user.Last_name}"",
								""facts"": [
									{{
										""name"": ""Date submitted:"",
										""value"": ""{DateTime.Now:yyyy-MM-dd HH:mm}""
									}},
									{{
										""name"": ""Details:"",
										""value"": ""Please approve or reject the new user: {user.Email} for the GreetingService""
									}}
								]
							}},
							{{
								""potentialAction"": [
									{{
										""@type"": ""HttpPOST"",
										""name"": ""Approve"",
										""target"": ""{_greetingServiceBaseUrl}/api/User/approve/{user.ApprovalCode}""
	
									}},
									{{
										""@type"": ""HttpPOST"",
										""name"": ""Reject"",
										""target"": ""{_greetingServiceBaseUrl}/api/User/reject/{user.ApprovalCode}""
									}}
								]
							}}
						]
					}}";

            var response = await _httpClient.PostAsync(_teamswebhookbaseurl, new StringContent(newjson));
            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content?.ReadAsStringAsync();
                _logger.LogError("Failed to send approval to Teams for user {email}. Received this response body: {response}", user.Email, responseBody ?? "null");
            }

            response.EnsureSuccessStatusCode();
        }
    }
}