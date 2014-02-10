
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TypeMock.ArrangeActAssert;
using TypeMockExamples.Annotations;

namespace TypeMockExamples.TypeMockUnitTests.PrivateMethods
{
    /// <summary>
    /// This test class shows how to fake non-public (private, protected or internal) methods, properties and indexers.
    /// 
    /// Controlling non-public members is done using the Isolate.NonPublic property, 
    /// and verifying using the Isolate.Verify.NonPublic property. 
    /// Access to non-public members is through a string of member name.
    /// 
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
    [Isolated(DesignMode.Pragmatic)] // Note: Use Isolated to clean up after all tests in class
    public class PrivateMethodTests
    {
        [TestMethod]
        public void PrivateMethod_ReturnRecursiveFake()
        {
            // arrange
            Dependency realDependency = new Dependency();
            Isolate.NonPublic.WhenCalled(realDependency, "GetGuard").ReturnRecursiveFake<IGuard>();
            ClassUnderTest classUnderTest = new ClassUnderTest();

            // act
            int result = classUnderTest.CalculateAndAlert(1, 2, realDependency);

            // assert
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void PrivateMethod_Return()
        {
            // arrange
            Dependency realDependency = new Dependency();
            Isolate.NonPublic.WhenCalled(realDependency, "InternalNumber").WillReturn(3);
            ClassUnderTest classUnderTest = new ClassUnderTest();

            // act
            int result = classUnderTest.Calculate(1, 2, realDependency);

            // assert
            Assert.AreEqual(6, result);
        }

        [TestMethod]
        [ExpectedException(typeof (Exception), "Typemock rocks")]
        public void PrivateMethod_Throw()
        {
            // arrange
            Dependency realDependency = new Dependency();
            Isolate.NonPublic.WhenCalled(realDependency, "InternalNumber").WillThrow(new Exception("Typemock rocks"));
            ClassUnderTest classUnderTest = new ClassUnderTest();

            // act
            classUnderTest.Calculate(1, 2, realDependency);
        }

        [TestMethod]
        public void PrivateStaticMethod_Ignore()
        {
            // arrange
            Isolate.NonPublic.WhenCalled<Dependency>("CallGuard").IgnoreCall();
            ClassUnderTest classUnderTest = new ClassUnderTest();

            // act
            int result = classUnderTest.Calculate(1, 2);

            // assert
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void PrivateMethod_CallOriginal()
        {
            // arrange
            Dependency fakeDependency = Isolate.Fake.Instance<Dependency>();
            // private works on public too
            Isolate.NonPublic.WhenCalled(fakeDependency, "GetNumberFromDatabase").CallOriginal();
            ClassUnderTest classUnderTest = new ClassUnderTest();

            // act
            int result = classUnderTest.Calculate(1, 2, fakeDependency);

            // assert
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void PrivateProperty_Return()
        {
            // arrange
            Dependency realDependency = new Dependency();
            Isolate.NonPublic.Property.WhenGetCalled(realDependency, "PrivateProp").WillReturn(3);
            ClassUnderTest classUnderTest = new ClassUnderTest();

            // act
            int result = classUnderTest.CalculateFromProperty(1, 2, realDependency);

            // assert
            Assert.AreEqual(6, result);
        }

        [TestMethod]
        public void PrivateProperty_Verified()
        {
            // arrange
            Dependency realDependency = new Dependency();
            Isolate.NonPublic.Property.WhenGetCalled(realDependency, "PrivateProp").WillReturn(3);
            ClassUnderTest classUnderTest = new ClassUnderTest();

            // act
            classUnderTest.CalculateFromProperty(1, 2, realDependency);

            // assert
            Isolate.Verify.NonPublic.Property.WasCalledGet(realDependency, "PrivateProp");
        }

        [TestMethod]
        public void VerifyPrivateStaticMethod_WasCalledWithAnyArg()
        {
            // arrange
            Isolate.NonPublic.WhenCalled<Dependency>("CallGuard").IgnoreCall();
            ClassUnderTest classUnderTest = new ClassUnderTest();

            // act
            classUnderTest.Calculate(1, 2);

            // assert
            Isolate.Verify.NonPublic.WasCalled(typeof (Dependency), "CallGuard");
        }

        [TestMethod]
        public void VerifyPrivateStaticMethod_WasCalledWithExactArg()
        {
            // arrange
            Isolate.NonPublic.WhenCalled<Dependency>("CallGuard").IgnoreCall();
            ClassUnderTest classUnderTest = new ClassUnderTest();

            // act
            classUnderTest.Calculate(1, 2);

            // assert
            Isolate.Verify.NonPublic.WasCalled(typeof (Dependency), "CallGuard").WithArguments("typemock", "rocks");
        }
    }

    //------------------
    // Classes under test
    // - Dependency: Class with private Methods that need to be faked out
    // - ClassUnderTest: Class that uses Dependency
    // - IGuard: an unimplemented interface
    //------------------

    public interface IGuard
    {
        void Alart();
    }

    public class Dependency
    {
        private int PrivateProp { get; [UsedImplicitly] set; }

        public static void CheckSecurity(string name, string password)
        {
            CallGuard(name, password);
        }

        private static void CallGuard(string name, string password)
        {
            throw new NotImplementedException();
        }

        public void Alert()
        {
            GetGuard("unit", "testing");
        }

        private IGuard GetGuard(string name, string password)
        {
            throw new NotImplementedException();
        }

        public int GetNumberFromDatabase()
        {
            return InternalNumber();
        }

        private int InternalNumber()
        {
            throw new NotImplementedException();
        }

        public int GetNumberFromProperty()
        {
            return PrivateProp;
        }
    }

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
}