using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportLibraryStructures
{
    public class Options
    {
        [Option('u', "url", Required =true, HelpText = "URL of the library.")]
        public string Url { get; set; }

        [Option('c', "column", Required = true, HelpText = "The structure column name (as displayed in browser)")]
        public string ColumnName { get; set; }

        [Option('f', "format", Required = false, Default = "mrv", HelpText = "The file format (mrv or sdf)")]
        public string Format { get; set; }

        [Option('o', "output", Required = false, HelpText = "The output file name. If not specified the library name will be used instead.")]
        public string Output { get; set; }
    }
}
