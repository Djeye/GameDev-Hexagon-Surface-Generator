using UnityEngine;

namespace MeshCreation
{
    public readonly struct ChunkData
    {
        public readonly Vector2Int chunkPosition;
        public readonly ChunkGenerator chunkGenerator;
        private readonly HexType[,,] _hexagons;

        public bool IsGenerated => _hexagons != null;
        public bool IsBuilded => chunkGenerator.IsBuilded;

        private bool IsVisible => chunkGenerator.gameObject.activeSelf;


        public ChunkData(Vector2Int chunkIndex, ChunkGenerator generator)
        {
            chunkPosition = chunkIndex;
            chunkGenerator = generator;

            _hexagons = null;
            
            SetActive(false);
        }

        public ChunkData(Vector2Int chunkIndex, ChunkGenerator generator, HexType[,,] chunkTerrain)
        {
            chunkPosition = chunkIndex;
            chunkGenerator = generator;

            _hexagons = chunkTerrain;
            chunkGenerator.Init(this);
        }

        public void SetActive(bool state)
        {
            if (IsVisible == state)
            {
                return;
            }

            chunkGenerator.gameObject.SetActive(state);
        }

        public void Regenerate()
        {
            chunkGenerator.RegenerateChunk();
        }

        public HexType GetHexTypeByPos(Vector3Int hexPos)
        {
            return _hexagons[hexPos.x, hexPos.y, hexPos.z];
        }

        public void SetHexType(Vector3Int hexPos, HexType hexType)
        {
            _hexagons[hexPos.x, hexPos.y, hexPos.z] = hexType;
        }
    }

    public enum HexType : byte
    {
        Void = 0,
        Dirt = 1
    }
}