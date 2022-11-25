using UnityEngine;

namespace MeshCreation
{
    public class HexagonInteractor
    {
        private Vector3Int _chunkSize;
        private readonly float _hexagonHeight;
        private readonly float _hexagonSize;
        private readonly Transform _player;
        private readonly Camera _cam;

        private Vector2Int _currentChunk;

        public HexagonInteractor(Transform player, Camera cam)
        {
            _player = player;
            _cam = cam;

            _chunkSize = WorldGenerator.Instance.ChunkSize;
            _hexagonHeight = WorldGenerator.Instance.HexagonSize;
            _hexagonSize = WorldGenerator.Instance.HexagonHeight;

            _currentChunk = new Vector2Int(int.MaxValue, int.MaxValue);
        }


        public void Update()
        {
            InteractWithHexes();
        }

        public void GenerateTerrainAroundPlayer()
        {
            Vector3Int hexLocalPos = HexInfo.GetHexagonCoords(_player.position);
            Vector2Int chunkPos = GetChunkByHexPosition(hexLocalPos);

            if (chunkPos == _currentChunk)
            {
                return;
            }

            _currentChunk = chunkPos;
            WorldGenerator.Instance.GenerateWorld(chunkPos);
        }

        private void InteractWithHexes()
        {
            if (!InputSystem.Instance.IsMouseButtonPressed)
            {
                return;
            }

            Ray cameraRay = _cam.ViewportPointToRay(InputSystem.SCREEN_CENTRE);

            if (!Physics.Raycast(cameraRay, out RaycastHit hitInfo))
            {
                return;
            }

            bool isDestroying = InputSystem.Instance.buttonPressed[InputSystem.InputType.MouseLeft];
            int multiplier = isDestroying ? -1 : 1;

            Vector3 hexCentrePoint = HexInfo.FLAT_NORMALS.ContainsValue(Vector3Int.RoundToInt(hitInfo.normal))
                ? hitInfo.point + multiplier * _hexagonHeight / 2f * hitInfo.normal
                : hitInfo.point + multiplier * _hexagonSize * hitInfo.normal;

            Vector3Int hexLocalPos = HexInfo.GetHexagonCoords(hexCentrePoint);
            Vector2Int chunkPos = GetChunkByHexPosition(hexLocalPos);

            if (!WorldGenerator.Instance.IsGenerated(chunkPos, out ChunkData chunkData))
            {
                return;
            }

            Vector3Int hexPos = GetHexPositionInChunk(hexLocalPos, chunkPos);

            chunkData.chunkGenerator.ChangeHexTypeAtPosition(hexPos, isDestroying ? HexType.Void : HexType.Dirt);
        }

        private Vector2Int GetChunkByHexPosition(Vector3Int hexLocalCoords)
        {
            int chunkPosX = Mathf.FloorToInt((float)hexLocalCoords.x / _chunkSize.x);
            int chunkPosY = Mathf.FloorToInt((float)hexLocalCoords.z / _chunkSize.z);

            return new Vector2Int(chunkPosX, chunkPosY);
        }

        private Vector3Int GetHexPositionInChunk(Vector3Int hexLocalCoords, Vector2Int chunkPos)
        {
            Vector3Int chunkGridPos3D = new Vector3Int(chunkPos.x, 0, chunkPos.y);

            return hexLocalCoords - chunkGridPos3D * _chunkSize;
        }
    }
}