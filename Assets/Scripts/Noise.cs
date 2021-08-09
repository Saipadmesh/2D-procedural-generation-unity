using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise
{

    public static float[] GenerateNoise(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        float[] noiseMap = new float[mapWidth * mapHeight];
        var random = new System.Random(seed);
        //minimum 1 octave
        if(octaves < 1)
        {
            octaves = 1;
        }
        Vector2[] octaveOffsets = new Vector2[octaves];
        for(int i = 0; i < octaves; i++)
        {
            float offsetX = random.Next(-100000, 100000);
            float offsetY = random.Next(-100000, 100000);
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if(scale <= 0f)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        // When changing noise scale, it zooms from top-right corner
        // We will use this to make it zoom from the center instead

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int x=0,y; x < mapWidth; x++)
        {
            for (y = 0; y < mapHeight; y++)
            {
                //Base values
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                //Calculate noise for each octave
                for(int i = 0; i < octaves; i++)
                {
                    //Sample point
                    float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

                    //Perlin Noise by Unity
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);

                    //noiseHeight is final noise where we add all the octave noise
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                //We do this so that later we can interpolate from 0 to 1
                if (noiseHeight > maxNoiseHeight)
                    maxNoiseHeight = noiseHeight;
                else if (noiseHeight < minNoiseHeight)
                    minNoiseHeight = noiseHeight;

                //Assign our noise
                noiseMap[y * mapWidth + x] = noiseHeight;
            }
        }
        

        for(int x = 0, y; x < mapWidth; x++)
        {
            for (y = 0; y < mapHeight; y++)
            {
                noiseMap[y * mapWidth + x] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[y * mapWidth + x]);
            }
        }



        return noiseMap;
    }

    public static float[] GenerateCircularIslandGradientMap(int mapWidth, int mapHeight)
    {
        float[] map = new float[mapWidth * mapHeight];

        // Circular Gradient for island like shape 
        int centerX = Mathf.FloorToInt(mapWidth/2);
        int centerY = Mathf.FloorToInt(mapHeight / 2);
        for (int x = 0; x < mapWidth; x++)
        {
            for(int y = 0; y < mapHeight; y++)
            {
                float distanceX = Mathf.Pow((centerX - x), 2);
                float distanceY = Mathf.Pow((centerY - y), 2);

                map[y * mapWidth + x] = Mathf.Sqrt(distanceX + distanceY) / mapWidth;
            }
        }

        return map;
    }

    public static float[] GenerateSquareIslandGradientMap(int mapWidth, int mapHeight)
    {
        float[] map = new float[mapWidth * mapHeight];

        // Square Gradient for island like shape 
        
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                float distanceX = x/(float)mapWidth * 2 - 1;
                float distanceY = y/(float)mapHeight * 2 - 1;

                // Find closest x or y to the edge of the map
                float value = Mathf.Max(Mathf.Abs(distanceX), Mathf.Abs(distanceY));

                // Apply a curve graph to have more values around 0 on the edge, and more values >= 3 in the middle
                float a = 2.3f;
                float b = 4.0f;
                float islandGradientValue = Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));

                map[y * mapWidth + x] = islandGradientValue;
            }
        }
        

        return map;
    }
}
