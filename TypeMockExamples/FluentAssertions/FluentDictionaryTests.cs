
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TypeMockExamples.FluentAssertions
{
    [TestClass]
    public class FluentDictionaryTests
    {
        [TestMethod]
        public void TestDictionary()
        {
            Dictionary<int, string> dictionary = null;
            dictionary.Should().BeNull();
            dictionary = new Dictionary<int, string>();
            dictionary.Should().BeEmpty();

            Dictionary<int, string> dictionary1 = new Dictionary<int, string>
            {
                {1, "One"},
                {2, "Two"}
            };

            Dictionary<int, string> dictionary2 = new Dictionary<int, string>
            {
                {1, "One"},
                {2, "Two"}
            };

            Dictionary<int, string> dictionary3 = new Dictionary<int, string>
            {
                {3, "Three"},
            };

            dictionary1.Should().NotBeNull();
            dictionary1.Should().NotBeEmpty();
            dictionary1.Should().Equal(dictionary2);
            dictionary1.Should().NotEqual(dictionary3);
            dictionary1.Should().ContainKey(1);
            dictionary1.Should().NotContainKey(9);
            dictionary1.Should().ContainValue("One");
            dictionary1.Should().NotContainValue("Nine");
            dictionary1.Should().HaveCount(2);

            KeyValuePair<int, string> item = new KeyValuePair<int, string>(1, "One");

            dictionary1.Should().Contain(item);
            dictionary1.Should().Contain(2, "Two");
            dictionary1.Should().NotContain(9, "Nine");
        }
    }
}