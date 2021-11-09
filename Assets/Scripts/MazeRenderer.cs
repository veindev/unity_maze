using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeRenderer : MonoBehaviour
{


    static int width = 8;
    static int height = 8;

    public float cellSize;

    [SerializeField]
    private Transform wallPrefab = null;

    [SerializeField]
    MazeGenerator mazeGenerator = new MazeGenerator();
    // curent state of game
    WallState[,] maze = MazeGenerator.Generate(width, height);


    public Transform ballPrefab;
    public Transform targetPrefab; 
    public Transform linePrefab;
    Transform ballTransform;
    Transform targetTransform;
    Coord ballCoord;
    Coord targetCoord;
    Transform mazeHolder;

    public GameObject winMenu;     
    bool win;

    void Start(){
        Draw();
    }

    void Update()
    {
        if(win){
            winMenu.SetActive(true);
        }

        if(ballTransform.position == targetTransform.position){
            win =  true; 
        }

        if(Input.GetKeyDown(KeyCode.RightArrow)){
            if(!maze[ballCoord.x, ballCoord.y].HasFlag(WallState.RIGHT)){
                ballCoord.x +=1;
                Vector3 prevBallPosition  = ballTransform.position;
                ballTransform.position += Vector3.right;

                Transform newLine = Instantiate(linePrefab) as Transform;
                newLine.GetComponent<Line>().positionA = prevBallPosition;
                newLine.GetComponent<Line>().positionB = ballTransform.position;
                newLine.parent = mazeHolder;
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (!maze[ballCoord.x, ballCoord.y].HasFlag(WallState.LEFT))
            {
                ballCoord.x -= 1;
                Vector3 prevBallPosition  = ballTransform.position;     
                ballTransform.position += Vector3.left;
                Transform newLine = Instantiate(linePrefab) as Transform;
                newLine.GetComponent<Line>().positionA = prevBallPosition;
                newLine.GetComponent<Line>().positionB = ballTransform.position;
                newLine.parent = mazeHolder;
                
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (!maze[ballCoord.x, ballCoord.y].HasFlag(WallState.UP))
            {
                ballCoord.y += 1;
                Vector3 prevBallPosition  = ballTransform.position;     
                ballTransform.position += Vector3.forward;
                Transform newLine = Instantiate(linePrefab) as Transform;
                newLine.GetComponent<Line>().positionA = prevBallPosition;
                newLine.GetComponent<Line>().positionB = ballTransform.position;
                newLine.parent = mazeHolder;
                
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (!maze[ballCoord.x, ballCoord.y].HasFlag(WallState.DOWN))
            {
                ballCoord.y -= 1;
                Vector3 prevBallPosition  = ballTransform.position;     
                ballTransform.position += Vector3.back;
                Transform newLine = Instantiate(linePrefab) as Transform;
                newLine.GetComponent<Line>().positionA = prevBallPosition;
                newLine.GetComponent<Line>().positionB = ballTransform.position;
                newLine.parent = mazeHolder;
                
            }
        }
    }


    public void Draw()
    {
        winMenu.SetActive(false);
        win = false; 
    
        print("draw");  
        // Generate new maze again
        maze = MazeGenerator.Generate(width, height);
        // clear all rendered wall 
        string holderName = "Generated walls";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        mazeHolder = new GameObject(holderName).transform;
        mazeHolder.parent = transform;

        // draw ball at random Coord
        var rng = new System.Random(/*seed*/);
        ballCoord = new Coord { x = rng.Next(0, width), y = rng.Next(0, height) };
        ballTransform = Instantiate(ballPrefab ) as Transform;
        ballTransform.position = CoordToPosition(ballCoord);
        ballTransform.parent = mazeHolder;

        // Generate target coord and draw target circle
        targetCoord =  new Coord { x = rng.Next(0, width), y = rng.Next(0, height) } ;
        targetTransform = Instantiate(targetPrefab) as Transform;
        targetTransform.position = CoordToPosition(targetCoord);
        targetTransform.parent = mazeHolder;
        

        // Draw walls
        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                var cell = maze[i, j];
                var position = new Vector3(-width / 2 + i, 0, -height / 2 + j) ;

                if (cell.HasFlag(WallState.UP))
                {
                    var topWall = Instantiate(wallPrefab) as Transform;
                    topWall.position = position + new Vector3(0, 0, cellSize / 2);
                    topWall.localScale = new Vector3(cellSize, topWall.localScale.y, topWall.localScale.z);
                    topWall.parent = mazeHolder;
                }

                if (cell.HasFlag(WallState.LEFT))
                {
                    var leftWall = Instantiate(wallPrefab) as Transform;
                    leftWall.position = position + new Vector3(-cellSize / 2, 0, 0);
                    leftWall.localScale = new Vector3(cellSize, leftWall.localScale.y, leftWall.localScale.z);
                    leftWall.eulerAngles = new Vector3(0, 90, 0);
                    leftWall.parent = mazeHolder;
                }

                if (i == width - 1)
                {
                    if (cell.HasFlag(WallState.RIGHT))
                    {
                        var rightWall = Instantiate(wallPrefab) as Transform;
                        rightWall.position = position + new Vector3(+cellSize / 2, 0, 0);
                        rightWall.localScale = new Vector3(cellSize, rightWall.localScale.y, rightWall.localScale.z);
                        rightWall.eulerAngles = new Vector3(0, 90, 0);
                        rightWall.parent = mazeHolder;

                    }
                }

                if (j == 0)
                {
                    if (cell.HasFlag(WallState.DOWN))
                    {
                        var bottomWall = Instantiate(wallPrefab) as Transform;
                        bottomWall.position = position + new Vector3(0, 0, -cellSize / 2);
                        bottomWall.localScale = new Vector3(cellSize, bottomWall.localScale.y, bottomWall.localScale.z);
                        bottomWall.parent = mazeHolder;
                    }
                }
            }

        }

    }

    Vector3 CoordToPosition(Coord coord){
        return new Vector3(-width/2 + coord.x, 0, -height/2 + coord.y);
    }

    Coord PositionToCoord(Vector3 pos){
        return new Coord( (int)pos.x + width/2, (int)pos.z + height/2 );
    }


}
