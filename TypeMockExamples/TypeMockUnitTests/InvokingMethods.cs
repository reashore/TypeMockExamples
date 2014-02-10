using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TypeMock.ArrangeActAssert;

namespace TypeMockExamples.TypeMockUnitTests.InvokingMethods
{
    /// <summary>
    /// This class demonstrates the ability of firing events and invoking private methods using Isolator.
    /// </summary>
    [TestClass]
    [Isolated(DesignMode.Pragmatic)] // Note: Use Isolated to clean up after all tests in class
    public class InvokingMethods
    {
        [TestMethod]
        public void FireEvent_RunEvent()
        {
            // arrange
            ClassUnderTest underTest = new ClassUnderTest();
            Counter counter = new Counter(underTest);

            // act
            // Note how adding a dummy event is the way to fire it
            Isolate.Invoke.Event(() => underTest.RunEvent += null, 0);

            // assert
            Assert.AreEqual(1, counter.Times);
        }

        [TestMethod]
        public void InvokePrivateMethod()
        {
            // arrange
            ClassUnderTest underTest = new ClassUnderTest();

            // act
            object result = Isolate.Invoke.Method(underTest, "Sum", 2, 5);

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

    //------------------
    // Classes under test
    // - Dependency: Class with Methods that need to be faked out
    // - ClassUnderTest: Class that creates and uses Dependency
    // - Counter: A Class that registers to our ClassUnderTest events
    //------------------

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
            underTest.RunEvent += underTest_RunEvent;
        }

        public int Times { get; set; }

        private void underTest_RunEvent(int obj)
        {
            Times++;
        }
    }

    public class ClassUnderTest
    {
        public event Action<int> RunEvent;

        private int Sum(int a, int b)
        {
            return a + b;
        }

        private static int Multiply(int a, int b)
        {
            return a*b;
        }
    }
}