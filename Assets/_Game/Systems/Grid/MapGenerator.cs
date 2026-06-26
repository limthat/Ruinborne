using UnityEngine;
using Ruinborne.Core;
using Ruinborne.Data;
using Ruinborne.Definitions;

namespace Ruinborne.Systems.Grid
{
    public class MapGenerator : ManagerBase<MapGenerator>
    {
        [Header("Perlin Noise 설정")]
        [SerializeField] private float noiseScale = 20f;
        [SerializeField] private int octaves = 4;
        [SerializeField] private float persistence = 0.5f;
        [SerializeField] private float lacunarity = 2f;
        [SerializeField] private int seed = 0;

        [Header("바이옴 임계값 (0~1)")]
        [SerializeField] private float waterThreshold = 0.2f;
        [SerializeField] private float desertThreshold = 0.35f;
        [SerializeField] private float grassThreshold = 0.6f;
        [SerializeField] private float forestThreshold = 0.8f;
        // 0.8 초과 = Mountain

        private GridManager _gridManager;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            _gridManager = ServiceLocator.Get<GridManager>();
            if (_gridManager != null)
                GenerateMap();
        }

        public void GenerateMap()
        {
            if (seed == 0) seed = Random.Range(1, 99999);

            float[,] noiseMap = GenerateNoiseMap(
                _gridManager.Width,
                _gridManager.Height,
                seed, noiseScale,
                octaves, persistence, lacunarity
            );

            for (int x = 0; x < _gridManager.Width; x++)
            {
                for (int z = 0; z < _gridManager.Height; z++)
                {
                    float noiseVal = noiseMap[x, z];
                    TileType tileType = GetTileTypeFromNoise(noiseVal);
                    TerrainDef terrainDef = _gridManager.GetTerrainDefForTileType(tileType);
                    _gridManager.SetTerrainDef(x, z, terrainDef);
                }
            }

            Debug.Log($"[MapGenerator] 맵 생성 완료. Seed: {seed}");
            GameEventBus.Publish(new MapGeneratedEvent { Seed = seed });
        }

        private TileType GetTileTypeFromNoise(float noiseVal)
        {
            if (noiseVal < waterThreshold)   return TileType.Water;
            if (noiseVal < desertThreshold)  return TileType.Desert;
            if (noiseVal < grassThreshold)   return TileType.Grass;
            if (noiseVal < forestThreshold)  return TileType.Forest;
            return TileType.Mountain;
        }

        private float[,] GenerateNoiseMap(int width, int height, int seed,
            float scale, int octaves, float persistence, float lacunarity)
        {
            float[,] noiseMap = new float[width, height];
            System.Random rng = new System.Random(seed);

            Vector2[] octaveOffsets = new Vector2[octaves];
            for (int i = 0; i < octaves; i++)
            {
                float offsetX = rng.Next(-100000, 100000);
                float offsetZ = rng.Next(-100000, 100000);
                octaveOffsets[i] = new Vector2(offsetX, offsetZ);
            }

            if (scale <= 0f) scale = 0.0001f;

            float maxVal = float.MinValue;
            float minVal = float.MaxValue;

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    float amplitude = 1f;
                    float frequency = 1f;
                    float noiseHeight = 0f;

                    for (int o = 0; o < octaves; o++)
                    {
                        float sampleX = (x + octaveOffsets[o].x) / scale * frequency;
                        float sampleZ = (z + octaveOffsets[o].y) / scale * frequency;
                        float perlinVal = Mathf.PerlinNoise(sampleX, sampleZ) * 2f - 1f;
                        noiseHeight += perlinVal * amplitude;
                        amplitude *= persistence;
                        frequency *= lacunarity;
                    }

                    if (noiseHeight > maxVal) maxVal = noiseHeight;
                    if (noiseHeight < minVal) minVal = noiseHeight;
                    noiseMap[x, z] = noiseHeight;
                }
            }

            // 0~1 정규화
            for (int x = 0; x < width; x++)
                for (int z = 0; z < height; z++)
                    noiseMap[x, z] = Mathf.InverseLerp(minVal, maxVal, noiseMap[x, z]);

            return noiseMap;
        }
    }
}
