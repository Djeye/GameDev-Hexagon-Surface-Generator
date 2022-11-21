using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace MeshCreation
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter), typeof(MeshCollider))]
    public class ChunkGenerator : MonoBehaviour
    {
        private ChunkData _chunkData;

        private readonly List<Vector3> _verticies = new List<Vector3>();
        private readonly List<int> _triangles = new List<int>();
        private readonly List<Vector2> _uv = new List<Vector2>();

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
        }


        public void Init(ChunkData chunkData)
        {
            _chunkData = chunkData;
        }

        public void RegenerateChunk()
        {
            GenerateSurface();
            ApplyMesh();
        }


        private void GenerateSurface()
        {
            _chunkMesh = new Mesh();

            _triangles.Clear();
            _verticies.Clear();
            _uv.Clear();

            Iterations.Iterate(GenerateHexagon, ChunkSize);
        }

        private void ApplyMesh()
        {
            _chunkMesh.vertices = _verticies.ToArray();
            _chunkMesh.triangles = _triangles.ToArray();
            _chunkMesh.uv = _uv.ToArray();

            _chunkMesh.Optimize();
            _chunkMesh.RecalculateNormals();
            _chunkMesh.RecalculateBounds();

            _meshFilter.mesh = _chunkMesh;
            _meshCollider.sharedMesh = _chunkMesh;
        }

        private void GenerateHexagon(int x, int y, int z)
        {
            Vector3Int hexGridPosition = new Vector3Int(x, y, z);

            if (GetHexAtPosition(hexGridPosition) == HexType.Void)
            {
                return;
            }

            Vector3 hexPosition = HexInfo.GetWorldCoords(hexGridPosition);

            GenerateFlatHex(HexInfo.Sides.Top, hexGridPosition, hexPosition);
            GenerateFlatHex(HexInfo.Sides.Bottom, hexGridPosition, hexPosition);
            ConnectSidesTriangles(hexGridPosition);
        }

        private void GenerateFlatHex(HexInfo.Sides side, Vector3Int gridPosition, Vector3 position)
        {
            CreateVertecies(side, position);
            CreateUVs(side);
            TryConnectTriangles(side, gridPosition + HexInfo.FLAT_NORMALS[side], FlatOffset);
        }

        private void CreateVertecies(HexInfo.Sides side, Vector3 position)
        {
            foreach (Vector3 vertex in HexInfo.HEX_COORDS)
            {
                Vector3 newVertex = vertex + HexInfo.FLAT_OFFSET[side];
                _verticies.Add(newVertex + position);
            }
        }

        private void CreateUVs(HexInfo.Sides side)
        {
            for (int i = 0; i < HexInfo.HEX_NORMAL_COORDS.Count; i++)
            {
                Vector2 vertex = HexInfo.HEX_NORMAL_COORDS[(i + HexInfo.UV_SHIFT[side]) % HexInfo.HEX_VERTECIES];
                _uv.Add(vertex);
            }
        }

        private void ConnectSidesTriangles(Vector3Int gridPosition)
        {
            foreach ((HexInfo.Sides side, Vector3Int neighbor) in HexInfo.HEX_SIDE_NEIGHBORS)
            {
                TryConnectTriangles(side, gridPosition + neighbor, SidesOffset);
            }
        }

        private void TryConnectTriangles(HexInfo.Sides side, Vector3Int neighborPosition, int offset)
        {
            if (GetHexAtPosition(neighborPosition) != HexType.Void)
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

        public void ChangeHexTypeAtPosition(Vector3Int hexPos, HexType hexType)
        {
            _chunkData.SetHexType(hexPos, hexType);

            foreach (Vector3Int neighbor in HexInfo.HEX_SIDE_NEIGHBORS.Values)
            {
                Vector3Int neighborHexPos = hexPos + neighbor;

                if (IsInsideChunk(neighborHexPos))
                {
                    continue;
                }

                Vector2Int adjChunkPosition = GetAdjChunk(neighborHexPos);

                if (WorldGenerator.Instance.terrain.TryGetValue(adjChunkPosition, out ChunkData adjChunkData))
                {
                    adjChunkData.chunkGenerator.RegenerateChunk();
                }
            }

            RegenerateChunk();
        }

        private HexType GetHexAtPosition(Vector3Int hexPos)
        {
            if (IsInsideChunk(hexPos))
            {
                return _chunkData.GetHexByPos(hexPos);
            }

            if (IsOutsideChunkHeight(hexPos))
            {
                return HexType.Void;
            }

            Vector2Int adjChunkPosition = GetAdjChunk(hexPos, out Vector3Int adjHexPos);

            if (WorldGenerator.Instance.terrain.TryGetValue(adjChunkPosition, out ChunkData adjChunk))
            {
                return adjChunk.GetHexByPos(adjHexPos);
            }

            return HexType.Void;
        }

        private Vector2Int GetAdjChunk(Vector3Int hexPos, out Vector3Int adjHexPos)
        {
            Vector2Int adjChunkPosition = _chunkData.chunkPosition;
            adjHexPos = hexPos;

            if (adjHexPos.x < 0)
            {
                adjChunkPosition.x--;
                adjHexPos.x += ChunkSize.x;
            }
            else if (adjHexPos.x >= ChunkSize.x)
            {
                adjChunkPosition.x++;
                adjHexPos.x -= ChunkSize.x;
            }

            if (adjHexPos.z < 0)
            {
                adjChunkPosition.y--;
                adjHexPos.z += ChunkSize.z;
            }
            else if (adjHexPos.z >= ChunkSize.z)
            {
                adjChunkPosition.y++;
                adjHexPos.z -= ChunkSize.z;
            }

            return adjChunkPosition;
        }

        private Vector2Int GetAdjChunk(Vector3Int hexPos)
        {
            return GetAdjChunk(hexPos, out Vector3Int _);
        }


        private bool IsInsideChunk(Vector3Int pos) => pos.x >= 0 && pos.x < ChunkSize.x &&
                                                      pos.y >= 0 && pos.y < ChunkSize.y &&
                                                      pos.z >= 0 && pos.z < ChunkSize.z;

        private bool IsOutsideChunkHeight(Vector3Int pos) => pos.y < 0 || pos.y >= ChunkSize.y;

        #endregion
    }
}