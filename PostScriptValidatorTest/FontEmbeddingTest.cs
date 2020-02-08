using System;
using System.IO;
using NUnit.Framework;

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
                    var result = pdfAValidator.Validate(outputName);
                    Assert.True(result);
                    File.Delete(outputName);
                }
            }
        }
    }
}