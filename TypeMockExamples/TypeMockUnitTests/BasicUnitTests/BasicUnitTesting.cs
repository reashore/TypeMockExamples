
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
    [Isolated]
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
            Isolate.WhenCalled(() => processFake.MainModule.Site.Name).WillReturn("SpecialSiteName");

            // Act 
            bool result = ClassUnderTest.IsSpecialSiteName(processFake);

            // Assert 
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void VerifyThatFakedMethodsReturnDefaultForThatType()
        {
            // arrange
            Dependency dependencyFake = Isolate.Fake.Instance<Dependency>();

            // Act 
            int result1 = dependencyFake.ReturnsInt();
            string result2 = dependencyFake.ReturnsString();
            double result3 = dependencyFake.ReturnsDouble();
            decimal result4 = dependencyFake.ReturnsDecimal();
            bool result5 = dependencyFake.ReturnsBool();
            DateTime result6 = dependencyFake.ReturnsDateTime();

            // Assert 
            Assert.AreEqual(default(int), result1);
            //Assert.AreEqual(default(string), result2);  // assert fails because default(string) is null
            Assert.AreEqual(string.Empty, result2);
            Assert.AreEqual(default(double), result3);
            Assert.AreEqual(default(decimal), result4);
            Assert.AreEqual(default(bool), result5);
            Assert.AreEqual(default(DateTime), result6);
        }


        [TestMethod]
        public void ShowIsolateMethodsAndProperties()
        {
            // Arrange
            ClassUnderTest classUnderTest = new ClassUnderTest();
            DateTime futureDateTime = new DateTime(2016, 2, 29);
            Exception exception = new Exception();
            Dependency dependency1 = new Dependency();

            // **** Isolate Methods:
            // 1) WhenCalled()
            Isolate.WhenCalled(() => DateTime.Now).WillReturn(futureDateTime);

            // 2) GetFake()
            //Dependency dependencyFake = Isolate.GetFake<Dependency>(dependency1);

            // 3) CleanUp()
            Isolate.CleanUp();

            // **** Isolate Properties:
            // 1) Fake
            Dependency dependencyFake = Isolate.Fake.Instance<Dependency>();

            // 2) Invoke
            object result = Isolate.Invoke.Method(classUnderTest, "PublicMethodReturnsInteger");

            // 3) NonPublic
            Isolate.NonPublic.WhenCalled(classUnderTest, "PrivateMethodReturnsInteger").WillReturn(100);

            // 4) Swap
            Isolate.Swap.NextInstance<Dependency>().ConstructorWillThrow(exception);

            // 5) Verify
            Isolate.Verify.WasNotCalled(() => dependencyFake.PublicMethod());

            // Act 
            // not applicable

            // Assert 
            Assert.IsTrue(true);
        }
    }

    //Isolated DesignMode be either:
    //1) Pragmatic - Pragmatic mode, the default, allows faking any type and method, including sealed, static, or private.
    //2) InterfaceOnly - InterfaceOnly mode allows faking only abstract or interface types, and public non-virtual methods. Otherwise it throws a DesignModeException.
    [TestClass]
    [Isolated(DesignMode.Pragmatic)]
    //[Isolated(DesignMode.InterfaceOnly)]
    public class BasicUnitTestingTests2
    {
        [TestMethod]
        public void PrivateMethodsAreNotVisibleInInterfaceMode()
        {
            // Arrange
            ClassUnderTest classUnderTest = new ClassUnderTest();
            Isolate.NonPublic.WhenCalled(classUnderTest, "PrivateMethodReturnsInteger").WillReturn(100);

            // Act 
            int result = classUnderTest.PublicMethodReturnsInteger();

            // Assert 
            Assert.AreEqual(100, result);
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

        public static bool IsSpecialSiteName(Process process)
        {
            if (process.MainModule.Site.Name.StartsWith("SpecialSiteName"))
            {
                return true;
            }

            return false;
        }

        public int PublicMethodReturnsInteger()
        {
            return PrivateMethodReturnsInteger();
        }

        private int PrivateMethodReturnsInteger()
        {
            return -10;
        }

    }

    public class Dependency
    {
        public bool ReturnsBool()
        {
            return true;
        }

        public int ReturnsInt()
        {
            return 100;
        }

        public string ReturnsString()
        {
            return "string1";
        }

        public double ReturnsDouble()
        {
            return 123.456;
        }

        public decimal ReturnsDecimal()
        {
            return 100.00m;
        }

        public DateTime ReturnsDateTime()
        {
            return DateTime.Now;
        }

        public int PublicMethod()
        {
            return PrivateMethod();
        }

        private int PrivateMethod()
        {
            return 75;
        }
    }
}