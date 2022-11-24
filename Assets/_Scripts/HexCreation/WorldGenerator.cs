using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace MeshCreation
{
    public class WorldGenerator : Singleton<WorldGenerator>
    {
        [Header("Hexagon Info")]
        [SerializeField] private float hexagonSize = 0.57735f;
        [SerializeField] private float hexagonHeight = 1f;

        [Header("World Generation")]
        [SerializeField] private int warmUpChunks;
        [SerializeField] private TerrainGenerator.NoiseOctave[] noiseGenerator;

        [Header("Chunk Info")]
        [SerializeField] private Vector3Int chunkSize;
        [SerializeField] private int radius;

        [Header("Prefabs")]
        [SerializeField] private ChunkGenerator chunkGenerator;

        public float HexagonSize => hexagonSize != 0 ? hexagonSize : HexInfo.HEX_DEF_SIZE;
        public float HexagonHeight => hexagonHeight != 0 ? hexagonHeight : HexInfo.HEX_DEF_HEIGHT;
        public Vector3Int ChunkSize => chunkSize;

        public readonly Dictionary<Vector2Int, ChunkData> terrain = new Dictionary<Vector2Int, ChunkData>();
        private readonly List<Vector2Int> _loadedChunks = new List<Vector2Int>();
        private readonly List<Vector2Int> _unloadedChunks = new List<Vector2Int>();

        private TerrainGenerator _terrainGenerator;

        private void Start()
        {
            _terrainGenerator = new TerrainGenerator(noiseGenerator, chunkSize);

            //PrepareChunks();
        }

        private void PrepareChunks()
        {
            for (int x = 0; x < warmUpChunks; x++)
            {
                for (int y = 0; y < warmUpChunks; y++)
                {
                    
                    Vector2Int chunkIndex = new Vector2Int(x - warmUpChunks / 2, y - warmUpChunks / 2);

                    Vector3 chunkOffset = HexInfo.GetWorldCoords(chunkIndex.x * chunkSize.x, 0, chunkIndex.y * chunkSize.z);
                    ChunkGenerator chunk = Instantiate(chunkGenerator, _transform.position + chunkOffset, Quaternion.identity, _transform);
                    ChunkData chunkData = new ChunkData(chunkIndex, null, chunk);
                    chunkData.SetActive(false);
                    
                    terrain.Add(chunkIndex, chunkData);
                }
            }
        }

        public void GenerateWorld(Vector2Int pos)
        {
            _loadedChunks.Clear();
            _unloadedChunks.Clear();

            for (int x = 0; x < 2 * radius; x++)
            {
                for (int y = 0; y < 2 * radius; y++)
                {
                    Vector2Int chunkIndex = new Vector2Int(x + pos.x - radius, y + pos.y - radius);

                    _loadedChunks.Add(chunkIndex);

                    if (!IsInsideCircle(x - radius, y - radius, radius) || terrain.ContainsKey(chunkIndex))
                    {
                        continue;
                    }

                    HexType[,,] chunkTerrain = _terrainGenerator.GenerateChunkTerrain(chunkIndex);

                    Vector3 chunkOffset =
                        HexInfo.GetWorldCoords(chunkIndex.x * chunkSize.x, 0, chunkIndex.y * chunkSize.z);
                    ChunkGenerator chunk = Instantiate(chunkGenerator, _transform.position + chunkOffset,
                        Quaternion.identity, _transform);

                    ChunkData chunkData = new ChunkData(chunkIndex, chunkTerrain, chunk);
                    chunkData.SetActive(true);
                    terrain.Add(chunkIndex, chunkData);
                }
            }

            foreach (var data in terrain.Keys)
            {
                if (!_loadedChunks.Contains(data))
                {
                    terrain[data].chunkGenerator.gameObject.SetActive(false);
                    _unloadedChunks.Add(data);
                }
            }

            foreach (var data in _unloadedChunks)
            {
                terrain.Remove(data);
            }

            foreach (ChunkData chunk in terrain.Values)
            {
                chunk.chunkGenerator.RegenerateChunk();
            }
        }


        private bool IsInsideCircle(int x, int y, int r)
        {
            return x * x + y * y <= r * r;
        }
        
        [ContextMenu("Regenerate terrain")]
        void Regenerate()
        {
            _terrainGenerator = new TerrainGenerator(noiseGenerator, chunkSize);

            foreach (var data in terrain)
            {
                Destroy(data.Value.chunkGenerator.gameObject);
            }

            terrain.Clear();

            GenerateWorld(Vector2Int.zero);
        }
    }
}