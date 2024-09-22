namespace PdfPigConsole
{
    using System;
    using GetSomeInput;
    using PdfPigTest;
    using Timestamps;

    public static class Program
    {
        private static PdfExtractor _Pdf = new PdfExtractor();
        private static bool _EnableConsole = true;
        private static LayoutAnalysisMode _Mode = LayoutAnalysisMode.RecursiveXyCut;

        public static void Main(string[] args)
        {
            _Pdf.EnableConsole = _EnableConsole;
            _Pdf.Mode = _Mode;

            while (true)
            {
                string filename = Inputty.GetString("Filename:", null, false);
                Timestamp ts = new Timestamp();
                _Pdf.Process(filename);
                Console.WriteLine(Environment.NewLine);
                ts.End = DateTime.UtcNow;
                Console.WriteLine("Total time: " + ts.TotalMs + "ms");
                Console.WriteLine(Environment.NewLine);
            }
        }
    }
}