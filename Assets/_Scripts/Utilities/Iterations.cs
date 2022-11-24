using System;
using UnityEngine;

namespace Utilities
{
    public static class Iterations
    {
        public static void Iterate(Action<int, int> action, Vector2Int vector)
        {
            Iterate(action, vector.x, vector.y);
        }

        public static void Iterate(Action<int, int, int> action, Vector3Int vector)
        {
            Iterate(action, vector.x, vector.y, vector.z);
        }

        public static void Iterate(Action<int> action, int sizeX)
        {
            for (int x = 0; x < sizeX; x++)
            {
                action?.Invoke(x);
            }
        }

        public static void Iterate(Action<int, int> action, int sizeX, int sizeY)
        {
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    action?.Invoke(x, y);
                }
            }
        }

        public static void Iterate(Action<int, int, int> action, int sizeX, int sizeY, int sizeZ)
        {
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    for (int z = 0; z < sizeZ; z++)
                    {
                        action?.Invoke(x, y, z);
                    }
                }
            }
        }
    }
}