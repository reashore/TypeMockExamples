
namespace TypeMockExamples.TypeMockUnitTests.BasicUnitTests
{
    using System;
    using System.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeMock.ArrangeActAssert;

    /// <summary>
    /// Basic examples of how to use Typemock Isolator Syntax.
    /// The concept behind Arrange-Act-Assert is simple; we aspire for the test code to be divided into three stages:
    ///     - Arrange: here we set up our test objects and their behavior for the test duration
    ///     - Act: here we run the method under test using the test objects we created earlier
    ///     - Assert: here we verify that the outcome of running the test code with the test set up yielded the expected results    
    /// </summary>
    [TestClass]
    [Isolated(DesignMode.Pragmatic)] // Note: Use Isolated to clean up after the test. Faking static methods requires Pragmatic mode
    public class BasicUnitTestingTests
    {
        [TestMethod]
        public void FakingDateTime()
        {
            // Arrange
            DateTime futureDateTime = new DateTime(2016, 2, 29);
            Isolate.WhenCalled(() => DateTime.Now).WillReturn(futureDateTime);

            // Act 
            int result = ClassUnderTest.Return1234OnFutureDate(futureDateTime);

            // Assert 
            Assert.AreEqual(1234, result);
        }

        [TestMethod]
        public void FakeAConcreteObjectExample()
        {
            // Arrange - Fake a Process, default is that all Members.ReturnRecursiveFakes 
            Process processFake = Isolate.Fake.Instance<Process>();
            Isolate.WhenCalled(() => processFake.MainModule.Site.Name).WillReturn("Typemock rocks");

            // Act 
            bool result = ClassUnderTest.IsMySiteNameTypemock(processFake);

            // Assert 
            Assert.AreEqual(true, result);
        }
    }

    //------------------
    // Classes under test
    // Class that are Dependant on DateTime and Process and needs to be isolated from them to be unit tested
    //------------------

    public class ClassUnderTest
    {
        public static int Return1234OnFutureDate(DateTime dateTime)
        {
            if (DateTime.Now == dateTime)
            {
                return 1234;
            }

            return 0;
        }

        public static bool IsMySiteNameTypemock(Process process)
        {
            if (process.MainModule.Site.Name.StartsWith("Typemock"))
            {
                return true;
            }

            return false;
        }
    }
}