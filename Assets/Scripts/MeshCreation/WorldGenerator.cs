using System.Collections.Generic;
using Tools;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private ChunkGenerator chunkGenerator;

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

    public readonly Dictionary<Vector2Int, ChunkData> terrain = new Dictionary<Vector2Int, ChunkData>();

    private Transform _transform;
    private Vector3 _position;
    private Camera _cam;


    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }

        Instance = this;

        _transform = transform;
        _position = transform.position;
        _cam = Camera.main;
    }

    private void Start()
    {
        Iterations.Iterate(InitializeTerrain, chunkGridSize);
        GenerateTerrain();
    }

    private void Update()
    {
        InteractWithHexes();
    }


    private void InitializeTerrain(int chunkX, int chunkY)
    {
        Vector2Int chunkIndex = new Vector2Int(chunkX, chunkY);
        HexType[,,] chunkTerrain = TerrainGenerator.GenerateChunkTerrain(chunkSize, chunkIndex);

        Vector3 chunkOffset = HexInfo.GetWorldCoords(chunkX * chunkSize.x, 0, chunkY * chunkSize.z);
        ChunkGenerator chunk = Instantiate(chunkGenerator, _position + chunkOffset, Quaternion.identity, _transform);

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

    private void InteractWithHexes()
    {
        if (!InputController.Instance.IsMouseButtonPressed)
        {
            return;
        }

        Ray cameraRay = _cam.ViewportPointToRay(InputController.SCREEN_CENTRE);

        if (!Physics.Raycast(cameraRay, out RaycastHit hitInfo))
        {
            return;
        }

        bool isDestroying = InputController.Instance.buttonPressed[InputController.InputType.MouseLeft];
        int multiplier = isDestroying ? -1 : 1;

        Vector3 hexCentrePoint = HexInfo.FLAT_NORMALS.ContainsValue(Vector3Int.RoundToInt(hitInfo.normal))
            ? hitInfo.point + multiplier * hitInfo.normal * hexagonHeight / 2f
            : hitInfo.point + multiplier * hitInfo.normal * hexagonSize;

        Vector3Int hexLocalPos = HexInfo.GetHexagonCoords(hexCentrePoint);
        Vector2Int chunkPos = GetChunkByHexPosition(hexLocalPos);

        if (!terrain.TryGetValue(chunkPos, out ChunkData chunkData))
        {
            return;
        }

        Vector3Int hexPos = GetHexPositionInChunk(hexLocalPos);

        chunkData.chunkGenerator.ChangeHexTypeAtPosition(hexPos, isDestroying ? HexType.Void : HexType.Dirt);
    }

    private Vector2Int GetChunkByHexPosition(Vector3Int hexLocalCoords)
    {
        return new Vector2Int(hexLocalCoords.x / chunkSize.x, hexLocalCoords.z / chunkSize.y);
    }

    private Vector3Int GetHexPositionInChunk(Vector3Int hexLocalCoords)
    {
        Vector2Int chunkPos = GetChunkByHexPosition(hexLocalCoords);
        Vector3Int chunkPos3D = new Vector3Int(chunkPos.x, 0, chunkPos.y);

        return hexLocalCoords - chunkPos3D * chunkSize;
    }
}