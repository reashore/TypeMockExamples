﻿
namespace TypeMockExamples.FluentAssertionsTests
{
    using System;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class FluentReferenceTypeTests
    {
        [TestMethod]
        public void TestReferenceTypes()
        {
            object theObject = null;
            theObject.Should().BeNull("because the value is null");

            theObject = "whatever";
            theObject.Should().BeOfType<string>("because a {0} is set", typeof(string));
            theObject.Should().NotBeNull();

            const string otherObject = "whatever";
            theObject.Should().Be(otherObject, "because they have the same values");

            theObject = otherObject;
            theObject.Should().BeSameAs(otherObject);
            //theObject.Should().NotBeSameAs(otherObject);

            ArgumentException ex = new ArgumentException();
            ex.Should().BeAssignableTo<Exception>("because it is an exception");

            object dummy = new object();
            dummy.Should().Match(d => (d.ToString() == "System.Object"));
            //dummy.Should().Match<string>(d => (d == "System.Object"));
            //dummy.Should().Match((string d) => (d == "System.Object"));
        }
    }
}
