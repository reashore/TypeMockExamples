
namespace TypeMockExamples.TypeMockUnitTests.FakingDependencies
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeMock.ArrangeActAssert;

    /// <summary>
    /// This test class shows different ways of faking dependencies of a class.
    /// This method, creates a type by creating fakes for the longest constructor and calling it.
    /// This is a great tool to make sure that adding dependencies to the type won't fail the tests.
    /// </summary>
    [TestClass]
    [Isolated] // Note: Use Isolated to clean up after the test
    public class FakingDependenciesTests
    {
        [TestMethod]
        public void FakeAllDependenciesInTheConstructor()
        {
            // arrange
            ClassUnderTest realClassUnderTest = Isolate.Fake.Dependencies<ClassUnderTest>();

            // act
            int result = realClassUnderTest.Calculate(1, 2);

            // assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void FakeAllDependencies_ChangeBehaviorOfADependency()
        {
            // arrange
            ClassUnderTest realClassUnderTest = Isolate.Fake.Dependencies<ClassUnderTest>();
            Dependency fakeDependency = Isolate.GetFake<Dependency>(realClassUnderTest);
            Isolate.WhenCalled(() => fakeDependency.Multiplier).WillReturn(2);

            // act
            int result = realClassUnderTest.Calculate(1, 2);

            // assert
            Assert.AreEqual(6, result);
        }

        [TestMethod]
        public void FakeAllDependencies_OverrideDefaultArguments()
        {
            // arrange
            Dependency realDependency = new Dependency();
            ClassUnderTest realClassUnderTest = Isolate.Fake.Dependencies<ClassUnderTest>(realDependency, 4);

            // act
            int result = realClassUnderTest.Calculate(1, 2);

            // assert
            Assert.AreEqual(7, result);
        }
    }

    //------------------
    // Classes under test
    // - Dependency: A dependency of ClassUnderTest
    // - Dependency2: Methods are not implemented - these need to be faked out
    // - ClassUnderTest: Class that uses Dependency
    //------------------

    public class Dependency
    {
        public Dependency()
        {
            Multiplier = 1;
        }

        public virtual int Multiplier { get; set; }
    }

    public class Dependency2
    {
        public virtual int Check()
        {
            throw new NotImplementedException();
        }
    }

    public class ClassUnderTest
    {
        private readonly int _additional;
        private readonly Dependency _d1;
        private readonly Dependency2 _d2;

        public ClassUnderTest(int additional, Dependency2 d2, Dependency d1)
        {
            _additional = additional;
            _d2 = d2;
            _d1 = d1;
        }

        public int Calculate(int a, int b)
        {
            _d2.Check();
            return ((a + b) * _d1.Multiplier) + _additional;
        }
    }
}