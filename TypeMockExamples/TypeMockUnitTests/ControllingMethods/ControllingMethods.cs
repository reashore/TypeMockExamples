
namespace TypeMockExamples.TypeMockUnitTests.ControllingMethods
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeMock.ArrangeActAssert;

    /// <summary>
    /// This test class shows different ways of controlling the behavior of fake objects using the Isolate.WhenCalled() API.
    /// The supported behaviors are:
    /// <list type="bullet">
    ///     <item>ReturnRecursiveFake (default)- return a zero or equivalent, and return fake objects for reference types. The returned fake objects will behave in the same way.</item>
    ///     <item>WillReturn - specify a return value for the call. Only applicable for methods returning values.</item>
    ///     <item>IgnoreCall - this method will be ignored. Only applicable for void methods.</item>
    ///     <item>WillThrow - will throw an exception when the method is called.</item>
    ///     <item>CallOriginal - will call the method's original implementation.</item>
    ///     <item>WillReturnCollectionValuesOf - will replace the collection returned by the method with a provided one. Only applies to methods returning collections</item>
    /// </list>
    /// </summary>
    [TestClass]
    [Isolated(DesignMode.Pragmatic)]
    public class ControllingMethodTests
    {
        [TestMethod]
        public void ReturnRecursiveFake()
        {
            // arrange
            Dependency realDependency = new Dependency();

            // return fake objects for reference types
            Isolate.WhenCalled(() => realDependency.GetPatent()).ReturnRecursiveFake();
            ClassUnderTest classUnderTest = new ClassUnderTest();

            // act
            string result = classUnderTest.ReturnPatentName(realDependency);

            // assert
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void WillReturn_ReturnValue()
        {
            // arrange
            Dependency realDependency = new Dependency();
            Isolate.WhenCalled(() => realDependency.GetId()).WillReturn(2);
            ClassUnderTest classUnderTest = new ClassUnderTest();

            // act
            int result = classUnderTest.AddToDependency(1, realDependency);

            // assert
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void CallOriginal_OnFakeObject()
        {
            // arrange
            Dependency fakeDependency = Isolate.Fake.Instance<Dependency>();
            Isolate.WhenCalled(() => fakeDependency.GetId()).CallOriginal();
            ClassUnderTest classUnderTest = new ClassUnderTest();

            // act
            int result = classUnderTest.AddToDependency(1, fakeDependency);

            // assert
            // original GetID returns 10
            Assert.AreEqual(11, result);
        }

        [TestMethod]
        public void IgnoreCall_OnRealObject()
        {
            // arrange
            Dependency realDependency = new Dependency();

            // do not convert lambda expression to method group (as it breaks the test)
            Isolate.WhenCalled(() => realDependency.Check()).IgnoreCall();
            ClassUnderTest classUnderTest = new ClassUnderTest();

            // act
            int result = classUnderTest.GetIdWithCheck(realDependency);

            // assert
            Assert.AreEqual(10, result);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "fakes fault")]
        public void ThrowException_OnRealObject()
        {
            // arrange
            Dependency realDependency = new Dependency();
            Isolate.WhenCalled(() => realDependency.GetId()).WillThrow(new Exception("fakes fault"));
            ClassUnderTest classUnderTest = new ClassUnderTest();

            // act
            classUnderTest.AddToDependency(1, realDependency);
        }

        [TestMethod]
        public void DoInstead_OnRealObject()
        {
            // arrange
            int returnValue = 2;
            Dependency realDependency = new Dependency();
            //// return value dynamically
            Isolate.WhenCalled(() => realDependency.GetId()).DoInstead(x => { return returnValue; });
            ClassUnderTest classUnderTest = new ClassUnderTest();

            // act
            int result1 = classUnderTest.AddToDependency(1, realDependency);

            // assert
            Assert.AreEqual(3, result1);

            // arrange
            returnValue = 4;

            // act
            int result2 = classUnderTest.AddToDependency(1, realDependency);

            // assert
            Assert.AreEqual(5, result2);
        }

        [TestMethod]
        public void SequencedWillReturn_OnRealObject()
        {
            // arrange
            Dependency realDependency = new Dependency();
            //// Sequenced calls will return values in sequence, 
            // last value will stay the default
            Isolate.WhenCalled(() => realDependency.GetId()).WillReturn(2);
            Isolate.WhenCalled(() => realDependency.GetId()).WillReturn(9);
            ClassUnderTest classUnderTest = new ClassUnderTest();

            // act
            int result = classUnderTest.AddToDependency3Times(1, realDependency);

            // assert
            Assert.AreEqual(21, result);
        }

        [TestMethod]
        public void OverloadedMethodConsideredSequenced_OnFakeObject()
        {
            // arrange
            Dependency realDependency = Isolate.Fake.Instance<Dependency>();
            //// Overloaded method calls without using exact argument matching are considered sequenced calls
            Isolate.WhenCalled(() => realDependency.OverloadedMethod(1)).WillReturn(2);
            Isolate.WhenCalled(() => realDependency.OverloadedMethod("Typemock Rocks")).WillReturn(9);
            ClassUnderTest classUnderTest = new ClassUnderTest();

            // act
            int result = classUnderTest.CallTwoOverloadedDependency(realDependency);

            // assert
            Assert.AreEqual(11, result);
        }

        [TestMethod]
        public void SequencedOverloadedByType_OnRealObject()
        {
            // arrange
            Dependency realDependency = new Dependency();
            //// Each overloaded method will act as a separate sequence
            Isolate.WhenCalled(() => realDependency.OverloadedMethod(1)).WillReturn(2);
            Isolate.WhenCalled(() => realDependency.OverloadedMethod(1)).WillReturn(4);
            Isolate.WhenCalled(() => realDependency.OverloadedMethod("Typemock Rocks")).WillReturn(9);
            Isolate.WhenCalled(() => realDependency.OverloadedMethod("Typemock Rocks")).WillReturn(10);
            ClassUnderTest classUnderTest = new ClassUnderTest();

            // act
            int result1 = classUnderTest.CallTwoOverloadedDependency(realDependency);
            int result2 = classUnderTest.CallTwoOverloadedDependency(realDependency);

            // asset
            Assert.AreEqual(11, result1);
            Assert.AreEqual(14, result2);
        }

        [TestMethod]
        public void SettingBehaviorForCallChain_OnRealObject()
        {
            // arrange
            Dependency fakeDependency = new Dependency();
            Isolate.WhenCalled(() => fakeDependency.GetPatent().GetId()).WillReturn(2);
            ClassUnderTest classUnderTest = new ClassUnderTest();

            // act
            int result = classUnderTest.AddToChainedDependency(1, fakeDependency);
            
            // assert
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void ExtensionMethod_Example()
        {
            // arrange
            Dependency dependency = new Dependency();
            //// Call the extension method as normal (even though it is actually a static method)
            Isolate.WhenCalled(() => dependency.Multiply(6)).WillReturn(10);
            ClassUnderTest cut = new ClassUnderTest();

            // act
            int result = cut.AddToDependency(0, dependency);

            // assert
            // Verify the returned values
            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void MockLinqQuery_Example()
        {
            // arrange
            List<int> realList = new List<int> { 1, 2, 4, 5 };
            int[] dummyData = { 10, 20 };
            Isolate.WhenCalled(() => from c in realList where c > 3 select c).WillReturn(dummyData);

            // act
            List<int> result = new ClassUnderTest().DoLinq(realList);

            // assert
            // Note: Returns dummyData results
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(10, result[0]);
            Assert.AreEqual(20, result[1]);
        }
    }

    ////------------------
    //// Classes under test
    //// - ExtendDependency - an extension method of Dependency
    //// - Dependency: Methods are not implemented - these need to be faked out
    //// - ClassUnderTest: Class that uses Dependency
    ////------------------

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
            return (from c in list where c > 3 select c).ToList();
        }
    }
}