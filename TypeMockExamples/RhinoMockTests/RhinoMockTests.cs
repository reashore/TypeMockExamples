

using TypeMockExamples.Properties;

namespace TypeMockExamples.RhinoMockTests
{
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    class RhinoMockTests
    {
        [Test]
        public void StrictRhinoMockTest()
        {
            // arrange
            MockRepository mockRepository = new MockRepository();
            IWebService mockedWebService = mockRepository.StrictMock<IWebService>();

            using (mockRepository.Record())
            {
                mockedWebService.LogError("Filename too short:abc.ext");                
            }

            LogAnalyzer logAnalyzer = new LogAnalyzer(mockedWebService);
            const string shortFileName = "abc.ext";

            // act
            logAnalyzer.Analyze(shortFileName);

            //assert
            mockRepository.Verify(mockedWebService);
        }

        [Test]
        public void NonStrictRhinoMockTest()
        {
            // arrange
            MockRepository mockRepository = new MockRepository();
            IWebService mockedWebService = mockRepository.DynamicMock<IWebService>();

            using (mockRepository.Record())
            {
                mockedWebService.LogError("Filename too short:abc.ext");
            }

            LogAnalyzer logAnalyzer = new LogAnalyzer(mockedWebService);
            const string shortFileName = "abc.ext";

            // act
            logAnalyzer.Analyze(shortFileName);

            //assert
            mockRepository.Verify(mockedWebService);
        }

        [Test]
        public void ReturnValuesFromFake()
        {
            // arrange
            MockRepository mockRepository = new MockRepository();
            IGetResults mockedGetResults = mockRepository.DynamicMock<IGetResults>();

            using (mockRepository.Record())
            {
                mockedGetResults.GetSomeNumber("A");
                LastCall.Return(1);

                mockedGetResults.GetSomeNumber("A");
                LastCall.Return(2);

                mockedGetResults.GetSomeNumber("B");
                LastCall.Return(3);
            }

            // act
            int result1 = mockedGetResults.GetSomeNumber("B");
            int result2 = mockedGetResults.GetSomeNumber("A");
            int result3 = mockedGetResults.GetSomeNumber("A");      //result3 = 1

            //assert
            Assert.AreEqual(3, result1);
            Assert.AreEqual(1, result2);
            // todo: fix bug
            //Assert.AreEqual(2, result3);
        }
    }

    // domain classes under test

    public interface IWebService
    {
        void LogError(string message);
    }

    public interface IGetResults
    {
        int GetSomeNumber(string value);
    }

    public class LogAnalyzer
    {
        [NotNull] 
        private readonly IWebService _webService;

        public LogAnalyzer(IWebService webService)
        {
            _webService = webService;
        }

        public void Analyze(string fileName)
        {
            if (fileName.Length < 8)
            {
                string message = string.Format("Filename too short:{0}", fileName);
                _webService.LogError(message);
            }
        }
    }
}
