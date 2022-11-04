using UnityEngine;

public struct ChunkData
{
    public readonly Vector2Int chunkPosition;

    public HexType[,,] hexagons;

    public ChunkData(Vector2Int chunkIndex, HexType[,,] chunkTerrain)
    {
        chunkPosition = chunkIndex;
        hexagons = chunkTerrain;
    }
}

public enum HexType : byte
{
    Void = 0,
    Dirt = 1
}