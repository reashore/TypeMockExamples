﻿
namespace TypeMockExamples.TypeMockUnitTests.MethodRedirection
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeMock.ArrangeActAssert;

    /// <summary>
    /// This test class demonstrates performing "Duck-type" swapping between objects using 
    /// Isolate.Swap.CallsOn(object).WithCallsTo(object). 
    /// The concept of duck-typing can be phrased simply as "if it walks like a duck and talks like a duck, it must be a duck". 
    /// In this context duck-typing is used to substitute behavior between two objects that are not necessarily identical. 
    /// When calling a method on the first object that also exists in the second object (i.e. has the same name, arguments 
    /// and return value), the second object's implementation will be called instead.
    /// </summary>
    [TestClass]
    [Isolated(DesignMode.Pragmatic)] // Note: Use Isolated to clean up after all tests in class
    public class MethodRedirectionTests
    {
        /// <summary>
        /// This test demonstrates swapping method calls between two partially compatible objects - a duck and a dog.
        /// Both a duck and a dog can walk and talk, so when a duck is swapped by a dog it will go 'woof' instead of
        /// 'quack' when talking, and chase cars instead of waddle when walking. However, a duck can lay eggs while a 
        /// dog can't - this behavior is preserved by the swapped object.
        /// </summary>
        [TestMethod]
        public void DuckTypeSwap_ReplaceADuckWithADog()
        {
            // arrange
            Duck duck = new Duck();
            Dog dog = new Dog();
            Isolate.Swap.CallsOn(duck).WithCallsTo(dog);

            // act
            string result = duck.Talk();
            duck.LayEgg();
            duck.Walk();

            // assert
            // The duck object will now go 'Woof!' instead of 'Quack!'
            Assert.AreEqual("Woof!", result);
            //// It is still a duck in every aspect that a dog can't do
            Assert.AreEqual(1, duck.EggCount);
            //// Even though duck calls are now redirected to dog calls, we can still verify the duck calls are made
            // Note: converting to method group will break the test
            Isolate.Verify.WasCalledWithAnyArguments(() => duck.Walk());
        }
    }

    //------------------
    // Classes under test
    // - Duck: Class with Methods that will be redirected
    // - Dog: Class with new implementation of some Duck methods
    //------------------
    public class Duck
    {
        public int EggCount { get; private set; }

        public void Walk()
        {
            Waddle();
        }

        public string Talk()
        {
            return "Quack!";
        }

        public void LayEgg()
        {
            EggCount += 1;
        }

        private void Waddle()
        {
        }
    }

    public class Dog
    {
        public void Walk()
        {
            ChaseCar();
        }

        public string Talk()
        {
            return "Woof!";
        }

        private void ChaseCar()
        {
        }
    }
}