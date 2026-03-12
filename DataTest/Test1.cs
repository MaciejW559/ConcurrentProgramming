using Data;

namespace DataTest
{
    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Liczby numers = new Liczby(10, 12);
            Assert.AreEqual(10, numers.getA());
            Assert.AreEqual(12, numers.getB());
        }
    }
}
