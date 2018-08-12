using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Passenger : MonoBehaviour
{
    public gridPosition currentGridPosition = new gridPosition();
    public gridPosition startGridPosition = new gridPosition();
    public gridPosition endGridPosition = new gridPosition();


    //старое
    public GameObject map;
    Level LevelSettings;
    public float speed = 1.0f;
    public Transform Coords; //текущие координаты
    bool MoveBool = false;
    public Vector3[] ExitPoint;//точки выхода, в которых объект уничтожается
    private float tempstep = 0;
    private ArrayList path = new ArrayList();
    private Vector2 currentPosition;
    private Vector2 nextPosition;
    private Vector2 currentDirect;

    public class MySolver<TPathNode, TUserContext> : SettlersEngine.SpatialAStar<TPathNode,
    TUserContext> where TPathNode : SettlersEngine.IPathNode<TUserContext>
    {
        protected override Double Heuristic(PathNode inStart, PathNode inEnd)
        {


            int formula = GameManager.distance;
            int dx = Math.Abs(inStart.X - inEnd.X);
            int dy = Math.Abs(inStart.Y - inEnd.Y);

            if (formula == 0)
                return Math.Sqrt(dx * dx + dy * dy); //Euclidean distance

            else if (formula == 1)
                return (dx * dx + dy * dy); //Euclidean distance squared

            else if (formula == 2)
                return Math.Min(dx, dy); //Diagonal distance

            else if (formula == 3)
                return (dx * dy) + (dx + dy); //Manhatten distance



            else
                return Math.Abs(inStart.X - inEnd.X) + Math.Abs(inStart.Y - inEnd.Y);

            //return 1*(Math.Abs(inStart.X - inEnd.X) + Math.Abs(inStart.Y - inEnd.Y) - 1); //optimized tile based Manhatten
            //return ((dx * dx) + (dy * dy)); //Khawaja distance
        }

        protected override Double NeighborDistance(PathNode inStart, PathNode inEnd)
        {
            return Heuristic(inStart, inEnd);
        }

        public MySolver(TPathNode[,] inGrid)
            : base(inGrid)
        {
        }
    } 
       
    void Start()
    {
        map = GameObject.Find("Bus"); // получение ссылки на автобус, к которому прикреплен скрипт
        LevelSettings = map.GetComponent<Level>(); //получение ссылки на скрипт
       // ExitPoint = LevelSettings.getEntryPoints(); //получение точек выхода

        Coords = gameObject.GetComponent<Transform>();// получение текущих координат
               
        //новое
        currentPosition = getCurrentPosition(Coords); // сохраняем текущую позицию
        currentDirect = Vector2.down;
       // generatePath();//генерируем путь

        MySolver<MyPathNode, System.Object> aStar = new MySolver<MyPathNode, System.Object>(LevelSettings.grid);
        LinkedList<MyPathNode> Newpath = aStar.Search(new Vector2(6, 3), new Vector2(0, Level.Y-1), null);
      
        generatePath(Newpath);//генерируем путь
    }
    private ArrayList generatePath(Vector2 startPosition, Vector2 finishPosition)
    {

        ArrayList path = new ArrayList();
        return path;
    }
    private int generatePath(LinkedList<MyPathNode> localpath)
    {
        foreach (MyPathNode tempstep in localpath)
        {
            path.Add(new Vector2(tempstep.X, tempstep.Y));
        }
        return path.Count;
    }
    private Vector2 getNewStep()
    {
        if (path.Count > 0)
        {
            Vector2 temp = (Vector2)path[0];
            path.RemoveAt(0);
            return temp;
        }
        else
            return Vector2.zero;
        
    }
    //получение текущих координат
    private Vector2 getCurrentPosition(Transform tempCoords)
    {
        return new Vector2((int)(tempCoords.position.x / LevelSettings.scale),(int)(tempCoords.position.y / LevelSettings.scale));
    }
    private void MoveToRoute()
    {

       if(MoveBool)
       {
           MoveBool = MoveToNewPoint(nextPosition);
       }
       else
       {
           nextPosition = getNewStep();
           if (nextPosition != Vector2.zero)
           {
               RotateTo(AngleForRotate(currentPosition, nextPosition , currentDirect));
               MoveBool = true;
           }
           
       }
    }
    private bool MoveToNewPoint(Vector2 newPosition, int step = 1, double error = 0.01)
    {
        //Идем к следующей точке
        tempstep += MoveStep(Vector2.down,speed);
        //если мы ушли из начальной точки
        //если мы подошли к новой точке
        if ((step * LevelSettings.scale - tempstep < 0.1)&&(Math.Abs(nextPosition.x - (Coords.position.x / LevelSettings.scale)) < error) & (Math.Abs(nextPosition.y - (Coords.position.y / LevelSettings.scale)) < error))
        {
            currentDirect = nextPosition - currentPosition;
            currentPosition = nextPosition;
            tempstep = 0;
            return false;
        }
        return true;
    }
    private float MoveStep(Vector2 direction, float speed)
    {
        Coords.Translate(direction * speed * Time.deltaTime);
        return speed * Time.deltaTime;
    }
  
/*
   void MoveTo(Vector2 direction, int step=1, double error = 0.01)
    {
        
        tempstep += MoveStep(direction, speed);
       
        if ((step * LevelSettings.scale - tempstep <0.1)&&(Math.Abs(nextPosition.x - (Coords.position.x / LevelSettings.scale))<error) & (Math.Abs(nextPosition.y - (Coords.position.y / LevelSettings.scale))<error))
        {
            currentDirect = nextPosition - currentPosition;
            currentPosition = nextPosition;
            nextPosition = getNewStep();
            
            oldmove = false;
            tempstep = 0;
        }
       

    }*/
      
    //уничтожение объектов 
  /*  private void checkAndDestroy()
    {
        for (var i = 0; i < ExitPoint.length; i++)
        {
            if ((((Vector2)ExitPoint[i]).x == getCurrentPosition().x) && (((Vector2)ExitPoint[i]).y == getCurrentPosition().y))
            {
                Destroy(Coords.gameObject);
                LevelSettings.deletePass();
            }
        }
    }*/
    
    private float AngleForRotate(Vector2 curPoint, Vector2 nextPoint, Vector2 curDirect)
    {
        Vector2 nextDirect = nextPoint - curPoint;
        return Mathf.Rad2Deg*(Mathf.Atan2(nextDirect.y, nextDirect.x) - Mathf.Atan2(curDirect.y, curDirect.x));
    }
    void RotateTo(float Angle)
    {
        transform.Rotate(Vector3.forward, Angle);
    }
    /*void ChangeHead()
    {
        SpriteRenderer sp = new SpriteRenderer();
        sp.material.SetTexture("Left", new Texture());

    }*/
    void Update()
    {
   
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveToRoute();
        
            /*
        else
        {
            RotateTo(AngleForRotate(currentPosition, nextPosition,currentDirect));
            oldmove = !oldmove;
        }*/
       /*
          if (Input.GetMouseButtonDown(0))
              Debug.Log("Pressed left click.");

          if (Input.GetMouseButtonDown(1))
              Debug.Log("Pressed right click.");

          if (Input.GetMouseButtonDown(2))
              Debug.Log("Pressed middle click.");

          */

        /* if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
         {
             rb.AddRelativeForce(Vector2.up);
             //rb.AddForce(Vector2.up *speed);

             // pass.rigidbody2D pass.transform.position += pass.transform.forward * speed * Time.deltaTime; 
         }
         if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
         {
           
         }
         if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
         {
             pass.transform.Rotate(Vector3.down * speedRotation);
         }
         if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
         {
             pass.transform.Rotate(Vector3.up * speedRotation);
         }
         if (Input.GetKeyDown(KeyCode.Space))
         {
             pass.transform.position += pass.transform.up * jumpSpeed * Time.deltaTime;
         }*/
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        //pass.transform.Rotate(0, 0, 89);
        //  pass.transform.Rotate(Vector3.forward* 90);

        //pass.transform.RotateAroundLocal(new Vector3(0,0,0), 89);
        //pass.transform.Rotate(Vector3.one);
        /*
        
        Quaternion target = Quaternion.Euler(0, toAngle, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);
        */
        // Debug.Log("trigger");
    }
    void OnCollisionEnter2D(Collision2D coll)
    {
        /*pass.transform.Rotate(0, 0, 10);
        Debug.Log("coll");
        /*
        if (coll.gameObject.tag == "Enemy")
            coll.gameObject.SendMessage("ApplyDamage", 10);
        */
    }
    public class gridPosition
    {
        public int x = 0;
        public int y = 0;

        public gridPosition()
        {
        }

        public gridPosition(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    };
}
