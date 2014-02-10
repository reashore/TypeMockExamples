using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TypeMockExamples.FluentAssertions
{
    [TestClass]
    public class NullableTypesTests
    {
        [TestMethod]
        public void TestMethod1()
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