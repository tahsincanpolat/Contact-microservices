using Microsoft.Extensions.Options;
using Reports.Abstract;
using Reports.Config;
using Reports.Model;
using System.Text.Json;

namespace Reports.Business
{
    public class ReportCreator : IReportCreator
    {
        private readonly IHttpClientFactory _http;
        private readonly ILogger<ReportCreator> _logger;
        private readonly ClientConfig _clientConfig;

        public ReportCreator(IHttpClientFactory http, ILogger<ReportCreator> logger,IOptions<ClientConfig> clientConfig)
        {
            _http = http;
            _logger = logger;
            _clientConfig = clientConfig.Value;
        }
        public async Task<Response> CreateReport()
        {
            try
            {
                var httpClient = _http.CreateClient();

                var userData = await FetchUserData(httpClient);
                _logger.LogInformation($"users: {userData}");

                var userDatas = userData
                    .SelectMany(info => info.ContactInfos, (info, user) => new { info, user })
                    .Where(pair => pair.user.Type == "Location")
                    .GroupBy(pair => pair.user.Detail, pair => pair.info);

                var responseModel = new List<Report>();

                foreach (var u in userDatas)
                {
                    var location = u.Key;
                    var userCount = u.Count();

                    var phoneNumbers = u
                        .SelectMany(info => info.ContactInfos, (info, user) => new { info, user })
                        .Where(pair => pair.user.Type == "Phone")
                        .GroupBy(pair => pair.user.Detail, pair => pair.info);

                    var phoneCount = phoneNumbers.Count();

                    responseModel.Add(new Report
                    {
                        Location = location,
                        UserCount = userCount,
                        PhoneCount = phoneCount
                    });
                }
                Console.WriteLine(responseModel);
                //CreateExcelFile(responseModel);

                return new Response(200, "Report created successfully", responseModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"Create Report Exception: {e}");
                return new Response(500, "Error !");
            }
        }
        private async Task<List<User>> FetchUserData(HttpClient httpClient)
        {
            var endpoint = BuildUserServiceEndpoint();
            var userRecords = await httpClient.GetAsync(endpoint);
            var jsonSerializeOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var userData = await userRecords.Content.ReadFromJsonAsync<List<User>>(jsonSerializeOptions);

            return userData ?? new List<User>();
        }

        private string BuildUserServiceEndpoint()
        {
            var clientServiceProtocol = _clientConfig.ClientProtocol;
            var clientServiceHost = _clientConfig.ClientHost;
            var clientServicePort = _clientConfig.ClientPort;

            return $"{clientServiceProtocol}://{clientServiceHost}:{clientServicePort}/userDetails";
        }

    }
}
