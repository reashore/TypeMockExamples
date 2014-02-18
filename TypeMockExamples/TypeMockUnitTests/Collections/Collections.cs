
namespace TypeMockExamples.TypeMockUnitTests.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeMock.ArrangeActAssert;

    [TestClass]
    [Isolated]
    public class CollectionTests
    {
        private ClassUnderTest _classUnderTest;
        private Dependency _dependency;

        [TestInitialize]
        public void InitializeTest()
        {
            _classUnderTest = new ClassUnderTest();
            _dependency = new Dependency();
        }

        [TestCleanup]
        public void CleanupTest()
        {
            _classUnderTest = null;
            _dependency = null;
        }

        // These unit tests demonstrate
        // 1) configuring a method to return a different collection

        [TestMethod]
        public void SwapCollectionWithFakeData2()
        {
            // arrange
            List<int> intList = new List<int> {1, 2, 3};
            Isolate.WhenCalled(() => _dependency.GetList()).WillReturnCollectionValuesOf(intList);

            // act
            int result = _classUnderTest.Sum(_dependency);

            // assert
            Assert.AreEqual(6, result);
        }

        [TestMethod]
        public void ImplictCollectionCreationByFakingLastItem2()
        {
            // arrange
            // configure index 5 of list to return 3
            // causes list of length 6 to be created
            Isolate.WhenCalled(() => _dependency.GetList()[5]).WillReturn(3);

            // act
            int result = _classUnderTest.Count(_dependency);

            // assert
            Assert.AreEqual(6, result);
            Assert.AreEqual(3, _dependency.GetList()[5]);
        }
    }

    // **** Classes under test ****

    public class ClassUnderTest
    {
        public int Sum(Dependency dependency)
        {
            return dependency.GetList().Sum();
        }

        public int Count(Dependency dependency)
        {
            return dependency.GetList().Count;
        }
    }

    public class IntList : IList<int>
    {
        #region IList implementation

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public int this[int index]
        {
            get { throw new NotImplementedException(); }

            set { throw new NotImplementedException(); }
        }

        public int IndexOf(int item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, int item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void Add(int item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(int item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(int[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(int item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<int> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class Dependency
    {
        public virtual IntList GetList()
        {
            throw new NotImplementedException();
        }
    }
}