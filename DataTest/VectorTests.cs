using Data;

namespace DataTest
{
    [TestClass]
    public sealed class VectorTests
    {
        [TestMethod]
        public void Vector_Initialization_Test()
        {
            var vector = new Vector { X = 0.05, Y = -0.05 };

            Assert.AreEqual(0.05, vector.X, 0.0001, "the initial value of X should be different");
            Assert.AreEqual(-0.05, vector.Y, 0.0001, "the initial value of Y should be different");
        }

        [TestMethod]
        public void Vector_Dot_Test()
        {
            IVector vector1 = new Vector { X = 1, Y = 5 };
            IVector vector2 = new Vector { X = -1, Y = 3 };

            Assert.AreEqual(14, vector1.Dot(vector2), 0.0001, "the dot product should be 14");
            Assert.AreEqual(14, vector2.Dot(vector1), 0.0001, "the dot product should be 14");
        }
    }
}
