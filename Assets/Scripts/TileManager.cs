using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathing;

public class TileManager : MonoBehaviour
{
    [SerializeField] public int selected; //selected tiles variable
    [SerializeField] public float traveltime; //total travel amount variable
    [SerializeField] public GameObject[] selectedTiles = new GameObject[2]; //the maximum tiles selected variable list
    [SerializeField] public GameObject[] nodes = new GameObject[64]; //the overall generated tile list
    [SerializeField] public RaycastHit[] hits; //hit raycast variable
    
    public List<GameObject> findNeighbours; //Neighbours variable
    int cost = 0; //cost value variable
    public IList<GameObject> path; //list of path gameobject(red) tiles

    //Inheriting the AStar Comparison between the shortest value of the tiles
    private class OpenSorter : IComparer<GameObject>
    {
        private Dictionary<GameObject, float> fScore;

        public OpenSorter(Dictionary<GameObject, float> f)
        {
            fScore = f;
        }

        public int Compare(GameObject x, GameObject y)
        {
            if (x != null && y != null)
                return fScore[x].CompareTo(fScore[y]);
            else
                return 0;
        }
    }

    //Inheriting the AStar Variables
    private static List<GameObject> closed;
    private static List<GameObject> open;
    private static Dictionary<GameObject, GameObject> cameFrom;
    private static Dictionary<GameObject, float> gScore;
    private static Dictionary<GameObject, float> hScore;
    private static Dictionary<GameObject, float> fScore;

    void Update()
    {
        //check if 2 tiles are selected & cost is 0...
        if (selected == 2 && cost == 0)
        {          
            path = GetPath(selectedTiles[0], selectedTiles[1]); //Path between "start" and "goal" is equals
            for(int i=0; i<path.Count; i++) 
            {
                path[i].transform.Find("Path").gameObject.SetActive(true); //enable the created (red) path HexhObject
                traveltime += path[i].GetComponent<TileInteraction>().valueCost; //travel time is equals to each tiles cost
            }            
        } 
        //check if 2 tiles are deselected & cost is not 0...
        else if(selected != 2 && cost != 0)
        {
            for (int i = 0; i < path.Count; i++)
            {
                path[i].transform.Find("Path").gameObject.SetActive(false); //Disable the created (red) path HexhObject
            }
            RemovePath(); //Remove neighbours and path
            cost = 0; //Back to default value
            traveltime = 0; //Back to default travel time
        }
    }

    //Estimating the cost between "start" and "goal" method
    private float EstimatedCost(GameObject start, GameObject goal)
    {
        cost = 0;

        //Getting the rayLength of the hit of the raycast
        float rayLength = Mathf.Sqrt(Mathf.Pow(goal.transform.position.x - start.transform.position.x, 2) + Mathf.Pow(goal.transform.position.z - start.transform.position.z, 2));

        //Getting the angle of the selected tiles
        float angle = (Vector3.Angle(goal.transform.position - start.transform.position, transform.forward) * Mathf.PI) / 180;

        //Getting direction
        Vector3 dir = new Vector3(0, 0, 0);

        //Check if the "goal" position is greater than the "start"...
        if (goal.transform.position.x > start.transform.position.x)
        {
            dir = new Vector3(1 * Mathf.Sin(angle), 0, 1 * Mathf.Cos(angle)); //Add positive angle direction
            Debug.DrawRay(start.transform.position, dir * rayLength, Color.green);        
        }
        else
        {
            dir = new Vector3(-1 * Mathf.Sin(angle), 0, 1 * Mathf.Cos(angle)); //Add negative angle direction
            Debug.DrawRay(start.transform.position, dir * rayLength, Color.green);           
        }

        //Implementing raycast with calculated algorithm information
        hits = Physics.RaycastAll(start.transform.position, dir, rayLength);

        //Check each tile when hit within this for loop
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.gameObject.CompareTag("Water"))
            {
                cost += 30; 
            }
            else if (hits[i].collider.gameObject.CompareTag("Mountains"))
            {
                cost += 10;
            }
            else if (hits[i].collider.gameObject.CompareTag("Desert"))
            {
                cost += 5;
            }
            else if (hits[i].collider.gameObject.CompareTag("Forest"))
            {
                cost += 2;
            }
            else
            {
                cost += 1;
            }
        }

        //After for loop, add the value from the selected "start" to calculate overall travelTime
        if (start.gameObject.CompareTag("Water"))
        {
            cost += 30;
        }
        else if (start.gameObject.CompareTag("Mountains"))
        {
            cost += 10;
        }
        else if (start.gameObject.CompareTag("Desert"))
        {
            cost += 5;
        }
        else if (start.gameObject.CompareTag("Forest"))
        {
            cost += 2;
        }
        else
        {
            cost += 1;
        }
        return cost;
    }

    //Inheriting the AStar method
    private IList<GameObject> GetPath(GameObject start, GameObject goal)
    {
        closed = new List<GameObject>();
        open = new List<GameObject>();
        cameFrom = new Dictionary<GameObject, GameObject>();
        gScore = new Dictionary<GameObject, float>();
        hScore = new Dictionary<GameObject, float>();
        fScore = new Dictionary<GameObject, float>();
        closed.Clear();
        open.Clear();
        open.Add(start);

        cameFrom.Clear();
        gScore.Clear();
        hScore.Clear();
        fScore.Clear();

        gScore.Add(start, 0f);
       
        hScore.Add(start, EstimatedCost(start, goal));
        fScore.Add(start, hScore[start]);

        OpenSorter sorter = new OpenSorter(fScore);
        GameObject current,
                        from = null;
        float tentativeGScore;
        bool tentativeIsBetter; 
        while (open.Count > 0)
        {
            current = open[0];
            if (current == goal)
            {
                return ReconstructPath(new List<GameObject>(), cameFrom, goal);
            }
            open.Remove(current);
            closed.Add(current);

            if (current != start)
            {
                from = cameFrom[current];
            }
            foreach (GameObject next in current.GetComponent<TileInteraction>().neighbours)
            {
                if (from != next && !closed.Contains(next))
                {
                    tentativeGScore = gScore[current] + next.GetComponent<TileInteraction>().valueCost;
                    tentativeIsBetter = true;

                    if (!open.Contains(next))
                    {
                        open.Add(next);
                    }
                    else
                    if (tentativeGScore >= gScore[next])
                    {
                        tentativeIsBetter = false;
                    }

                    if (tentativeIsBetter)
                    {
                        cameFrom[next] = current;
                        gScore[next] = tentativeGScore;
                        hScore[next] = EstimatedCost(next, goal);
                        fScore[next] = gScore[next] + hScore[next];
                    }
                }
            }
            open.Sort(sorter);
        }
        return null;
    }
  
    private IList<GameObject> ReconstructPath(IList<GameObject> path, Dictionary<GameObject, GameObject> cameFrom, GameObject currentNode)
    {
        if (cameFrom.ContainsKey(currentNode))
        {
            ReconstructPath(path, cameFrom, cameFrom[currentNode]);
        }
        path.Add(currentNode);
        return path;
    }

    //Removing the neighbours and path
    private void RemovePath()
    {       
        while (findNeighbours.Count > 0)
        {
            findNeighbours.Remove(findNeighbours[findNeighbours.Count - 1]);
        }
        while (path.Count > 0)
        {
            path.Remove(path[path.Count - 1]);
        }        
    }
}
