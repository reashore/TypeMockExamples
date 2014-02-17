
namespace TypeMockExamples.FluentAssertionsTests
{
    using System;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    [TestClass]
    public class FluentExceptionTests
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
        public void ShouldThrowExceptionTest()
        {
            // arrange
            const string exceptionMessage = "Exception message";

            // act
            Action act = () => _classUnderTest.ThrowException(exceptionMessage);

            // assert
            act.ShouldThrow<Exception>().WithMessage(exceptionMessage);
        }

        [TestMethod]
        public void ShouldThrowArgumentNullExceptionTest()
        {
            // arrange

            // act
            Action act = () => _classUnderTest.ThrowException(null);

            // assert
            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().BeEquivalentTo("message");
        }

        [TestMethod]
        public void ShouldNotThrowExceptionTest()
        {
            // arrange

            // act
            Action act = () => _classUnderTest.DoNotThrowException(100);

            // assert
            act.ShouldNotThrow<Exception>();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void CatchDerivedExceptionTest()
        {
            // arrange

            // act
            _classUnderTest.ThrowDerivedException("some string");

            // assert
            // should throw exception
        }
    }

    // Class under test

    public class ClassUnderTest
    {
        public void ThrowException(string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            throw new Exception(message);
        }

        public void ThrowDerivedException(string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            throw new InvalidOperationException(message);
        }

        public int DoNotThrowException(int value)
        {
            return value * 2;
        }
    }
}