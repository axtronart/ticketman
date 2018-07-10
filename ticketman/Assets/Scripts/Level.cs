using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityScript.Lang;

public class Level : MonoBehaviour
{
    public int mapScale = 128; // количество пикселей в одной клетке
    private int height = 14; // количество клеток в высоту
    private int width = 7; // количество клеток в ширину
    public int scale = 2; // масштаб для стульев, позднее заменить и использовать mapscale
    private int maxcount = 10; // максимальное количество пассажиров
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

    private int[,] mapArr = {
      {1,1,1,1,1,1,1},
      {1,2,2,0,2,2,1},
      {1,2,2,0,0,0,3},
      {1,2,2,0,2,2,1},
      {1,2,2,0,2,2,1},
      {1,2,2,0,2,2,1},
      {1,2,2,0,2,2,1},
      {1,2,2,0,2,2,1},
      {1,2,2,0,0,0,1},
      {1,2,2,2,2,2,1},
      {1,1,1,1,1,1,1},
      {1,1,1,1,1,1,1},
      {1,1,1,1,1,1,1},
      {1,1,1,1,1,1,1},
};
    // Use this for initialization
    void Start()
    {
        //Получение точек, в которых будут появляться пассажиры
        entryPoints = getEntryPoints();
        //создание внутренностей автобуса
        createBusFromMap();
        //запуск функции добавления пассажиров
        InvokeRepeating("busstation", 0, 5);
        //загрузка ресурсов 
        LoadSourcePassenger();

    }
    void LoadSourcePassenger()
    {
        headsprites = Resources.LoadAll<Sprite>("head_m");  //Resources.LoadAll<Sprite>("Sprites");
        bodysprites = Resources.LoadAll<Sprite>("body_m");  //Resources.LoadAll<Sprite>("Sprites");
        footsprites = Resources.LoadAll<Sprite>("foot_m");  //Resources.LoadAll<Sprite>("Sprites");
        Debug.Log("Загружено голов {0}, тел {1}, ног {2}");
        Debug.Log(headsprites.Length + " " + bodysprites.Length + " " + footsprites.Length);
        //Resources.Load <Sprite> ("Sprites/Graphics_3");
    }

    public void generateNewPassenger()
    {
        person.GetComponentsInChildren<SpriteRenderer>()[0].sprite = headsprites[Random.Range(0, headsprites.Length)];
        person.GetComponentsInChildren<SpriteRenderer>()[1].sprite = bodysprites[Random.Range(0, bodysprites.Length)];
        person.GetComponentsInChildren<SpriteRenderer>()[2].sprite = footsprites[Random.Range(0, footsprites.Length)];
    }

    public Passenger[] generateListPassenger()
    {
        passList = new Passenger[10];
        for (var i = 0; i < 10; i++)
        {

        }
        return passList;
    }

    

    void createBusFromMap()
    {
        //Здесь происходит первоначальная отрисовка матрицы автобуса
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (mapArr[y, x] == 0)
                {
                    Instantiate(brick, new Vector3(x * scale, y * scale, 0), Quaternion.identity);
                }
                else
                    if (mapArr[y, x] == 1)
                    {
                        Instantiate(floor, new Vector3(x * scale, y * scale, 0), Quaternion.identity);
                    }
                    else
                        if (mapArr[y, x] == 2)
                        {
                            Instantiate(chair, new Vector3(x * scale, y * scale, 0), Quaternion.identity);
                        }
                        else
                            Instantiate(back, new Vector3(x * scale, y * scale, 0), Quaternion.identity);
                //     Debug.Log("Xp = " + x * scale);
                //     Debug.Log("Yp = " + y * scale);


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
    public Array getEntryPoints()
    {
        var arr = new Array();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (mapArr[y, x] == 3)
                {
                    var vec = new Vector2(x, y);
                    arr.Add(vec);
                }
            }
        }
        return arr;
    }

    private void busstation()
    {
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


