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
        public void Vector_Mirror_Test()
        {
            var vector = new Vector { X = 1, Y = 5 };

            // mirror along 2x - 3y = 0
            vector.MirrorAlongStraight(2, -3);

            Assert.AreEqual(5, vector.X, 0.0001);
            Assert.AreEqual(-1, vector.Y, 0.0001);

            // mirroring again along the same line should return to the original position
            vector.MirrorAlongStraight(2, -3);

            Assert.AreEqual(1, vector.X, 0.0001);
            Assert.AreEqual(5, vector.Y, 0.0001);

            // mirroring along the line 5x - y = 0 shouldn't change the vector since it lies on that line
            vector.MirrorAlongStraight(5, -1);

            Assert.AreEqual(1, vector.X, 0.0001);
            Assert.AreEqual(5, vector.Y, 0.0001);
        }
    }
}
