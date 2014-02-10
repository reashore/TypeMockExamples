
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TypeMock.ArrangeActAssert;

namespace TypeMockExamples.TypeMockUnitTests.AssertingCallsWhereMade
{
    /// <summary>
    /// This test class shows different ways of verifying calls on fake objects using the Isolate.Verify API.
    /// Calls can be verified in the following ways:
    ///     - WasCalledWithAnyArguments(): verifies the call was made regardless of arguments passed to it
    ///     - WasCalledWithExactArguments(): verifies the call was made with the exact arguments specified
    ///     - WasNotCalled: verifies the call was not made
    ///     - WasCalledWithArguments: use Matching to match arguments
    /// </summary>
    [TestClass]
    [Isolated] // Note: Use Isolated to clean up after all tests in class
    public class VerifyingCallsWhereMade
    {
        [TestMethod]
        public void Verify_CallWasMade_WithAnyArgument()
        {
            // arrange
            Dependency fakeDependency = Isolate.Fake.Instance<Dependency>();
            ClassUnderTest classUnderTest = new ClassUnderTest();

            // act
            classUnderTest.DoAction(2, fakeDependency);

            // assert
            Isolate.Verify.WasCalledWithAnyArguments(() => fakeDependency.CheckSecurity(null, null));
        }

        [TestMethod]
        public void Verify_CallWasNeverMade()
        {
            // arrange
            Dependency fakeDependency = Isolate.Fake.Instance<Dependency>();
            ClassUnderTest classUnderTest = new ClassUnderTest();

            // act
            classUnderTest.DoAction(2, fakeDependency);

            // assert
            Isolate.Verify.WasNotCalled(() => fakeDependency.CallGuard());
        }

        [TestMethod]
        public void Verify_CallWasMadeTwice()
        {
            // arrange
            Dependency fakeDependency = Isolate.Fake.Instance<Dependency>();
            ClassUnderTest classUnderTest = new ClassUnderTest();

            // act
            classUnderTest.DoAction(2, fakeDependency);
            classUnderTest.DoAction(3, fakeDependency);

            // assert
            int count = Isolate.Verify.GetTimesCalled(() => fakeDependency.CheckSecurity("", ""));
            Assert.AreEqual(2, count);
        }

        [TestMethod]
        public void Verify_CallWasNeverMade_OnChain()
        {
            // arrange
            Dependency fakeDependency = Isolate.Fake.Instance<Dependency>();
            ClassUnderTest classUnderTest = new ClassUnderTest();

            // act
            classUnderTest.DoAction(2, fakeDependency);

            // assert
            Isolate.Verify.WasNotCalled(() => fakeDependency.CallGuard().CheckSecurity(null, null));
        }

        [TestMethod]
        public void Verify_CallWasMade_WithExactArguments()
        {
            // arrange
            Dependency fakeDependency = Isolate.Fake.Instance<Dependency>();
            ClassUnderTest classUnderTest = new ClassUnderTest();

            // act
            classUnderTest.DoAction(2, fakeDependency);

            // assert
            Isolate.Verify.WasCalledWithExactArguments(() => fakeDependency.CheckSecurity("typemock", "rules"));
        }

        [TestMethod]
        public void Verify_CallWasMade_WithMatchingArguments()
        {
            // arrange
            Dependency fakeDependency = Isolate.Fake.Instance<Dependency>();
            ClassUnderTest classUnderTest = new ClassUnderTest();

            // act
            classUnderTest.DoAction(2, fakeDependency);

            // assert
            // todo: fix possible null reference exception
            Isolate.Verify.WasCalledWithArguments(() => fakeDependency.CheckSecurity(null, null)).Matching(
                a => (a[0] as string).StartsWith("type") && (a[1] as string).StartsWith("rule"));
        }
    }

    //------------------
    // Classes under test
    // - Dependency: Methods are not implemented - these need to be faked out
    // - ClassUnderTest: Class that uses Dependency
    //------------------

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

    public class ClassUnderTest
    {
        public void DoAction(int i, Dependency dependency)
        {
            dependency.CheckSecurity("typemock", "rules");

            if (i < 2)
                dependency.CallGuard();
        }
    }
}