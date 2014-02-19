
using TypeMockExamples.Properties;

namespace TypeMockExamples.TypeMockUnitTests.PrivateMethods
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeMock.ArrangeActAssert;

    // This unit tests demonstrate faking non-public (private, protected, or internal) methods, properties and indexers 
    // The behavior is configured using Isolate.NonPublic and verified using the Isolate.Verify.NonPublic 
    // Supported configuration:
    // 1) ReturnRecursiveFake
    // 2) WillReturn()
    // 3) IgnoreCall()
    // 4) WillThrow()
    // 5) CallOriginal()

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

        [TestMethod]
        public void PrivateMethodReturnRecursiveFake()
        {
            // arrange
            Isolate.NonPublic.WhenCalled(_dependency, "GetGuard").ReturnRecursiveFake<IGuard>();

            // act
            int result = _classUnderTest.CalculateAndAlert(1, 2, _dependency);

            // assert
            // 3 = 1 + 2
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
            // 6 = 1 + 2 + 3
            Assert.AreEqual(6, result);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void PrivateMethodThrow()
        {
            // arrange
            Exception exception = new Exception();
            Isolate.NonPublic.WhenCalled(_dependency, "InternalNumber").WillThrow(exception);

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
            // 6 = 1 + 2 + 3
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

        [TestMethod]
        public void PrivateStaticMethodIgnore()
        {
            // arrange
            Isolate.NonPublic.WhenCalled<Dependency>("CallGuard").IgnoreCall();

            // act
            int result = _classUnderTest.Calculate(1, 2);

            // assert
            // 3 = 1 + 2
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void PrivateMethodCallOriginal()
        {
            // arrange
            Dependency dependencyFake = Isolate.Fake.Instance<Dependency>();
            Isolate.NonPublic.WhenCalled(dependencyFake, "GetNumberFromDatabase").CallOriginal();

            // act
            // Note the icon that appears when dependencyFake is clicked
            int result = _classUnderTest.Calculate(1, 2, dependencyFake);

            // assert
            // 3 = 1 + 2
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void VerifyPrivateStaticMethodWasCalledWithAnyArg()
        {
            // arrange
            Isolate.NonPublic.WhenCalled<Dependency>("CallGuard").IgnoreCall();

            // act
            int result = _classUnderTest.Calculate(1, 2);

            // assert
            // 3 = 1 + 2
            Assert.AreEqual(3, result);
            Isolate.Verify.NonPublic.WasCalled(typeof(Dependency), "CallGuard");
        }

        [TestMethod]
        public void VerifyPrivateStaticMethodWasCalledWithExactArg()
        {
            // arrange
            Isolate.NonPublic.WhenCalled<Dependency>("CallGuard").IgnoreCall();

            // act
            int result = _classUnderTest.Calculate(1, 2);

            // assert
            Assert.AreEqual(3, result);
            Isolate.Verify.NonPublic.WasCalled(typeof(Dependency), "CallGuard").WithArguments("username", "password");
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
            Dependency.CheckSecurity("username", "password");

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
        private int PrivateProp { get; set; }

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