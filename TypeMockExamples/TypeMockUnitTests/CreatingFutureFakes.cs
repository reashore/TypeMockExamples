using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TypeMock.ArrangeActAssert;

namespace TypeMockExamples.TypeMockUnitTests.CreatingFutureFakes
{
    /// <summary>
    /// This test class demonstrates handling of objects instantiated outside the test's scope. 
    /// This is useful to eliminate dependencies in objects created by the business logic being tested
    /// </summary>
    [TestClass]
    [Isolated(DesignMode.Pragmatic)] // Note: Use Isolated to clean up after all tests in class
    public class CreatingFutureFakes
    {
        [TestMethod]
        public void Fake_SingleFutureInstance()
        {
            // arrange
            Isolate.Fake.NextInstance<Dependency>();

            // act
            int result = ClassUnderTest.AddCheckingInternalDependency(1, 2);

            // assert
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void Fake_MultipleFutureInstances()
        {
            // arrange
            Isolate.Fake.AllInstances<Dependency>();

            // act
            int result = ClassUnderTest.AddCheckingTwoInternalDependencies(1, 2);

            // assert
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void FakeSingleton()
        {
            // arrange
            // Here we are setting the same behavior on all instances.
            // The behavior we set on fake will apply to past instance as well
            Singleton fakeSingleton = Isolate.Fake.AllInstances<Singleton>();
            Isolate.WhenCalled(() => fakeSingleton.ReturnZero()).WillReturn(10);

            // act
            int result = Singleton.Instance.ReturnZero();
            
            // assert
            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void Fake_ImplementedDependency()
        {
            // arrange
            Isolate.Fake.NextInstance<IDependency>();

            // act
            int result = ClassUnderTest.AddCheckingDerivedDependency(1, 2);

            // assert
            Assert.AreEqual(3, result);
        }
    }

    //------------------
    // Classes under test
    // - Dependency: Methods are not implemented - these need to be faked out
    // - ClassUnderTest: Class that uses Dependency
    // - Singleton: A Singleton 
    //------------------
    public class Dependency
    {
        public void Check(int x, int y)
        {
            throw new Exception("Not checked!");
        }
    }

    public interface IDependency
    {
        void Check(int x, int y);
    }

    public class ConcreteDependency : IDependency
    {
        public void Check(int x, int y)
        {
            throw new Exception("Not checked!");
        }
    }

    public class ClassUnderTest
    {
        public static int AddCheckingInternalDependency(int x, int y)
        {
            Dependency dependency = new Dependency();
            dependency.Check(x, y);

            return x + y;
        }

        public static int AddCheckingDerivedDependency(int x, int y)
        {
            ConcreteDependency dependency = new ConcreteDependency();
            dependency.Check(x, y);

            return x + y;
        }

        public static int AddCheckingTwoInternalDependencies(int x, int y)
        {
            Dependency dependency = new Dependency();
            dependency.Check(x, y);

            Dependency dependency2 = new Dependency();
            dependency2.Check(x, y);

            return x + y;
        }
    }

    public class Singleton
    {
        private static readonly Singleton instance = new Singleton();

        private Singleton()
        {
        }

        public static Singleton Instance
        {
            get { return instance; }
        }

        public int ReturnZero()
        {
            return 10;
        }
    }
}