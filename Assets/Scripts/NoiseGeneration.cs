using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "NoiseGeneration",menuName = "Algorithms/NoiseGeneration")]
public class NoiseGeneration : AlgorithmBase
{
    [Header("Noise Settings")]
    //The more octaves, the longer generation will take
    public int Octaves;
    [Range(0, 1)]
    public float Persistance;
    public float Lacunarity;
    public float NoiseScale;
    public Vector2 Offset;
    public bool RandomShape;
    [HideInInspector]
    public bool ApplySquareIslandGradient;

    [System.Serializable]
    class NoiseValues
    {
        [Range(0f, 1f)]
        public float Height;
        public GroundTileType GroundTile;
    }

    [SerializeField]
    private NoiseValues[] TileTypes;

    public override void Apply(TilemapStructure tilemap)
    {
        ApplySquareIslandGradient = !RandomShape;
        
        // make sure tiletypes are ordered in ascending order
        TileTypes = TileTypes.OrderBy(a => a.Height).ToArray();

        // Pass along parameters to fenerate noise map
        var noiseMap = Noise.GenerateNoise(tilemap.Width, tilemap.Height, tilemap.seed, NoiseScale, Octaves, Persistance, Lacunarity, Offset);

        float[] islandGradient;
        if (RandomShape)
        {
            islandGradient = Noise.GenerateCircularIslandGradientMap(tilemap.Width, tilemap.Height);
        }
        else
        {
            
            islandGradient = Noise.GenerateSquareIslandGradientMap(tilemap.Width, tilemap.Height);
        }
            
        for(int x = 0, y; x < tilemap.Width; x++)
        {
            for (y = 0; y < tilemap.Height; y++)
            {
               //Subtract islandGradient value from noisemap
                float subtractedValue = noiseMap[y * tilemap.Width + x] - islandGradient[y * tilemap.Width + x];

                //Clamp and apply
                noiseMap[y * tilemap.Width + x] = Mathf.Clamp01(subtractedValue);
            }
        }

        
        for (int x = 0; x < tilemap.Width; x++)
        {
            for(int y = 0; y < tilemap.Height; y++)
            {
                var height = noiseMap[y * tilemap.Width + x];

                for(int i = 0; i < TileTypes.Length; i++)
                {
                    if(height <= TileTypes[i].Height)
                    {
                        
                        tilemap.SetTile(x, y, (int)TileTypes[i].GroundTile);
                        break;
                    }
                }
            }
        }

        

        //Code for sand
        //generateSandborders(tilemap);

        //!!--------Debug code(remove later)------------!!
        /*int randX=0, randY=0;
        //while (debugtile != 0)
        {
            randX = new System.Random().Next(0, tilemap.Width);
            randY = new System.Random().Next(0, tilemap.Height);
            debugtile = tilemap.GetTile(randX,randY);
            Debug.Log("Debugtile: " + debugtile);
        }
        randX = new System.Random().Next(1, 3);
        randY = new System.Random().Next(1, 3);
        int debugtile = tilemap.GetTile(randX, randY);
        
        tilemap.SetTile(randX, randY, 6);*/

        // !------------------------------------------------!



    }

    public void generateSandborders(TilemapStructure tilemap)
    {
        for (int x = 0; x < tilemap.Width; x++)
        {
            for (int y = 0; y < tilemap.Height; y++)
            {


                int current = tilemap.GetTile(x, y);
                int beachval = 3;
                if(current  == beachval)
                {
                    for (int i = x - 2; i < x + 3; i++)
                    {
                        for (int j = y - 2; j < y + 3; j++)
                        {
                            if (tilemap.GetTile(i, j) == current + 2)
                            {
                                tilemap.SetTile(i, j, current + 1);
                            }
                        }
                    }
                }
                


            }
        }
    }
}
