using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityScript.Lang;

public class Level : MonoBehaviour
{
    public MyPathNode[,] grid;

    //public int enemyNumber = 6, playerNumber = 3; //public для возможности редактировать через инспектор 
    //public static int X, Y; // ширина и высота поля
    public static int Width = 8;
    public static int Height = 14;
    
    public int mapScale = 128; // количество пикселей в одной клетке
   // private int height = 14; // количество клеток в высоту
   // private int width = 7; // количество клеток в ширину
    public int scale = 2; // масштаб для стульев, позднее заменить и использовать mapscale
    private int maxcount = 25; // максимальное количество пассажиров
    private int current = 0; // текущее количество пассажиров
    public Sprite[] headsprites; // массив голов
    public Sprite[] bodysprites; // массив туловищ
    public Sprite[] footsprites; // массив ног
    public Passenger[] passList; // массив всех пассажиров

    //public GameObject sprite;
    public Transform brick;
    public Transform chair;
    public Transform floor;
    public Transform person;
    public Transform back;
    public Array entryPoints = new Array();
    public ArrayList ExitPoint;//точки выхода, в которых объект уничтожается

    public static bool ready = false; //мы не сможем начать искать путь, пока не разместим юнитов на поле.

    //1-непроходимая точка
    public static int[,] mapArrTemp = {
      {1,1,1,1,1,1,1,1},
      {1,2,2,0,2,2,1,4},
      {1,2,2,0,0,0,0,4},
      {1,2,2,0,2,2,1,4},
      {1,2,2,0,2,2,1,4},
      {1,2,2,0,2,2,1,4},
      {1,2,2,0,2,2,1,4},
      {1,2,2,0,2,2,1,4},
      {1,2,2,0,0,0,0,4},
      {1,2,2,2,2,2,1,4},
      {1,1,1,1,1,1,1,4},
      {1,1,1,1,1,1,1,4},
      {1,1,1,1,1,1,1,5},
      {1,1,1,1,1,1,1,1},
};

    public int[,] Map = new int[Width, Height]; // массив для загрузки уровня

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
            }
        }



       // addNewPass();
        ready = true; // можем начинать искать путь

    }
    void LoadSourcePassenger()
    {
        headsprites = Resources.LoadAll<Sprite>("head_m");  //Resources.LoadAll<Sprite>("Sprites");
        bodysprites = Resources.LoadAll<Sprite>("body_m");  //Resources.LoadAll<Sprite>("Sprites");
        footsprites = Resources.LoadAll<Sprite>("foot_m");  //Resources.LoadAll<Sprite>("Sprites");
        Debug.Log("Загружено голов {0}, тел {1}, ног {2}");
        Debug.Log("Длина голов, тела, ног "+ headsprites.Length + " " + bodysprites.Length + " " + footsprites.Length);
        //Resources.Load <Sprite> ("Sprites/Graphics_3");
    }

    public void generateNewPassenger()
    {
        person.GetComponentsInChildren<SpriteRenderer>()[0].sprite = headsprites[Random.Range(0, headsprites.Length)];
        person.GetComponentsInChildren<SpriteRenderer>()[1].sprite = bodysprites[Random.Range(0, bodysprites.Length)];
        person.GetComponentsInChildren<SpriteRenderer>()[2].sprite = footsprites[Random.Range(0, footsprites.Length)];
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
                //Debug.Log("X="+x +"Y="+y);
                //Debug.Log(mapArr[y, x]);
                
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
        var tempcoord = (Vector2)entryPoints[Random.Range(0, entryPoints.length)];
        generateNewPassenger();//присвоение текстур
        Instantiate(person, new Vector3(tempcoord.x * scale, tempcoord.y * scale, 0), Quaternion.identity);
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
            for (var i = 0; i < Random.Range(0, maxcount - current); i++)
            {
                addNewPass();
                current++;
            }
        }

    }
    public void deletePass()
    {
        current--;
    }


   
}


