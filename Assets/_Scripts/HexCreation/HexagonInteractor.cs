using UnityEngine;

namespace MeshCreation
{
    public class HexagonInteractor
    {
        private Vector3Int _chunkSize;
        private readonly float _hexagonSize;
        private readonly float _hexagonHeight;
        private readonly Transform _player;
        private readonly Camera _cam;

        private Vector2Int _currentChunk;
        
        private readonly RocketLauncher _launcher;

        public HexagonInteractor(Transform player, Camera cam, RocketLauncher launcher)
        {
            _player = player;
            _cam = cam;

            _chunkSize = WorldGenerator.Instance.ChunkSize;
            _hexagonSize = WorldGenerator.Instance.HexagonSize;
            _hexagonHeight = WorldGenerator.Instance.HexagonHeight;

            _currentChunk = new Vector2Int(int.MaxValue, int.MaxValue);

            _launcher = launcher;
            _launcher.Init(this);
        }


        public void Update()
        {
            HandleCameraRay(out RaycastHit hitInfo);
            InteractWithHexes(hitInfo);
            LaunchRockets(hitInfo);
        }

        public void HandleRocketExplosion(Vector3 blastPosition)
        {
            Debug.LogWarning(blastPosition);
        }
        
        private void HandleCameraRay(out RaycastHit raycastHit)
        {
            Ray cameraRay = _cam.ViewportPointToRay(InputSystem.SCREEN_CENTRE);

            Physics.Raycast(cameraRay, out raycastHit);
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

        private void InteractWithHexes(RaycastHit hitInfo)
        {
            if (!InputSystem.Instance.IsMouseButtonPressed)
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

            if (WorldGenerator.Instance.IsGenerated(chunkPos, out ChunkData chunkData) && hexLocalPos.y < _chunkSize.y)
            {
                Vector3Int hexPos = GetHexPositionInChunk(hexLocalPos, chunkPos);

                chunkData.chunkGenerator.ChangeHexTypeAtPosition(hexPos, isDestroying ? HexType.Void : HexType.Dirt);
            }
        }

        private void LaunchRockets(RaycastHit hitInfo)
        {
            if (!InputSystem.Instance.buttonPressed[InputSystem.InputType.Launch])
            {
                return;
            }

            _launcher.LaunchRocket(hitInfo.point);
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