
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
            Isolate.WhenCalled(() => dependencyFake.MethodReturnInt("string1", 1)).WithExactArguments().WillReturn(10);
            Isolate.WhenCalled(() => dependencyFake.MethodReturnInt("string2", 2)).WithExactArguments().WillReturn(50);

            // act
            int result = _classUnderTest.SimpleCalculation(dependencyFake);

            // assert
            Assert.AreEqual(60, result);
        }

        // wierd test
        //[TestMethod]
        //public void FakeVoidMethodBasedOnExactMethodArgs()
        //{
        //    // arrange
        //    Dependency dependency = new Dependency();
        //    Isolate.WhenCalled(() => dependency.MethodReturnVoid(4)).WithExactArguments().IgnoreCall();
        //    bool exceptionWasThrown = false;

        //    // act
        //    try
        //    {
        //        _classUnderTest.CallVoid(dependency, 4);
        //    }
        //    catch (NotImplementedException)
        //    {
        //        exceptionWasThrown = true;
        //    }

        //    // assert
        //    Assert.IsFalse(exceptionWasThrown);
        //}

        [TestMethod]
        public void FakeVoidMethodBasedOnExactMethodArgs1()
        {
            // arrange
            Dependency dependency = new Dependency();
            Isolate.WhenCalled(() => dependency.MethodReturnVoid(4)).WithExactArguments().IgnoreCall();

            // act
            _classUnderTest.CallVoid(dependency, 4);

            // assert
            // got here because exception was not thrown
            Assert.IsTrue(true);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void FakeVoidMethodBasedOnExactMethodArgs2()
        {
            // arrange
            Dependency dependency = new Dependency();
            Isolate.WhenCalled(() => dependency.MethodReturnVoid(4)).WithExactArguments().IgnoreCall();

            // act
            _classUnderTest.CallVoid(dependency, 5);

            // assert
            // got here because exception was not thrown
            Assert.IsTrue(true);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void FakeVoidMethodBasedOnExactMethodArgs3()
        {
            // arrange
            Dependency dependency = new Dependency();

            // act
            _classUnderTest.CallVoid(dependency, 4);

            // assert
            // exception is thrown
        }

        [TestMethod]
        public void FakeReturnValueBasedOnCustomArgumentsChecking()
        {
            // arrange
            Dependency dependencyFake = Isolate.Fake.Instance<Dependency>();
            Isolate.WhenCalled((string s, int x) => dependencyFake.MethodReturnInt(s, x))
                .AndArgumentsMatch((s, x) => s.StartsWith("string3") && x < 300)
                .WillReturn(1000);

            // act
            int result = _classUnderTest.CallWithString3And200(dependencyFake);

            // assert
            // All the arguments match our custom checker - the returned value is faked.
            Assert.AreEqual(1000, result);
        }

        [TestMethod]
        public void FakeReturnValueBasedOnCustomArgumentsCheckingCheckOneArgument()
        {
            // arrange
            Dependency dependencyFake = Isolate.Fake.Instance<Dependency>();
            Isolate.WhenCalled((int x) => dependencyFake.MethodReturnInt(string.Empty, x))
                .AndArgumentsMatch(x => x < 300)
                .WillReturn(1000);

            // act
            int result = _classUnderTest.CallWithString3And200(dependencyFake);

            // assert
            // All the arguments match our custom checker - the returned value is faked.
            Assert.AreEqual(1000, result);
        }

        [TestMethod]
        public void FakeReturnValueBasedOnMixedChecker()
        {
            // arrange
            Dependency dependencyFake = Isolate.Fake.Instance<Dependency>();
            Isolate.WhenCalled((int x) => dependencyFake.MethodReturnInt("string3", x))
                .AndArgumentsMatch(x => x < 300)
                .WithExactArguments()
                .WillReturn(1000);

            // act
            int result = _classUnderTest.CallWithString3And200(dependencyFake);

            // assert
            // All the arguments match our custom checker - the returned value is faked.
            Assert.AreEqual(1000, result);
        }
    }

    // **** Classes under test ****

    public class ClassUnderTest
    {
        public int SimpleCalculation(Dependency dependency)
        {
            int value1 = dependency.MethodReturnInt("string1", 1);
            int value2 = dependency.MethodReturnInt("string2", 2);

            return value1 + value2;
        }

        public void CallVoid(Dependency dependency, int i)
        {
            dependency.MethodReturnVoid(i);
        }

        public int CallWithString3And200(Dependency dependency)
        {
            return dependency.MethodReturnInt("string3", 200);
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