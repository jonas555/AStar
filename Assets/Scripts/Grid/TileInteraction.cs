using Pathing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileInteraction : MonoBehaviour
{
    [SerializeField] private TileManager tileManager; //Get TileManager script
    [SerializeField] private GameObject highlight; //Highlighted tiles variable. A yellow HexObject within the tile Prefab
    [SerializeField] public GameObject selected; //Selected tiles variable. A green HexObject within the tile Prefab
    [SerializeField] public GameObject paths; //Path tiles from start to goal. A red HexObject within the tile Prefab
    [SerializeField] private Text costText; //Cost of travel display
    [SerializeField] private GameObject neighboursHighlight; //Neighbours surrounding the selected tile (demonstration purpose). A grey HexObject
    [SerializeField] public List<GameObject> neighbours = new List<GameObject>(); //List of neighbours

    int tileselected; //Amount selected variable

    public float valueCost = 0; //Cost value variable

    public bool tileIsSelected = false; //check if tile is selected


    void Start()
    {
        GameObject parent = transform.parent.gameObject; //This gameobject(Tile) prefab
        tileManager = parent.GetComponent<TileManager>(); //Parent the TileManager when Play
  
        costText = GameObject.Find("Amountofdays").GetComponent<Text>(); //Find the Text within the TileParent prefab
        findNeighbours(); //Start finding the neighbours based on Overlapping collision with other tiles
    }

    void OnMouseEnter() //Hovering the tiles when MouseOn
    {
        if (!gameObject.CompareTag("Water")){
            highlight.SetActive(true);
            costText.text = "This tile takes " + valueCost + " days to cross";          
        }
    }

    void OnMouseExit() //Exiting hovering when MouseOff
    {
        highlight.SetActive(false);
        costText.text = "This tile takes ... days to cross";
    }

    void OnMouseDown() //Clicking
    {
        if (!tileIsSelected && tileManager.selected<2) //Check selected tiles and the amount
        {           
            if(gameObject.CompareTag("Forest"))
            {
                tileIsSelected = true;
                selected.SetActive(true);
            }
            else if (gameObject.CompareTag("Mountains"))
            {
                tileIsSelected = true;
                selected.SetActive(true);
            }
            else if (gameObject.CompareTag("Desert"))
            {
                tileIsSelected = true;
                selected.SetActive(true);
            }
            else if (gameObject.CompareTag("Grass"))
            {
                tileIsSelected = true;
                selected.SetActive(true);
            }

            if (!gameObject.CompareTag("Water"))
            {
                tileselected = tileManager.selected++;
                tileManager.selectedTiles[tileselected] = this.gameObject;
            }
            Highlight(); //Highlight the neighbours after selecting 2 tiles
        }
        else if (tileIsSelected) //Check again if selected, disable the components
        {
            tileIsSelected = false;
            selected.SetActive(false);
            paths.SetActive(false);
            if (!gameObject.CompareTag("Water")) //Prevent a tile with "Water" tag to be interactive
            {
                if(tileManager.selectedTiles[0] != null) //if "start" tile is selected...
                {
                    tileManager.selectedTiles[0].GetComponent<TileInteraction>().removeHighlight();
                    tileManager.selectedTiles[0].GetComponent<TileInteraction>().selected.SetActive(false);
                    tileManager.selectedTiles[0].GetComponent<TileInteraction>().tileIsSelected = false;
                    tileManager.selectedTiles[0].GetComponent<TileInteraction>().paths.SetActive(false);
                    tileManager.selectedTiles[0] = null; //is selected                   
                }
                if(tileManager.selectedTiles[1] != null) //if "goal" tile is selected...
                {
                    tileManager.selectedTiles[1].GetComponent<TileInteraction>().removeHighlight();
                    tileManager.selectedTiles[1].GetComponent<TileInteraction>().selected.SetActive(false);
                    tileManager.selectedTiles[1].GetComponent<TileInteraction>().tileIsSelected = false;
                    tileManager.selectedTiles[1].GetComponent<TileInteraction>().paths.SetActive(false);
                    tileManager.selectedTiles[1] = null; //is selected
                }
                if(tileManager.selectedTiles[1] == null && tileManager.selectedTiles[0] == null) //if both tiles deselected...
                {
                    tileManager.selected = 0; //back to default value                   
                }                   
            }
        } 
    }

    //Locate neighbours when two tiles are selected
    void findNeighbours()
    {
        Collider[] colliders = Physics.OverlapSphere(this.gameObject.transform.position, 1); //current selected gameobjects position and the radius of its Collider
        foreach (var collider in colliders) //for each Collider on each selected tile...
        {
            GameObject obj = collider.gameObject; 
            if (!neighbours.Contains(obj) && obj!=this.gameObject && obj.tag!="Water") //check if a selected tile is surrounded by neighbours & not equals to the "Water" tag tile
            {
                neighbours.Add(obj); //Display them              
            }           
        }
    }

    //Highlight the neighbours
    void Highlight()
    {
        for(int i=0; i< neighbours.Count; i++)
        {
            GameObject obj = neighbours[i];
            obj.GetComponent<TileInteraction>().neighboursHighlight.SetActive(true);
        }
    }

    //Remove highlighted neighbours
    void removeHighlight()
    { 
        for(int i = 0; i< neighbours.Count; i++)
        {
            GameObject obj = neighbours[i];
            obj.GetComponent<TileInteraction>().neighboursHighlight.SetActive(false);
        }
    }
}
