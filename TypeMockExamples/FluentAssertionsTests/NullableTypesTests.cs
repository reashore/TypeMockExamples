
namespace TypeMockExamples.FluentAssertionsTests
{
    using System;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class FluentNullableTypesTests
    {
        [TestMethod]
        public void TestNullableTypes()
        {
            short? theShort = null;
            theShort.Should().NotHaveValue();

            int? theInt = 3;
            theInt.Should().HaveValue();

            DateTime? theDate = null;
            theDate.Should().NotHaveValue();
        }
    }
}