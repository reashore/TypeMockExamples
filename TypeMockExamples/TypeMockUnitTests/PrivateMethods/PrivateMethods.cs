
using TypeMockExamples.Properties;

namespace TypeMockExamples.TypeMockUnitTests.PrivateMethods
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeMock.ArrangeActAssert;

    /// <summary>
    /// This test class shows how to fake non-public (private, protected or internal) methods, properties and indexers.
    /// Controlling non-public members is done using the Isolate.NonPublic property, 
    /// and verifying using the Isolate.Verify.NonPublic property. 
    /// Access to non-public members is through a string of member name.
    /// Supported behaviors are:
    /// <list type="bullet">
    ///     <item>ReturnRecursiveFake - return a zero or equivalent, and return fake objects for reference types. The returned fake objects will behave in the same way.</item>
    ///     <item>WillReturn - specify a return value for the call. Only applicable for methods returning values.</item>
    ///     <item>IgnoreCall - this method will be ignored. Only applicable for void methods.</item>
    ///     <item>WillThrow - will throw an exception when the method is called.</item>
    ///     <item>CallOriginal - will call the method's original implementation.</item>
    /// </list>
    /// </summary>
    [TestClass]
    [Isolated]
    public class PrivateMethodTests
    {
        private ClassUnderTest _classUnderTest;
        private Dependency _dependency;

        [TestInitialize]
        public void InitializeTest()
        {
            _classUnderTest = new ClassUnderTest();
            _dependency = new Dependency();
        }

        [TestCleanup]
        public void CleanupTest()
        {
            _classUnderTest = null;
            _dependency = null;
        }

        [TestMethod]
        public void PrivateMethodReturnRecursiveFake()
        {
            // arrange
            Isolate.NonPublic.WhenCalled(_dependency, "GetGuard").ReturnRecursiveFake<IGuard>();

            // act
            int result = _classUnderTest.CalculateAndAlert(1, 2, _dependency);

            // assert
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void PrivateMethodReturn()
        {
            // arrange
            Isolate.NonPublic.WhenCalled(_dependency, "InternalNumber").WillReturn(3);

            // act
            int result = _classUnderTest.Calculate(1, 2, _dependency);

            // assert
            Assert.AreEqual(6, result);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "exception message")]
        public void PrivateMethodThrow()
        {
            // arrange
            Isolate.NonPublic.WhenCalled(_dependency, "InternalNumber").WillThrow(new Exception("exception message"));

            // act
            _classUnderTest.Calculate(1, 2, _dependency);

            // assert
            // exception was thrown
        }

        [TestMethod]
        public void PrivatePropertyReturn()
        {
            // arrange
            Isolate.NonPublic.Property.WhenGetCalled(_dependency, "PrivateProp").WillReturn(3);

            // act
            int result = _classUnderTest.CalculateFromProperty(1, 2, _dependency);

            // assert
            Assert.AreEqual(6, result);
        }

        [TestMethod]
        public void PrivatePropertyVerified()
        {
            // arrange
            Isolate.NonPublic.Property.WhenGetCalled(_dependency, "PrivateProp").WillReturn(3);

            // act
            _classUnderTest.CalculateFromProperty(1, 2, _dependency);

            // assert
            Isolate.Verify.NonPublic.Property.WasCalledGet(_dependency, "PrivateProp");
        }
    }

    [TestClass]
    [Isolated]
    public class PrivateMethodTestsWithFakes
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

        [TestMethod]
        public void PrivateStaticMethodIgnore()
        {
            // arrange
            Isolate.NonPublic.WhenCalled<Dependency>("CallGuard").IgnoreCall();

            // act
            int result = _classUnderTest.Calculate(1, 2);

            // assert
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void PrivateMethodCallOriginal()
        {
            // arrange
            Dependency dependencyFake = Isolate.Fake.Instance<Dependency>();
            // private works on public too
            Isolate.NonPublic.WhenCalled(dependencyFake, "GetNumberFromDatabase").CallOriginal();

            // act
            int result = _classUnderTest.Calculate(1, 2, dependencyFake);

            // assert
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void VerifyPrivateStaticMethodWasCalledWithAnyArg()
        {
            // arrange
            Isolate.NonPublic.WhenCalled<Dependency>("CallGuard").IgnoreCall();

            // act
            _classUnderTest.Calculate(1, 2);

            // assert
            Isolate.Verify.NonPublic.WasCalled(typeof(Dependency), "CallGuard");
        }

        [TestMethod]
        public void VerifyPrivateStaticMethodWasCalledWithExactArg()
        {
            // arrange
            Isolate.NonPublic.WhenCalled<Dependency>("CallGuard").IgnoreCall();

            // act
            _classUnderTest.Calculate(1, 2);

            // assert
            Isolate.Verify.NonPublic.WasCalled(typeof(Dependency), "CallGuard").WithArguments("typemock", "rocks");
        }
    }

    // **** Classes under test ****

    public class ClassUnderTest
    {
        public int Calculate(int a, int b, Dependency dependency)
        {
            return a + b + dependency.GetNumberFromDatabase();
        }

        public int CalculateAndAlert(int a, int b, Dependency dependency)
        {
            dependency.Alert();
            return a + b;
        }

        public int Calculate(int a, int b)
        {
            Dependency.CheckSecurity("typemock", "rocks");

            return a + b;
        }

        public int CalculateFromProperty(int a, int b, Dependency dependency)
        {
            return a + b + dependency.GetNumberFromProperty();
        }
    }

    public interface IGuard
    {
        void Alert();
    }

    public class Dependency
    {
        private int PrivateProp { get; [UsedImplicitly] set; }

        public static void CheckSecurity(string name, string password)
        {
            CallGuard(name, password);
        }

        public void Alert()
        {
            GetGuard("unit", "testing");
        }

        public int GetNumberFromDatabase()
        {
            return InternalNumber();
        }

        public int GetNumberFromProperty()
        {
            return PrivateProp;
        }

        // private methods called by public methods

        private static void CallGuard(string name, string password)
        {
            string message = string.Format("username = {0}, Password = {1}", name, password);
            throw new NotImplementedException(message);
        }

        private IGuard GetGuard(string name, string password)
        {
            string message = string.Format("username = {0}, Password = {1}", name, password);
            throw new NotImplementedException(message);
        }

        private int InternalNumber()
        {
            throw new NotImplementedException();
        }
    }
}