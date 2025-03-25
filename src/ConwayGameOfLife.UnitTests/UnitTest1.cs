using FluentAssertions;

namespace ConwayGameOfLife.UnitTests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var testSuccess = true;
            testSuccess.Should().BeTrue();
        }
    }
}