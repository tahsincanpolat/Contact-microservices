using ClosedXML.Excel;
using Microsoft.Extensions.Options;
using Reports.Abstract;
using Reports.Config;
using Reports.Model;
using Reports.Utility.Constants;
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
                    .Where(pair => pair.user.Type.ToLower() == Constants.Location.ToLower())
                    .GroupBy(pair => pair.user.Detail, pair => pair.info);

                //var test = userData;
                var responseModel = new List<Report>();

                foreach (var u in userDatas)
                {
                    var location = u.Key;
                    var userCount = u.Count();

                    var phoneNumbers = u
                        .SelectMany(info => info.ContactInfos, (info, user) => new { info, user })
                         .Where(pair => pair.user.Type.ToLower() == Constants.Phone.ToLower())
                        .GroupBy(pair => pair.user.Detail, pair => pair.info);

                    var phoneCount = phoneNumbers.Count();

                    responseModel.Add(new Report
                    {
                        Location = location,
                        UserCount = userCount,
                        PhoneCount = phoneCount
                    });
                }

                //Console.WriteLine("Response: {0}", responseModel);
                CreateExcelFile(responseModel);

                return new Response(200, "Report created successfully", responseModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"Create Report Exception: {e}");
                return new Response(500, "Error !");
            }
        }

        public void CreateExcelFile (List<Report> reports)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Users Info");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "User Count";
                worksheet.Cell(currentRow, 2).Value = "Phone Count";
                worksheet.Cell(currentRow, 3).Value = "Location";
                foreach (var repor in reports)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = repor.UserCount;
                    worksheet.Cell(currentRow, 2).Value = repor.PhoneCount;
                    worksheet.Cell(currentRow, 3).Value = repor.Location;
                }

                workbook.SaveAs($"ReportsFile/Reports-{DateTime.Now.ToString("yyyyMMddTHHmmss")}.xlsx");
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
