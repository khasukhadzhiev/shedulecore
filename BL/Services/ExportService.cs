using BL;
using DinkToPdf;
using DinkToPdf.Contracts;
using DTL.Dto;
using System.Runtime.InteropServices;
using BL.ServiceInterface;
using ClosedXML.Excel;
using HtmlAgilityPack;

namespace BL.Services
{
    public class ExportService : IExportService
    {
        private readonly IConverter _convertor;
        public ExportService(IConverter convertor)
        {
            _convertor = convertor;
        }

        ///<inheritdoc/>
        public async Task<Stream> SaveTimetableToPdfAsync(SavFileModelDto saveFileModelDto)
        {
            string style = @"<style>
                            .timetable{border-spacing:0px;border-collapse: collapse; text-align:center; margin: 0px;}
                            .timetable th {  border: 1px solid black;  padding: 1px;  font-size: 10px;}
                            .timetable th:nth-child(1) {  border: 1px solid black;  padding: 1px;  max-width: 30px;}
                            .timetable th:nth-child(3) {  width: 150px;}
                            .timetable tr,td {  border: 1px solid black; border-spacing:0px;border-collapse: collapse;}
                            .timetable tr {  height: 1.15rem;}
                            .td-style {  max-width: 6rem;  width: 6rem;  min-width: 6rem;  height: .5rem;  font-size: 8px;}
                            .rotated 
                                    {  
                                        display: block;  
                                        position: relative;  
                                        max-width: 50px;  
                                        -ms-transform: rotate(270deg); /* IE 9 */  
                                        -webkit-transform: rotate(270deg); /* Chrome, Safari, Opera */  
                                        transform: rotate(270deg);
                                    }
                            .timetable-border {  border: 1px solid black;}
                            .timetable-border-right {  border-right: 1px solid black;}
                            .timetable-border-bottom {  border-bottom: 1px solid black;}
                            </style>";

            Task<Stream> task = new Task<Stream>(() =>
            {
                var context = new CustomAssemblyLoadContext();
                var coreApiPuth = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

                var libraryPuth = coreApiPuth ;

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    libraryPuth += @"\libwkhtmltox.so";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    libraryPuth += @"\libwkhtmltox.dylib";
                }
                else
                {
                    libraryPuth += @"\libwkhtmltox.dll";
                }
                                
                context.LoadUnmanagedLibrary(libraryPuth);

                var doc = new HtmlToPdfDocument()
                {
                    GlobalSettings = {
                        ColorMode = ColorMode.Color,
                        Orientation = Orientation.Landscape,
                        PaperSize = PaperKind.A4,
                        DPI = 300,
                    },
                    Objects = {
                        new ObjectSettings() {
                            PagesCount = true,
                            HtmlContent = saveFileModelDto.Html + style,
                            WebSettings = { DefaultEncoding = "utf-8" },

                        }
                    }
                };

                byte[] pdf = _convertor.Convert(doc);

                var pdfStream = new MemoryStream(pdf);

                return pdfStream;
            });

            task.Start();

            return await task;
        }

        ///<inheritdoc/>
        public async Task<Stream> SaveTimetableReportingToPdfAsync(SavFileModelDto saveFileModelDto)
        {
            string style = @"<style>
                            .reporting-timetable { border-spacing:0px; border-collapse: collapse; text-align:center; border: 1px solid rgb(105, 105, 105); padding: 5px;}
                            .reporting-timetable th, td {text-align:center; border: 1px solid rgb(105, 105, 105); padding: 5px;}
                            .reporting-timetable td:nth-child(2), td:nth-child(6), td:nth-child(7){ white-space: nowrap;}
                            .reporting-timetable td:nth-child(3), td:nth-child(4){ text-align: left;}
                            </style>";

            Task<Stream> task = new Task<Stream>(() =>
            {
                var context = new CustomAssemblyLoadContext();
                var coreApiPuth = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

                var libraryPuth = coreApiPuth;

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    libraryPuth += @"\libwkhtmltox.so";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    libraryPuth += @"\libwkhtmltox.dylib";
                }
                else
                {
                    libraryPuth += @"\libwkhtmltox.dll";
                }

                context.LoadUnmanagedLibrary(libraryPuth);

                var doc = new HtmlToPdfDocument()
                {
                    GlobalSettings = {
                        ColorMode = ColorMode.Color,
                        Orientation = Orientation.Landscape,
                        PaperSize = PaperKind.A4,
                        DPI = 300,
                    },
                    Objects = {
                        new ObjectSettings() {
                            PagesCount = true,
                            HtmlContent = saveFileModelDto.Html + style,
                            WebSettings = { DefaultEncoding = "utf-8" },
                        }
                    }
                };

                byte[] pdf = _convertor.Convert(doc);

                var pdfStream = new MemoryStream(pdf);

                return pdfStream;
            });

            task.Start();

            return await task;
        }

        ///<inheritdoc/>
        public async Task<Stream> SaveTimetableToXlsxAsync(SavFileModelDto saveFileModelDto)
        {
            return await Task.Run(() =>
            {
                var memoryStream = new MemoryStream();

                string sheetName = saveFileModelDto.Name.Length > 25 ? saveFileModelDto.Name.Substring(0, 25) : saveFileModelDto.Name;

                // Парсинг HTML
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(saveFileModelDto.Html);

                // Создание Excel-книги и рабочего листа
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add(sheetName);

                    // Обработка HTML-контента и заполнение рабочего листа
                    var tables = htmlDoc.DocumentNode.SelectNodes("//table");

                    int currentRow = 1;

                    if (tables != null)
                    {
                        foreach (var table in tables)
                        {
                            // Обработка каждой таблицы
                            ProcessHtmlTable(table, worksheet, ref currentRow);
                        }
                    }

                    // Убедимся, что MemoryStream пуст перед сохранением
                    memoryStream.SetLength(0);

                    // Сохраняем книгу в MemoryStream в формате XLSX
                    workbook.SaveAs(memoryStream);

                    // Сбрасываем позицию в начале потока
                    memoryStream.Position = 0;
                }

                return memoryStream;
            });
        }

        ///<inheritdoc/>
        public async Task<Stream> SaveTimetableReportingToXlsxAsync(SavFileModelDto saveFileModelDto)
        {
            return await Task.Run(() =>
            {
                var memoryStream = new MemoryStream();

                string sheetName = saveFileModelDto.Name.Length > 25 ? saveFileModelDto.Name.Substring(0, 25) : saveFileModelDto.Name;

                // Парсинг HTML
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(saveFileModelDto.Html);

                // Создание Excel-книги и рабочего листа
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add(sheetName);

                    // Обработка HTML-контента и заполнение рабочего листа
                    var tables = htmlDoc.DocumentNode.SelectNodes("//table");

                    int currentRow = 1;

                    if (tables != null)
                    {
                        foreach (var table in tables)
                        {
                            // Обработка каждой таблицы
                            ProcessHtmlTable(table, worksheet, ref currentRow);
                        }
                    }

                    // Убедимся, что MemoryStream пуст перед сохранением
                    memoryStream.SetLength(0);

                    // Сохраняем книгу в MemoryStream в формате XLSX
                    workbook.SaveAs(memoryStream);

                    // Сбрасываем позицию в начале потока
                    memoryStream.Position = 0;
                }

                return memoryStream;
            });
        }

        private void ProcessHtmlTable(HtmlNode tableNode, IXLWorksheet worksheet, ref int currentRow)
        {
            var rowNodes = tableNode.SelectNodes(".//tr");
            if (rowNodes == null)
                return;

            var occupiedCells = new HashSet<(int Row, int Col)>();

            int rowIndex = currentRow;

            int maxColIndex = 0;

            foreach (var rowNode in rowNodes)
            {
                int colIndex = 1;

                var cellNodes = rowNode.SelectNodes("./th|./td");
                if (cellNodes == null)
                {
                    rowIndex++;
                    continue;
                }

                foreach (var cellNode in cellNodes)
                {
                    // Корректировка для занятых ячеек
                    while (occupiedCells.Contains((rowIndex, colIndex)))
                    {
                        colIndex++;
                    }

                    int rowSpan = GetAttributeValueAsInt(cellNode, "rowspan", 1);
                    int colSpan = GetAttributeValueAsInt(cellNode, "colspan", 1);

                    var cell = worksheet.Cell(rowIndex, colIndex);

                    // Установка значения ячейки
                    cell.Value = cellNode.InnerText.Trim();

                    // Обновление максимального индекса столбца
                    if (colIndex + colSpan - 1 > maxColIndex)
                        maxColIndex = colIndex + colSpan - 1;

                    // Применение форматирования ячейки
                    ApplyCellFormatting(cell, cellNode);

                    // Обработка объединенных ячеек
                    if (rowSpan > 1 || colSpan > 1)
                    {
                        var lastCell = worksheet.Cell(rowIndex + rowSpan - 1, colIndex + colSpan - 1);
                        worksheet.Range(cell, lastCell).Merge();

                        // Применяем границы к объединенному диапазону
                        var mergedRange = worksheet.Range(cell, lastCell);
                        ApplyBordersToRange(mergedRange);
                    }

                    // Отмечаем занятые ячейки
                    for (int i = 0; i < rowSpan; i++)
                    {
                        for (int j = 0; j < colSpan; j++)
                        {
                            occupiedCells.Add((rowIndex + i, colIndex + j));
                        }
                    }

                    // Увеличиваем colIndex на величину colSpan
                    colIndex += colSpan;
                }

                rowIndex++;
            }

            int lastRowIndex = rowIndex - 1;

            // Применение границ ко всем пустым ячейкам в таблице
            ApplyBordersToEmptyCells(worksheet, currentRow, lastRowIndex, maxColIndex);

            currentRow = rowIndex;
        }

        private void ApplyBordersToRange(IXLRange range)
        {
            // Устанавливаем тонкие границы вокруг диапазона
            range.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
            range.Style.Border.SetInsideBorderColor(XLColor.Black);

            range.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            range.Style.Border.SetOutsideBorderColor(XLColor.Black);
        }

        private void ApplyBordersToEmptyCells(IXLWorksheet worksheet, int startRow, int endRow, int maxColIndex)
        {
            for (int row = startRow; row <= endRow; row++)
            {
                for (int col = 1; col <= maxColIndex; col++)
                {
                    var cell = worksheet.Cell(row, col);
                    if (cell.IsEmpty() && !cell.IsMerged())
                    {
                        // Устанавливаем тонкие границы для пустой ячейки
                        cell.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        cell.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        cell.Style.Border.RightBorder = XLBorderStyleValues.Thin;

                        cell.Style.Border.TopBorderColor = XLColor.Black;
                        cell.Style.Border.BottomBorderColor = XLColor.Black;
                        cell.Style.Border.LeftBorderColor = XLColor.Black;
                        cell.Style.Border.RightBorderColor = XLColor.Black;
                    }
                }
            }
        }

        private int GetAttributeValueAsInt(HtmlNode node, string attributeName, int defaultValue)
        {
            if (node.Attributes[attributeName] != null)
            {
                if (int.TryParse(node.Attributes[attributeName].Value, out int value))
                {
                    return value;
                }
            }
            return defaultValue;
        }

        private void ApplyCellFormatting(IXLCell cell, HtmlNode cellNode)
        {
            IXLRange range;
            if (cell.IsMerged())
            {
                range = cell.MergedRange();
            }
            else
            {
                range = cell.AsRange();
            }

            // Применение тонких границ вокруг диапазона
            range.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
            range.Style.Border.SetInsideBorderColor(XLColor.Black);

            range.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            range.Style.Border.SetOutsideBorderColor(XLColor.Black);

            // Применение дополнительного форматирования при необходимости
            if (cellNode.Attributes["style"] != null)
            {
                var style = cellNode.Attributes["style"].Value;
                //ApplyStyle(range, style);
            }
        }

    }
}
