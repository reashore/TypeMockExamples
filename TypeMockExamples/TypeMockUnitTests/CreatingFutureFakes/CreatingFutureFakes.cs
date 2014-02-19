
namespace TypeMockExamples.TypeMockUnitTests.CreatingFutureFakes
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeMock.ArrangeActAssert;

    // These unit tests demonstrate instantiating objects outside the scope of the ClassUnderTest. 

    [TestClass]
    [Isolated]
    public class CreatingFutureFakesTests
    {
        [TestMethod]
        public void FakeSingleFutureInstance()
        {
            // arrange
            Isolate.Fake.NextInstance<Dependency>();
            ClassUnderTest classUnderTest = new ClassUnderTest();

            // act
            int result = classUnderTest.AddCheckingInternalDependency(1, 2);

            // assert
            // 1 + 2
            // fake does not throw an exception
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void FakeMultipleFutureInstances()
        {
            // arrange
            ClassUnderTest classUnderTest = new ClassUnderTest();
            Isolate.Fake.AllInstances<Dependency>();

            // act
            int result = classUnderTest.AddCheckingTwoInternalDependencies(1, 2);

            // assert
            // 3 = 1 + 2
            // fake does not throw exception
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void FakeImplementedDependency()
        {
            // arrange
            ClassUnderTest classUnderTest = new ClassUnderTest();
            Isolate.Fake.NextInstance<IDependency>();

            // act
            int result = classUnderTest.AddCheckingDerivedDependency(1, 2);

            // assert
            // 1 + 2
            // fake does not throw exception
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void FakeSingleton()
        {
            // arrange
            // fake all instances (past and future) of singleton
            Singleton singletonFake = Isolate.Fake.AllInstances<Singleton>();
            Isolate.WhenCalled(() => singletonFake.ReturnZero()).WillReturn(10);

            // act
            int result = Singleton.Instance.ReturnZero();

            // assert
            Assert.AreEqual(10, result);
        }
    }

    // **** Classes under test ****

    public class ClassUnderTest
    {
        public int AddCheckingInternalDependency(int x, int y)
        {
            Dependency dependency = new Dependency();

            dependency.Check(x, y);

            return x + y;
        }

        public int AddCheckingDerivedDependency(int x, int y)
        {
            ConcreteDependency dependency = new ConcreteDependency();

            dependency.Check(x, y);

            return x + y;
        }

        public int AddCheckingTwoInternalDependencies(int x, int y)
        {
            Dependency dependency1 = new Dependency();

            dependency1.Check(x, y);

            Dependency dependency2 = new Dependency();

            dependency2.Check(x, y);

            return x + y;
        }
    }

    public interface IDependency
    {
        void Check(int x, int y);
    }

    public class Dependency
    {
        public void Check(int x, int y)
        {
            throw new Exception("Not checked!");
        }
    }

    public class ConcreteDependency : IDependency
    {
        public void Check(int x, int y)
        {
            throw new Exception("Not checked!");
        }
    }

    public class Singleton
    {
        private static readonly Singleton SingletonInstance = new Singleton();

        private Singleton()
        {
        }

        public static Singleton Instance
        {
            get { return SingletonInstance; }
        }

        public int ReturnZero()
        {
            return 0;
        }
    }
}