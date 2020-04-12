using NUnit.Framework;

namespace Tests
{
    public class PostScriptValidatorTest
    {
        [Test]
        public void ShouldDetectCompliantPostscript()
        {
            using (var postscriptValidator = new PostScriptValidator.PostScriptValidator())
            {
                var result = postscriptValidator.Validate(@"./TestData/valid.ps");
                Assert.True(result);
            }
        }

        [Test]
        public void ShouldDetectNonCompliantPostscript()
        {
            using (var postscriptValidator = new PostScriptValidator.PostScriptValidator())
            {
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

        [Test]
        public void ShouldNotFailOnMultipleDisposeWithoutEvenUsingOnceCalls()
        {
            var postscriptValidator = new PostScriptValidator.PostScriptValidator();
            postscriptValidator.Dispose();
            postscriptValidator.Dispose();
        }
    }
}