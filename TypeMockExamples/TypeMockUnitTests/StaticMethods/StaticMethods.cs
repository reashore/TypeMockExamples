
namespace TypeMockExamples.TypeMockUnitTests.StaticMethods
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeMock.ArrangeActAssert;

    [TestClass]
    [Isolated]
    public class StaticMethodAndConstructorTests1
    {
        private ClassUnderTest _classUnderTest;

        [TestInitialize]
        public void InitializeTest()
        {
            _classUnderTest = new ClassUnderTest();
        }

        [TestMethod]
        public void FakeAllStaticMethods()
        {
            // arrange
            Isolate.Fake.StaticMethods<Dependency>();

            // act
            int result = _classUnderTest.Calculate(1, 2);

            // assert
            // 1 + 3
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void FakeOneStaticMethod()
        {
            // arrange
            Isolate.WhenCalled(() => Dependency.CheckSecurity(null, null)).IgnoreCall();

            // act
            int result = _classUnderTest.Calculate(1, 2);

            // assert
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void VerifyStaticMethodWasCalled()
        {
            // arrange
            Isolate.Fake.StaticMethods<Dependency>();

            // act
            _classUnderTest.Calculate(1, 2);

            // assert
            Isolate.Verify.WasCalledWithAnyArguments(() => Dependency.CheckSecurity(null, null));
            Isolate.Verify.WasCalledWithExactArguments(() => Dependency.CheckSecurity("username", "password"));
        }
    }

    [TestClass]
    [Isolated]
    public class StaticMethodAndConstructorTests2
    {
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
        /// Typemock Isolator does this automatically, but here is a way to force a static-constructor call
        /// </summary>
        [TestMethod]
        public void CallingStaticConstructorTest()
        {
            // arrange
            StaticConstructorExample.TrueOnStaticConstructor = false;

            // act
            // force static constructor to be called
            Isolate.Invoke.StaticConstructor(typeof(StaticConstructorExample));

            // assert
            Assert.IsTrue(StaticConstructorExample.TrueOnStaticConstructor);
        }
    }

    // **** Classes under test ****

    public class ClassUnderTest
    {
        public int Calculate(int a, int b)
        {
            Dependency.CheckSecurity("username", "password");

            return a + b;
        }
    }

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
}