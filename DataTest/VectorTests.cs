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
        public void Vector_Flip_Test()
        {
            var vector = new Vector { X = 0.05, Y = -0.05 };

            vector.FlipX();
            vector.FlipY();

            Assert.AreEqual(-0.05, vector.X, 0.0001, "the X value should be flipped");
            Assert.AreEqual(0.05, vector.Y, 0.0001, "the Y value should be flipped");
        }
    }
}
