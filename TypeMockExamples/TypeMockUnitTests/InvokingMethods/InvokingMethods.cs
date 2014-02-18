
using TypeMockExamples.Properties;

namespace TypeMockExamples.TypeMockUnitTests.InvokingMethods
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeMock.ArrangeActAssert;

    // These unit tests demonstrate
    // 1) firing events 
    // 2) invoking private methods

    [TestClass]
    [Isolated]
    public class InvokingMethodsTests
    {
        [TestMethod]
        public void FireEvent_RunEvent()
        {
            // arrange
            ClassUnderTest classUnderTest = new ClassUnderTest();
            Counter counter = new Counter(classUnderTest);

            // act
            // an event is fired by adding it
            Isolate.Invoke.Event(() => classUnderTest.RunEvent += null, 0);

            // assert
            Assert.AreEqual(1, counter.Times);
        }

        [TestMethod]
        public void InvokePrivateMethod()
        {
            // arrange
            ClassUnderTest classUnderTest = new ClassUnderTest();

            // act
            object result = Isolate.Invoke.Method(classUnderTest, "Sum", 2, 5);

            // assert
            // 2 + 5
            Assert.AreEqual(7, result);
        }

        [TestMethod]
        public void InvokePrivateStaticMethod()
        {
            // act
            object result = Isolate.Invoke.Method<ClassUnderTest>("Multiply", 2, 5);

            // assert
            // 2 * 5
            Assert.AreEqual(10, result);
        }
    }

    // **** Classes under test ****

    public class ClassUnderTest
    {
        public event Action<int> RunEvent;

        private static int Multiply(int a, int b)
        {
            return a * b;
        }

        private int Sum(int a, int b)
        {
            return a + b;
        }
    }

    public class Dependency
    {
        public int Age;

        public string Name;

        public Dependency(int age, string name)
        {
            Age = age;
            Name = name;
        }
    }

    public class Counter
    {
        public Counter(ClassUnderTest underTest)
        {
            underTest.RunEvent += RunEventHandler;
        }

        public int Times { get; set; }

        private void RunEventHandler(int obj)
        {
            Times++;
        }
    }
}