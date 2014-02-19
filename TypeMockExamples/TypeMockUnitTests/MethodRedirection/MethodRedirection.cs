
namespace TypeMockExamples.TypeMockUnitTests.MethodRedirection
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeMock.ArrangeActAssert;

    // These unit tests demonstrate swapping calls on compatible objects using Swap.CallsOn(object1).WithCallsTo(object2). 
    // The classes Duck and Dog are compatible in that they share the common methods Walk() and Talk().

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

        [TestMethod]
        public void DuckTypeSwap_ReplaceADuckWithADog1()
        {
            // arrange
            Isolate.Swap.CallsOn(_duck).WithCallsTo(_dog);

            // act
            string result = _duck.Talk();

            // assert
            // the call to duck was redirected to dog
            Assert.AreEqual("Woof", result);
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
            Isolate.Verify.WasCalledWithExactArguments(() => _duck.Walk());
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
            return "Quack";
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
            return "Woof";
        }

        private void ChaseCar()
        {
        }
    }
}