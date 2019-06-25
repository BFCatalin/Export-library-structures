using ChemAxon.JChemSharePoint.Model.Chemistry.Data.ImportExport;
using ChemAxon.JChemSharePoint.Model.SP.Data;
using ChemAxon.JChemSharePoint.Services.Model.SP.ImportExport;
using CommandLine;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ExportLibraryStructures
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(opts => RunOptionsAndReturnExitCode(opts));
        }

        private static void RunOptionsAndReturnExitCode(Options opts)
        {
            try
            {
                using (var site = new SPSite(opts.Url))
                {
                    using (var web = site.OpenWeb())
                    {
                        var assembly = Assembly.GetAssembly(typeof(ExportManager));
                        var type = assembly.GetType("ChemAxon.JChemSharePoint.Services.Model.SP.Data.JChemAppContext");
                        var createContext = type.GetMethod("CreateContext", new Type[] { typeof(JChemContextInfo) });
                        JChemContextInfo ctxInfo = new JChemContextInfo(true, Environment.UserName,
                            Convert.ToBase64String(web.CurrentUser.UserToken.BinaryToken),
                            Guid.NewGuid(),
                            site.ID,
                            web.ID,
                            web.Locale,
                            web.Locale,
                            Guid.NewGuid());

                        using (DataContext ctx = (DataContext)createContext.Invoke(null, new object[] { ctxInfo }))
                        {
                            var list = web.GetListFromUrl(opts.Url);
                            var outputFile = string.IsNullOrWhiteSpace(opts.Output) ? list.Title : Path.GetFileNameWithoutExtension(opts.Output);
                            var fileFormat = string.IsNullOrWhiteSpace(opts.Format) ? "mrv" : opts.Format;
                            ExportManager exportManager = new ExportManager(ctx);
                            var exportOptions = new ExportOptions()
                            {
                                FileEncoding = "UTF-8",
                                FileName = outputFile,
                                IncludeNoStructures = false,
                                OutputFormat = fileFormat,
                                ListId = list.ID,
                                Columns = new List<ExportColumn>
                            {
                                new ExportColumn() {
                                    ColumnName = opts.ColumnName,
                                    IsMainStructureColumn = true,
                                    IsStructureColumn = true,
                                    IsSelectedForSaving = true
                                }
                            }
                            };
                            using (var stream = exportManager.ExportList(exportOptions))
                            {
                                using (var fs = new FileStream(string.Format("{0}.{1}", outputFile, fileFormat), FileMode.Create))
                                {
                                    stream.CopyTo(fs);
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error while exporting: ",ex.Message);
                Console.WriteLine("Stack: ", ex.StackTrace);
            }
        }
    }
}
