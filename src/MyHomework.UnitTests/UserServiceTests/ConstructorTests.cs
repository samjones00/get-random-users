using MyHomework.Services;
using MyHomework.UnitTests;

namespace UserServiceTests
{
    public class Tests : TestBase
    {
        [Test]
        public void Constructor()
        {
            RunGuardClauseChecks<UserService>();
        }
    }
}