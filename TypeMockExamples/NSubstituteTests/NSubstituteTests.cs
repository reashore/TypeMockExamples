
namespace TypeMockExamples.NSubstituteTests
{
    using System;
    using NUnit.Framework;
    using NSubstitute;

    [TestFixture]
    class NSubstituteTests
    {
        [Test]
        public void NSubstituteTest1()
        {
            // arrange
            ICalculator calculator = Substitute.For<ICalculator>();
            calculator.Add(1, 2).Returns(3);

            // act
            int result = calculator.Add(1, 2);

            // assert
            Assert.IsTrue(result == 3);
            Assert.That(result, Is.EqualTo(3));
        }

        [Test]
        public void NSubstituteTest2()
        {
            // arrange
            ICalculator calculator = Substitute.For<ICalculator>();
            calculator.Add(1, 2).Returns(3);

            // act
            int result = calculator.Add(1, 2);

            // assert
            calculator.Received().Add(1, 2);
            calculator.DidNotReceive().Add(5, 7);
        }


        [Test]
        public void NSubstituteTest3()
        {
            // arrange
            ICalculator calculator = Substitute.For<ICalculator>();
            calculator.Add(1, 2).Returns(3);
            calculator.Mode.Returns("DEC");

            // act
            int result = calculator.Add(1, 2);

            // assert
            Assert.That(calculator.Mode, Is.EqualTo("DEC"));
        }

        [Test]
        public void NSubstituteTest4()
        {
            // arrange
            ICalculator calculator = Substitute.For<ICalculator>();
            //calculator.Add(1, 2).Returns(3);

            // act
            int result = calculator.Add(10, -5);

            // assert
            calculator.Received().Add(10, Arg.Any<int>());
            calculator.Received().Add(10, Arg.Is<int>(x => x < 0));
        }

        [Test]
        public void NSubstituteTest5()
        {
            // arrange
            ICalculator calculator = Substitute.For<ICalculator>();
            calculator.Add(1, 2).Returns(3);
            calculator.Mode.Returns("HEX", "DEC", "BIN");

            // act
            int result = calculator.Add(1, 2);

            // assert
            Assert.That(calculator.Mode, Is.EqualTo("HEX"));
            Assert.That(calculator.Mode, Is.EqualTo("DEC"));
            Assert.That(calculator.Mode, Is.EqualTo("BIN"));
        }

        [Test]
        public void NSubstituteTest6()
        {
            // arrange
            ICalculator calculator = Substitute.For<ICalculator>();
            calculator.Add(1, 2).Returns(3);
            bool eventWasRaised = false;
            calculator.PowerUp += () => eventWasRaised = true;

            // act
            int result = calculator.Add(1, 2);
            calculator.PowerUp += Raise.Event<Action>();

            // assert
            Assert.That(eventWasRaised);
        }
    }

    // Domain classes

    public interface ICalculator
    {
        int Add(int a, int b);
        string Mode { get; set; }
        event Action PowerUp;
    }
}
