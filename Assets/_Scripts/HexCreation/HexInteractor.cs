using UnityEngine;

namespace MeshCreation
{
    public class HexInteractor
    {
        private Vector3Int _chunkSize;
        private readonly float _hexagonHeight;
        private readonly float _hexagonSize;
        private readonly Camera _cam;

        public HexInteractor(Camera cam)
        {
            _chunkSize = WorldGenerator.Instance.ChunkSize;
            _hexagonHeight = WorldGenerator.Instance.HexagonSize;
            _hexagonSize = WorldGenerator.Instance.HexagonHeight;
            _cam = cam;
        }
        
        public void Update()
        {
            InteractWithHexes();
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
                ? hitInfo.point + multiplier * hitInfo.normal * _hexagonHeight / 2f
                : hitInfo.point + multiplier * hitInfo.normal * _hexagonSize;

            Vector3Int hexLocalPos = HexInfo.GetHexagonCoords(hexCentrePoint);
            Vector2Int chunkPos = GetChunkByHexPosition(hexLocalPos);

            if (!WorldGenerator.Instance.terrain.TryGetValue(chunkPos, out ChunkData chunkData))
            {
                return;
            }

            Vector3Int hexPos = GetHexPositionInChunk(hexLocalPos);

            chunkData.chunkGenerator.ChangeHexTypeAtPosition(hexPos, isDestroying ? HexType.Void : HexType.Dirt);


            Vector2Int GetChunkByHexPosition(Vector3Int hexLocalCoords)
            {
                return new Vector2Int(hexLocalCoords.x / _chunkSize.x, hexLocalCoords.z / _chunkSize.y);
            }

            Vector3Int GetHexPositionInChunk(Vector3Int hexLocalCoords)
            {
                Vector2Int chunkGridPos = GetChunkByHexPosition(hexLocalCoords);
                Vector3Int chunkGridPos3D = new Vector3Int(chunkGridPos.x, 0, chunkGridPos.y);

                return hexLocalCoords - chunkGridPos3D * _chunkSize;
            }
        }
    }
}