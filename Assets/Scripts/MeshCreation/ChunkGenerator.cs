using MeshCreation;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    [Header("Prefabs")] [SerializeField] private MeshGenerator meshGenerator;

    [Space]
    [Header("World Parameters")]
    [SerializeField]
    [Tooltip("Default: 0.57735f")]
    private float hexagonSize = 0.57735f;

    [SerializeField] private Vector3Int chunkSize;
    [SerializeField] private Vector2Int chunkGrid;

    public static ChunkGenerator Instance { get; private set; }

    public float HexagonSize => hexagonSize != 0 ? hexagonSize : HexInfo.HEX_DEF_SIZE;
    public Vector3Int ChunkSize => chunkSize;

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
        for (int x = 0; x < chunkGrid.x; x++)
        {
            for (int z = 0; z < chunkGrid.y; z++)
            {
                var xOffset = x * hexagonSize * chunkSize.x * HexInfo.SQRT3;
                xOffset += z * hexagonSize * (chunkSize.z / 2f) * HexInfo.SQRT3 ;
                var zOffset = z * hexagonSize * chunkSize.z * 1.5f;
                
                Vector3 offset = new Vector3(xOffset, 0, zOffset);
                
                var chunk = Instantiate(meshGenerator, _position + offset, Quaternion.identity, _transform);
                chunk.GenerateChunk();
            }
        }
    }
}