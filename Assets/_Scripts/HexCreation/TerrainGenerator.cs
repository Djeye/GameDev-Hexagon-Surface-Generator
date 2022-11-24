using System;
using UnityEngine;

namespace MeshCreation
{
    public class TerrainGenerator
    {
        private Vector3Int _chunkSize;

        private readonly NoiseOctave[] _noiseOctaves;

        public TerrainGenerator(NoiseOctave[] octaves, Vector3Int chunkSize)
        {
            _chunkSize = chunkSize;
            _noiseOctaves = octaves;

            foreach (NoiseOctave octave in octaves)
            {
                octave.Init();
            }
        }

        [Serializable]
        public class NoiseOctave
        {
            [SerializeField] private FastNoiseLite.NoiseType noiseType;
            [SerializeField] private float frequency;
            [SerializeField] private float amplitude;

            public FastNoiseLite noiseGen;

            public float Amplitude => amplitude;

            public void Init()
            {
                noiseGen = new FastNoiseLite();

                noiseGen.SetNoiseType(noiseType);
                noiseGen.SetFrequency(frequency);
            }
        }

        public HexType[,,] GenerateChunkTerrain(Vector2Int chunkIndex)
        {
            var result = new HexType[_chunkSize.x, _chunkSize.y, _chunkSize.z];

            for (int x = 0; x < _chunkSize.x; x++)
            {
                for (int z = 0; z < _chunkSize.z; z++)
                {
                    float xScale = 1f / _chunkSize.x;
                    float zScale = 1f / _chunkSize.z;

                    float height = GetNoise(x * xScale + chunkIndex.x, z * zScale + chunkIndex.y);

                    for (int y = 0; y < height; y++)
                    {
                        result[x, y, z] = HexType.Dirt;
                    }
                }
            }

            return result;
        }

        private float GetNoise(float x, float y)
        {
            float result = 0;

            foreach (NoiseOctave octave in _noiseOctaves)
            {
                result += (octave.noiseGen.GetNoise(x, y) + 1) * 0.5f * octave.Amplitude;
            }
            
            //result = result * _chunkSize.y / _noiseOctaves.Length;
            result = Mathf.Clamp(result * _chunkSize.y, 0, _chunkSize.y);

            return result;
        }
    }
}