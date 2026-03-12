using Data;
using Logic;

namespace LogicTest
{
    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Obliczenia obliczenia = new Obliczenia(new Liczby(10, 12));
            Assert.AreEqual(22, obliczenia.dodaj());
        }
    }
}
