using System;
using System.Collections;
using UnityEngine;
using System.Linq;


[CreateAssetMenu(fileName = "GroundCellularAutomata", menuName = "Algorithms/CellularAutomata/Correction")]
public class CorrectionAutomata : AlgorithmBase
{
    public int  Repetitions;
    public override void Apply(TilemapStructure tilemap)
    {
        int shallowWaterVal = (int)Enum.Parse(typeof(GroundTileType), "UndeepWater");
        int deepWaterVal = (int)Enum.Parse(typeof(GroundTileType), "DeepWater");
        int grassVal = (int)Enum.Parse(typeof(GroundTileType), "Grass");

        for (int i = 0; i < Repetitions; i++)
        {
            for (int x = 0; x < tilemap.Width; x++)
            {
                for (int y = 0; y < tilemap.Height; y++)
                {
                    var tile = tilemap.GetTile(x, y);
                    if (tile == deepWaterVal)
                    {
                        var neighbors = tilemap.GetNeighbors(x, y);
                        int count = neighbors.Count(t => t.Value == grassVal);
                        if(count >= 6 || neighbors.Count(t => t.Value == shallowWaterVal) != 0)
                        {
                            tilemap.SetTile(x, y, grassVal);
                        }
                          
                    }

                }
            }
        }
    }
}


