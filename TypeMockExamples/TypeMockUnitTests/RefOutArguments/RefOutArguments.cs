
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TypeMock.ArrangeActAssert;

namespace TypeMockExamples.TypeMockUnitTests.RefOutArguments
{
    /// <summary>
    /// This test class shows different ways of controlling the behavior of ref and out arguments
    /// To set the return value of a ref or out arguement, set them before calling the WhenCalled API.
    /// </summary>
    [TestClass]
    [Isolated] // Note: Use Isolated to clean up after all tests in class
    public class RefOutArgumentTests
    {
        [TestMethod]
        public void ReturnValuesInRefArgument()
        {
            // arrange
            string outStr = "typemock";
            // todo: if this is an out parameter why is an initial value necessary?
            List<int> outList = new List<int> {100};
            Dependency realDependency = new Dependency();
            Isolate.WhenCalled(() => realDependency.SomeMethod(ref outStr, out outList)).IgnoreCall();
            ClassUnderTest classUnderTest = new ClassUnderTest();

            // act
            string result = classUnderTest.GetString(realDependency);

            // assert
            Assert.AreEqual("typemock1", result);
        }

        [TestMethod]
        public void VerifyRefArguments()
        {
            // arrange
            string outStr = "typemock";
            Dependency fake = new Dependency();
            Isolate.WhenCalled(() => fake.SomeMethod(ref outStr)).IgnoreCall();
            ClassUnderTest classUnderTest = new ClassUnderTest();

            // act
            classUnderTest.UseRef(fake);

            // assert
            string inputShouldbe = "unit testing";
            Isolate.Verify.WasCalledWithExactArguments(() => fake.SomeMethod(ref inputShouldbe));
        }
    }

    //------------------
    // Classes under test
    // - Dependency: Methods are not implemented - these need to be faked out
    // - ClassUnderTest: Class that uses Dependency
    //------------------

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
}