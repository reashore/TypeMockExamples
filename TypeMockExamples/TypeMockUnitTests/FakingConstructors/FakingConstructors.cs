﻿
namespace TypeMockExamples.TypeMockUnitTests.FakingConstructors
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeMock.ArrangeActAssert;

    /// <summary>
    /// This test class demonstrates controlling arguments passed to constructor of a fake 
    /// and controlling the constructors that are called
    /// </summary>
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

        [TestCleanup]
        public void CleanupTest()
        {
            _classUnderTest = null;
        }

        [TestMethod]
        public void CallConstructorAndPassArgumentsFakeAllMethods()
        {
            // arrange
            // The constructor is not faked here.      
            Dependency dependencyFake = Isolate.Fake.Instance<Dependency>(Members.ReturnRecursiveFakes, ConstructorWillBe.Called, 5, "ConstructorArgument");

            // act
            string result = _classUnderTest.GetString(dependencyFake);

            // assert
            Assert.AreEqual("ConstructorArgument5", result);
        }

        [TestMethod]
        public void IgnoringOnlyConstrutorRestOfMethodsCalled()
        {
            // arrange
            Dependency dependencyFake = Isolate.Fake.Instance<Dependency>(Members.CallOriginal, ConstructorWillBe.Ignored);

            // act
            string result = _classUnderTest.GetString(dependencyFake);

            // assert
            Assert.AreEqual("0", result);
        }

        [TestMethod]
        public void FutureInstanceVerifyThrowingExceptionOnCreation()
        {
            // arrange
            // We want a memory handling exception to be thrown the next time a Dependency is instantiated
            OutOfMemoryException outOfMemoryException = new OutOfMemoryException();
            Isolate.Swap.NextInstance<Dependency>().ConstructorWillThrow(outOfMemoryException);

            // act
            Dependency result = _classUnderTest.Create();

            // assert
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void CallConstructorFakeBaseClassConstructor()
        {
            // assert
            // create an instance of Derived, but avoid calling the base class constructor
            Derived dependencyFake = Isolate.Fake.Instance<Derived>(Members.CallOriginal, ConstructorWillBe.Called, BaseConstructorWillBe.Ignored);

            // act
            int result = _classUnderTest.GetSize(dependencyFake);

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
            try
            {
                return new Dependency(0, string.Empty);
            }
            catch (Exception)
            {
                return null;
            }
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