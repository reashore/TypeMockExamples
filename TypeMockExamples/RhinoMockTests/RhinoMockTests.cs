

namespace TypeMockExamples.RhinoMockTests
{
    using NUnit.Framework;
    using Rhino.Mocks;
    using Annotations;

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
    }

    // domain classes under test

    public interface IWebService
    {
        void LogError(string message);
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
