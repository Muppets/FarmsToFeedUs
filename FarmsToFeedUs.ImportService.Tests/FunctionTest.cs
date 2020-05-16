using Amazon.Lambda.TestUtilities;
using System.Threading.Tasks;
using Xunit;

namespace FarmsToFeedUs.ImportService.Tests
{
    public class FunctionTest
    {
        [Fact]
        public async Task TestFunction()
        {
            var context = new TestLambdaContext();
            var function = new Function();

            await function.FunctionHandler(context);

            var testLogger = context.Logger as TestLambdaLogger;
            Assert.Contains("Completed import service", testLogger?.Buffer.ToString());
        }
    }
}
