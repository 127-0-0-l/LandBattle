using System.Windows.Shapes;

namespace Land_Battle
{
    class Lines
    {
        Rectangle rectangle;
        int x;
        int y;

        public Lines(Rectangle rct, int i, int j)
        {
            rectangle = rct;
            x = i;
            y = j;
        }

        public Rectangle Rct
        {
            get { return rectangle; }
        }

        public int X
        {
            get { return x; }
        }

        public int Y
        {
            get { return y; }
        }
    }
}
