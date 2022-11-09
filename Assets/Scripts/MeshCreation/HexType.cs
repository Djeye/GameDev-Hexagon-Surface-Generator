using UnityEngine;

public readonly struct ChunkData
{
    public readonly Vector2Int chunkPosition;
    public readonly ChunkGenerator chunkGenerator;
    private readonly HexType[,,] _hexagons;

    public ChunkData(Vector2Int chunkIndex, HexType[,,] chunkTerrain, ChunkGenerator generator)
    {
        chunkPosition = chunkIndex;
        _hexagons = chunkTerrain;
        chunkGenerator = generator;
        
        chunkGenerator.Init(this);
    }

    public HexType GetHexByPos(Vector3Int hexPos)
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