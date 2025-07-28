using ClosedXML.Excel;
using LMS.Application.Abstractions;
using LMS.Application.Abstractions.Reports;
using LMS.Application.DTOs.Reports;
using LMS.Application.DTOs.Stock;
using LMS.Application.Filters.Reports;

namespace LMS.Infrastructure.Helpers.Reports
{
    public class ReportsGeneratorHelper : IReportsGeneratorHelper
    {
        public byte[] GenerateRevenueReport(
           ICollection<RevenueReportDto> revenues,
           RevenueReportFilter filter)
        {
            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add(filter.Title ?? "Revenues Report");

            int col = 1;
            var header = sheet.Row(1);
            if (filter.IncludeEmployeeName) sheet.Cell(1, col++).Value = "Employee";
            if (filter.IncludeCustomerName) sheet.Cell(1, col++).Value = "Customer";
            if (filter.IncludeAmount) sheet.Cell(1, col++).Value = "Amount";
            if (filter.IncludeService) sheet.Cell(1, col++).Value = "Service";
            if (filter.IncludeDate) sheet.Cell(1, col++).Value = "Date";

            int row = 2;
            foreach (var item in revenues)
            {
                col = 1;
                if (filter.IncludeEmployeeName) sheet.Cell(row, col++).Value = item.EmployeeName;
                if (filter.IncludeCustomerName) sheet.Cell(row, col++).Value = item.CustomerName;
                if (filter.IncludeAmount) sheet.Cell(row, col++).Value = item.Amount;
                if (filter.IncludeService) sheet.Cell(row, col++).Value = item.Service.ToString();
                if (filter.IncludeDate) sheet.Cell(row, col++).Value = item.CreatedAt.ConvertToSyrianTime();
                row++;
            }

            if (filter.IncludeTotalRow && filter.IncludeAmount)
            {
                int amountCol = 1;
                if (filter.IncludeEmployeeName) amountCol++;
                if (filter.IncludeCustomerName) amountCol++;
                sheet.Cell(row, amountCol - 1).Value = "Total:";
                sheet.Cell(row, amountCol).FormulaA1 = $"=SUM({GetExcelColumnName(amountCol)}2:{GetExcelColumnName(amountCol)}{row - 1})";
                sheet.Row(row).Style.Fill.BackgroundColor = XLColor.LightYellow;
                sheet.Row(row).Style.Font.Bold = true;
                sheet.Row(row).Style.Font.FontName = filter.FontFamily ?? "Tajawal";
            }

            // Apply styles
            ApplySheetStyling(sheet, filter, 2, row);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }


        public byte[] GeneratePaymentReport(
            ICollection<PaymentReportDto> payments,
            PaymentReportFilter filter)
        {
            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add(filter.Title ?? "Payments Report");

            int col = 1;
            var header = sheet.Row(1);
            if (filter.IncludeEmployee) sheet.Cell(1, col++).Value = "Employee";
            if (filter.IncludeAmount) sheet.Cell(1, col++).Value = "Amount";
            if (filter.IncludeDetails) sheet.Cell(1, col++).Value = "Details";
            if (filter.IncludeDate) sheet.Cell(1, col++).Value = "Date";

            int row = 2;
            foreach (var item in payments)
            {
                col = 1;
                if (filter.IncludeEmployee) sheet.Cell(row, col++).Value = item.EmployeeName;
                if (filter.IncludeAmount) sheet.Cell(row, col++).Value = item.Amount;
                if (filter.IncludeDetails) sheet.Cell(row, col++).Value = item.Details;
                if (filter.IncludeDate) sheet.Cell(row, col++).Value = item.Date;
                row++;
            }

            if (filter.IncludeTotalRow && filter.IncludeAmount)
            {
                int amountCol = 1;
                if (filter.IncludeEmployee) amountCol++;
                sheet.Cell(row, amountCol - 1).Value = "Total:";
                sheet.Cell(row, amountCol).FormulaA1 = $"=SUM({GetExcelColumnName(amountCol)}2:{GetExcelColumnName(amountCol)}{row - 1})";
                sheet.Row(row).Style.Fill.BackgroundColor = XLColor.LightYellow;
                sheet.Row(row).Style.Font.Bold = true;
            }

            ApplySheetStyling(sheet, filter, 2, row);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }


        public byte[] GenerateFinancialReport(
            ICollection<RevenueReportDto> revenues,
            ICollection<PaymentReportDto> payments,
            FinancialReportFilter filter)
        {
            using var workbook = new XLWorkbook();
            var paymentsSheet = workbook.Worksheets.Add(filter.PaymentTitle ?? "Payments Report");
            var revenuesSheet = workbook.Worksheets.Add(filter.RevenueTitle ?? "Revenues Report");

            int revenueCol = 1;
            var revenuesHeader = revenuesSheet.Row(1);
            if (filter.IncludeEmployeeName) revenuesSheet.Cell(1, revenueCol++).Value = "Employee";
            if (filter.IncludeCustomerName) revenuesSheet.Cell(1, revenueCol++).Value = "Customer";
            if (filter.IncludeAmount) revenuesSheet.Cell(1, revenueCol++).Value = "Amount";
            if (filter.IncludeService) revenuesSheet.Cell(1, revenueCol++).Value = "Service";
            if (filter.IncludeDate) revenuesSheet.Cell(1, revenueCol++).Value = "Date";

            int revenueRow = 2;
            foreach (var item in revenues)
            {
                revenueCol = 1;
                if (filter.IncludeEmployeeName) revenuesSheet.Cell(revenueRow, revenueCol++).Value = item.EmployeeName;
                if (filter.IncludeCustomerName) revenuesSheet.Cell(revenueRow, revenueCol++).Value = item.CustomerName;
                if (filter.IncludeAmount) revenuesSheet.Cell(revenueRow, revenueCol++).Value = item.Amount;
                if (filter.IncludeService) revenuesSheet.Cell(revenueRow, revenueCol++).Value = item.Service.ToString();
                if (filter.IncludeDate) revenuesSheet.Cell(revenueRow, revenueCol++).Value = item.CreatedAt.ConvertToSyrianTime();
                revenueRow++;
            }

            if (filter.IncludeTotalRow && filter.IncludeAmount)
            {
                int amountCol = 1;
                if (filter.IncludeEmployeeName) amountCol++;
                if (filter.IncludeCustomerName) amountCol++;
                revenuesSheet.Cell(revenueRow, amountCol - 1).Value = "Total:";
                revenuesSheet.Cell(revenueRow, amountCol).FormulaA1 = $"=SUM({GetExcelColumnName(amountCol)}2:{GetExcelColumnName(amountCol)}{revenueRow - 1})";
                revenuesSheet.Row(revenueRow).Style.Fill.BackgroundColor = XLColor.LightYellow;
                revenuesSheet.Row(revenueRow).Style.Font.Bold = true;
                revenuesSheet.Row(revenueRow).Style.Font.FontName = filter.FontFamily ?? "Tajawal";
            }

            ApplySheetStyling(revenuesSheet, filter, 2, revenueRow);


            int paymentCol = 1;
            var paymentHeader = paymentsSheet.Row(1);
            if (filter.IncludeEmployeeName) paymentsSheet.Cell(1, paymentCol++).Value = "Employee";
            if (filter.IncludeAmount) paymentsSheet.Cell(1, paymentCol++).Value = "Amount";
            if (filter.IncludeDetails) paymentsSheet.Cell(1, paymentCol++).Value = "Details";
            if (filter.IncludeDate) paymentsSheet.Cell(1, paymentCol++).Value = "Date";

            int paymentRow = 2;
            foreach (var item in payments)
            {
                paymentCol = 1;
                if (filter.IncludeEmployeeName) paymentsSheet.Cell(paymentRow, paymentCol++).Value = item.EmployeeName;
                if (filter.IncludeAmount) paymentsSheet.Cell(paymentRow, paymentCol++).Value = item.Amount;
                if (filter.IncludeDetails) paymentsSheet.Cell(paymentRow, paymentCol++).Value = item.Details;
                if (filter.IncludeDate) paymentsSheet.Cell(paymentRow, paymentCol++).Value = item.Date;
                paymentRow++;
            }

            if (filter.IncludeTotalRow && filter.IncludeAmount)
            {
                int amountCol = 1;
                if (filter.IncludeEmployeeName) amountCol++;
                paymentsSheet.Cell(paymentRow, amountCol - 1).Value = "Total:";
                paymentsSheet.Cell(paymentRow, amountCol).FormulaA1 = $"=SUM({GetExcelColumnName(amountCol)}2:{GetExcelColumnName(amountCol)}{paymentRow - 1})";
                paymentsSheet.Row(paymentRow).Style.Fill.BackgroundColor = XLColor.LightYellow;
                paymentsSheet.Row(paymentRow).Style.Font.Bold = true;
            }

            ApplySheetStyling(paymentsSheet, filter, 2, paymentRow);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }


        public byte[] GenerateStockReportAsync(
            ICollection<StockSnapshotDto> inventory,
            StockReportFilter filter)
        {
            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add(filter.Title ?? "Inventory Report");

            int col = 1;
            var header = sheet.Row(1);

            if (filter.IncludeProductId) sheet.Cell(1, col++).Value = "Product ID";
            if (filter.IncludeProductName) sheet.Cell(1, col++).Value = "Product Name";
            if (filter.IncludeProductStock) sheet.Cell(1, col++).Value = "Quantity";
            if (filter.IncludeProductPrice) sheet.Cell(1, col++).Value = "Price";
            if (filter.IncludeTotalValue) sheet.Cell(1, col++).Value = "Total Price";
            if (filter.IncludeUpdatedAt) sheet.Cell(1, col++).Value = "Last Update";
            if (filter.IncludeLogsCount) sheet.Cell(1, col++).Value = "Edit Count";

            int row = 2;
            foreach (var item in inventory)
            {
                col = 1;
                if (filter.IncludeProductId) sheet.Cell(row, col++).Value = item.ProductId.ToString();
                if (filter.IncludeProductName) sheet.Cell(row, col++).Value = item.ProductName;
                if (filter.IncludeProductStock) sheet.Cell(row, col++).Value = item.ProductStock;
                if (filter.IncludeProductPrice) sheet.Cell(row, col++).Value = item.ProductPrice;
                if (filter.IncludeTotalValue) sheet.Cell(row, col++).Value = item.TotalValue;
                if (filter.IncludeUpdatedAt) sheet.Cell(row, col++).Value = item.UpdatedAt;
                if (filter.IncludeLogsCount) sheet.Cell(row, col++).Value = item.LogsCount;
                row++;
            }

            // حساب مجموع الكمية إن وُجد
            if (filter.IncludeTotalRow && filter.IncludeProductStock)
            {
                int quantityCol = 1;
                if (filter.IncludeProductId) quantityCol++;
                if (filter.IncludeProductName) quantityCol++;

                sheet.Cell(row, quantityCol - 1).Value = "Total:";
                sheet.Cell(row, quantityCol).FormulaA1 =
                    $"=SUM({GetExcelColumnName(quantityCol)}2:{GetExcelColumnName(quantityCol)}{row - 1})";

                sheet.Row(row).Style.Fill.BackgroundColor = XLColor.LightYellow;
                sheet.Row(row).Style.Font.Bold = true;
                sheet.Row(row).Style.Font.FontName = filter.FontFamily ?? "Tajawal";
            }

            ApplySheetStyling(sheet, filter, 2, row);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }


        public byte[] GenerateDeadStockReport(
            ICollection<DeadStockDto> deadStock,
            DeadStockReportFilter filter)
        {
            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add(filter.Title ?? "Dead Stock Report");

            int col = 1;
            var header = sheet.Row(1);
            sheet.Cell(1, col++).Value = "Product ID";
            if (filter.IncludeProductStock) sheet.Cell(1, col++).Value = "Quantity";
            if (filter.IncludeProductPrice) sheet.Cell(1, col++).Value = "Price";
            if (filter.IncludeLastMovementDate) sheet.Cell(1, col++).Value = "Last Movement";
            if (filter.IncludeCreatedAt) sheet.Cell(1, col++).Value = "Created At";

            int row = 2;
            foreach (var item in deadStock)
            {
                col = 1;
                sheet.Cell(row, col++).Value = item.ProductId.ToString();
                if (filter.IncludeProductStock) sheet.Cell(row, col++).Value = item.ProductStock;
                if (filter.IncludeProductPrice) sheet.Cell(row, col++).Value = item.ProductPrice;
                if (filter.IncludeLastMovementDate) sheet.Cell(row, col++).Value = item.LastMovementDate;
                if (filter.IncludeCreatedAt) sheet.Cell(row, col++).Value = item.CreatedAt;
                row++;
            }

            // Optional total row for quantity
            if (filter.IncludeTotalRow && filter.IncludeProductStock)
            {
                int quantityCol = 2;
                sheet.Cell(row, quantityCol - 1).Value = "Total:";
                sheet.Cell(row, quantityCol).FormulaA1 =
                    $"=SUM({GetExcelColumnName(quantityCol)}2:{GetExcelColumnName(quantityCol)}{row - 1})";

                sheet.Row(row).Style.Fill.BackgroundColor = XLColor.LightYellow;
                sheet.Row(row).Style.Font.Bold = true;
                sheet.Row(row).Style.Font.FontName = filter.FontFamily ?? "Tajawal";
            }

            ApplySheetStyling(sheet, filter, 2, row);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }


        private void ApplySheetStyling(IXLWorksheet sheet, BaseReportFilter filter, int dataStartRow, int dataEndRow)
        {
            var usedRange = sheet.RangeUsed();
            if (usedRange == null) return;

            // ===== Header Styling =====
            var headerRow = sheet.Row(1);

            if (!string.IsNullOrEmpty(filter.HeaderBackgroundColor))
                headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml(filter.HeaderBackgroundColor);

            if (!string.IsNullOrEmpty(filter.HeaderTextColor))
                headerRow.Style.Font.FontColor = XLColor.FromHtml(filter.HeaderTextColor);

            if (!string.IsNullOrEmpty(filter.FontFamily))
                headerRow.Style.Font.FontName = filter.FontFamily;

            if (filter.HeaderFontSize.HasValue)
                headerRow.Style.Font.FontSize = filter.HeaderFontSize.Value;

            headerRow.Style.Font.Bold = true;

            foreach (var cell in headerRow.CellsUsed())
            {
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            }

            // ===== Body Styling =====
            for (int i = dataStartRow; i <= dataEndRow; i++)
            {
                var row = sheet.Row(i);

                if (!string.IsNullOrEmpty(filter.BodyBackgroundColor))
                    row.Style.Fill.BackgroundColor = XLColor.FromHtml(filter.BodyBackgroundColor);

                if (!string.IsNullOrEmpty(filter.BodyTextColor))
                    row.Style.Font.FontColor = XLColor.FromHtml(filter.BodyTextColor);

                if (!string.IsNullOrEmpty(filter.FontFamily))
                    row.Style.Font.FontName = filter.FontFamily;

                if (filter.BodyFontSize.HasValue)
                    row.Style.Font.FontSize = filter.BodyFontSize.Value;

                foreach (var cell in row.CellsUsed())
                {
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                }
            }

            // Alignment and auto fit
            usedRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            usedRange.Style.Alignment.WrapText = true;
            sheet.Columns().AdjustToContents();
            sheet.SheetView.FreezeRows(1);
        }



        private string GetExcelColumnName(int columnNumber)
        {
            var dividend = columnNumber;
            string columnName = string.Empty;
            while (dividend > 0)
            {
                var modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo) + columnName;
                dividend = (dividend - modulo) / 26;
            }
            return columnName;
        }
    }
}
