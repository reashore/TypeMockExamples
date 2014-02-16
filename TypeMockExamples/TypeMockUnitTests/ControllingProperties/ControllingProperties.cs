
namespace TypeMockExamples.TypeMockUnitTests.ControllingProperties
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeMock.ArrangeActAssert;

    /// <summary>
    /// This test class shows different ways of controlling the behavior of fake properties
    /// The supported behaviors are:
    /// <list type="bullet">
    ///     <item>Using WhenCalled - Like other methods</item>
    ///     <item>Using True Properties - To fake property to act like an auto-property</item>
    /// </list>
    /// </summary>
    [TestClass]
    [Isolated]
    public class ControllingPropertiesTests
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
        public void FakePropertyGetterUsingWhenCalled()
        {
            // arrange
            Isolate.WhenCalled(() => _dependency.Number).WillReturn(5);

            // act
            int result = _classUnderTest.SimpleCalculation(2, _dependency);

            // assert
            Assert.AreEqual(7, result);
        }

        [TestMethod]
        public void FakePropertyGetterUsingTrueProperty()
        {
            // arrange
            _dependency = Isolate.Fake.Instance<Dependency>();
            _dependency.Number = 5;

            // act
            int result = _classUnderTest.SimpleCalculation(2, _dependency);

            // assert
            Assert.AreEqual(7, result);
        }

        [TestMethod]
        public void FakePropertySetterUsingWhenCalled()
        {
            // arrange
            int countCalls = 0;
            Isolate.WhenCalled(() => _dependency.Number = 5).DoInstead(c => countCalls++);

            // act
            _classUnderTest.SimpleCalculation(2, _dependency);

            // assert
            Assert.AreEqual(1, countCalls);
        }
    }

    //------------------
    // Classes under test
    // - Dependency: Methods are not implemented - these need to be faked out
    // - ClassUnderTest: Class that uses Dependency
    //------------------

    public class ClassUnderTest
    {
        public int SimpleCalculation(int a, Dependency dependency)
        {
            int result = a + dependency.Number;
            dependency.Number = result;
            return result;
        }
    }

    public class Dependency
    {
        private int _number;

        public virtual int Number
        {
            get
            {
                return _number;
            }

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
}