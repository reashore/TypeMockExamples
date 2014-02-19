
namespace TypeMockExamples.TypeMockUnitTests.LiveObjects
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeMock.ArrangeActAssert;

    // These unit tests demonstrate modifying the behavior of objects that have been instantiated normally.
    // The behavior of such objects is configured by using WhenCalled() and verified using Verify().

    // When a live object is used in WasCalled or its NonPublic counterparts, it will become a viable fake object. 
    // This behavior applies similarly to static methods which have not had their behavior defaults set up by 
    // using Isolate.Fake.StaticMethods().

    [TestClass]
    [Isolated]
    public class LiveObjectTests
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
        public void CreateRealObjectFakeVoidMethod()
        {
            // arrange
            Isolate.WhenCalled(() => _dependency.CheckSecurity(null, null)).IgnoreCall();

            // act
            int result = _classUnderTest.Calculate(1, 2, _dependency, null, null);

            // assert
            Assert.AreEqual(3, result);
            Isolate.Verify.WasCalledWithExactArguments(() => _dependency.CheckSecurity(null, null));
        }

        [TestMethod]
        public void VerifyMethodsOfRealObject1()
        {
            // arrange
            Isolate.WhenCalled(() => _dependency.CheckSecurity(null, null)).IgnoreCall();

            // act
            int result = _classUnderTest.Calculate(1, 2, _dependency, null, null);

            // assert
            // 3 = 1 + 2
            Assert.AreEqual(3, result);
            Isolate.Verify.WasCalledWithExactArguments(() => _dependency.CheckSecurity(null, null));
            Isolate.Verify.WasCalledWithAnyArguments(() => _dependency.CheckSecurity(null, null));
        }

        [TestMethod]
        public void VerifyMethodsOfRealObject2()
        {
            // arrange
            // can also use CallOriginal(), IgnoreCall(), DoInstead(), WillThrow(), WithExactArguments()
            Isolate.WhenCalled(() => _dependency.CheckSecurity(null, null)).IgnoreCall();

            // act
            int result = _classUnderTest.Calculate(1, 2, _dependency);

            // assert
            Assert.AreEqual(3, result);
            Isolate.Verify.WasCalledWithAnyArguments(() => _dependency.CheckSecurity(null, null));
        }
    }

    // **** Classes under test ****

    public class ClassUnderTest
    {
        public int Calculate(int a, int b, Dependency dependency, string username = "username", string password = "password")
        {
            dependency.CheckSecurity(username, password);

            return a + b;
        }

        public int Calculate(int a, Dependency dependency)
        {
            return a + dependency.GetId();
        }
    }

    public class Dependency
    {
        public virtual void CheckSecurity(string name, string password)
        {
            throw new NotImplementedException();
        }

        public virtual int GetId()
        {
            return 10;
        }
    }
}