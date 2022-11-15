using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace MeshCreation
{
    public class WorldGenerator : Singleton<WorldGenerator>
    {
        [Header("Prefabs")]
        [SerializeField] private ChunkGenerator chunkGenerator;

        [Header("Hexagon Info")]
        [SerializeField] private float hexagonSize = 0.57735f;
        [SerializeField] private float hexagonHeight = 1f;

        [Header("Chunk Info")]
        [SerializeField] private Vector3Int chunkSize;
        [SerializeField] private Vector2Int chunkGridSize;

        public float HexagonSize => hexagonSize != 0 ? hexagonSize : HexInfo.HEX_DEF_SIZE;
        public float HexagonHeight => hexagonHeight != 0 ? hexagonHeight : HexInfo.HEX_DEF_HEIGHT;
        public Vector3Int ChunkSize => chunkSize;

        public readonly Dictionary<Vector2Int, ChunkData> terrain = new Dictionary<Vector2Int, ChunkData>();

        public void GenerateWorld()
        {
            Iterations.Iterate(InitializeTerrain, chunkGridSize);
            GenerateTerrain();
        }

        private void InitializeTerrain(int chunkX, int chunkY)
        {
            Vector2Int chunkIndex = new Vector2Int(chunkX, chunkY);
            HexType[,,] chunkTerrain = TerrainGenerator.GenerateChunkTerrain(chunkSize, chunkIndex);

            Vector3 chunkOffset = HexInfo.GetWorldCoords(chunkX * chunkSize.x, 0, chunkY * chunkSize.z);
            ChunkGenerator chunk =
                Instantiate(chunkGenerator, _transform.position + chunkOffset, Quaternion.identity, _transform);

            ChunkData chunkData = new ChunkData(chunkIndex, chunkTerrain, chunk);

            terrain.Add(chunkIndex, chunkData);
        }

        private void GenerateTerrain()
        {
            foreach (ChunkData value in terrain.Values)
            {
                value.chunkGenerator.RegenerateChunk();
            }
        }
    }
}