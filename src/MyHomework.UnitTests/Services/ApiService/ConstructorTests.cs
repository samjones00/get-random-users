using MyHomework.Services;
using MyHomework.UnitTests;

namespace ApiServiceTests
{
    public class ConstructorTests : TestBase
    {
        [Test]
        public void Constructor()
        {
            RunGuardClauseChecks<ApiService>();
        }
    }
}