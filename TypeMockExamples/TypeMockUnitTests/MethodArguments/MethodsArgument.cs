
namespace TypeMockExamples.TypeMockUnitTests.MethodArguments
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeMock.ArrangeActAssert;

    /// <summary>
    /// This test class shows different ways of controlling the behavior of fake objects using the Isolate.WhenCalled() API.
    /// The supported behaviors are:
    /// <list type="bullet">
    ///     <item>Using Exact Argument Matching</item>
    ///     <item>Using Custom Checkers on Arguments</item>
    ///     <item>Mixing WithExactArguments and Custom Checkers</item>
    /// </list>
    /// </summary>
    [TestClass]
    [Isolated]
    public class MethodsArgumentTests
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
        public void FakeReturnValueBasedOnExactMethodArgumentsAtRuntime()
        {
            // arrange
            Dependency dependencyFake = Isolate.Fake.Instance<Dependency>();
            Isolate.WhenCalled(() => dependencyFake.MethodReturnInt("typemock", 1)).WithExactArguments().WillReturn(10);
            Isolate.WhenCalled(() => dependencyFake.MethodReturnInt("unit tests", 2)).WithExactArguments().WillReturn(50);

            // act
            int result = _classUnderTest.SimpleCalculation(dependencyFake);

            // assert
            Assert.AreEqual(60, result);
        }

        // todo: use expected exception
        [TestMethod]
        public void FakeVoidMethodBasedOnExactMethodArgs()
        {
            // arrange
            Dependency dependency = new Dependency();
            Isolate.WhenCalled(() => dependency.MethodReturnVoid(4)).WithExactArguments().IgnoreCall();
            bool exceptionWasThrown = false;

            // act
            try
            {
                _classUnderTest.CallVoid(dependency, 4);
            }
            catch (NotImplementedException)
            {
                exceptionWasThrown = true;
            }

            // assert
            Assert.IsFalse(exceptionWasThrown);
        }

        [TestMethod]
        public void FakeReturnValueBasedOnCustomArgumentsChecking()
        {
            // arrange
            Dependency dependencyFake = Isolate.Fake.Instance<Dependency>();
            Isolate.WhenCalled((string s, int x) => dependencyFake.MethodReturnInt(s, x))
                .AndArgumentsMatch((s, x) => s.StartsWith("Gui") && x < 300)
                .WillReturn(1000);

            // act
            int result = _classUnderTest.CallWithGuitar100(dependencyFake);

            // assert
            // All the arguments match our custom checker - the returned value is faked.
            Assert.AreEqual(1000, result);
        }

        [TestMethod]
        public void FakeReturnValueBasedOnCustomArgumentsChecking_CheckOneArgument()
        {
            // arrange
            Dependency dependencyFake = Isolate.Fake.Instance<Dependency>();
            Isolate.WhenCalled((int x) => dependencyFake.MethodReturnInt(string.Empty, x))
                .AndArgumentsMatch(x => x < 300)
                .WillReturn(1000);

            // act
            int result = _classUnderTest.CallWithGuitar100(dependencyFake);

            // assert
            // All the arguments match our custom checker - the returned value is faked.
            Assert.AreEqual(1000, result);
        }

        [TestMethod]
        public void FakeReturnValueBasedOnMixedChecker()
        {
            // arrange
            Dependency dependencyFake = Isolate.Fake.Instance<Dependency>();
            Isolate.WhenCalled((int x) => dependencyFake.MethodReturnInt("Guitar", x))
                .AndArgumentsMatch(x => x < 300)
                .WithExactArguments()
                .WillReturn(1000);

            // act
            int result = _classUnderTest.CallWithGuitar100(dependencyFake);

            // assert
            // All the arguments match our custom checker - the returned value is faked.
            Assert.AreEqual(1000, result);
        }
    }

    //------------------
    // Classes under test
    // - Dependency: Class with Methods that need to be faked out
    // - ClassUnderTest: Class that uses Dependency
    //------------------

    public class ClassUnderTest
    {
        public int SimpleCalculation(Dependency dependency)
        {
            int value1 = dependency.MethodReturnInt("typemock", 1);
            int value2 = dependency.MethodReturnInt("unit tests", 2);

            return value1 + value2;
        }

        public void CallVoid(Dependency dependency, int i)
        {
            dependency.MethodReturnVoid(i);
        }

        public int CallWithGuitar100(Dependency dependency)
        {
            return dependency.MethodReturnInt("Guitar", 200);
        }
    }

    public class Dependency
    {
        public virtual int MethodReturnInt(string arg1, int arg2)
        {
            throw new NotImplementedException();
        }

        public virtual void MethodReturnVoid(int n)
        {
            throw new NotImplementedException();
        }
    }
}