using MyHomework.Services;
using MyHomework.UnitTests;

namespace ApiServiceTests
{
    public class Tests : TestBase
    {
        [Test]
        public void Constructor()
        {
            RunGuardClauseChecks<ApiService>();
        }
    }
}