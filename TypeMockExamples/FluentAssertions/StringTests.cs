using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TypeMockExamples.FluentAssertions
{
    [TestClass]
    public class FluentStringTests
    {
        [TestMethod]
        public void TestStrings()
        {
            string theString = null;
            theString.Should().BeNull();

            theString = "";
            theString.Should().BeEmpty();
            theString.Should().HaveLength(0);
            theString.Should().BeNullOrWhiteSpace();

            theString = "This is a String";
            theString.Should().NotBeEmpty("because the string is not empty");
            theString.Should().NotBeNull();
            theString.Should().NotBeNullOrWhiteSpace();
            theString.Should().Be("This is a String");
            theString.Should().NotBe("This is another String");
            theString.Should().BeEquivalentTo("THIS IS A STRING");
            theString.Should().EndWith("a String");
            theString.Should().EndWithEquivalent("a string");
            theString.Should().Contain("is a");
            //theString.Should().NotContain("is a");
            //theString.Should().ContainEquivalentOf("WE DONT CARE ABOUT THE CASING");
            //theString.Should().NotContainEquivalentOf("HeRe ThE CaSiNg Is IgNoReD As WeLl");
            theString.Should().StartWith("This");
            theString.Should().StartWithEquivalent("this");
        }
    }
}