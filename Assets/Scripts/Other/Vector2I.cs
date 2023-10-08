namespace LumosLabs.Raindrops
{
    /// <summary>
    /// Really just a Vector2 with integers.
    /// </summary>
    public struct Vector2I
    {
        public int x;
        public int y;

        public Vector2I(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static bool operator ==(Vector2I v1, Vector2I v2)
        {
            return v1.x == v2.x && v1.y == v2.y;
        }

        public static bool operator !=(Vector2I v1, Vector2I v2)
        {
            return v1.x != v2.x || v1.y != v2.y;
        }

        public override string ToString()
        {
            return "[" + x + ", " + y + "]";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector2I))
                return false;

            Vector2I vector = (Vector2I)obj;

            return (x == vector.x) && (y == vector.y);
        }

        public override int GetHashCode()
        {
            return x ^ y;
        }
    }
}