using AliMertOyunMagaza.Data;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AliMertOyunMagaza.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var wishlistItems = await _context.WishlistItems
                .Include(w => w.Game)
                .ThenInclude(g => g.Category)
                .Include(w => w.User)
                .ToListAsync();

            var chartData = wishlistItems
                .Where(w => w.Game != null && w.Game.Category != null)
                .GroupBy(w => w.Game.Category.Name)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToList();

            ViewBag.ChartLabels = chartData.Select(x => x.Category).ToList();
            ViewBag.ChartData = chartData.Select(x => x.Count).ToList();

            return View(wishlistItems);
        }

        public async Task<IActionResult> ExportExcel()
        {
            var items = await _context.WishlistItems
                .Include(w => w.Game)
                .ThenInclude(g => g.Category)
                .Include(w => w.User)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Istek Listeleri");
            
            worksheet.Cell(1, 1).Value = "Kullanıcı";
            worksheet.Cell(1, 2).Value = "Oyun";
            worksheet.Cell(1, 3).Value = "Kategori";
            worksheet.Cell(1, 4).Value = "Tarih";

            int row = 2;
            foreach (var item in items)
            {
                worksheet.Cell(row, 1).Value = item.User?.UserName;
                worksheet.Cell(row, 2).Value = item.Game?.Title;
                worksheet.Cell(row, 3).Value = item.Game?.Category?.Name;
                worksheet.Cell(row, 4).Value = item.AddedAt.ToString("dd.MM.yyyy");
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "IstekListeleri.xlsx");
        }

        public async Task<IActionResult> ExportPdf()
        {
            var items = await _context.WishlistItems
                .Include(w => w.Game)
                .ThenInclude(g => g.Category)
                .Include(w => w.User)
                .ToListAsync();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header().Text("Kullanici Istek Listesi Raporu").SemiBold().FontSize(20).FontColor(Colors.Blue.Darken2);

                    page.Content().PaddingVertical(1, Unit.Centimetre).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Kullanici").SemiBold();
                            header.Cell().Text("Oyun").SemiBold();
                            header.Cell().Text("Kategori").SemiBold();
                            header.Cell().Text("Tarih").SemiBold();
                        });

                        foreach (var item in items)
                        {
                            table.Cell().Text(item.User?.UserName ?? "");
                            table.Cell().Text(item.Game?.Title ?? "");
                            table.Cell().Text(item.Game?.Category?.Name ?? "");
                            table.Cell().Text(item.AddedAt.ToString("dd.MM.yyyy"));
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Sayfa ");
                        x.CurrentPageNumber();
                    });
                });
            });

            var pdfBytes = document.GeneratePdf();
            return File(pdfBytes, "application/pdf", "IstekListeleri.pdf");
        }
    }
}
