
namespace TypeMockExamples.FluentAssertionsTests
{
    using System;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class FluentDateTimeTests
    {
        [TestMethod]
        public void TestDateTime()
        {
            DateTime theDatetime = 1.March(2010).At(22, 15);

            theDatetime.Should().BeAfter(1.February(2010));
            theDatetime.Should().BeBefore(2.March(2010));
            theDatetime.Should().BeOnOrAfter(1.March(2010));

            theDatetime.Should().Be(1.March(2010).At(22, 15));
            //theDatetime.Should().NotBe(1.March(2010).At(22, 15));

            theDatetime.Should().HaveDay(1);
            theDatetime.Should().HaveMonth(3);
            theDatetime.Should().HaveYear(2010);
            theDatetime.Should().HaveHour(22);
            theDatetime.Should().HaveMinute(15);
            theDatetime.Should().HaveSecond(0);

            //theDatetime.Should().BeLessThan(10.Minutes()).Before(otherDatetime); // Equivalent to <
            //theDatetime.Should().BeWithin(2.Hours()).After(otherDatetime);       // Equivalent to <=
            //theDatetime.Should().BeMoreThan(1.Days()).Before(deadline);          // Equivalent to >
            //theDatetime.Should().BeAtLeast(2.Days()).Before(deliveryDate);       // Equivalent to >=
            //theDatetime.Should().BeExactly(24.Hours()).Before(appointement);     // Equivalent to ==
        }
    }
}