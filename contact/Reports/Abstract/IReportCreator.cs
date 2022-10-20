using Reports.Model;

namespace Reports.Abstract
{
    public interface IReportCreator
    {
        public Task<Response> CreateReport();
    }
}
