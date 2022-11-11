using System.Collections.Generic;
using UnityEngine;

namespace MeshCreation
{
    public static class HexInfo
    {
        public const int HEX_VERTECIES = 6;
        public const float HEX_DEF_SIZE = 0.57735f; // 1 / sqrt(3)
        public const float HEX_DEF_HEIGHT = 1f;

        private const float SQRT3 = 1.73205f; // sqrt(3)

        private static readonly float _hexWidth = SQRT3 * HexSize;
        private static readonly float _hexHeight = 2f * HexSize;

        public static readonly float HEX_HORIZONTAL_SHIFT = 0.5f * _hexWidth;
        public static readonly float HEX_HORIZONTAL_OFFSET = _hexWidth;
        public static readonly float HEX_VERTICAL_OFFSET = 3 / 4f * _hexHeight;

        private static readonly float _hexHeightOffset = HexagonHeight;
        private static readonly float _hexHeightHalfOffset = _hexHeightOffset * 0.5f;

        private static float HexSize => WorldGenerator.Instance.HexagonSize;
        private static float HexagonHeight => WorldGenerator.Instance.HexagonHeight;


        public enum Sides : byte
        {
            Top = 0,
            Bottom = 1,
            LeftFront = 2,
            Left = 3,
            LeftBack = 4,
            RightBack = 5,
            Right = 6,
            RightFront = 7
        }

        public static readonly Dictionary<Sides, Vector3> FLAT_OFFSET = new Dictionary<Sides, Vector3>()
        {
            {Sides.Top, _hexHeightHalfOffset * Vector3.up},
            {Sides.Bottom, _hexHeightHalfOffset * Vector3.down}
        };

        public static readonly Dictionary<Sides, Vector3Int> FLAT_NORMALS = new Dictionary<Sides, Vector3Int>()
        {
            {Sides.Top, Vector3Int.up},
            {Sides.Bottom, Vector3Int.down}
        };

        public static readonly List<Vector3> HEX_COORDS = new List<Vector3>()
        {
            new Vector3(-_hexWidth * 0.5f, 0, -_hexHeight * 0.25f),
            new Vector3(-_hexWidth * 0.5f, 0, _hexHeight * 0.25f),
            new Vector3(0, 0, _hexHeight * 0.5f),

            new Vector3(_hexWidth * 0.5f, 0, _hexHeight * 0.25f),
            new Vector3(_hexWidth * 0.5f, 0, -_hexHeight * 0.25f),
            new Vector3(0, 0, -_hexHeight * 0.5f)
        };

        public static readonly Dictionary<Sides, List<Vector3Int>> HEX_TRIANGLES =
            new Dictionary<Sides, List<Vector3Int>>()
            {
                {
                    Sides.Top, new List<Vector3Int>()
                    {
                        new Vector3Int(0, 4, 5),
                        new Vector3Int(0, 1, 4),
                        new Vector3Int(1, 3, 4),
                        new Vector3Int(1, 2, 3)
                    }
                },
                {
                    Sides.Bottom, new List<Vector3Int>()
                    {
                        new Vector3Int(0, 5, 4),
                        new Vector3Int(0, 4, 1),
                        new Vector3Int(1, 4, 3),
                        new Vector3Int(1, 3, 2)
                    }
                },
                {
                    Sides.LeftFront, new List<Vector3Int>()
                    {
                        new Vector3Int(0, 5, 11),
                        new Vector3Int(11, 6, 0)
                    }
                },
                {
                    Sides.Left, new List<Vector3Int>()
                    {
                        new Vector3Int(1, 0, 6),
                        new Vector3Int(6, 7, 1),
                    }
                },
                {
                    Sides.LeftBack, new List<Vector3Int>()
                    {
                        new Vector3Int(2, 1, 7),
                        new Vector3Int(7, 8, 2),
                    }
                },
                {
                    Sides.RightBack, new List<Vector3Int>()
                    {
                        new Vector3Int(3, 2, 8),
                        new Vector3Int(8, 9, 3),
                    }
                },
                {
                    Sides.Right, new List<Vector3Int>()
                    {
                        new Vector3Int(4, 3, 9),
                        new Vector3Int(9, 10, 4),
                    }
                },
                {
                    Sides.RightFront, new List<Vector3Int>()
                    {
                        new Vector3Int(5, 4, 10),
                        new Vector3Int(10, 11, 5)
                    }
                }
            };

        public static readonly Dictionary<Sides, Vector3Int> HEX_SIDE_NEIGHBORS =
            new Dictionary<Sides, Vector3Int>()
            {
                {Sides.LeftFront, new Vector3Int(0, 0, -1)},
                {Sides.Left, new Vector3Int(-1, 0, 0)},
                {Sides.LeftBack, new Vector3Int(-1, 0, 1)},
                {Sides.RightBack, new Vector3Int(0, 0, 1)},
                {Sides.Right, new Vector3Int(1, 0, 0)},
                {Sides.RightFront, new Vector3Int(1, 0, -1)}
            };

        public static Vector3 GetWorldCoords(Vector3Int pos)
        {
            return GetWorldCoords(pos.x, pos.y, pos.z);
        }

        public static Vector3 GetWorldCoords(int x, int y, int z)
        {
            float hexX = x * HEX_HORIZONTAL_OFFSET + z * HEX_HORIZONTAL_SHIFT;
            float hexY = y * _hexHeightOffset;
            float hexZ = z * HEX_VERTICAL_OFFSET;

            return new Vector3(hexX, hexY, hexZ);
        }

        public static Vector3Int GetHexagonCoords(Vector3 pos)
        {
            return GetHexagonCoords(pos.x, pos.y, pos.z);
        }

        public static Vector3Int GetHexagonCoords(float hexX, float hexY, float hexZ)
        {
            float dirtyZ = hexZ / HEX_VERTICAL_OFFSET;

            int x = Mathf.RoundToInt((hexX - dirtyZ * HEX_HORIZONTAL_SHIFT) / HEX_HORIZONTAL_OFFSET);
            int y = Mathf.RoundToInt(hexY / _hexHeightOffset);
            int z = Mathf.RoundToInt(dirtyZ);

            return new Vector3Int(x, y, z);
        }
    
        public static bool IsOutsideCircle(int radius, int r, int q)
        {
            int innerRange = radius - 1;
            int outerRange = 3 * radius - 3;
            int maxDistance = 2 * radius - 2;

            return r > maxDistance || q > maxDistance || r + q > outerRange || r + q < innerRange;
        }
    }
}