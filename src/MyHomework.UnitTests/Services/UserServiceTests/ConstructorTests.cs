using MyHomework.Services;
using MyHomework.UnitTests;

namespace UserServiceTests
{
    public class ConstructorTests : TestBase
    {
        [Test]
        public void Constructor()
        {
            RunGuardClauseChecks<UserService>();
        }
    }
}