using System;
using System.Collections;
using UnityEngine;
using System.Linq;


[CreateAssetMenu(fileName = "GroundCellularAutomata", menuName = "Algorithms/CellularAutomata/Ground")]
public class GroundCellularAutomata : AlgorithmBase
{
    public int MinAlive, Repetitions;

    [Tooltip("If this is checked, ReplacedBy will have no effect.")]
    public bool ReplaceByDominantTile;

    public GroundTileType TargetTile, ReplacedBy;

    /*public override void Apply(TilemapStructure tilemap)
    {
        int targetTileId = (int)TargetTile;
        int replaceTileId = (int)ReplacedBy;
        for (int i = 0; i < Repetitions; i++)
        {
            for (int x = 0; x < tilemap.Width; x++)
            {
                for (int y = 0; y < tilemap.Height; y++)
                {
                    var tile = tilemap.GetTile(x, y);
                    if (tile == targetTileId)
                    {
                        var neighbors = tilemap.GetNeighbors_2(x, y);
                        //Count all neighbours
                        int targetTilesCount = neighbors.Count(async => async.Value != targetTileId);

                        //If min alive count is not reached, we replace the tile
                        if (targetTilesCount < MinAlive)
                        {
                            //var remainingTiles = neighbors.Where(r => r.Value == replaceTileId);

                            if (ReplaceByDominantTile)
                            {
                                var dominantTile = neighbors.GroupBy(async => async.Value).OrderByDescending(a => a.Count()).Select(a => a.Key).First();
                                tilemap.SetTile(x, y, dominantTile);
                                //tilemap.SetTile(x,y,replaceTileId);
                            }
                            else
                            {
                                tilemap.SetTile(x, y, replaceTileId);
                            }

                        }
                    }
                }
            }
        }
    }*/

    public override void Apply(TilemapStructure tilemap)
    {
        int targetTileId = (int)TargetTile;
        int replaceTileId = (int)ReplacedBy;
        for (int i = 0; i < Repetitions; i++)
        {
            for (int x = 0; x < tilemap.Width; x++)
            {
                for (int y = 0; y < tilemap.Height; y++)
                {
                    var tile = tilemap.GetTile(x, y);
                    if (tile == targetTileId)
                    {
                        var neighbors = tilemap.GetNeighbors(x, y);
                        //Count all neighbours
                        int targetTilesCount = neighbors.Count(async => async.Value == targetTileId);

                        //If min alive count is not reached, we replace the tile
                        if (targetTilesCount < MinAlive)
                        {
                            //var remainingTiles = neighbors.Where(r => r.Value == replaceTileId);

                            if (ReplaceByDominantTile)
                            {
                                
                                var dominantTile = neighbors.GroupBy(async => async.Value).OrderByDescending(a => a.Count()).Select(a => a.Key).First();
                                tilemap.SetTile(x, y, dominantTile);
                                //tilemap.SetTile(x,y,replaceTileId);
                            }
                            else
                            {
                                tilemap.SetTile(x, y, replaceTileId);
                            }

                        }
                    }
                }
            }
        }
    }
}

