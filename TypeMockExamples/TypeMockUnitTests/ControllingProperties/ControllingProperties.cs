
namespace TypeMockExamples.TypeMockUnitTests.ControllingProperties
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeMock.ArrangeActAssert;

    // These unit tests demonstarte ways to fake properties:
    // 1) Using WhenCalled to fake property return values 
    // 2) Using properties to configure a fake property

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

        [TestMethod]
        public void FakePropertyGetterUsingWhenCalled()
        {
            // arrange
            Isolate.WhenCalled(() => _dependency.Number).WillReturn(5);

            // act
            int result = _classUnderTest.SimpleCalculation(2, _dependency);

            // assert
            // 7 = 2 + 5
            Assert.AreEqual(7, result);
            // verify configured property value
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
            Assert.AreEqual(8, dependencyFake.Number);
        }

        [TestMethod]
        public void FakePropertySetterUsingWhenCalled()
        {
            // arrange
            int callCount = 0;
            Isolate.WhenCalled(() => _dependency.Number = 5).DoInstead(c => callCount++);

            // act
            _classUnderTest.SimpleCalculation(2, _dependency);
            _classUnderTest.SimpleCalculation(2, _dependency);

            // assert
            // called twice
            Assert.AreEqual(2, callCount);
            Isolate.Verify.WasCalledWithAnyArguments(() => _dependency.Number);
            Assert.AreEqual(2, Isolate.Verify.GetTimesCalled(() => _dependency.Number));
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
            dependency.Number = 8;

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