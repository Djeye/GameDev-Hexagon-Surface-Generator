using System.Collections.Generic;
using Tools;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter), typeof(MeshCollider))]
public class ChunkMeshGenerator : MonoBehaviour
{
    private ChunkData _chunkData;

    private readonly List<Vector3> _verticies = new List<Vector3>();
    private readonly List<int> _triangles = new List<int>();

    private Vector3Int ChunkSize => WorldGenerator.Instance.ChunkSize;
    private int FlatOffset => _verticies.Count - HexInfo.HEX_VERTECIES;
    private int SidesOffset => _verticies.Count - 2 * HexInfo.HEX_VERTECIES;

    private Mesh _chunkMesh;
    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;


    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();

        //_hexagons = new HexType[ChunkSize.x, ChunkSize.y, ChunkSize.z];
    }


    public void GenerateChunk(ChunkData chunkData)
    {
        _chunkData = chunkData;

        GenerateSurface();
        ApplyMesh();
    }

    private void GenerateSurface()
    {
        _chunkMesh = new Mesh();
        Iterations.Iterate(GenerateHexagon, ChunkSize);
    }

    private void ApplyMesh()
    {
        _chunkMesh.vertices = _verticies.ToArray();
        _chunkMesh.triangles = _triangles.ToArray();

        _chunkMesh.Optimize();
        _chunkMesh.RecalculateNormals();
        _chunkMesh.RecalculateBounds();

        _meshFilter.mesh = _chunkMesh;
        _meshCollider.sharedMesh = _chunkMesh;
    }

    private void GenerateHexagon(int x, int y, int z)
    {
        Vector3Int hexGridPosition = new Vector3Int(x, y, z);

        if (GetHexagonAtPosition(hexGridPosition) == HexType.Void)
        {
            return;
        }

        Vector3 hexPosition = HexInfo.ConvertToHexagonAxis(x, y, z);

        GenerateFlatHex(HexInfo.Sides.Top, hexGridPosition, hexPosition);
        GenerateFlatHex(HexInfo.Sides.Bottom, hexGridPosition, hexPosition);
        ConnectSidesTriangles(hexGridPosition);
    }

    private void GenerateFlatHex(HexInfo.Sides side, Vector3Int blockPosition, Vector3 position)
    {
        CreateVertecies(side, position);
        TryConnectTriangles(side, blockPosition + HexInfo.FLAT_NORMALS[side], FlatOffset);
    }

    private void ConnectSidesTriangles(Vector3Int blockPosition)
    {
        foreach (var sideNeighbors in HexInfo.HEX_SIDE_NEIGHBORS)
        {
            TryConnectTriangles(sideNeighbors.Key, blockPosition + sideNeighbors.Value, SidesOffset);
        }
    }

    private void CreateVertecies(HexInfo.Sides side, Vector3 position)
    {
        foreach (Vector3 vertex in HexInfo.HEX_COORDS)
        {
            Vector3 newVertex = vertex + HexInfo.FLAT_OFFSET[side];
            _verticies.Add(newVertex + position);
        }
    }

    private void TryConnectTriangles(HexInfo.Sides side, Vector3Int neighborPosition, int offset)
    {
        if (GetHexagonAtPosition(neighborPosition) != HexType.Void)
        {
            return;
        }

        ConnectTriangles(side, offset);
    }

    private void ConnectTriangles(HexInfo.Sides side, int offset)
    {
        foreach (Vector3Int triangle in HexInfo.HEX_TRIANGLES[side])
        {
            for (int i = 0; i < 3; i++)
            {
                _triangles.Add(triangle[i] + offset);
            }
        }
    }

    #region Tools

    private HexType GetHexagonAtPosition(Vector3Int hexPos)
    {
        if (IsInsideChunk(hexPos))
        {
            return _chunkData.hexagons[hexPos.x, hexPos.y, hexPos.z];
        }

        if (IsOutsideChunkHeight(hexPos))
        {
            return HexType.Void;
        }

        Vector2Int adjChunkPosition = _chunkData.chunkPosition;

        if (hexPos.x < 0)
        {
            adjChunkPosition.x--;
            hexPos.x += ChunkSize.x;
        }
        else if (hexPos.x >= ChunkSize.x)
        {
            adjChunkPosition.x++;
            hexPos.x -= ChunkSize.x;
        }

        if (hexPos.z < 0)
        {
            adjChunkPosition.y--;
            hexPos.z += ChunkSize.z;
        }
        else if (hexPos.z >= ChunkSize.z)
        {
            adjChunkPosition.y++;
            hexPos.z -= ChunkSize.z;
        }

        if (WorldGenerator.Instance.terrain.TryGetValue(adjChunkPosition, out ChunkData adjChunk))
        {
            return adjChunk.hexagons[hexPos.x, hexPos.y, hexPos.z];
        }

        return HexType.Void;
    }

    private bool IsInsideChunk(Vector3Int pos) => pos.x >= 0 && pos.x < ChunkSize.x &&
                                                  pos.y >= 0 && pos.y < ChunkSize.y &&
                                                  pos.z >= 0 && pos.z < ChunkSize.z;

    private bool IsOutsideChunkHeight(Vector3Int pos) => pos.y < 0 || pos.y >= ChunkSize.y;

    #endregion
}