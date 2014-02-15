
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
    [Isolated(DesignMode.Pragmatic)] // Note: Use Isolated to clean up after all tests in class
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
        public void CallConstructorAndPassArguments_FakeAllMethods()
        {
            // arrange
            // The constructor is not faked here.      
            Dependency fake = Isolate.Fake.Instance<Dependency>(Members.ReturnRecursiveFakes, ConstructorWillBe.Called, 5, "Typemock");

            // act
            string result = _classUnderTest.GetString(fake);

            // assert
            Assert.AreEqual("Typemock5", result);
        }

        [TestMethod]
        public void IgnoringOnlyConstrutor_RestOfMethodsCalled()
        {
            // arrange
            Dependency fake = Isolate.Fake.Instance<Dependency>(Members.CallOriginal, ConstructorWillBe.Ignored);

            // act
            string result = _classUnderTest.GetString(fake);

            // assert
            Assert.AreEqual("0", result);
        }

        [TestMethod]
        public void FutureInstance_VerifyThrowingExceptionOnCreation()
        {
            // arrange
            // We want a memory handling exception to be thrown the next time a Dependency is instantiated
            Isolate.Swap.NextInstance<Dependency>().ConstructorWillThrow(new OutOfMemoryException());

            // act
            Dependency result = _classUnderTest.Create();

            // assert
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void CallConstructor_FakeBaseClassConstructor()
        {
            // assert
            // create an instance of Derived, but avoid calling the base class constructor
            Derived dependency = Isolate.Fake.Instance<Derived>(Members.CallOriginal, ConstructorWillBe.Called, BaseConstructorWillBe.Ignored);

            // act
            int result = _classUnderTest.GetSize(dependency);

            // assert
            Assert.AreEqual(100, result);
        }
    }

    ////------------------
    //// Classes under test
    //// - Dependency: Class with Methods that need to be faked out
    //// - ClassUnderTest: Class that creates and uses Dependency
    //// - Base and Derived: Class Hierarchy but Base still needs to implement its constructor
    ////------------------

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

    public class Derived : Base
    {
        public Derived()
        {
            Size = 100;
        }
    }

    public class ClassUnderTest
    {
        public string GetString(Dependency user)
        {
            return user.Name + user.Age;
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
}