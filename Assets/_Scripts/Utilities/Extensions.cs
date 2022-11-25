using UnityEngine;

namespace Utilities
{
    public static class Extensions
    {
        public static int Sign(this int num)
        {
            return num >= 0 ? 1 : -1;
        }
        public static bool IsOdd(this int entity)
        {
            return entity % 2 != 0;
        }

        public static bool IsEven(this int entity)
        {
            return entity % 2 == 0;
        }
        
        public static void DestroyChildren(this Transform t) {
            foreach (Transform child in t)
            {
                Object.Destroy(child.gameObject);
            }
        }
    }
}