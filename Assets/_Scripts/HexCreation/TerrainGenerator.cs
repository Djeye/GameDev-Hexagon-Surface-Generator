using Utilities;
using UnityEngine;

namespace MeshCreation
{
    public static class TerrainGenerator
    {
        public static HexType[,,] GenerateChunkTerrain(Vector3Int chunkSize, Vector2Int chunkIndex)
        {
            int chunkSizeX = chunkSize.x;
            int chunkSizeY = chunkSize.y;
            int chunkSizeZ = chunkSize.z;

            var result = new HexType[chunkSizeX, chunkSizeY, chunkSizeZ];

            Vector2Int chunk2DSize = new Vector2Int(chunkSizeX, chunkSizeZ);

            Iterations.Iterate(GenerateHexagonTerrain, chunk2DSize);

            return result;

            void GenerateHexagonTerrain(int x, int z)
            {
                float xScale = 1f / chunkSizeX;
                float zScale = 1f / chunkSizeZ;

                float noise = Mathf.PerlinNoise(x * xScale + chunkIndex.x, z * zScale + chunkIndex.y) * chunkSizeY;
                float height = Mathf.Clamp(noise, 0, chunkSizeY);

                for (int y = 0; y < height; y++)
                {
                    result[x, y, z] = HexType.Dirt;
                }
            }
        }
    }
}