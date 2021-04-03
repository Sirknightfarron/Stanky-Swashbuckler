using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

//using System.Collections.Generic;


public class MapGenerator : MonoBehaviour
{
    public enum DrawMode
    {
        NoiseMap,
        ColourMap,
        Mesh
    }
    public DrawMode drawMode;

    public const int mapChunkSize_plus_1 = 241;
    [Range(0, 6)]
    public int levelOfDetailinEditor;



    [Range(0f, 100f)]
    public float noiseScale;
    public Noise.NomaliseMode nomaliseMode;

    [Range(0f, 40f)]
    public int octaves;
    [Range(0f, 200f)]
    public float persistence;
    [Range(0f, 1000f)]
    public float lacunarity;

    public int seed;
    public Vector2 offset;
    [SerializeField]
    float highAmplitude;
    [SerializeField]
    AnimationCurve animationCurve;

    public bool autoUpdate;

    public TerrainType[] regions;

    Queue<MapThreadInfo<MapData>> mapDataInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> meshDataInfoQueue = new Queue<MapThreadInfo<MeshData>>();
    
    public void RequestMapData(Vector2 center, Action<MapData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(center ,callback);

        };
        new Thread(threadStart).Start();
    }
    void MapDataThread(Vector2 center, Action<MapData> callback)
    {
        MapData mapData = GenerateMapData(center);
        lock (mapDataInfoQueue)
        {
            mapDataInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }
    public void requestMeshDate(MapData mapData, int lod, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(mapData,lod, callback);

        };
        new Thread(threadStart).Start();
    }
    public void MeshDataThread(MapData mapData,int lod, Action<MeshData> callback)
    {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heigtMap, highAmplitude, animationCurve, lod);
        lock (meshDataInfoQueue)
        {
            meshDataInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }


    }
    public void Update()
    {
        if(mapDataInfoQueue.Count > 0)
        {
            for (int i = 0; i< mapDataInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);

            }
        } 
        if (meshDataInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshDataInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = meshDataInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);

            }
        }
    }

    struct MapThreadInfo<T>
    {
        public Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
    
    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMapData(Vector2.zero);
        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heigtMap));
        }
        else if (drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(mapData.ColorMap, mapChunkSize_plus_1, mapChunkSize_plus_1));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heigtMap, highAmplitude, animationCurve, levelOfDetailinEditor), TextureGenerator.TextureFromColourMap(mapData.ColorMap, mapChunkSize_plus_1, mapChunkSize_plus_1));
        }
    }


    MapData GenerateMapData(Vector2 center)
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize_plus_1, mapChunkSize_plus_1, seed, noiseScale, octaves, persistence * 0.01f, lacunarity, offset+center, nomaliseMode);


        Color[] colourMap = new Color[mapChunkSize_plus_1 * mapChunkSize_plus_1];
        for (int y = 0; y < mapChunkSize_plus_1; y++)
        {
            for (int x = 0; x < mapChunkSize_plus_1; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        colourMap[y * mapChunkSize_plus_1 + x] = regions[i].colour;
                        break;
                    }
                }
            }
        }
        return new MapData(noiseMap, colourMap);



    }

    private void OnValidate()
    {
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
    }
    
}
    [System.Serializable]
    public struct TerrainType
    {
        public string name;
        public float height;
        public Color colour;
    }


    public struct MapData
    {
        public readonly float[,] heigtMap;
        public  readonly Color[] ColorMap;

        public MapData(float[,] heigtMap, Color[] colorMap)
        {
            this.heigtMap = heigtMap;
            this.ColorMap = colorMap;
        }
    }
    


