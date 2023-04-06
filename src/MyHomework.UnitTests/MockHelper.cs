using AutoFixture;

namespace MyHomework.UnitTests
{
    public static class MockHelper
    {
        public static T Create<T>(params Action<T>[] properties) where T : class
        {
            var result = new Fixture().Create<T>();

            foreach (var property in properties)
            {
                property(result);
            }

            return result;
        }
    }
}