using System.Collections.Generic;
using Tools;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private ChunkMeshGenerator chunkMeshGenerator;

    [Header("Hexagon Info")]
    [SerializeField] private float hexagonSize = 0.57735f;
    [SerializeField] private float hexagonHeight = 1f;

    [Header("Chunk Info")]
    [SerializeField] private Vector3Int chunkSize;
    [SerializeField] private Vector2Int chunkGridSize;

    public static WorldGenerator Instance { get; private set; }

    public float HexagonSize => hexagonSize != 0 ? hexagonSize : HexInfo.HEX_DEF_SIZE;
    public float HexagonHeight => hexagonHeight != 0 ? hexagonHeight : HexInfo.HEX_DEF_HEIGHT;
    public Vector3Int ChunkSize => chunkSize;

    
    private readonly Dictionary<Vector2Int, HexType[,,]> _terrain = new Dictionary<Vector2Int, HexType[,,]>();

    private Transform _transform;
    private Vector3 _position;


    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }

        Instance = this;

        _transform = transform;
        _position = transform.position;
    }

    private void Start()
    {
        Iterations.Iterate(CreateChunk, chunkGridSize);
    }


    private void CreateChunk(int chunkX, int chunkY)
    {
        Vector2Int chunkIndex = new Vector2Int(chunkX, chunkY);
        HexType[,,] chunkTerrain = TerrainGenerator.GenerateChunkTerrain(chunkSize, chunkIndex);
        _terrain.Add(chunkIndex, chunkTerrain);

        float xOffset = (chunkX * chunkSize.x + chunkY * chunkSize.z * 0.5f) * hexagonSize * HexInfo.SQRT3;
        float zOffset = chunkY * hexagonSize * chunkSize.z * 1.5f;

        Vector3 posOffset = new Vector3(xOffset, 0, zOffset);

        ChunkMeshGenerator chunk = Instantiate(chunkMeshGenerator, _position + posOffset, Quaternion.identity,
            _transform);

        chunk.GenerateChunk(_terrain[chunkIndex]);
    }
}