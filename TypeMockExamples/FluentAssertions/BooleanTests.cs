using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TypeMockExamples.FluentAssertions
{
    [TestClass]
    public class FluentBooleanTests
    {
        [TestMethod]
        public void TestBooleans()
        {
            bool theBoolean = false;
            theBoolean.Should().BeFalse("it's set to false");

            theBoolean = true;
            theBoolean.Should().BeTrue();
            //theBoolean.Should().Be(otherBoolean);
        }
    }
}