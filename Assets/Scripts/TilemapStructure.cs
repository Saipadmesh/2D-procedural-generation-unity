using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public enum GroundTileType
{
    Empty = 0,
    DeepWater = 1,
    UndeepWater = 5,
    Beach = 3,
    Grass = 4,
    Dirt = 2,
    Mountain = 6,
    Snow = 7
}

public enum TilemapType
{
    Ground,
    Object
}

public abstract class AlgorithmBase : ScriptableObject
{
    public abstract void Apply(TilemapStructure tilemap);
}
public class TilemapStructure : MonoBehaviour
{
    //System.Random rand = new System.Random();
    [HideInInspector]
    public int Width, Height, seed, TileSize ;
    //private Dictionary<int, Tile> _tileTypeDictionary;
    private int[] _tiles;
    private Tilemap _graphicMap;

    [HideInInspector]
    public TileGrid Grid;

    [SerializeField]
    private TilemapType _type;
    public TilemapType Type { get { return _type; } }

    //Bit Masking

    private Dictionary<int, int> maskDict = new Dictionary<int, int>() { { 2, 1 }, { 8, 2 }, { 10, 3 }, { 11, 4 }, { 16, 5 }, { 18, 6 }, { 22, 7 }, { 24, 8 }, { 26, 9 }, { 27, 10 }, { 30, 11 }, { 31, 12 }, { 64, 13 }, { 66, 14 }, { 72, 15 }, { 74, 16 }, { 75, 17 }, { 80, 18 }, { 82, 19 }, { 86, 20 }, { 88, 21 }, { 90, 22 }, { 91, 23 }, { 94, 24 }, { 95, 25 }, { 104, 26 }, { 106, 27 }, { 107, 28 }, { 120, 29 }, { 122, 30 }, { 123, 31 }, { 126, 32 }, { 127, 33 }, { 208, 34 }, { 210, 35 }, { 214, 36 }, { 216, 37 }, { 218, 38 }, { 219, 39 }, { 222, 40 }, { 223, 41 }, { 248, 42 }, { 250, 43 }, { 251, 44 }, { 254, 45 }, { 255, 46 }, { 0, 47 } };
    public Texture2D[] cornerTextures;
    public Texture2D[] objectTextures;

    /*[Serializable]
    class TileType
    {
        public GroundTileType GroundTile;
        public Color Color;
    }

    [SerializeField]
    private TileType[] TileTypes; */

    [SerializeField]
    private AlgorithmBase[] _algorithms;
    /// <summary>
    /// Method called by unity automatically
    /// </summary>
    public void Initialize()
    {
        //seed = rand.Next();
        // Retrieve the Tilemap component from the same object this script is attached to
        _graphicMap = GetComponent<Tilemap>();

        // Retrieve the TileGrid component from parent gameObject
        Grid = transform.parent.GetComponent<TileGrid>();

        Width = Grid.Width;
        Height = Grid.Height;
        seed = Grid.Seed;
        TileSize = Grid.TileSize;
        // Initialize the one-dimensional array with our map size
        _tiles = new int[Width * Height];
        
        /*// Initialize a dictionary lookup table 
        _tileTypeDictionary = new Dictionary<int, Tile>();
        // The sprite of the tile
        var tileSprite = Sprite.Create(new Texture2D(TileSize, TileSize), new Rect(0, 0, TileSize, TileSize), new Vector2(0.5f, 0.5f), TileSize);
        
        // Create a tile for each GroundTileType
        foreach(var tileType in TileTypes)
        {
            //Create a scriptable object instance
            var tile = ScriptableObject.CreateInstance<Tile>();
            // Make sure color is not transparent
            tileType.Color.a = 1;
            //Set tile color
            tile.color = tileType.Color;
            //Assign the sprite to our tiles
            tile.sprite = tileSprite;
            // Add to dictionary
            _tileTypeDictionary.Add((int)tileType.GroundTile, tile);
        } */

        //Apply all algorithms to tilemap
        foreach(var _algorithm in _algorithms){
            Generate(_algorithm);
        }
        //Render the data
        Vector3Int[] posArray;
        Tile[] tileArray;
        (posArray,tileArray) = RenderAllTiles();
        if(this.Type == TilemapType.Ground)
        {
            (posArray,tileArray) = RenderCorners(posArray, tileArray);
        }
        else if(this.Type == TilemapType.Object)
        {
            (posArray, tileArray) = RenderObjects(posArray, tileArray);
        }
        _graphicMap.SetTiles(posArray, tileArray);
        _graphicMap.RefreshAllTiles();

    }

    public void Generate(AlgorithmBase algorithm)
    {
        algorithm.Apply(this);
    }
    
    public int GetTile(int x, int y)
    {
        return InBounds(x, y) ? _tiles[y * Width + x] : 0; 
    }

    public void SetTile(int x, int y, int value)
    {
        if (InBounds(x, y))
        {
            _tiles[y * Width + x] = value;
        }
    }

    /// <summary>
    /// Renders the entire data structure to unity's tilemap
    /// </summary>
    /// 
    //changed from void to tuple and returned
    public (Vector3Int[], Tile[]) RenderAllTiles()
    {
        var positionsArray = new Vector3Int[Width * Height];
        var tilesArray = new Tile[Width * Height];

        for(int x = 0; x < Width; x++)
        {
            for(int y = 0; y < Height; y++)
            {
                //Add the position at the same index position as the tile
                positionsArray[y * Width + x] = new Vector3Int(x, y, 0);
                var typeOfTile = GetTile(x, y);
                //tilesArray[y * Width + x] = _tileTypeDictionary[typeOfTile];
                if(!Grid.Tiles.TryGetValue(typeOfTile, out Tile tile))
                {
                    if (typeOfTile != 0)
                    {
                        Debug.LogError("Tile not defined for tile id: " + typeOfTile);
                    }

                    tilesArray[y * Width + x] = null;
                    continue;
                }
                //changed 
                if(tilesArray[y*Width+x] == null)
                {
                    tilesArray[y * Width + x] = tile;
                }
                

            }
            
        }

        return (positionsArray, tilesArray);
        
    }

    public (Vector3Int[] posArr, Tile[] tileArr) RenderCorners(Vector3Int[] posArr,Tile[] tileArr)
    {
        int waterval = (int)Enum.Parse(typeof(GroundTileType),"UndeepWater");
        int grassVal = (int)Enum.Parse(typeof(GroundTileType), "Grass");

        for (int x = 0; x < Width; x++)
        {
            for(int y = 0; y < Height; y++)
            {
                if(GetTile(x,y) == grassVal || GetTile(x,y) == waterval)
                {
                    int cornerval = deepWaterCorners(x, y);
                    int arrLoc = maskDict[cornerval];
                    if(GetTile(x,y) == waterval)
                    {
                        arrLoc += 48;
                    }
                    
                    /*if (arrLoc == 26) //Debug
                    {
                        Debug.Log("(" + x + "," + y+")");
                        Debug.Log("North west:" + GetTile(x - 1, y + 1));
                        Debug.Log("North:" + GetTile(x, y + 1));
                        Debug.Log("North east:" + GetTile(x + 1, y + 1));
                        Debug.Log("West:" + GetTile(x - 1, y ));
                        Debug.Log("East:" + GetTile(x + 1, y ));
                        Debug.Log("South west:" + GetTile(x - 1, y - 1));
                        Debug.Log("South:" + GetTile(x , y - 1));
                        Debug.Log("South east:" + GetTile(x + 1, y - 1));
                        

                    }*/
                    //else
                    {
                        
                        
                        var sprite = Sprite.Create(cornerTextures[arrLoc], new Rect(0, 0, TileSize, TileSize), new Vector2(0.5f, 0.5f), TileSize);
                        var tile = ScriptableObject.CreateInstance<Tile>();
                        tile.sprite = sprite;
                        tileArr[y * Width + x] = tile;
                    }
                    

                }
            }
        }

        return (posArr, tileArr);
        //_graphicMap.SetTiles(posArr, tileArr);
        //_graphicMap.RefreshAllTiles();
    }

    public (Vector3Int[] posArr, Tile[] tileArr) RenderObjects(Vector3Int[] posArr, Tile[] tileArr)
    {
        //test code, add a loop later
        int flowerVal = (int)Enum.Parse(typeof(ObjectTileType), "Flower");
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if(GetTile(x,y) == flowerVal)
                {
                    //Random random = new System.Random();
                    int arrLoc = (int)UnityEngine.Random.Range(0,3);
                    var sprite = Sprite.Create(objectTextures[arrLoc], new Rect(0, 0, TileSize, TileSize), new Vector2(0.5f, 0.5f), TileSize);
                    var tile = ScriptableObject.CreateInstance<Tile>();
                    tile.sprite = sprite;
                    tileArr[y * Width + x] = tile;

                }
            }
        }
                return (posArr, tileArr);
    }

    private bool InBounds(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    private int sandCorners(int x, int y)
    {

        int current = GetTile(x, y);
        ArrayList arr = new ArrayList();
        /*int north = GetTile(x,y+1)/current;
        int south = GetTile(x, y - 1);
        int east = GetTile(x + 1, y);
        int west = GetTile(x - 1, y);
        int northeast = GetTile(x + 1, y + 1);
        int northwest = GetTile(x - 1, y + 1);
        int southeast = GetTile(x + 1, y - 1);
        int southwest = GetTile(x - 1, y - 1);*/

        int retval=0;
        for(int i = x - 1; i < x + 2; i++)
        {
            for(int j = y+1; j> y-2; j--)
            {
                if(i == x && j == y)
                {
                    continue;
                }
                float val = Mathf.Clamp01((float)GetTile(i, j) / current);
                int valToBeAdded = Mathf.RoundToInt(val);
                arr.Add(valToBeAdded);
            }
        }

        for(int a = 0; a < arr.Count; a++)
        {
            int power = (int)Mathf.Pow(2, a);
            retval += power*(int)arr[a];
        }
        
        return retval;
    }

    private int changeGrassTile(int tile, int current)
    {
        int shallowWaterVal = (int)Enum.Parse(typeof(GroundTileType), "UndeepWater");
        return (tile == shallowWaterVal) ? current : tile;
    }
    
    private int deepWaterCorners(int x, int y)
    {

        int current = GetTile(x, y);
        
        //int testCurrent = 4; //change to current
        
        
        
        bool bnorth = !Convert.ToBoolean(current - changeGrassTile(GetTile(x, y + 1),current));
        int north = Convert.ToInt32(bnorth);
        bool bsouth = !Convert.ToBoolean(current - changeGrassTile(GetTile(x, y - 1),current));
        int south = Convert.ToInt32(bsouth);
        bool beast = !Convert.ToBoolean(current-changeGrassTile(GetTile(x+1, y),current));
        int east = Convert.ToInt32(beast);
        bool bwest = !Convert.ToBoolean(current-changeGrassTile(GetTile(x-1, y),current));
        int west = Convert.ToInt32(bwest);
        int northeast = Convert.ToInt32(!Convert.ToBoolean((current - changeGrassTile(GetTile(x+1, y + 1),current))) && bnorth && beast);
        int northwest = Convert.ToInt32(!Convert.ToBoolean((current - changeGrassTile(GetTile(x - 1, y + 1),current))) && bnorth && bwest);
        int southeast = Convert.ToInt32(!Convert.ToBoolean((current - changeGrassTile(GetTile(x + 1, y - 1),current))) && bsouth && beast);
        int southwest = Convert.ToInt32(!Convert.ToBoolean((current - changeGrassTile(GetTile(x - 1, y - 1),current))) && bsouth && bwest);

        

        
        int retval = northwest + 2 * north + 4 * northeast + 8 * west + 16 * east + 32 * southwest + 64 * south + 128 * southeast;
        /*if(retval == 104)
        {
            Debug.Log("North west tile:" + northwest);
            Debug.Log("North tile:" + north);
            Debug.Log("North east tile:" + northeast);
            Debug.Log("West tile:" + west);
            Debug.Log("East tile:" + east);
            Debug.Log("South west tile:" + southwest);
            Debug.Log("South tile:" + south);
            Debug.Log("South east tile:" + southeast);
        }*/
        /*for (int i = x - 1; i < x + 2; i++)
        {
            for (int j = y + 1; j > y - 2; j--)
            {
                if (i == x && j == y)
                {
                    continue;
                }
                int tileVal = GetTile(i, j);

                int valToBeAdded;
                
                if(testCurrent == tileVal)
                {
                    valToBeAdded = 0;
                }
                else
                {
                    valToBeAdded = 1;
                }
                //float val = Mathf.Clamp01((float)((tileVal - current) / current));
                //int valToBeAdded = Mathf.RoundToInt(val);
                
                arr.Add(valToBeAdded);
                
            }
        } */

        /*for (int a = 0; a < arr.Count; a++)
        {
            
            int power = (int)Mathf.Pow(2, a);
            retval += power * (int)arr[a];
            Debug.Log("Array Value: " + arr[a]+" Retval current: "+retval);
        }*/
        //Debug.Log("( " + x + " , " + y + " ) = " + retval);
        return retval;
    }

    public List<KeyValuePair<Vector2Int, int>> GetNeighbors(int tileX, int tileY)
    {
        int startX = tileX - 1;
        int startY = tileY - 1;
        int endX = tileX + 1;
        int endY = tileY + 1;

        var neighbors = new List<KeyValuePair<Vector2Int, int>>();
        for (int x = startX; x < endX + 1; x++)
        {
            for (int y = startY; y < endY + 1; y++)
            {
                
                if (x == tileX && y == tileY) continue;

                // Check if the tile is within the tilemap, otherwise we don't need to pass it along
                // As it would be an invalid neighbor
                if (InBounds(x, y))
                {
                    // Pass along a key value pair of the coordinate + the tile type
                    neighbors.Add(new KeyValuePair<Vector2Int, int>(new Vector2Int(x, y), GetTile(x, y)));
                }
            }
        }
        return neighbors;
    }


    public List<KeyValuePair<Vector2Int, int>> GetCornerNeighbors(int tileX, int tileY)
    {
        int startX = tileX - 1;
        int startY = tileY - 1;
        int endX = tileX + 1;
        int endY = tileY + 1;

        var neighbors = new List<KeyValuePair<Vector2Int, int>>();
        for (int x = startX; x < endX + 1; x++)
        {
            for (int y = startY; y < endY + 1; y++)
            {

                if (x == tileX || y == tileY ) continue;

                // Check if the tile is within the tilemap, otherwise we don't need to pass it along
                // As it would be an invalid neighbor
                if (InBounds(x, y))
                {
                    // Pass along a key value pair of the coordinate + the tile type
                    neighbors.Add(new KeyValuePair<Vector2Int, int>(new Vector2Int(x, y), GetTile(x, y)));
                }
            }
        }
        return neighbors;
    }


}
