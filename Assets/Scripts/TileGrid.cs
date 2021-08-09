using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum ObjectTileType
{
    Empty = 0,
    Rock = 2001,
    Flower = 2002
}

public class TileGrid : MonoBehaviour
{
    public int Width, Height;
    public int TileSize, Seed;
    System.Random rand = new System.Random();
    public Dictionary<int, Tile> Tiles { get; private set; }

    [System.Serializable]
    class GroundTiles
    {
        public GroundTileType TileType;
        public Texture2D Texture;
        public Color Color;
    }

    [System.Serializable]
    class ObjectTiles
    {
        public ObjectTileType TileType;
        public Texture2D Texture;
        public Color Color;
    }

    [SerializeField]
    private GroundTiles[] GroundTileTypes;

    [SerializeField]
    private ObjectTiles[] ObjectTileTypes;


    


    private Tile CreateTile(Color color, Texture2D texture)
    {
        bool setColor = false;
        if(texture == null)
        {
            setColor = true;
            texture = new Texture2D(TileSize, TileSize);
        }

        texture.filterMode = FilterMode.Point;

        var sprite = Sprite.Create(texture, new Rect(0, 0, TileSize, TileSize), new Vector2(0.5f, 0.5f), TileSize);
        //Create a scriptable object instance
        var tile = ScriptableObject.CreateInstance<Tile>();
        if (setColor)
        {
            // Make sure color is not transparent
            color.a = 1;
            //Set tile color
            tile.color = color;
        }
        
        //Assign the sprite to our tiles
        tile.sprite = sprite;

        return tile;

    }

    private Dictionary<int, Tile> InitializeTiles()
    {
        var dictionary = new Dictionary<int, Tile>();

        var tileSprite = Sprite.Create(new Texture2D(TileSize, TileSize), new Rect(0, 0, TileSize, TileSize), new Vector2(0.5f, 0.5f), TileSize);
        
        foreach (var tileType in GroundTileTypes)
        {
            //Dont make tile for empty
            if (tileType.TileType == 0) continue;
            
            var tile = CreateTile(tileType.Color, tileType.Texture);

            // Add to dictionary
            dictionary.Add((int)tileType.TileType, tile);
        }

        // Create a Tile for each ObjectTileType
        foreach (var tileType in ObjectTileTypes)
        {
            //Dont make tile for empty
            if (tileType.TileType == 0) continue;

            // Create tile scriptable object
            var tile = CreateTile(tileType.Color, tileType.Texture);
            // Add to dictionary by key int value, value Tile
            dictionary.Add((int)tileType.TileType, tile);
        }

        return dictionary;
    }
    public Dictionary<TilemapType, TilemapStructure> Tilemaps;

    private void Awake()
    {
        Seed = rand.Next();
        Tiles = InitializeTiles();

        Tilemaps = new Dictionary<TilemapType, TilemapStructure>();

        foreach(Transform child in transform)
        {
            var tilemap = child.GetComponent<TilemapStructure>();
            if (tilemap == null) continue;
            if (Tilemaps.ContainsKey(tilemap.Type))
            {
                throw new System.Exception("Duplicate tilemap type: " + tilemap.Type);
            }
            Tilemaps.Add(tilemap.Type, tilemap);

        }

        // Initilaize the tilemaps
        foreach(var tilemap in Tilemaps.Values)
        {
            tilemap.Initialize();
        }
        
        
    }

}
