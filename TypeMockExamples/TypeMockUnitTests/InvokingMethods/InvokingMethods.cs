
using TypeMockExamples.Properties;

namespace TypeMockExamples.TypeMockUnitTests.InvokingMethods
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeMock.ArrangeActAssert;

    /// <summary>
    /// This class demonstrates the ability of firing events and invoking private methods using Isolator.
    /// </summary>
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
            // Note how adding a dummy event is the way to fire it
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
            Assert.AreEqual(7, result);
        }

        [TestMethod]
        public void InvokePrivateStaticMethod()
        {
            // act
            object result = Isolate.Invoke.Method<ClassUnderTest>("Multiply", 2, 5);

            // assert
            Assert.AreEqual(10, result);
        }
    }

    // **** Classes under test ****

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
            underTest.RunEvent += UnderTest_RunEvent;
        }

        public int Times { get; set; }

        private void UnderTest_RunEvent(int obj)
        {
            Times++;
        }
    }

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
}