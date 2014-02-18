
namespace TypeMockExamples.TypeMockUnitTests.ControllingMethods
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeMock.ArrangeActAssert;

    // These unit tests demonstrate ways of controlling the behavior of fake objects:
    // 1) ReturnRecursiveFake
    // 2) WillReturn
    // 3) IgnoreCall
    // 4) WillThrow
    // 5) CallOriginal
    // 6) WillReturnCollectionValuesOf

    [TestClass]
    [Isolated]
    public class ControllingMethodTests1
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
        public void ReturnRecursiveFake()
        {
            // arrange
            // return fake objects for reference types
            Isolate.WhenCalled(() => _dependency.GetPatent()).ReturnRecursiveFake();

            // act
            string result = _classUnderTest.ReturnPatentName(_dependency);

            // assert
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void WillReturnValue()
        {
            // arrange
            Isolate.WhenCalled(() => _dependency.GetId()).WillReturn(2);

            // act
            int result = _classUnderTest.AddToDependency(1, _dependency);

            // assert
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void IgnoreCallOnRealObject()
        {
            // arrange
            // Note: do not convert lambda expression to method group (as it breaks the test)
            Isolate.WhenCalled(() => _dependency.Check()).IgnoreCall();

            // act
            int result = _classUnderTest.GetIdWithCheck(_dependency);

            // assert
            Assert.AreEqual(10, result);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ThrowExceptionOnRealObject()
        {
            // arrange
            Exception exception = new Exception();
            Isolate.WhenCalled(() => _dependency.GetId()).WillThrow(exception);

            // act
            _classUnderTest.AddToDependency(1, _dependency);

            // assert
            // exception is thrown
        }

        [TestMethod]
        public void DoInsteadOnRealObject()
        {
            // arrange
            int returnValue = 2;
            // return value dynamically
            Isolate.WhenCalled(() => _dependency.GetId()).DoInstead(x => returnValue);

            // act
            int result1 = _classUnderTest.AddToDependency(1, _dependency);

            // assert
            // 1 + 2
            Assert.AreEqual(3, result1);

            // arrange
            returnValue = 4;

            // act
            int result2 = _classUnderTest.AddToDependency(1, _dependency);

            // assert
            // 1 + 4
            Assert.AreEqual(5, result2);
        }

        [TestMethod]
        public void SequencedWillReturnOnRealObject()
        {
            // arrange
            // Sequenced calls will return values in sequence,  last value will stay the default
            Isolate.WhenCalled(() => _dependency.GetId()).WillReturn(2);
            Isolate.WhenCalled(() => _dependency.GetId()).WillReturn(9);

            // act
            int result = _classUnderTest.AddToDependency3Times(1, _dependency);

            // assert
            // 1 + 2 + 9 + 9
            Assert.AreEqual(21, result);
        }

        [TestMethod]
        public void SequencedOverloadedByTypeOnRealObject()
        {
            // arrange
            // Each overloaded method will act as a separate sequence
            Isolate.WhenCalled(() => _dependency.OverloadedMethod(1)).WillReturn(2);
            Isolate.WhenCalled(() => _dependency.OverloadedMethod(1)).WillReturn(4);
            Isolate.WhenCalled(() => _dependency.OverloadedMethod("Typemock Rocks")).WillReturn(9);
            Isolate.WhenCalled(() => _dependency.OverloadedMethod("Typemock Rocks")).WillReturn(10);

            // act
            int result1 = _classUnderTest.CallTwoOverloadedDependency(_dependency);
            int result2 = _classUnderTest.CallTwoOverloadedDependency(_dependency);

            // asset
            // 2 + 9
            Assert.AreEqual(11, result1);
            // 4 + 10
            Assert.AreEqual(14, result2);
        }

        [TestMethod]
        public void SettingBehaviorForCallChainOnRealObject()
        {
            // arrange
            // chaining calls
            Isolate.WhenCalled(() => _dependency.GetPatent().GetId()).WillReturn(2);

            // act
            int result = _classUnderTest.AddToChainedDependency(1, _dependency);
            
            // assert
            // 1 + 2
            Assert.AreEqual(3, result);
        }

        // this test appears broken??
        [TestMethod]
        public void ExtensionMethodExample()
        {
            // arrange
            // Call the extension method as normal (even though it is actually a static method)
            Isolate.WhenCalled(() => _dependency.Multiply(6)).WillReturn(10);

            // act
            int result = _classUnderTest.AddToDependency(0, _dependency);

            // assert
            // Verify the returned values
            Assert.AreEqual(10, result);
        }
    }

    [TestClass]
    [Isolated]
    public class ControllingMethodTests2
    {
        private ClassUnderTest _classUnderTest;
        private Dependency _dependencyFake;

        [TestInitialize]
        public void InitializeTest()
        {
            _classUnderTest = new ClassUnderTest();
            _dependencyFake = Isolate.Fake.Instance<Dependency>();
        }

        [TestCleanup]
        public void CleanupTest()
        {
            _classUnderTest = null;
            _dependencyFake = null;
        }

        [TestMethod]
        public void CallOriginalOnFakeObject()
        {
            // arrange
            Isolate.WhenCalled(() => _dependencyFake.GetId()).CallOriginal();

            // act
            int result = _classUnderTest.AddToDependency(1, _dependencyFake);

            // assert
            // original GetID returns 10
            Assert.AreEqual(11, result);

            Assert.AreEqual(10, _dependencyFake.GetId());
        }

        [TestMethod]
        public void CallOriginalOnFakeObject2()
        {
            // arrange
            // no additional configuration

            // act
            int result = _classUnderTest.AddToDependency(1, _dependencyFake);

            // assert
            // GetId() on fake returns default(int), which is 0
            Assert.AreEqual(1, result);
            Assert.AreEqual(0, _dependencyFake.GetId());
        }

        [TestMethod]
        public void OverloadedMethodConsideredSequencedOnFakeObject()
        {
            // arrange
            // Overloaded method calls without using exact argument matching are considered sequenced calls
            Isolate.WhenCalled(() => _dependencyFake.OverloadedMethod(1)).WillReturn(2);
            Isolate.WhenCalled(() => _dependencyFake.OverloadedMethod("Typemock Rocks")).WillReturn(9);

            // act
            int result = _classUnderTest.CallTwoOverloadedDependency(_dependencyFake);

            // assert
            Assert.AreEqual(11, result);
        }
    }

    [TestClass]
    [Isolated]
    public class ControllingMethodTests3
    {
        [TestMethod]
        public void MockLinqQueryExample()
        {
            // arrange
            List<int> realList = new List<int> { 1, 2, 4, 5 };
            int[] dummyData = { 10, 20 };
            Isolate.WhenCalled(() => from c in realList where c > 3 select c).WillReturn(dummyData);
            ClassUnderTest classUnderTest = new ClassUnderTest();

            // act
            List<int> result = classUnderTest.DoLinq(realList);

            // assert
            // Note: Returns dummyData results
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(10, result[0]);
            Assert.AreEqual(20, result[1]);
        }
    }

    // **** Classes under test ****

    public class ClassUnderTest
    {
        public int SimpleCalculation(int a, int b, Dependency dependency)
        {
            dependency.Check();

            return a + b;
        }

        public string ReturnPatentName(Dependency dependency)
        {
            Dependency patent = dependency.GetPatent();

            return patent.Name;
        }

        public int GetIdWithCheck(Dependency dependency)
        {
            dependency.Check();

            return dependency.GetId();
        }

        public int AddToDependency(int a, Dependency dependency)
        {
            return a + dependency.GetId();
        }

        public int AddToDependency3Times(int a, Dependency dependency)
        {
            return a + dependency.GetId() + dependency.GetId() + dependency.GetId();
        }

        public int CallTwoOverloadedDependency(Dependency dependency)
        {
            return dependency.OverloadedMethod(12) + dependency.OverloadedMethod("typemock");
        }

        public int AddToChainedDependency(int a, Dependency dependency)
        {
            return a + dependency.GetPatent().GetId();
        }

        public List<int> DoLinq(List<int> list)
        {
            IEnumerable<int> query = (from c in list where c > 3 select c);

            return query.ToList();
        }
    }

    public static class ExtendDependency
    {
        public static int Multiply(this Dependency extendedInstance, int scalar)
        {
            return extendedInstance.GetId() * scalar;
        }
    }

    public class Dependency
    {
        public virtual string Name
        {
            get { throw new NotImplementedException(); }
        }

        public virtual void Check()
        {
            throw new NotImplementedException();
        }

        public virtual Dependency GetPatent()
        {
            throw new NotImplementedException();
        }

        public virtual int GetId()
        {
            return 10;
        }

        public virtual int OverloadedMethod(int arg)
        {
            return 10;
        }

        public virtual int OverloadedMethod(string arg)
        {
            return 10;
        }
    }
}