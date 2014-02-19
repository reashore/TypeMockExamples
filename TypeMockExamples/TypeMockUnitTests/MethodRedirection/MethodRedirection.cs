
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
    [Isolated]
    public class MethodRedirectionTests
    {
        private Duck _duck;
        private Dog _dog;

        [TestInitialize]
        public void InitializeTest()
        {
            _duck = new Duck();
            _dog = new Dog();
        }

         // These tests demonstrates swapping method calls between two partially compatible objects - a duck and a dog.
         // Both a duck and a dog can walk and talk, so when a duck is swapped by a dog it will go 'woof' instead of
         // 'quack' when talking, and chase cars instead of waddle when walking. However, a duck can lay eggs while a 
         // dog can't - this behavior is preserved by the swapped object.

        [TestMethod]
        public void DuckTypeSwap_ReplaceADuckWithADog1()
        {
            // arrange
            Isolate.Swap.CallsOn(_duck).WithCallsTo(_dog);

            // act
            string result = _duck.Talk();

            // assert
            // duck returns 'Woof!' instead of 'Quack!'
            Assert.AreEqual("Woof!", result);
        }

        [TestMethod]
        public void DuckTypeSwap_ReplaceADuckWithADog2()
        {
            // arrange
            Isolate.Swap.CallsOn(_duck).WithCallsTo(_dog);

            // act
            _duck.LayEgg();

            // assert
            Assert.AreEqual(1, _duck.EggCount);
        }

        [TestMethod]
        public void DuckTypeSwap_ReplaceADuckWithADog3()
        {
            // arrange
            Isolate.Swap.CallsOn(_duck).WithCallsTo(_dog);

            // act
            _duck.Walk();

            // assert
            // Note that converting lambda expression to method group will break the test
            Isolate.Verify.WasCalledWithAnyArguments(() => _duck.Walk());
        }
    }

    // **** Classes under test ****

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