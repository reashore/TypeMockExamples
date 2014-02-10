
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace TypeMockExamples.FluentAssertions
{
    [TestClass]
    public class ReferenceTypeTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            object theObject = null;
            theObject.Should().BeNull("because the value is null");

            theObject = "whatever";
            theObject.Should().BeOfType<String>("because a {0} is set", typeof(String));
            theObject.Should().NotBeNull();

            const string otherObject = "whatever";
            theObject.Should().Be(otherObject, "because they have the same values");

            theObject = otherObject;
            theObject.Should().BeSameAs(otherObject);
            //theObject.Should().NotBeSameAs(otherObject);

            ArgumentException ex = new ArgumentException();
            ex.Should().BeAssignableTo<Exception>("because it is an exception");

            object dummy = new Object();
            dummy.Should().Match(d => (d.ToString() == "System.Object"));
            //dummy.Should().Match<string>(d => (d == "System.Object"));
            //dummy.Should().Match((string d) => (d == "System.Object"));
        }
    }

    [TestClass]
    public class CollectionTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            IEnumerable collection = new[] { 1, 2, 5, 8 };

            collection.Should().NotBeEmpty()
                 .And.HaveCount(4)
                 .And.ContainInOrder(new[] { 2, 5 })
                 .And.ContainItemsAssignableTo<int>();

            collection.Should().Equal(new List<int> { 1, 2, 5, 8 });
            collection.Should().Equal(1, 2, 5, 8);
            collection.Should().BeEquivalentTo(8, 2, 1, 5);
            //collection.Should().NotBeEquivalentTo(8, 2, 3, 5);

            collection.Should().HaveCount(c => c > 3).And.OnlyHaveUniqueItems();
            collection.Should().HaveSameCount(new[] { 6, 2, 0, 5 });

            collection.Should().BeSubsetOf(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, });
            collection.Should().Contain(8).And.HaveElementAt(2, 5).And.NotBeSubsetOf(new[] { 11, 56 });
            //collection.Should().Contain(x => x > 3);
            //collection.Should().Contain(collection, 5, 6); // It should contain the original items, plus 5 and 6.
            //collection.Should().OnlyContain(x => x < 10);
            //collection.Should().OnlyContainItemsOfType<int>();
            collection.Should().NotContain(82);
            collection.Should().NotContainNulls();
            //collection.Should().NotContain(x => x > 10);

            collection = new int[0];
            collection.Should().BeEmpty();
            collection.Should().BeInAscendingOrder();
            //collection.Should().NotBeInAscendingOrder();
            //collection.Should().IntersectWith(otherCollection);
            //collection.Should().NotIntersectWith(otherCollection);
            //collection.Should().ContainInOrder(new[] { 1, 5, 8 });
        }
    }

}
