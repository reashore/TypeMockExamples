
namespace TypeMockExamples.TypeMockUnitTests.FakingConstructors
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeMock.ArrangeActAssert;

    // These unit tests demonstrate:
    // 1) controlling arguments passed to constructor of a fake 
    // 2) controlling the constructors that are called


    [TestClass]
    [Isolated]
    public class FakingConstructorTests
    {
        private ClassUnderTest _classUnderTest;

        [TestInitialize]
        public void InitializeTest()
        {
            _classUnderTest = new ClassUnderTest();
        }

        [TestMethod]
        public void CallConstructorAndPassArgumentsFakeAllMethods()
        {
            // arrange
            // The non-faked constructor is called    
            Dependency dependencyFake = Isolate.Fake.Instance<Dependency>(Members.ReturnRecursiveFakes, ConstructorWillBe.Called, 5, "ConstructorArgument");

            // act
            string result = _classUnderTest.GetString(dependencyFake);

            // assert
            // concat "ConstructorArgument" + 5
            Assert.AreEqual("ConstructorArgument5", result);
        }

        [TestMethod]
        public void IgnoringOnlyConstrutorRestOfMethodsCalled()
        {
            // arrange
            // the constructor is ignored
            Dependency dependencyFake = Isolate.Fake.Instance<Dependency>(Members.CallOriginal, ConstructorWillBe.Ignored);

            // act
            string result = _classUnderTest.GetString(dependencyFake);

            // assert
            // concat "" + 0
            Assert.AreEqual("0", result);
        }

        [TestMethod]
        [ExpectedException(typeof(OutOfMemoryException))]
        public void FutureInstanceVerifyThrowingExceptionOnCreation()
        {
            // arrange
            // An OutOfMemoryException to be thrown the next time a Dependency is instantiated
            OutOfMemoryException outOfMemoryException = new OutOfMemoryException();
            Isolate.Swap.NextInstance<Dependency>().ConstructorWillThrow(outOfMemoryException);

            // act
            _classUnderTest.Create();

            // assert
            // OutOfMemoryException is thrown
        }

        [TestMethod]
        public void CallConstructorFakeBaseClassConstructor()
        {
            // assert
            // fake instance of Derived and do not call the base class constructor
            Derived derivedFake = Isolate.Fake.Instance<Derived>(Members.CallOriginal, ConstructorWillBe.Called, BaseConstructorWillBe.Ignored);

            // act
            int result = _classUnderTest.GetSize(derivedFake);

            // assert
            Assert.AreEqual(100, result);
        }
    }

    // **** Classes under test ****

    public class ClassUnderTest
    {
        public string GetString(Dependency dependency)
        {
            return dependency.Name + dependency.Age;
        }

        public Dependency Create()
        {
            return new Dependency(0, string.Empty);
        }

        public int GetSize(Derived derived)
        {
            return derived.Size;
        }
    }

    public class Dependency
    {
        public int Age;
        public string Name;

        // replacing public fields with properties causes test to fail
        //public int Age { get; set; }
        //public string Name { get; set; }

        public Dependency(int age, string name)
        {
            Age = age;
            Name = name;
        }

        public virtual void DoSomething()
        {
            throw new NotImplementedException();
        }
    }

    public class Base
    {
        public Base()
        {
            throw new NotImplementedException();
        }

        public virtual int Size { get; set; }
    }

    public sealed class Derived : Base
    {
        public Derived()
        {
            Size = 100;
        }
    }
}