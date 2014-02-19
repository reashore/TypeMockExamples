
namespace TypeMockExamples.TypeMockUnitTests.StaticMethods
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeMock.ArrangeActAssert;

    [TestClass]
    [Isolated]
    public class StaticMethodAndConstructorTests1
    {
        [TestMethod]
        public void FakeAllStaticMethods()
        {
            // arrange
            Isolate.Fake.StaticMethods<Dependency>();

            // act
            int result = ClassUnderTest.Calculate(1, 2);

            // assert
            // 3 = 1 + 2
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void FakeOneStaticMethod()
        {
            // arrange
            Isolate.WhenCalled(() => Dependency.CheckSecurity(null, null)).IgnoreCall();

            // act
            int result = ClassUnderTest.Calculate(1, 2);

            // assert
            // 3 = 1 + 2
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void VerifyStaticMethodWasCalled()
        {
            // arrange
            Isolate.Fake.StaticMethods<Dependency>();

            // act
            int result = ClassUnderTest.Calculate(1, 2);

            // assert
            Assert.AreEqual(3, result);
            Isolate.Verify.WasCalledWithAnyArguments(() => Dependency.CheckSecurity(null, null));
            Isolate.Verify.WasCalledWithExactArguments(() => Dependency.CheckSecurity("username", "password"));
        }
    }

    [TestClass]
    [Isolated]
    public class StaticMethodAndConstructorTests2
    {
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
            // verify the static constructor was faked
            Assert.IsFalse(StaticConstructorExample.TrueOnStaticConstructor);
        }

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
        public static int Calculate(int a, int b)
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