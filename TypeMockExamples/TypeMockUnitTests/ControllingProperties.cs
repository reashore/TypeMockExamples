
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TypeMock.ArrangeActAssert;

namespace TypeMockExamples.TypeMockUnitTests.ControllingProperties
{
    /// <summary>
    /// This test class shows different ways of controlling the behavior of fake properties
    /// The supported behaviors are:
    /// <list type="bullet">
    ///     <item>Using WhenCalled - Like other methods</item>
    ///     <item>Using True Properties - To fake property to act like an auto-property</item>
    /// </list>
    /// </summary>
    [TestClass]
    [Isolated] // Note: Use Isolated to clean up after all tests in class
    public class ControllingProperties
    {
        [TestMethod]
        public void FakePropertyGetter_UsingWhenCalled()
        {
            // arrange
            Dependency fakeDependency = new Dependency();
            Isolate.WhenCalled(() => fakeDependency.Number).WillReturn(5);
            ClassUnderTest classUnderTest = new ClassUnderTest();

            // act
            int result = classUnderTest.SimpleCalculation(2, fakeDependency);

            // assert
            Assert.AreEqual(7, result);
        }

        [TestMethod]
        public void FakePropertyGetter_UsingTrueProperty()
        {
            // arrange
            Dependency fakeDependency = Isolate.Fake.Instance<Dependency>();
            fakeDependency.Number = 5;
            ClassUnderTest classUnderTest = new ClassUnderTest();

            // act
            int result = classUnderTest.SimpleCalculation(2, fakeDependency);

            // assert
            Assert.AreEqual(7, result);
        }

        [TestMethod]
        public void FakePropertySetter_UsingWhenCalled()
        {
            // arrange
            Dependency fakeDependency = new Dependency();
            int countCalls = 0;
            Isolate.WhenCalled(() => fakeDependency.Number = 5).DoInstead(c => countCalls++);
            ClassUnderTest classUnderTest = new ClassUnderTest();

            // act
            classUnderTest.SimpleCalculation(2, fakeDependency);

            // assert
            Assert.AreEqual(1, countCalls);
        }
    }


    //------------------
    // Classes under test
    // - Dependency: Methods are not implemented - these need to be faked out
    // - ClassUnderTest: Class that uses Dependency
    //------------------

    public class Dependency
    {
        private int _number;

        public virtual int Number
        {
            get { return _number; }
            set
            {
                CheckSecurity();
                _number = value;
            }
        }

        public void CheckSecurity()
        {
            throw new NotImplementedException();
        }
    }

    public class ClassUnderTest
    {
        public int SimpleCalculation(int a, Dependency dependency)
        {
            int result = a + dependency.Number;
            dependency.Number = result;
            return result;
        }
    }
}