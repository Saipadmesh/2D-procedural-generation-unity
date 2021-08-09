using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RockGeneration", menuName = "Algorithms/RockGeneration")]
public class RandomGenerationAlgorithm : AlgorithmBase
{
    [SerializeField]
    private RockConfiguration[] RockSelection;

    [Serializable]
    class RockConfiguration
    {
        public ObjectTileType Rock;
        public GroundTileType[] SpawnOnGrounds;
        [Range(0, 100)]
        public int SpawnChancePerCell;
    }
    public override void Apply(TilemapStructure tilemap)
    {
        var groundTilemap = tilemap.Grid.Tilemaps[TilemapType.Ground];
        var random = new System.Random(tilemap.Grid.Seed);
        for(int x = 0; x < tilemap.Width; x++)
        {
            for(int y = 0; y < tilemap.Height; y++)
            {
                foreach(var rock in RockSelection)
                {
                    var groundTile = groundTilemap.GetTile(x, y);
                    if(rock.SpawnOnGrounds.Any(tile => (int)tile == groundTile))
                    {
                        if(random.Next(0,100) <= rock.SpawnChancePerCell)
                        {
                            tilemap.SetTile(x, y, (int)rock.Rock);
                        }
                    }
                }
            }
        }
    }
}
