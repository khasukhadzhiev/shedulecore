using BL;
using DinkToPdf;
using DinkToPdf.Contracts;
using DTL.Dto;
using System.Runtime.InteropServices;
using BL.ServiceInterface;

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
        public async Task<Stream> SaveTimetableToXlsxAsync(SavFileModelDto saveFileModelDto)
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
        public async Task<Stream> SaveScheduleToXlsxAsync(SavFileModelDto saveFileModelDto)
        {
            return await Task.Run(() =>
            {
                return new MemoryStream(); //Добавил пустышку.
            });
        }
    }
}
