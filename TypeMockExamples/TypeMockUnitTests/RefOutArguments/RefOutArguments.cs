
namespace TypeMockExamples.TypeMockUnitTests.RefOutArguments
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeMock.ArrangeActAssert;

    // These units tests demostrate the behavior of ref and out arguments.
    // Set ref or out arguments before calling WhenCalled()

    [TestClass]
    [Isolated]
    public class RefOutArgumentTests
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
        public void ReturnValuesInRefArgument()
        {
            // arrange
            string refString = "typemock";
            // todo: why is an initial value required on an out parameter?
            List<int> outList = new List<int> { 100 };
            Isolate.WhenCalled(() => _dependency.SomeMethod(ref refString, out outList)).IgnoreCall();

            // act
            string result = _classUnderTest.GetString(_dependency);

            // assert
            // "typemock" + "1"
            Assert.AreEqual("typemock1", result);
        }

        [TestMethod]
        public void VerifyRefArguments()
        {
            // arrange
            string refString = "typemock";
            Isolate.WhenCalled(() => _dependency.SomeMethod(ref refString)).IgnoreCall();

            // act
            string result = _classUnderTest.UseRef(_dependency);

            // assert
            string inputShouldbe = "unit testing";
            Assert.AreEqual("typemock", result);
            Isolate.Verify.WasCalledWithExactArguments(() => _dependency.SomeMethod(ref inputShouldbe));
        }
    }

    // **** Classes under test ****

    public class ClassUnderTest
    {
        public string GetString(Dependency dependency)
        {
            string name = "unit testing";
            List<int> list;

            dependency.SomeMethod(ref name, out list);

            return name + list.Count;
        }

        public string UseRef(Dependency dependency)
        {
            string name = "unit testing";

            dependency.SomeMethod(ref name);

            return name;
        }
    }

    public class Dependency
    {
        public virtual void SomeMethod(ref string name, out List<int> list)
        {
            throw new NotImplementedException();
        }

        public virtual void SomeMethod(ref string name)
        {
            throw new NotImplementedException();
        }
    }
}