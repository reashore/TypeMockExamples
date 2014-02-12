
namespace TypeMockExamples.FluentAssertions
{
    using global::FluentAssertions;
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

            const double TheDouble = 5.1;
            TheDouble.Should().BeGreaterThan(5);

            const byte TheByte = 2;
            TheByte.Should().Be(2);
        }
    }
}