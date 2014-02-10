
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TypeMock.ArrangeActAssert;

namespace TypeMockExamples.TypeMockUnitTests.StaticMethods
{
    /// <summary>
    /// This test class shows how to fake static methods. 
    /// </summary>
    [TestClass]
    [Isolated(DesignMode.Pragmatic)] // Note: Use Isolated to clean up after all tests in class
    public class StaticMethodsAndConstructors
    {
        [TestMethod]
        public void FakeAllStaticMethods()
        {
            // arrange
            Isolate.Fake.StaticMethods<Dependency>();
            ClassUnderTest classUnderTest = new ClassUnderTest();

            // act
            int result = classUnderTest.Calculate(1, 2);

            // assert
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void FakeOneStaticMethod()
        {
            // arrange
            Isolate.WhenCalled(() => Dependency.CheckSecurity(null, null)).IgnoreCall();
            ClassUnderTest classUnderTest = new ClassUnderTest();

            // act
            int result = classUnderTest.Calculate(1, 2);

            // assert
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void VerifyStaticMethodWasCalled()
        {
            // arrange
            Isolate.Fake.StaticMethods<Dependency>();
            ClassUnderTest classUnderTest = new ClassUnderTest();

            // act
            classUnderTest.Calculate(1, 2);

            // assert
            Isolate.Verify.WasCalledWithAnyArguments(() => Dependency.CheckSecurity(null, null));
        }

        /// <summary>
        /// This test shows to to fake calls to static constructors using Isolate.Fake.StaticConstructor().
        /// By default static constructors are called to fake them use Fake.StaticConstructor()
        /// </summary>
        [TestMethod]
        public void FakingStaticConstructor()
        {
            // arrange
            StaticConstructorExample.TrueOnStaticConstructor = false;
            Isolate.Fake.StaticConstructor<StaticConstructorExample>();

            // act
            // calling a static method on the class forces the static constructor to be called
            StaticConstructorExample.Foo();

            // assert
            // this verifies the static constructor was faked and not called
            Assert.IsFalse(StaticConstructorExample.TrueOnStaticConstructor);
        }

        /// <summary>
        /// As static constructors for a type is only executed once, if we fake it we need a way to invoke it in a test that 
        /// requires normal execution.
        /// 
        /// Typemock Isolator does this automatically, but here is a way to force a static-constructor call
        /// </summary>
        [TestMethod]
        public void CallingStaticConstructorTest()
        {
            // arrange
            StaticConstructorExample.TrueOnStaticConstructor = false;

            // act
            // force static constructor to be called
            Isolate.Invoke.StaticConstructor(typeof (StaticConstructorExample));

            // assert
            Assert.IsTrue(StaticConstructorExample.TrueOnStaticConstructor);
        }
    }

    //------------------
    // Classes under test
    // - Dependency: Methods are not implemented - these need to be faked out
    // - ClassUnderTest: Class that uses Dependency
    // - StaticConstructorExample: a class with a static constructor and a flag that indicates it was called.
    //------------------

    public class StaticConstructorExample
    {
        static StaticConstructorExample()
        {
            TrueOnStaticConstructor = true;
        }

        public static bool TrueOnStaticConstructor { get; set; }

        public static void Foo()
        {
        }
    }

    public class Dependency
    {
        public static void CheckSecurity(string name, string password)
        {
            throw new NotImplementedException();
        }

        public static void CallGuard()
        {
            throw new NotImplementedException();
        }
    }

    public class ClassUnderTest
    {
        public int Calculate(int a, int b)
        {
            Dependency.CheckSecurity("typemock", "rules");

            return a + b;
        }
    }
}