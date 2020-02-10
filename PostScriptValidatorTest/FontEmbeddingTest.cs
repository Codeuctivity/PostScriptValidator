using System;
using System.IO;
using NUnit.Framework;
using System.Linq;

namespace Tests
{
    public class FontEmbeddingTest
    {
        [Test]
        public void ShouldEmbeddFonts()
        {
            using (var postscriptValidator = new PostScriptValidator.PostScriptValidator())
            {
                using (var pdfAValidator = new PdfAValidator.PdfAValidator())
                {
                    var outputName = Guid.NewGuid().ToString() + ".pdf";
                    postscriptValidator.EmbedFonts(@"./TestData/FontsNotEmbedded.pdf", outputName);
                    Assert.That(File.Exists(outputName));
                    var resultOutcome = pdfAValidator.ValidateWithDetailedReport(outputName);
                    Assert.False(resultOutcome.Jobs.Job.ValidationReport.Details.Rule.Any(_ => _.Clause == "6.3.5"));
                    File.Delete(outputName);
                }
            }
        }
    }
}