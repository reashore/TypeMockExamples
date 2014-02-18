
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
    [Isolated]
    public class FakingDependenciesTests
    {
        [TestMethod]
        public void FakeAllDependenciesInTheConstructor()
        {
            // arrange
            ClassUnderTest classUnderTestFake = Isolate.Fake.Dependencies<ClassUnderTest>();

            // act
            int result = classUnderTestFake.Calculate(1, 2);

            // assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void FakeAllDependenciesChangeBehaviorOfADependency()
        {
            // arrange
            ClassUnderTest classUnderTestFake = Isolate.Fake.Dependencies<ClassUnderTest>();
            Dependency1 dependencyFake = Isolate.GetFake<Dependency1>(classUnderTestFake);
            Isolate.WhenCalled(() => dependencyFake.Multiplier).WillReturn(2);

            // act
            int result = classUnderTestFake.Calculate(1, 2);

            // assert
            // (1 + 2) * 3
            Assert.AreEqual(6, result);
        }

        [TestMethod]
        public void FakeAllDependenciesOverrideDefaultArguments()
        {
            // arrange
            Dependency1 dependency1 = new Dependency1();
            // what does this do?
            ClassUnderTest classUnderTestFake = Isolate.Fake.Dependencies<ClassUnderTest>(dependency1, 4);

            // act
            int result = classUnderTestFake.Calculate(1, 2);

            // assert
            // (1 + 2) * 1 + 4
            Assert.AreEqual(7, result);
        }
    }

    // **** Classes under test ****

    public class ClassUnderTest
    {
        private readonly int _additional;
        private readonly Dependency1 _depenency1;
        private readonly Dependency2 _dependency2;

        public ClassUnderTest(int additional, Dependency1 depenency1, Dependency2 dependency2)
        {
            _additional = additional;
            _depenency1 = depenency1;
            _dependency2 = dependency2;
        }

        public int Calculate(int a, int b)
        {
            _dependency2.Check();
            return ((a + b) * _depenency1.Multiplier) + _additional;
        }
    }

    public sealed class Dependency1
    {
        public Dependency1()
        {
            Multiplier = 1;
        }

        public int Multiplier { get; set; }
    }

    public class Dependency2
    {
        public virtual int Check()
        {
            throw new NotImplementedException();
        }
    }
}