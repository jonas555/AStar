using Pathing;
using UnityEngine;

public class TileMapGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] tiles; //Create Tile List
    [SerializeField] private GameObject tilesParent; //Switch the TileMapGenerator to TileManager script
    [SerializeField] private GameObject[] nodes = new GameObject[64]; //The maximum amount of tiles
    int biomes = 5; //Biomes: ocean, desert, forest, mountains, grassfield
    
    [SerializeField] int width = 7;
    [SerializeField] int height = 7;
    [SerializeField] public int tilesamount=0; //Overall tiles amount

    public float tileXOffset = 1.8f;
    public float tileZOffset = 1.5f;
    private GameObject parent; //Parent the script of TileManager
    void Start()
    {
        parent = Instantiate(tilesParent); //Instantiate the Parented TiledManager
        CreateHexTile(); //Generate Hex Tiles   
    }

    //Generating Hex Tiles Method
    void CreateHexTile()
    {
        for(int x=0; x <= width; x++) //X axis
        {
            for (int z = 0; z <= height; z++) //Z axis
            {            
                int randomBiomes = Random.Range(0, biomes); //Generate random biomes from 0 to 5
                GameObject TempObj = Instantiate(tiles[randomBiomes].gameObject); //Instantiate random object tiles within the scene             

                //Offseting the tiles
                if (z % 2 == 0) 
                {
                    TempObj.transform.position = new Vector3(x * tileXOffset, 0, z * tileZOffset);                  
                }
                else
                {
                    TempObj.transform.position = new Vector3(x * tileXOffset + tileXOffset / 2, 0, z * tileZOffset);                   
                }
                SetTileInfo(TempObj, x, z, tilesamount); //Placing additional information about the tiles position
                tilesamount++; //Adding the amount
            }
        }
        GameObject.Destroy(this.gameObject); //Destroy this game object when Play      
    }

    //Generated Tiles information
    void SetTileInfo(GameObject obj, int x, int z, int i)
    {
        obj.transform.parent = parent.transform;
        obj.name = x.ToString() + ", " + z.ToString();
        nodes[i] = obj;
        parent.GetComponent<TileManager>().nodes = nodes;
    }
}
