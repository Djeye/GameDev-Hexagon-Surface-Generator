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
        [SerializeField] private int warmUpChunksRadius;
        [SerializeField] private TerrainGenerator.NoiseOctave[] noiseGenerator;

        [Header("Chunk Info")]
        [SerializeField] private Vector3Int chunkSize;
        [SerializeField] private int radius;

        [Header("Prefabs")]
        [SerializeField] private ChunkGenerator chunkGenerator;

        public float HexagonSize => hexagonSize != 0 ? hexagonSize : HexInfo.HEX_DEF_SIZE;
        public float HexagonHeight => hexagonHeight != 0 ? hexagonHeight : HexInfo.HEX_DEF_HEIGHT;
        public Vector3Int ChunkSize => chunkSize;

        public bool IsGenerated(Vector2Int chunkPos, out ChunkData data) =>
            _terrain.TryGetValue(chunkPos, out data) && data.IsGenerated;

        private readonly Dictionary<Vector2Int, ChunkData> _terrain = new Dictionary<Vector2Int, ChunkData>();
        private readonly List<Vector2Int> _loadedChunks = new List<Vector2Int>();
        private readonly List<Vector2Int> _unloadedChunks = new List<Vector2Int>();

        private TerrainGenerator _terrainGenerator;


        public void PrepareChunks()
        {
            _terrainGenerator = new TerrainGenerator(noiseGenerator, chunkSize);

            for (int x = -warmUpChunksRadius; x < warmUpChunksRadius; x++)
            {
                for (int y = -warmUpChunksRadius; y < warmUpChunksRadius; y++)
                {
                    Vector2Int chunkIndex = new Vector2Int(x, y);
                    Vector3 chunkPosition =
                        HexInfo.GetWorldCoords(chunkIndex.x * chunkSize.x, chunkIndex.y * chunkSize.z);
                    ChunkGenerator chunk = Instantiate(chunkGenerator, chunkPosition, Quaternion.identity, _transform);
                    ChunkData chunkData = new ChunkData(chunkIndex, chunk);
                    _terrain.Add(chunkIndex, chunkData);
                }
            }
        }

        public void GenerateWorld(Vector2Int pos)
        {
            FillUnloadedChunks(pos);
            FillLoadedChunks(pos);

            DisableUnloadedChunks();
            RegenerateLoadedChunks();
        }


        private void FillUnloadedChunks(Vector2Int chunkPos)
        {
            _unloadedChunks.Clear();

            int doubledRadius = 2 * radius;
            for (int x = -doubledRadius; x < doubledRadius; x++)
            {
                for (int y = -doubledRadius; y < doubledRadius; y++)
                {
                    Vector2Int outerChunkInd = new Vector2Int(chunkPos.x + x, chunkPos.y + y);

                    if (!HexInfo.IsInsideCircle(x, y, doubledRadius))
                    {
                        continue;
                    }

                    _unloadedChunks.Add(outerChunkInd);
                }
            }
        }

        private void FillLoadedChunks(Vector2Int pos)
        {
            _loadedChunks.Clear();

            for (int x = -radius; x < radius; x++)
            {
                for (int y = -radius; y < radius; y++)
                {
                    Vector2Int chunkInd = new Vector2Int(pos.x + x, pos.y + y);

                    if (!HexInfo.IsInsideCircle(x, y, radius))
                    {
                        continue;
                    }

                    _unloadedChunks.Remove(chunkInd);

                    if (!_terrain.TryGetValue(chunkInd, out ChunkData data))
                    {
                        continue;
                    }

                    if (!data.IsGenerated)
                    {
                        HexType[,,] chunkTerrain = _terrainGenerator.GenerateChunkTerrain(chunkInd);

                        _terrain[chunkInd] = new ChunkData(chunkInd, data.chunkGenerator, chunkTerrain);
                    }

                    _loadedChunks.Add(chunkInd);
                    _terrain[chunkInd].SetActive(true);
                }
            }
        }

        private void DisableUnloadedChunks()
        {
            foreach (Vector2Int chunkInd in _unloadedChunks)
            {
                if (_terrain.ContainsKey(chunkInd))
                {
                    _terrain[chunkInd].SetActive(false);
                }
            }
        }

        private void RegenerateLoadedChunks()
        {
            foreach (Vector2Int chunkInd in _loadedChunks)
            {
                //TODO Умно перестраивать чанки. Не все, только ново добавленные, и соседние к ним
                if (!_terrain[chunkInd].IsBuilded)
                {
                    _terrain[chunkInd].Regenerate();
                }
            }
        }


        [ContextMenu("Regenerate terrain")]
        private void RegenerateEntireWorld()
        {
            _terrainGenerator = new TerrainGenerator(noiseGenerator, chunkSize);

            foreach (var data in _loadedChunks)
            {
                _terrain[data] = new ChunkData(data, _terrain[data].chunkGenerator);
            }

            GenerateWorld(Vector2Int.zero);
        }
    }
}