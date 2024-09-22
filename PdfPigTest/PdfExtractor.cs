namespace PdfPigTest
{
    using UglyToad.PdfPig;
    using UglyToad.PdfPig.Content;
    using UglyToad.PdfPig.Core;
    using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;
    using UglyToad.PdfPig.DocumentLayoutAnalysis;

    public class PdfExtractor
    {
        public bool EnableConsole { get; set; } = false;

        public LayoutAnalysisMode Mode { get; set; } = LayoutAnalysisMode.Docstrum;

        public PdfExtractor()
        {

        }

        public void Process(string filename)
        {
            using (var document = PdfDocument.Open(filename))
            {
                for (var i = 0; i < document.NumberOfPages; i++)
                {
                    if (EnableConsole)
                    {
                        Console.WriteLine("");
                        Console.WriteLine("");
                        Console.WriteLine("Page " + i);
                    }

                    var page = document.GetPage(i + 1);
                    var words = page.GetWords();

                    // Use default parameters
                    // - mode of letters' height and width used as gap size
                    // - no minimum block width 

                    IReadOnlyList<TextBlock> blocks;

                    #region Process

                    switch (Mode)
                    {
                        case LayoutAnalysisMode.Docstrum:

                            var docstrum = new DocstrumBoundingBoxes(new DocstrumBoundingBoxes.DocstrumBoundingBoxesOptions()
                            {
                                WithinLineBounds = new DocstrumBoundingBoxes.AngleBounds(-45, 45),
                                BetweenLineBounds = new DocstrumBoundingBoxes.AngleBounds(35, 170),
                                BetweenLineMultiplier = 2.5
                            });

                            blocks = docstrum.GetBlocks(words);
                            break;

                        case LayoutAnalysisMode.RecursiveXyCut:

                            var recursiveXYCut = new RecursiveXYCut(new RecursiveXYCut.RecursiveXYCutOptions()
                            {
                                MinimumWidth = page.Width / 3.0,
                                DominantFontWidthFunc = _ => (page.Letters.Average(l => l.GlyphRectangle.Width) * 2),
                                DominantFontHeightFunc = _ => (page.Letters.Average(l => l.GlyphRectangle.Height) * 2)
                            });

                            blocks = recursiveXYCut.GetBlocks(words);
                            break;

                        default:
                            throw new ArgumentException("Unsupported analysis mode: " + Mode.ToString());
                    }

                    #endregion

                    for (int j = 0; j < blocks.Count; j++)
                    {
                        if (EnableConsole)
                        {
                            if (j > 0) Console.WriteLine(Environment.NewLine);
                            Console.WriteLine("Block " + j);
                        }

                        TextBlock block = blocks[j];

                        // Do something
                        // E.g. Output the blocks
                        foreach (TextLine line in block.TextLines)
                        {
                            foreach (Word word in line.Words)
                            {
                                if (EnableConsole) Console.Write(word.Text + " ");
                            }
                        }
                    }
                }
            }
        }
    }
}
