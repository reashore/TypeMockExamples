
namespace TypeMockExamples.TypeMockUnitTests.LiveObjects
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeMock.ArrangeActAssert;

    /// <summary>
    /// This test class demonstrates using live objects - test objects that have been instantiated normally, rather than
    /// as fake objects using Isolate.Fake.Instance(). These objects' behavior can still be modified by using WhenCalled methods
    /// and verified using Verify.
    /// When a live object is used in WasCalled or its NonPublic counterparts, it will become a viable fake object. 
    /// This behavior applies similarly to static methods which have not had their behavior defaults set up by 
    /// using Isolate.Fake.StaticMethods().
    /// </summary>
    [TestClass]
    [Isolated]  //[Isolated(DesignMode.InterfaceOnly)]
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

        [TestCleanup]
        public void CleanupTest()
        {
            _classUnderTest = null;
            _dependency = null;
        }

        [TestMethod]
        public void CreateRealObject_FakeVoidMethod()
        {
            // arrange
            Isolate.WhenCalled(() => _dependency.CheckSecurity(null, null)).IgnoreCall();

            // act
            int result = _classUnderTest.Calculate(1, 2, _dependency);

            // assert
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void VerifyMethods_OfRealObject()
        {
            // arrange
            // Requires at least one WhenCalled, can be CallOriginal for Verify to work
            Isolate.WhenCalled(() => _dependency.CheckSecurity(null, null)).IgnoreCall();

            // act
            _classUnderTest.Calculate(1, 2, _dependency);

            // assert
            Isolate.Verify.WasCalledWithAnyArguments(() => _dependency.CheckSecurity(null, null));
        }
    }

    //------------------
    // Classes under test
    // - Dependency: Class with Methods that need to be faked out
    // - ClassUnderTest: Class that uses Dependency
    //------------------

    public class ClassUnderTest
    {
        public int Calculate(int a, int b, Dependency dependency)
        {
            dependency.CheckSecurity("username", "password");

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