using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode { 
        NoiseMap,
        ColourMap,
        Mesh
    }
    public DrawMode drawMode;

    public int mapWidth;
    public int mapHeight;
    [Range (0f, 50f)]
    public float noiseScale;

    
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

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistence*0.01f, lacunarity, offset);


        Color[] colourMap = new Color[mapWidth * mapHeight];
        for(int y = 0; y < mapHeight; y++)
        {
            for(int x = 0; x < mapWidth; x++)
            {
                float currentHeight = noiseMap[x, y];
                for(int i = 0; i < regions.Length; i++)
                {
                    if(currentHeight <= regions[i].height)
                    {
                        colourMap[y * mapWidth + x] = regions[i].colour;
                        break;
                    }
                }
            }
        }


        MapDisplay display = FindObjectOfType<MapDisplay> ();
        if(drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        } else if (drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, mapWidth, mapHeight));
        } else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, highAmplitude, animationCurve), TextureGenerator.TextureFromColourMap(colourMap, mapWidth, mapHeight));
        }

    }

    private void OnValidate()
    {
       if(mapWidth < 1)
        {
            mapWidth = 1;
        }
       if(mapHeight < 1)
        {
            mapHeight = 1;
        }
       if(lacunarity < 1)
        {
            lacunarity = 1;
        }
       if(octaves < 0)
        {
            octaves = 0;
        }
       if(highAmplitude <= 0)
        {
            highAmplitude = 1;
        }
    }

    [System.Serializable]
    public struct TerrainType
    {
        public string name;
        public float height;
        public Color colour;
    }
}

