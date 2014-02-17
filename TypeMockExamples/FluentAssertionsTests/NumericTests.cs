
namespace TypeMockExamples.FluentAssertionsTests
{
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class FluentNumericTests
    {
        [TestMethod]
        public void TestNumericTypes()
        {
            int theInt = 5;
            theInt.Should().BeGreaterOrEqualTo(5);
            theInt.Should().BeGreaterOrEqualTo(3);
            theInt.Should().BeGreaterThan(4);
            theInt.Should().BeLessOrEqualTo(5);
            theInt.Should().BeLessThan(6);
            theInt.Should().BePositive();
            theInt.Should().Be(5);
            theInt.Should().NotBe(10);
            theInt.Should().BeInRange(1, 10);

            theInt = -8;
            theInt.Should().BeNegative();

            int? nullableInt = 3;
            nullableInt.Should().Be(3);

            const double theDouble = 5.1;
            theDouble.Should().BeGreaterThan(5);

            const byte theByte = 2;
            theByte.Should().Be(2);
        }
    }
}