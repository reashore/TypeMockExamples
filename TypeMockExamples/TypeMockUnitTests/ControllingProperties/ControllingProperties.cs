
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
            Assert.AreEqual(5, _dependency.Number);
        }

        // These unit tests demonstrate working with properties

        [TestMethod]
        public void FakePropertyGetterUsingTrueProperty()
        {
            // arrange
            Dependency dependencyFake = Isolate.Fake.Instance<Dependency>();
            dependencyFake.Number = 5;

            // act
            int result = _classUnderTest.SimpleCalculation(2, dependencyFake);

            // assert
            // 2 + 5
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
            _classUnderTest.SimpleCalculation(2, _dependency);

            // assert
            // called twice
            Assert.AreEqual(2, countCalls);
        }
    }

    // **** Classes under test ****

    public class ClassUnderTest
    {
        public int SimpleCalculation(int a, Dependency dependency)
        {
            // get number
            int result = a + dependency.Number;
            // set number
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