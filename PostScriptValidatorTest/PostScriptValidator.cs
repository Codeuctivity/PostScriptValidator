using System.IO;
using NUnit.Framework;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ShouldDetectCompliantPostscript()
        {
            using (var postscriptValidator = new PostScriptValidator.PostScriptValidator())
            {
                Assert.True(File.Exists(@"./TestData/valid.ps"));
                var result = postscriptValidator.Validate(@"./TestData/valid.ps");
                Assert.True(result);
            }
        }
        [Test]
        public void ShouldDetectNonCompliantPostscript()
        {
            using (var postscriptValidator = new PostScriptValidator.PostScriptValidator())
            {
                Assert.True(File.Exists(@"./TestData/invalid.ps"));
                var result = postscriptValidator.Validate(@"./TestData/invalid.ps");
                Assert.False(result);
            }
        }
        [Test]
        public void ShouldNotFailOnMultipleDisposeCalls()
        {
            var postscriptValidator = new PostScriptValidator.PostScriptValidator();
            postscriptValidator.Validate(@"./TestData/valid.ps");
            postscriptValidator.Dispose();
            postscriptValidator.Dispose();
        }
    }
}