using System.Collections.Generic;
using UnityEngine;


public static class HexInfo
{
    public const int HEX_VERTECIES = 6;
    public const float HEX_DEF_SIZE = 0.57735f; // 1 / sqrt(3)
    public const float HEX_DEF_HEIGHT = 1f;

    public const float SQRT3 = 1.73205f; // sqrt(3)

    private static readonly float _hexHeight = 2f * HexRadius;
    private static readonly float _hexWidth = SQRT3 * HexRadius;

    private static readonly float _hexHorizontalOffset = 0.5f * _hexWidth;
    private static readonly float _hexHorizontalShift = _hexWidth;
    private static readonly float _hexHeightShift = HexagonHeight;
    private static readonly float _hexVerticalShift = 3 / 4f * _hexHeight;

    private static float HexRadius => WorldGenerator.Instance.HexagonSize;
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
        {Sides.Top, Vector3.zero},
        {Sides.Bottom, _hexHeightShift * Vector3.down}
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

    public static Vector3 ConvertToHexagonAxis(int x, int y, int z)
    {
        float hexX = x * _hexHorizontalShift + z * _hexHorizontalOffset;
        float hexY = y * _hexHeightShift;
        float hexZ = z * _hexVerticalShift;

        return new Vector3(hexX, hexY, hexZ);
    }

    public static Vector3Int ConvertToCommonAxis(float hexX, float hexY, float hexZ)
    {
        float dirtyZ = hexZ / _hexVerticalShift;

        int x = (int)((hexX - dirtyZ * _hexHorizontalOffset) / _hexHorizontalShift);
        int z = (int)dirtyZ;
        int y = (int)(hexY / _hexHeightShift);

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