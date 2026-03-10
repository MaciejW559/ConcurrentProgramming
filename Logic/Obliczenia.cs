using Data;

namespace Logic
{
    public class Obliczenia
    {
        private Liczby liczby;

        public Obliczenia(Liczby liczby)
        {
            this.liczby = liczby;
        }

        public int dodaj()
        {
            return liczby.getA() + liczby.getB();
        }

    }
}
