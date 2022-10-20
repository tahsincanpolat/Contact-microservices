using MassTransit;
using Reports.Abstract;
using Reports.DataAccess;

namespace Reports.Consumers
{
    public class ReportConsumer : IConsumer<AllReports>
    {
        private readonly IReportCreator _reportCreator;
        private readonly ReportsDBContext _db;
        private readonly ILogger<ReportConsumer> _logger;

        public ReportConsumer(ReportsDBContext db, ILogger<ReportConsumer> logger, IReportCreator reportCreator)
        {
            _db = db;
            _logger = logger;
            _reportCreator = reportCreator;
        }
        public async Task Consume(ConsumeContext<AllReports> context)
        {
            var report = context.Message;

            var createReport = await _reportCreator.CreateReport();

            if (createReport.ResponseCode == 200)
            {
                report.Status = "Completed";
                report.CreateDate = DateTime.Now;
                _logger.LogInformation($"Report completed: {createReport}");

                _db.Update(report);
                await _db.SaveChangesAsync();

                Console.WriteLine(createReport.ResponseMessage);
            }
            else
            {
                Console.WriteLine(createReport.ResponseMessage);
            }
        }
    }
}
