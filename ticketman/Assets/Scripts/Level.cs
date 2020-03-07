using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using System;

public class Level : MonoBehaviour
{
    public int money; //количество денег
    public int fuel; // количество бензина
    public Animator road;
    public Animator busstop;
    public bool isMoving; //можно ли двигаться персонажам
    public MyPathNode[,] grid;
    public List<MyPathNode> target;// массив точек для конечного пункта
    //public List<MyPathNode> targetForNextStep;// массив точек для конечного пункта
    //public int enemyNumber = 6, playerNumber = 3; //public для возможности редактировать через инспектор 
    //public static int X, Y; // ширина и высота поля
    public static int Width = 9;
    public static int Height = 15;
    
    public int mapScale = 128; // количество пикселей в одной клетке
   // private int height = 14; // количество клеток в высоту
   // private int width = 7; // количество клеток в ширину
    public int scale = 2; // масштаб для стульев, позднее заменить и использовать mapscale
    private int maxcount = 25; // максимальное количество пассажиров
  
    public Sprite[] headmsprites; // массив голов
    public Sprite[] bodymsprites; // массив туловищ
    public Sprite[] footmsprites; // массив ног
    public Sprite[] headwsprites; // массив голов
    public Sprite[] bodywsprites; // массив туловищ
    public Sprite[] footwsprites; // массив ног
    //public Passenger[] passList; // массив всех пассажиров

    //public GameObject sprite;
    public Transform brick;
    public Transform chair;
    public Transform floor;
    public Passenger person;
    public Transform back;
    public ArrayList entryPoints;
    public ArrayList exitPoints;//точки выхода, в которых объект уничтожается

    public ArrayList PassengerList;

    public bool isStation = true; // переменная указывает что автобус стоит на станции 
  
    //1-непроходимая точка
    public static int[,] mapArrTemp = {
      {1,1,1,1,1,1,1,5,5},
      {1,2,2,0,2,2,1,4,4},
      {1,2,2,0,2,2,0,4,4},
      {1,2,2,0,0,0,0,4,4},
      {1,2,2,0,2,2,1,4,4},
      {1,2,2,0,2,2,1,4,4},
      {1,2,2,0,2,2,1,4,4},
      {1,2,2,0,2,2,1,4,4},
      {1,2,2,0,2,2,0,4,4},
      {1,2,2,0,0,0,0,4,4},
      {1,2,2,0,0,0,1,4,4},
      {1,2,2,0,0,0,1,4,4},
      {1,2,2,0,0,0,0,4,4},
      {1,2,2,2,2,2,0,4,4},
      {1,1,1,1,1,1,1,5,5},
};

    public int[,] Map = new int[Width, Height]; // массив для загрузки уровня


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
    // Use this for initialization
    void Start()
    {
        money = 100;
        fuel = 100;
        //загрузка ресурсов 
        LoadSourcePassenger();
        //создание внутренностей автобуса
       // createBusFromMap();
        //создание матрицы автобуса
        createPathMatrix();
        //получение точек, в которых будут появляться пассажиры
        entryPoints = getEntryPoints();
        //получение точек выхода
        exitPoints = getExitPoints();
        //запуск функции добавления пассажиров
        PassengerList = new ArrayList();
        //InvokeRepeating("busstation", 0, 5);// закомментировал для отладки пути
        road = GameObject.Find("road").GetComponent<Animator>();
        
        Invoke("startingBus",1);
        
       // InvokeRepeating("boardingBus", 5, 10);// закомментировал для отладки пути
        
        //InvokeRepeating("movingBus", 15, 10);
        
    }

    public void startingBus()
    {
        isMoving = true;
        isStation = false;
        Invoke("movingBus", 1);
    }
    public void movingBus()
    {
        isStation = false;
        //road.SetInteger("State", 2);
        Invoke("boardingBus", 5);
    }

    public void boardingBus()
    {
        
        isMoving = true;
        isStation = true;
        road.SetInteger("State", 1);
        fuel -= 5;
        Invoke("boardingPass", 2);
    }

    public void boardingPass()
    {
        if (maxcount > PassengerList.Count)
        {
            for (var i = 0; i < UnityEngine.Random.Range(0, maxcount - PassengerList.Count); i++)//закомментировал, пока добавляется только один человек
            {
                addNewPass();
            }
        }
        
    }
    
  
    void FixedUpdate()
    {
       
    }

    public void createPathMatrix()
    {
        //Generate a grid - nodes according to the specified size
        grid = new MyPathNode[Width, Height];
        target = new List<MyPathNode>();
        // targetForNextStep = new List<MyPathNode>();
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                //Boolean isWall = ((y % 2) != 0) && (rnd.Next (0, 10) != 8);
                // bool isWall = false;
                grid[x, y] = new MyPathNode()
                {
                    IsWall = (Map[x, y] == 1),
                    X = x,
                    Y = y,
                };

                if ((Map[x, y] != 1) && (Map[x, y] != 4))
                {
                    target.Add(grid[x, y]);
                    //  targetForNextStep.Add(grid[x, y]);
                }
            }
        }
    }

    public ArrayList generateNewPath(Vector2 startPos, Vector2 endPos)
    {
        MySolver<MyPathNode, System.Object> aStar = new MySolver<MyPathNode, System.Object>(grid);
        LinkedList<MyPathNode> Newpath = aStar.Search(startPos, endPos, null);
        ArrayList path = new ArrayList();

        if (Newpath != null)
        {
            foreach (MyPathNode tempstep in Newpath)
            {
                path.Add(new Vector2(tempstep.X, tempstep.Y));
            }
            path.RemoveAt(0);//удаляем первый элемент, потому что он равен текущей позиции
        }
        else
            path.Add(Vector2.zero);
        return path;
    }

    public Vector2 generatePosition()//массив точек куда могут вообще сесть пассажиры
    {
        var temp = UnityEngine.Random.Range(0, target.Count);
        var tempV = new Vector2(target[temp].X, target[temp].Y);
       // target.RemoveAt(temp);
        return tempV;
    }

    public Vector2 generateExitPoint()
    {
        var temp = UnityEngine.Random.Range(0, exitPoints.Count);
        return (Vector2)exitPoints[temp];
    }

    
    void OnGUI()
    {
        //Переменные для размещения меню
        var style = new GUIStyle();
        style.fontSize = 70;
        style.normal.textColor = Color.red;
        style.fontStyle = FontStyle.Bold;

        float X = 0;
        float Y = 200;
        float width = Y;
        float height = 200;


        GUI.Label(new Rect(X, Y * 0, width, height), "Сейчас = " + PassengerList.Count.ToString(), style);
        GUI.Label(new Rect(X, Y * 1, width, height), "Всего = " + maxcount.ToString(), style);
        GUI.Label(new Rect(X, Y * 2, width, height), "Денег = " + money.ToString(), style);
        GUI.Label(new Rect(X, Y * 3, width, height), "Бензин = " + fuel.ToString(), style);



        if (GUI.Button(new Rect(X, Y * 4, width, height), "Добавить пассажира", style))
        {
            Debug.Log("create");
            addNewPass();
        }
        if (GUI.Button(new Rect(X, Y * 5, width, height), "Move = " + isMoving.ToString(), style))
        {
            Debug.Log("move");
            isMoving = !isMoving;
        }

        GUI.Label(new Rect(X, Y * 6, width, height), "В автобусе = " + numInBus().ToString(), style);
        GUI.Label(new Rect(X, Y * 7, width, height), "В движении = " + numInMoving().ToString(), style);
    }


    void LoadSourcePassenger()
    {
        headmsprites = Resources.LoadAll<Sprite>("head_m");  //Resources.LoadAll<Sprite>("Sprites");
        bodymsprites = Resources.LoadAll<Sprite>("body_m");  //Resources.LoadAll<Sprite>("Sprites");
        footmsprites = Resources.LoadAll<Sprite>("foot_m");  //Resources.LoadAll<Sprite>("Sprites");

        headwsprites = Resources.LoadAll<Sprite>("head_w");  //Resources.LoadAll<Sprite>("Sprites");
        bodywsprites = Resources.LoadAll<Sprite>("body_w");  //Resources.LoadAll<Sprite>("Sprites");
        footwsprites = Resources.LoadAll<Sprite>("foot_w");  //Resources.LoadAll<Sprite>("Sprites");
        Debug.Log("Загружено голов {0}, тел {1}, ног {2}");
        Debug.Log("Длина голов, тела, ног мужики"+ headmsprites.Length + " " + bodymsprites.Length + " " + footmsprites.Length);
        Debug.Log("Длина голов, тела, ног женщины" + headwsprites.Length + " " + bodywsprites.Length + " " + footwsprites.Length);
        //Resources.Load <Sprite> ("Sprites/Graphics_3");
    }
    // вариант рабочий надо будет раскомментировать если не получится с цветами
    public void generateNewPassenger(bool isMan)
    {
        if (isMan)
        {
            person.GetComponentsInChildren<SpriteRenderer>()[0].sprite = headmsprites[UnityEngine.Random.Range(0, headmsprites.Length)];
            //person.GetComponentsInChildren<SpriteRenderer>()[0].sharedMaterial.color = Color.blue;
            person.GetComponentsInChildren<SpriteRenderer>()[1].sprite = bodymsprites[UnityEngine.Random.Range(0, bodymsprites.Length)];
            person.GetComponentsInChildren<SpriteRenderer>()[2].sprite = footmsprites[UnityEngine.Random.Range(0, footmsprites.Length)];
        }
        else
        {
            person.GetComponentsInChildren<SpriteRenderer>()[0].sprite = headwsprites[UnityEngine.Random.Range(0, headwsprites.Length)];
            person.GetComponentsInChildren<SpriteRenderer>()[1].sprite = bodywsprites[UnityEngine.Random.Range(0, bodywsprites.Length)];
            person.GetComponentsInChildren<SpriteRenderer>()[2].sprite = footwsprites[UnityEngine.Random.Range(0, footwsprites.Length)];
        }
    }
  
    void createBusFromMap()
    {
        //Здесь происходит первоначальная отрисовка матрицы автобуса
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)    
            {
                Map[x, y] = mapArrTemp[y, x];
                 
                if (Map[x, y] == 0)
                {
                    Instantiate(brick, new Vector2(x * scale, y * scale), Quaternion.identity);
                }
                       else
                           if (Map[x, y] == 1)
                           {
                               Instantiate(floor, new Vector3(x * scale, y * scale,-1), Quaternion.identity);
                           }
                           else
                               if (Map[x, y] == 2)
                               {
                                   Instantiate(chair, new Vector3(x * scale, y * scale,-1), Quaternion.identity);
                               }
            }
        }
    }

    //добавление пассажиров
    private void addNewPass()
    {
        var tempcoord = (Vector2)entryPoints[UnityEngine.Random.Range(0, entryPoints.Count)];
        if (!grid[(int)tempcoord.x,(int)tempcoord.y].IsWall) 
        {
            generateNewPassenger(UnityEngine.Random.Range(0, 2) == 0);//присвоение текстур
            PassengerList.Add(Instantiate(person, new Vector3(tempcoord.x * scale, tempcoord.y * scale, 0), Quaternion.identity));
        }
        
    }


    public ArrayList getEntryPoints()
    {
        var arr = new ArrayList();

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (mapArrTemp[y, x] == 4)
                {
                    var vec = new Vector2(x, y);
                    arr.Add(vec);
                }
            }
        }
        return arr;
    }
    public ArrayList getExitPoints()//точка 5, точка выхода
    {
        var arr = new ArrayList();

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (mapArrTemp[y, x] == 5)
                {
                    var vec = new Vector2(x, y);
                    arr.Add(vec);
                }
            }
        }
        return arr;
    }
    /*
    public void annigilation() // все операции связанные с удалением текущего объекта
    {
        foreach (Passenger p in PassengerList)
        {
            if (p.isDelete)
            {
                p.annigilation();
                PassengerList.Remove(p);

                grid[(int)p.currentPosition.x, (int)p.currentPosition.y].IsWall = false;
            }
        }
    }*/
    public int numInBus()
    {
        int inbus = 0;
        foreach (Passenger p in PassengerList)
        {
            if (p.getCurrentPosition().x < Width - 2)
            {
                inbus++;
            }
        }
        return inbus;
    }
    public int numInMoving()
    {
        int inMoving = 0;
        foreach (Passenger p in PassengerList)
        {
            if (p.MoveBool)
            {
                inMoving++;
            }
        }
        return inMoving;
    }


    private bool MovingPassenger()
    {
        foreach(Passenger p in PassengerList)
        {
            Vector2 temp = new Vector2();
            if (p.inBus())
            {
                temp = generateExitPoint();
            }
            else
            {
                temp = generateExitPoint();
            }
            var path = generateNewPath(p.getCurrentPosition(), temp);
            
            p.isMoving = p.setPath(path); 
        }
        return false;

    }
   
}


