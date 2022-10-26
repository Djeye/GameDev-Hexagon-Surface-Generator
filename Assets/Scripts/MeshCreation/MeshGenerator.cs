using System;
using System.Collections.Generic;
using MeshCreation;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    private int[,,] _blocks;

    private readonly List<Vector3> _verticies = new List<Vector3>();
    private readonly List<int> _triangles = new List<int>();

    private Mesh _chunkMesh;
    private MeshFilter _meshFilter;


    private int ChunkWidthX => ChunkGenerator.Instance.ChunkSize.x;
    private int ChunkHeight => ChunkGenerator.Instance.ChunkSize.y;

    private int ChunkWidthZ => ChunkGenerator.Instance.ChunkSize.z;

    //private int ChunkRadius => ChunkGenerator.Instance.ChunkSize.x;
    private int FlatOffset => _verticies.Count - HexInfo.HEX_VERTECIES;
    private int SidesOffset => _verticies.Count - 2 * HexInfo.HEX_VERTECIES;


    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _blocks = new int[ChunkWidthX, ChunkHeight, ChunkWidthZ];
    }


    public void GenerateChunk()
    {
        GenerateSurface();
        ApplyMesh();
    }

    private void GenerateSurface()
    {
        _chunkMesh = new Mesh();
        IterateEachBlock(FillBlock);
        IterateEachBlock(GenerateHexagon);
    }

    private void ApplyMesh()
    {
        _chunkMesh.vertices = _verticies.ToArray();
        _chunkMesh.triangles = _triangles.ToArray();

        _chunkMesh.RecalculateBounds();
        _chunkMesh.RecalculateNormals();

        _meshFilter.mesh = _chunkMesh;
    }

    private void FillBlock(int x, int y, int z) => _blocks[x, y, z] = 1;

    private void GenerateHexagon(int x, int y, int z)
    {
        Vector3Int blockPosition = new Vector3Int(x, y, z);

        if (GetHexagonAtPosition(blockPosition) == 0)
        {
            return;
        }

        Vector3 hexPosition = HexInfo.ConvertToHexagonAxis(x, y, z);

        GenerateFlatHex(HexInfo.Sides.Top, blockPosition, hexPosition);
        GenerateFlatHex(HexInfo.Sides.Bottom, blockPosition, hexPosition);
        ConnectSidesTriangles(blockPosition);
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
        if (GetHexagonAtPosition(neighborPosition) != 0)
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

    private void IterateEachBlock(Action<int, int, int> actionInsideLoop)
    {
        for (int x = 0; x < ChunkWidthX; x++)
        {
            for (int y = 0; y < ChunkHeight; y++)
            {
                for (int z = 0; z < ChunkWidthZ; z++)
                {
                    actionInsideLoop?.Invoke(x, y, z);
                }
            }
        }
    }

    private int GetHexagonAtPosition(Vector3Int pos)
    {
        return IsOutsideChunk(pos) ? 0 : _blocks[pos.x, pos.y, pos.z];
    }

    private bool IsOutsideChunk(Vector3Int pos) => pos.x < 0 || pos.x >= ChunkWidthX ||
                                                   pos.y < 0 || pos.y >= ChunkHeight ||
                                                   pos.z < 0 || pos.z >= ChunkWidthZ;

    #endregion
}