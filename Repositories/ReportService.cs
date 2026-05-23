using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using mvvm_edp.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace mvvm_edp.Services
{
    public class ReportService
    {
        private readonly byte[] _logoBytes;

        public ReportService()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            _logoBytes = LoadLogoBytes();
        }

        private static byte[] LoadLogoBytes()
        {
            try
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Logo.png");
                if (File.Exists(path))
                    return File.ReadAllBytes(path);
            }
            catch { }
            return Array.Empty<byte>();
        }

        private const string Purple = "5748FA";
        private static readonly Color PurpleColor = Color.FromArgb(0x57, 0x48, 0xFA);
        private static readonly Color LightPurpleBg = Color.FromArgb(0xF0, 0xEE, 0xFF);
        private static readonly Color StripeBg = Color.FromArgb(0xFA, 0xF9, 0xFE);

        private void BuildHeader(ExcelWorksheet ws, string companyName, string reportTitle, int totalCols)
        {
            // Row 1 – Logo at top-left corner, company name beside it
            if (_logoBytes.Length > 0)
            {
                using var ms = new MemoryStream(_logoBytes);
                var pic = ws.Drawings.AddPicture("Logo", ms);
                pic.SetPosition(0, 2, 0, 2);
                pic.SetSize(36, 36);
            }

            ws.Cells[1, 2].Value = companyName;
            ws.Cells[1, 2, 1, totalCols].Merge = true;
            ws.Cells[1, 2, 1, totalCols].Style.Font.Size = 22;
            ws.Cells[1, 2, 1, totalCols].Style.Font.Bold = true;
            ws.Cells[1, 2, 1, totalCols].Style.Font.Color.SetColor(PurpleColor);
            ws.Cells[1, 2, 1, totalCols].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[1, 2, 1, totalCols].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            ws.Row(1).Height = 38;

            // Row 2 – Report title
            ws.Cells[2, 1, 2, totalCols].Merge = true;
            ws.Cells[2, 1, 2, totalCols].Value = reportTitle;
            ws.Cells[2, 1, 2, totalCols].Style.Font.Size = 14;
            ws.Cells[2, 1, 2, totalCols].Style.Font.Color.SetColor(Color.Gray);
            ws.Cells[2, 1, 2, totalCols].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[2, 1, 2, totalCols].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            // Row 3 – Meta info
            ws.Cells[3, 1, 3, totalCols].Merge = true;
            ws.Cells[3, 1, 3, totalCols].Value = $"Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}    |    User: {Environment.UserName}";
            ws.Cells[3, 1, 3, totalCols].Style.Font.Size = 10;
            ws.Cells[3, 1, 3, totalCols].Style.Font.Color.SetColor(Color.DimGray);
            ws.Cells[3, 1, 3, totalCols].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            // Row 4 – Purple accent bar
            ws.Cells[4, 1, 4, totalCols].Merge = true;
            ws.Cells[4, 1, 4, totalCols].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[4, 1, 4, totalCols].Style.Fill.BackgroundColor.SetColor(PurpleColor);
            ws.Row(4).Height = 6;

            // Row 5 – Spacer
            ws.Row(5).Height = 6;
        }

        private static void AddSignature(ExcelWorksheet ws, int row, int totalCols)
        {
            row += 1;

            // Thin separator line above signature
            ws.Cells[row, 1, row, totalCols].Merge = true;
            ws.Cells[row, 1, row, totalCols].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            ws.Cells[row, 1, row, totalCols].Style.Border.Top.Color.SetColor(PurpleColor);
            ws.Row(row).Height = 4;

            row += 1;
            ws.Cells[row, 1].Value = "Prepared by:";
            ws.Cells[row, 1].Style.Font.Bold = true;
            ws.Cells[row, 1].Style.Font.Size = 11;

            row++;
            ws.Cells[row, 1].Value = Environment.UserName;
            ws.Cells[row, 1].Style.Font.Size = 13;
            ws.Cells[row, 1].Style.Font.Bold = true;
            ws.Cells[row, 1].Style.Font.Color.SetColor(PurpleColor);

            row++;
            ws.Cells[row, 1].Value = "___________________________";
            ws.Cells[row, 1].Style.Font.Color.SetColor(Color.DarkGray);
            ws.Row(row).Height = 8;

            row++;
            ws.Cells[row, 1].Value = "Authorized Signature";
            ws.Cells[row, 1].Style.Font.Italic = true;
            ws.Cells[row, 1].Style.Font.Color.SetColor(Color.Gray);
            ws.Cells[row, 1].Style.Font.Size = 10;
        }

        private void BuildTable(ExcelWorksheet ws, int headerRow, int totalCols, int dataStart, int dataEnd)
        {
            // Header row
            using var header = ws.Cells[headerRow, 1, headerRow, totalCols];
            header.Style.Font.Bold = true;
            header.Style.Font.Size = 12;
            header.Style.Font.Color.SetColor(Color.White);
            header.Style.Fill.PatternType = ExcelFillStyle.Solid;
            header.Style.Fill.BackgroundColor.SetColor(PurpleColor);
            header.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            header.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Row(headerRow).Height = 32;

            if (dataEnd >= dataStart)
            {
                // All data borders
                using var data = ws.Cells[dataStart, 1, dataEnd, totalCols];
                data.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                data.Style.Border.Top.Color.SetColor(Color.LightGray);
                data.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                data.Style.Border.Left.Color.SetColor(Color.LightGray);
                data.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                data.Style.Border.Right.Color.SetColor(Color.LightGray);
                data.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                data.Style.Border.Bottom.Color.SetColor(Color.LightGray);
                data.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                // Alternating row colours
                for (int r = dataStart; r <= dataEnd; r++)
                {
                    ws.Row(r).Height = 24;
                    if (r % 2 == 0)
                    {
                        using var rowR = ws.Cells[r, 1, r, totalCols];
                        rowR.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        rowR.Style.Fill.BackgroundColor.SetColor(StripeBg);
                    }
                }
            }

            ws.Cells[headerRow, 1, dataEnd > 0 ? dataEnd : headerRow, totalCols].AutoFitColumns();
            // Give minimum widths
            for (int c = 1; c <= totalCols; c++)
            {
                if (ws.Column(c).Width < 12) ws.Column(c).Width = 12;
            }
        }

        private static void AddChart(ExcelWorksheet chartSheet, string title,
            int catCol, int valCol, int dataStart, int dataEnd,
            eChartType chartType = eChartType.ColumnClustered)
        {
            var chart = chartSheet.Drawings.AddChart("Chart", chartType);
            chart.Title.Text = title;
            chart.Title.Font.Size = 14;
            chart.Title.Font.Bold = true;
            chart.Title.Font.Color = System.Drawing.Color.FromArgb(0x57, 0x48, 0xFA);

            var catL = GetColLetter(catCol);
            var valL = GetColLetter(valCol);
            chart.Series.Add(
                chartSheet.Cells[$"{valL}{dataStart}:{valL}{dataEnd}"],
                chartSheet.Cells[$"{catL}{dataStart}:{catL}{dataEnd}"]
            );

            chart.Legend.Position = eLegendPosition.Bottom;
            chart.SetPosition(2, 10, 2, 10);
            chart.SetSize(680, 360);

            // Style the data table
            using var hdr = chartSheet.Cells[1, catCol, 1, valCol];
            hdr.Style.Font.Bold = true;
            hdr.Style.Fill.PatternType = ExcelFillStyle.Solid;
            hdr.Style.Fill.BackgroundColor.SetColor(LightPurpleBg);
            chartSheet.Cells[1, catCol, 1, valCol].AutoFitColumns();
        }

        private static string GetColLetter(int col)
        {
            if (col <= 0) throw new ArgumentOutOfRangeException(nameof(col));
            var n = "";
            while (col > 0)
            {
                col--;
                n = (char)('A' + col % 26) + n;
                col /= 26;
            }
            return n;
        }

        // ──────────────────────────────────────────────
        //  SALES REPORT
        // ──────────────────────────────────────────────
        public async Task ExportSalesReportAsync(
            List<Order> orders,
            List<Customer> customers,
            List<Payment> payments,
            List<OrderItem> orderItems,
            List<Product> products,
            string filePath,
            string username)
        {
            using var p = new ExcelPackage();
            var ws = p.Workbook.Worksheets.Add("Sales");
            const int cols = 6;

            BuildHeader(ws, "Mini-Ecom", "Sales Transaction Report", cols);

            // ── Section A: Order Summary ──
            int row = 7;
            ws.Cells[row, 1].Value = "A. Order Summary";
            ws.Cells[row, 1, row, cols].Merge = true;
            ws.Cells[row, 1, row, cols].Style.Font.Size = 13;
            ws.Cells[row, 1, row, cols].Style.Font.Bold = true;
            ws.Cells[row, 1, row, cols].Style.Font.Color.SetColor(PurpleColor);
            ws.Row(row).Height = 28;
            row++;

            int sumHdr = row;
            ws.Cells[row, 1].Value = "Order ID";
            ws.Cells[row, 2].Value = "Customer";
            ws.Cells[row, 3].Value = "Date";
            ws.Cells[row, 4].Value = "Total";
            ws.Cells[row, 5].Value = "Method";
            ws.Cells[row, 6].Value = "Status";

            row++;
            int sumStart = row;

            decimal grandTotal = 0;
            foreach (var o in orders)
            {
                var cust = customers.FirstOrDefault(c => c.CustomerId == o.CustomerId);
                var pay = payments.FirstOrDefault(p => p.OrderId == o.OrderId);
                ws.Cells[row, 1].Value = o.OrderId;
                ws.Cells[row, 2].Value = cust != null ? $"{cust.FirstName} {cust.LastName}" : "N/A";
                ws.Cells[row, 3].Value = o.OrderDate.ToString("yyyy-MM-dd");
                ws.Cells[row, 4].Value = pay?.Amount ?? 0;
                ws.Cells[row, 5].Value = pay?.PaymentMethod ?? "N/A";
                ws.Cells[row, 6].Value = pay?.PaymentStatus ?? "N/A";
                grandTotal += pay?.Amount ?? 0;
                row++;
            }
            int sumEnd = row - 1;

            BuildTable(ws, sumHdr, cols, sumStart, sumEnd);

            // ── Section B: Order Items ──
            row++;
            ws.Cells[row, 1].Value = "B. Order Items";
            ws.Cells[row, 1, row, cols].Merge = true;
            ws.Cells[row, 1, row, cols].Style.Font.Size = 13;
            ws.Cells[row, 1, row, cols].Style.Font.Bold = true;
            ws.Cells[row, 1, row, cols].Style.Font.Color.SetColor(PurpleColor);
            ws.Row(row).Height = 28;
            row++;

            int itemHdr = row;
            ws.Cells[row, 1].Value = "Order ID";
            ws.Cells[row, 2].Value = "Product";
            ws.Cells[row, 3].Value = "Price";
            ws.Cells[row, 4].Value = "Qty";
            ws.Cells[row, 5].Value = "Line Total";
            ws.Cells[row, 6].Value = "Customer";

            row++;
            int itemStart = row;

            foreach (var o in orders)
            {
                var cust = customers.FirstOrDefault(c => c.CustomerId == o.CustomerId);
                var custName = cust != null ? $"{cust.FirstName} {cust.LastName}" : "N/A";
                var items = orderItems.Where(oi => oi.OrderId == o.OrderId).ToList();

                if (items.Count == 0)
                {
                    ws.Cells[row, 1].Value = o.OrderId;
                    ws.Cells[row, 2].Value = "(no items)";
                    ws.Cells[row, 5].Value = 0;
                    ws.Cells[row, 6].Value = custName;
                    row++;
                }

                foreach (var oi in items)
                {
                    var prod = products.FirstOrDefault(p => p.ProductId == oi.ProductId);
                    var lineTotal = (prod?.Price ?? 0) * oi.Quantity;
                    ws.Cells[row, 1].Value = o.OrderId;
                    ws.Cells[row, 2].Value = prod?.Name ?? "Unknown";
                    ws.Cells[row, 3].Value = prod?.Price ?? 0;
                    ws.Cells[row, 4].Value = oi.Quantity;
                    ws.Cells[row, 5].Value = lineTotal;
                    ws.Cells[row, 6].Value = custName;
                    row++;
                }
            }
            int itemEnd = row - 1;

            BuildTable(ws, itemHdr, cols, itemStart, itemEnd);

            // ── Grand Total ──
            row++;
            ws.Cells[row, 1].Value = "GRAND TOTAL:";
            ws.Cells[row, 1, row, 4].Merge = true;
            ws.Cells[row, 1, row, 4].Style.Font.Size = 14;
            ws.Cells[row, 1, row, 4].Style.Font.Bold = true;
            ws.Cells[row, 1, row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            ws.Cells[row, 5].Value = grandTotal;
            ws.Cells[row, 5, row, cols].Merge = true;
            ws.Cells[row, 5, row, cols].Style.Font.Size = 16;
            ws.Cells[row, 5, row, cols].Style.Font.Bold = true;
            ws.Cells[row, 5, row, cols].Style.Font.Color.SetColor(PurpleColor);
            ws.Row(row).Height = 30;

            row++;
            AddSignature(ws, row, cols);

            // Sheet 2 – Orders per day chart
            var cs = p.Workbook.Worksheets.Add("Chart");
            var groups = orders
                .GroupBy(o => o.OrderDate.Date)
                .OrderBy(g => g.Key)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToList();

            cs.Cells[1, 1].Value = "Date";
            cs.Cells[1, 2].Value = "Orders";
            int r = 2;
            foreach (var g in groups)
            {
                cs.Cells[r, 1].Value = g.Date.ToString("yyyy-MM-dd");
                cs.Cells[r, 2].Value = g.Count;
                r++;
            }

            AddChart(cs, "Orders per Day", 1, 2, 2, r - 1);
            await p.SaveAsAsync(new FileInfo(filePath));
        }

        // ──────────────────────────────────────────────
        //  INVENTORY REPORT
        // ──────────────────────────────────────────────
        public async Task ExportInventoryReportAsync(List<Product> products, string filePath, string username)
        {
            using var p = new ExcelPackage();
            var ws = p.Workbook.Worksheets.Add("Inventory");
            const int cols = 5;

            BuildHeader(ws, "Mini-Ecom", "Inventory Report", cols);

            int hdrRow = 7;
            ws.Cells[hdrRow, 1].Value = "Product ID";
            ws.Cells[hdrRow, 2].Value = "Name";
            ws.Cells[hdrRow, 3].Value = "Description";
            ws.Cells[hdrRow, 4].Value = "Price";
            ws.Cells[hdrRow, 5].Value = "Stock";

            int row = hdrRow + 1;
            foreach (var pdt in products)
            {
                ws.Cells[row, 1].Value = pdt.ProductId;
                ws.Cells[row, 2].Value = pdt.Name;
                ws.Cells[row, 3].Value = pdt.Description;
                ws.Cells[row, 4].Value = pdt.Price;
                ws.Cells[row, 5].Value = pdt.Quantity;
                row++;
            }

            BuildTable(ws, hdrRow, cols, hdrRow + 1, row - 1);
            AddSignature(ws, row, cols);

            // Sheet 2 – Chart
            var cs = p.Workbook.Worksheets.Add("Chart");
            var sorted = products
                .OrderByDescending(x => x.Quantity)
                .Take(10)
                .ToList();

            cs.Cells[1, 1].Value = "Product";
            cs.Cells[1, 2].Value = "Stock";
            int r = 2;
            foreach (var pdt in sorted)
            {
                cs.Cells[r, 1].Value = pdt.Name;
                cs.Cells[r, 2].Value = pdt.Quantity;
                r++;
            }

            AddChart(cs, "Top 10 Products by Stock", 1, 2, 2, r - 1, eChartType.BarClustered);
            await p.SaveAsAsync(new FileInfo(filePath));
        }

        // ──────────────────────────────────────────────
        //  CUSTOMER REPORT
        // ──────────────────────────────────────────────
        public async Task ExportCustomerReportAsync(List<Customer> customers, string filePath, string username)
        {
            using var p = new ExcelPackage();
            var ws = p.Workbook.Worksheets.Add("Customers");
            const int cols = 5;

            BuildHeader(ws, "Mini-Ecom", "Customer Report", cols);

            int hdrRow = 7;
            ws.Cells[hdrRow, 1].Value = "Customer ID";
            ws.Cells[hdrRow, 2].Value = "First Name";
            ws.Cells[hdrRow, 3].Value = "Last Name";
            ws.Cells[hdrRow, 4].Value = "Phone";
            ws.Cells[hdrRow, 5].Value = "Address";

            int row = hdrRow + 1;
            foreach (var c in customers)
            {
                ws.Cells[row, 1].Value = c.CustomerId;
                ws.Cells[row, 2].Value = c.FirstName;
                ws.Cells[row, 3].Value = c.LastName;
                ws.Cells[row, 4].Value = c.Phone;
                ws.Cells[row, 5].Value = c.Address;
                row++;
            }

            BuildTable(ws, hdrRow, cols, hdrRow + 1, row - 1);
            AddSignature(ws, row, cols);

            // Sheet 2 – Chart
            var cs = p.Workbook.Worksheets.Add("Chart");
            var groups = customers
                .GroupBy(c => string.IsNullOrWhiteSpace(c.LastName) ? "?" : c.LastName[..1].ToUpper())
                .Select(g => new { Initial = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToList();

            cs.Cells[1, 1].Value = "Initial";
            cs.Cells[1, 2].Value = "Customers";
            int r = 2;
            foreach (var g in groups)
            {
                cs.Cells[r, 1].Value = g.Initial;
                cs.Cells[r, 2].Value = g.Count;
                r++;
            }

            AddChart(cs, "Customers by Last Name Initial", 1, 2, 2, r - 1, eChartType.Pie);
            await p.SaveAsAsync(new FileInfo(filePath));
        }
    }
}