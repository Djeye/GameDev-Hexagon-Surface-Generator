using UnityEngine;

namespace Utilities
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
        
        public static void DestroyChildren(this Transform t) {
            foreach (Transform child in t)
            {
                Object.Destroy(child.gameObject);
            }
        }
    }
}