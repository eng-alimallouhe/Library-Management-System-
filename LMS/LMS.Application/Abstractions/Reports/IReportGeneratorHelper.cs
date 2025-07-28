using LMS.Application.DTOs.Reports;
using LMS.Application.DTOs.Stock;
using LMS.Application.Filters.Reports;

namespace LMS.Application.Abstractions.Reports
{
    public interface IReportsGeneratorHelper
    {
        public byte[] GenerateRevenueReport(
            ICollection<RevenueReportDto> revenues, 
            RevenueReportFilter filter);


        public byte[] GeneratePaymentReport(
            ICollection<PaymentReportDto> payments,
            PaymentReportFilter filter);


        public byte[] GenerateFinancialReport(
            ICollection<RevenueReportDto> revenues,
            ICollection<PaymentReportDto> payments,
            FinancialReportFilter filter);

        public byte[] GenerateStockReportAsync(
            ICollection<StockSnapshotDto> inventory,
            StockReportFilter filter);


        public byte[] GenerateDeadStockReport(
            ICollection<DeadStockDto> deadStock,
            DeadStockReportFilter filter);
    }
}
