using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GridDisplay : MonoBehaviour
{
    public GameObject node;
    public GameObject wall;
    public Material final;
    public Material explore;


    GameObject[][][] mazeNodes;
    float halfWidths;

    LinkedList<GameObject> current = new LinkedList<GameObject>();
    HashSet<GameObject> explored = new HashSet<GameObject>();
    Stack<Point> backtrackStack= new Stack<Point>();

    Point currentPoint = new Point(0, 0);
    
    // Start is called before the first frame update
    void Start()
    {
        halfWidths = (int)Camera.main.orthographicSize;
        float wallTranslateOffset = node.transform.localScale.x / 2 - wall.transform.localScale.x / 2;
        mazeNodes = new GameObject[(int)halfWidths*2][][];
        for (int i = 0; i < halfWidths * 2; i++) {
            mazeNodes[i] = new GameObject[(int)halfWidths * 2][];
            for (int j = 0; j < Camera.main.orthographicSize * 2; j++) {
                mazeNodes[i][j] = new GameObject[]{
                    Instantiate(node, new Vector3(i, j, 0), Quaternion.identity),//index 0 is the node itself
                    Instantiate(wall, new Vector3(i,j+wallTranslateOffset,0),Quaternion.Euler(new Vector3(0,0,90))),//index 1 is the top wall
                    Instantiate(wall, new Vector3(i+wallTranslateOffset,j,0),Quaternion.identity),                  //index 2 is the right wall
                    Instantiate(wall, new Vector3(i,j-wallTranslateOffset,0),Quaternion.Euler(new Vector3(0,0,90))),//index 3 is the bottom wall
                    Instantiate(wall, new Vector3(i-wallTranslateOffset,j,0),Quaternion.identity)                   //index 4 is the left wall
                };
                foreach (GameObject o in mazeNodes[i][j]) o.transform.Translate(new Vector2(-halfWidths + node.transform.localScale.x/2, -halfWidths + node.transform.localScale.y / 2),Space.World); 
            }
        }
        backtrackStack.Push(currentPoint);
    }
    void Update()
    {
        if (backtrackStack.Count == 0) SceneManager.LoadScene(0);
        //Find out which nodes are neighbors and if they are open
        Point[] neighbors=checkNeighbors(currentPoint);
        if(Array.FindAll(neighbors, r => r != neighbors[0]).Length == 0)
        {
            //BackTrack if no available nodes
            GameObject nodeToSetExplored = (GameObject)mazeNodes[currentPoint.X][currentPoint.Y][0];
            nodeToSetExplored.GetComponent<Renderer>().material = final;
            explored.Add(nodeToSetExplored);
            Point pop = backtrackStack.Pop();
            mazeNodes[pop.X][pop.Y][0].GetComponent<Renderer>().material = final;
            currentPoint = backtrackStack.Peek();
        }
        else
        {
            int index; //Get an index of node that also reflects the wall to be removed
            do{
                index = UnityEngine.Random.Range(1, 5);
            }while (neighbors[index] == neighbors[0]);
            //Remove wall to destination;
            Destroy(mazeNodes[currentPoint.X][currentPoint.Y][index].gameObject);
            GameObject nodeToSetCurrent = (GameObject)mazeNodes[currentPoint.X][currentPoint.Y][0];
            backtrackStack.Push(currentPoint);
            nodeToSetCurrent.GetComponent<Renderer>().material = explore;
            currentPoint = neighbors[index];
            nodeToSetCurrent = (GameObject)mazeNodes[currentPoint.X][currentPoint.Y][0];
            current.AddFirst(mazeNodes[currentPoint.X][currentPoint.Y][0]);
            //Remove wall at destination
            Destroy(mazeNodes[currentPoint.X][currentPoint.Y][(index+=(index>2)?-2:2)].gameObject);





        }

        //set new neighbor as current
    }

    Point[] checkNeighbors(Point p){
        Point[] result = new Point[5];
        if (p.Y != mazeNodes.Length - 1 && !current.Contains(mazeNodes[p.X][p.Y+1][0]) && !explored.Contains(mazeNodes[p.X][p.Y+1][0])) result[1] = new Point(p.X, p.Y+1);//Top Node is Open (result[1])
        if (p.X != mazeNodes.Length - 1 && !current.Contains(mazeNodes[p.X+1][p.Y][0]) && !explored.Contains(mazeNodes[p.X+1][p.Y][0])) result[2] = new Point(p.X+1, p.Y);//Right Node is Open (result[2])
        if (p.Y != 0 && !current.Contains(mazeNodes[p.X][p.Y-1][0]) && !explored.Contains(mazeNodes[p.X][p.Y-1][0])) result[3] = new Point(p.X, p.Y-1);//Bottom Node is Open (result[3])
        if (p.X != 0 && !current.Contains(mazeNodes[p.X-1][p.Y][0]) && !explored.Contains(mazeNodes[p.X-1][p.Y][0])) result[4] = new Point(p.X-1, p.Y);//Left Node is Open (result[4])

        return result;
        //return Array.FindAll(result,r=>r!=result[0]);
    }
}
