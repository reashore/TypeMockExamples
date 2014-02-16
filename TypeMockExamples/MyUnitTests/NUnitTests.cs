
namespace TypeMockExamples.MyUnitTests
{
    using NUnit.Framework;

    [TestFixture]
    public class NUnitTests
    {
        [Test]
        public void TestNUnitTest1()
        {
            Assert.IsTrue(true);
        }

        [Test]
        [Ignore]
        public void TestNUnitTest2()
        {
            Assert.IsTrue(true);
        }
    }
}
