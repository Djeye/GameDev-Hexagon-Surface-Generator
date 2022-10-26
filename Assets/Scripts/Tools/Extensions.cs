namespace Tools
{
    public static class Extensions
    {
        public static bool IsOdd(this int entity)
        {
            return entity % 2 != 0;
        }

        public static bool IsEven(this int entity)
        {
            return entity % 2 == 0;
        }
    }
}