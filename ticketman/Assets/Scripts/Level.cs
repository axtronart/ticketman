using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityScript.Lang;
using System;

public class Level : MonoBehaviour
{
    public bool isMoving = false; //можно ли двигаться персонажам
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
    private int current = 0; // текущее количество пассажиров
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
    public Transform person;
    public Transform back;
    public ArrayList entryPoints;
    public ArrayList ExitPoint;//точки выхода, в которых объект уничтожается

    public static bool ready = false; //мы не сможем начать искать путь, пока не разместим юнитов на поле.

    //1-непроходимая точка
    public static int[,] mapArrTemp = {
      {1,1,1,1,1,1,1,4,4},
      {1,2,2,0,2,2,1,4,4},
      {1,2,2,0,2,2,0,4,4},
      {1,2,2,0,0,0,0,4,4},
      {1,2,2,0,2,2,1,1,1},
      {1,2,2,0,2,2,1,4,4},
      {1,2,2,0,2,2,1,4,4},
      {1,2,2,0,2,2,1,4,4},
      {1,2,2,0,2,2,0,4,4},
      {1,2,2,0,0,0,0,4,4},
      {1,2,2,0,0,0,1,1,1},
      {1,2,2,0,0,0,1,4,4},
      {1,2,2,0,0,0,0,4,4},
      {1,2,2,2,2,2,0,4,4},
      {1,1,1,1,1,1,1,4,5},
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
        //Получение точек, в которых будут появляться пассажиры
        entryPoints = getEntryPoints();

        ExitPoint = getExitPoints(); //получение точек выхода

        Debug.Log("entrypoints" + entryPoints);
                
        //создание внутренностей автобуса
        createBusFromMap();
        //запуск функции добавления пассажиров
        InvokeRepeating("busstation", 0, 5);// закомментировал для отладки пути
        //загрузка ресурсов 
        LoadSourcePassenger();

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

                if ((Map[x, y] != 1)&&(Map[x, y] != 4))
                {
                    target.Add(grid[x, y]);
                  //  targetForNextStep.Add(grid[x, y]);
                }
                    

            }
        }

       // addNewPass();
        ready = true; // можем начинать искать путь

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

    
    void OnGUI()
    {
        var style = new GUIStyle();
        style.fontSize = 100;
        style.normal.textColor = Color.red;
        style.fontStyle = FontStyle.Bold;
        

        if (GUI.Button(new Rect(0f, 100f, 200f,200f), "Create Enemy"))
        {
            Debug.Log("create");
            addNewPass();
        }
        if (GUI.Button(new Rect(0f, 300f, 200f, 200f), "Move"))
        {
            Debug.Log("move");
            isMoving = !isMoving;
        }
        GUI.Label(new Rect(0f, 300f, 200f, 200f), current.ToString(),style);
        GUI.Label(new Rect(10, 10, 100, 20), maxcount.ToString(),style);
        
        
        
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
  /*  public void generateNewPassenger(bool isMan)
    {

        person.GetComponentsInChildren<SpriteRenderer>()[0].sprite = headmsprites[UnityEngine.Random.Range(0, headmsprites.Length)];
       // person.GetComponentsInChildren<SpriteRenderer>()[0].sharedMaterial.color = Color.red;
        person.GetComponentsInChildren<SpriteRenderer>()[1].sprite = bodymsprites[UnityEngine.Random.Range(0, bodymsprites.Length)];
        person.GetComponentsInChildren<SpriteRenderer>()[2].sprite = footmsprites[UnityEngine.Random.Range(0, footmsprites.Length)];
        
    }
/*
    public Passenger[] generateListPassenger()
    {
        passList = new Passenger[10];
        for (var i = 0; i < 10; i++)
        {

        }
        return passList;
    }*/

    

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
                               Instantiate(floor, new Vector2(x * scale, y * scale), Quaternion.identity);
                           }
                           else
                               if (Map[x, y] == 2)
                               {
                                   Instantiate(chair, new Vector2(x * scale, y * scale), Quaternion.identity);
                               }
                               else
                                   Instantiate(chair, new Vector2(x * scale, y * scale), Quaternion.identity);
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
            Instantiate(person, new Vector3(tempcoord.x * scale, tempcoord.y * scale, 0), Quaternion.identity);
            current++;
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

    //Добавление новых персонажей
    private void busstation()
    {
        Debug.Log("busstation");
        if (maxcount > current)
        {
            for (var i = 0; i < UnityEngine.Random.Range(0, maxcount - current); i++)
            {
                addNewPass();
                
            }
        }

    }
    public void deletePass()
    {
        current--;
    }
}


