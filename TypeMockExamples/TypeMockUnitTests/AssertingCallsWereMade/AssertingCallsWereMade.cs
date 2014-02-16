
namespace TypeMockExamples.TypeMockUnitTests.AssertingCallsWereMade
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeMock.ArrangeActAssert;

    /// <summary>
    /// This test class shows different ways of verifying calls on fake objects using the Isolate.Verify API.
    /// Calls can be verified in the following ways:
    ///     - WasCalledWithAnyArguments(): verifies the call was made regardless of arguments passed to it
    ///     - WasCalledWithExactArguments(): verifies the call was made with the exact arguments specified
    ///     - WasNotCalled: verifies the call was not made
    ///     - WasCalledWithArguments: use Matching to match arguments
    /// </summary>
    [TestClass]
    [Isolated]
    public class VerifyingCallsWereMadeTests
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
        public void VerifyCallWasMadeWithAnyArgument()
        {
            // arrange
            Dependency dependencyFake = Isolate.Fake.Instance<Dependency>();

            // act
            _classUnderTest.DoAction(2, dependencyFake);

            // assert
            Isolate.Verify.WasCalledWithAnyArguments(() => dependencyFake.CheckSecurity(null, null));
        }

        [TestMethod]
        public void VerifyCallWasNeverMade()
        {
            // arrange
            Dependency dependencyFake = Isolate.Fake.Instance<Dependency>();

            // act
            _classUnderTest.DoAction(2, dependencyFake);

            // assert
            Isolate.Verify.WasNotCalled(() => dependencyFake.CallGuard());
        }

        [TestMethod]
        public void VerifyCallWasMadeTwice()
        {
            // arrange
            Dependency dependencyFake = Isolate.Fake.Instance<Dependency>();

            // act
            _classUnderTest.DoAction(2, dependencyFake);
            _classUnderTest.DoAction(3, dependencyFake);

            // assert
            const int expectedTimesCalled = 2;
            int actualTimesCalled = Isolate.Verify.GetTimesCalled(() => dependencyFake.CheckSecurity(string.Empty, string.Empty));
            Assert.AreEqual(expectedTimesCalled, actualTimesCalled);
        }

        [TestMethod]
        public void VerifyCallWasNeverMadeOnChain()
        {
            // arrange
            Dependency dependencyFake = Isolate.Fake.Instance<Dependency>();

            // act
            _classUnderTest.DoAction(2, dependencyFake);

            // assert
            Isolate.Verify.WasNotCalled(() => dependencyFake.CallGuard().CheckSecurity(null, null));
        }

        [TestMethod]
        public void VerifyCallWasMadeWithExactArguments()
        {
            // arrange
            Dependency dependencyFake = Isolate.Fake.Instance<Dependency>();

            // act
            _classUnderTest.DoAction(2, dependencyFake);

            // assert
            Isolate.Verify.WasCalledWithExactArguments(() => dependencyFake.CheckSecurity("username", "password"));
        }

        [TestMethod]
        public void VerifyCallWasMadeWithMatchingArguments()
        {
            // arrange
            Dependency dependencyFake = Isolate.Fake.Instance<Dependency>();

            // act
            _classUnderTest.DoAction(2, dependencyFake);

            // assert
            Isolate.Verify.WasCalledWithArguments(() => dependencyFake.CheckSecurity(null, null)).Matching(args => VerifyArguments(args, "user", "pass"));
        }

        private static bool VerifyArguments(object[] arguments, string value1, string value2)
        {
            if (arguments == null)
            {
                return false;
            }

            string argument0 = arguments[0] as string;
            string argument1 = arguments[1] as string;

            // cast to strings failed
            if (argument0 == null || argument1 == null)
            {
                return false;
            }

            return argument0.StartsWith(value1) && argument1.StartsWith(value2);
        }
    }

    //------------------
    // Classes under test
    // - Dependency: Methods are not implemented - these need to be faked out
    // - ClassUnderTest: Class that uses Dependency
    //------------------

    public class ClassUnderTest
    {
        public void DoAction(int i, Dependency dependency)
        {
            dependency.CheckSecurity("username", "password");

            if (i < 2)
            {
                dependency.CallGuard();
            }
        }
    }

    public class Dependency
    {
        public virtual void CheckSecurity(string name, string password)
        {
            throw new NotImplementedException();
        }

        public virtual Dependency CallGuard()
        {
            throw new NotImplementedException();
        }
    }
}