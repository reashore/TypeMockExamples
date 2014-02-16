
namespace TypeMockExamples.MyUnitTests
{
    using System.Diagnostics;
    using NUnit.Framework;

    [SetUpFixture]
    public class TestSetUpClass
    {
        [SetUp]
        public void RunBeforeAnyTests()
        {
	        Debug.WriteLine("Test run starting.");
        }

        [TearDown]
        public void RunAfterAnyTests()
        {
            Debug.WriteLine("Test run ending.");
        }
    }

    [TestFixture]
    public class NUnitTests
    {
        [Test]
        [Category("Basic")]
        public void TestNUnitTest1()
        {
            // assert
            Assert.IsTrue(true);
        }

        [Test]
        [Ignore]
        [Category("Ignored")]
        public void TestNUnitTest2()
        {
            // assert
            Assert.IsTrue(true);
        }

        [Test]
        [TestCase(1, 2, 3)]
        [TestCase(4, 5, 9)]
        [TestCase(3, 5, 8)]
        [TestCase(2, 5, 7)]
        [Category("Composite")]
        public void TestNUnitTest3(int x, int y, int expectedSum)
        {
            // arrange
            Calculator calculator = new Calculator();

            //act
            int actualSum = calculator.Add(x, y);

            // assert
            Assert.AreEqual(expectedSum, actualSum);
        }
    }

    // Domain

    public class Calculator
    {
        public int Add(int x, int y)
        {
            return x + y;
        }
    }
}
