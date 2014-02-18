﻿
namespace TypeMockExamples.TypeMockUnitTests.BasicUnitTests
{
    using System;
    using System.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeMock.ArrangeActAssert;

    [TestClass]
    [Isolated]
    public class BasicUnitTestingTests
    {
        private ClassUnderTest _classUnderTest;

        [TestInitialize]
        public void InitializeTest()
        {
            _classUnderTest = new ClassUnderTest();
        }

        [TestCleanup]
        public void CleanupTest()
        {
            _classUnderTest = null;
        }

        // These test demonstrate
        // 1) faking DateTime.Now()
        // 2) faking Process and configuring it
        // 3) the difference between Pragmatic and Interface DesignMode
        // 4) the default behavior of faked classes

        [TestMethod]
        public void FakingDateTime()
        {
            // Arrange
            DateTime futureDateTime = new DateTime(2016, 2, 29);
            Isolate.WhenCalled(() => DateTime.Now).WillReturn(futureDateTime);

            // Act 
            int result = _classUnderTest.Return1234OnFutureDate(futureDateTime);

            // Assert 
            Assert.AreEqual(1234, result);
        }

        [TestMethod]
        public void FakeAConcreteObjectExample()
        {
            // Arrange
            Process processFake = Isolate.Fake.Instance<Process>();
            Isolate.WhenCalled(() => processFake.MainModule.Site.Name).WillReturn("SpecialSiteName");

            // Act 
            bool result = _classUnderTest.IsSpecialSiteName(processFake);

            // Assert 
            Assert.AreEqual(true, result);
        }

        // The Isolated DesignMode constructor argument can be either:
        // 1) Pragmatic - The default, allows faking any type and method, including sealed, static, or private.
        // 2) InterfaceOnly - Alows faking only abstract or interface types, and public non-virtual methods, otherwise it throws a DesignModeException.
        [TestMethod]
        [Isolated(DesignMode.Pragmatic)]  // the default
        //[Isolated(DesignMode.InterfaceOnly)]
        public void PrivateMethodsAreNotVisibleInInterfaceMode()
        {
            // Arrange
            Isolate.NonPublic.WhenCalled(_classUnderTest, "PrivateMethodReturnsInteger").WillReturn(100);

            // Act 
            int result = _classUnderTest.PublicMethodReturnsInteger();

            // Assert 
            Assert.AreEqual(100, result);
        }

        [TestMethod]
        public void ShowIsolateMethodsAndProperties()
        {
            // Arrange
            DateTime futureDateTime = new DateTime(2016, 2, 29);
            Exception exception = new Exception();

            // **** Isolate Methods:

            // 1) WhenCalled()
            Isolate.WhenCalled(() => DateTime.Now).WillReturn(futureDateTime);

            // 2) GetFake()
            //Dependency dependencyFake = Isolate.GetFake<Dependency>(dependency);

            // 3) CleanUp()
            Isolate.CleanUp();


            // **** Isolate Properties:

            // 1) Fake
            Dependency dependencyFake = Isolate.Fake.Instance<Dependency>();

            // 2) Invoke
            Isolate.Invoke.Method(_classUnderTest, "PublicMethodReturnsInteger");

            // 3) NonPublic
            Isolate.NonPublic.WhenCalled(_classUnderTest, "PrivateMethodReturnsInteger").WillReturn(100);

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

    [TestClass]
    [Isolated]
    public class BasicUnitTestingTests2
    {
        [TestMethod]
        public void VerifyThatFakedMethodsReturnDefaultForThatType()
        {
            // arrange
            ClassUnderTest classUnderTestFake = Isolate.Fake.Instance<ClassUnderTest>();

            // Act 
            int result1 = classUnderTestFake.ReturnsInt();
            string result2 = classUnderTestFake.ReturnsString();
            double result3 = classUnderTestFake.ReturnsDouble();
            decimal result4 = classUnderTestFake.ReturnsDecimal();
            bool result5 = classUnderTestFake.ReturnsBool();
            DateTime result6 = classUnderTestFake.ReturnsDateTime();
            Dependency result7 = classUnderTestFake.ReturnsDependency();

            // Assert 
            Assert.AreEqual(default(int), result1);
            //Assert.AreEqual(default(string), result2);  // assert fails because default(string) is null
            Assert.AreEqual(string.Empty, result2);
            Assert.AreEqual(default(double), result3);
            Assert.AreEqual(default(decimal), result4);
            Assert.AreEqual(default(bool), result5);
            Assert.AreEqual(default(DateTime), result6);
            Assert.AreEqual(typeof(Dependency), result7.GetType());
        }
    }

    // **** Classes under test ****

    public class ClassUnderTest
    {
        public int Return1234OnFutureDate(DateTime dateTime)
        {
            return DateTime.Now == dateTime ? 1234 : 0;
        }

        public bool IsSpecialSiteName(Process process)
        {
            return process.MainModule.Site.Name.StartsWith("SpecialSiteName");
        }

        public int PublicMethodReturnsInteger()
        {
            return PrivateMethodReturnsInteger();
        }

        private int PrivateMethodReturnsInteger()
        {
            return -10;
        }

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

        public Dependency ReturnsDependency()
        {
            return new Dependency();
        }
    }

    public class Dependency
    {
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