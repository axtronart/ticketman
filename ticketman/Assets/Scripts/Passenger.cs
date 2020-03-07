using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Passenger : MonoBehaviour
{
    //старое
    public GameObject map;
    Level LevelSettings;
    public float speed;
  
    public Transform Coords; //текущие координаты
    public Animator PassAnim;
    public bool MoveBool = false;
    public bool isMoving = false;
   

    private float tempstep = 0;
    private ArrayList path = new ArrayList();
    private Vector2 currentPosition;
    private Vector2 nextPosition;
    private Vector2 currentDirect;
    private bool isActive = false;
    int countOfStand = 0;
    int maxStepsInRoute = 20; //Максимальное количество клеток, которое должен пройти пассажир, прежде чем идти на выход
    int minStepsInRoute = 5; //Минимальное количество клеток, которое должен пройти пассажир , прежде чем идти на выход

    bool isTicket = false;
    public bool isDelete = false;

    bool isGoal = false; // пассажир пришел;

   
       
    void Start()
    {
        map = GameObject.Find("Level"); // получение ссылки на автобус, к которому прикреплен скрипт
        LevelSettings = map.GetComponent<Level>(); //получение ссылки на скрипт
      
        Coords = gameObject.GetComponent<Transform>();// получение текущих координат
        Coords.transform.localScale=new Vector3(1.5f,1.5f,1.5f);

        PassAnim = gameObject.GetComponent<Animator>();
        
               
        //новое
        currentPosition = getCurrentPosition(Coords); // сохраняем текущую позицию
        currentDirect = Vector2.down;
       //generatePath();//генерируем путь

        LevelSettings.grid[(int)currentPosition.x,(int)currentPosition.y].IsWall = true;

        path = LevelSettings.generateNewPath(currentPosition, LevelSettings.generatePosition());

        speed = 4.0f;
        MoveBool = false;
        isGoal = false;
        
        //countOfStand = 
        
       // UnityEngine.Random();

        /*
        Debug.Log("Текущая позиция"+ currentPosition);
        foreach (Vector2 temp in path)
        {
            Debug.Log("ПУТЬ" + temp);
        }*/
     
    }
    
    private Vector2 getNewStep()
    {
        if (path.Count > 0)
        {
            Vector2 temp = (Vector2)path[0];
            if  (LevelSettings.grid[(int)temp.x,(int)temp.y].IsWall)
            {
                return Vector2.zero;
            }
            else
            {
                path.RemoveAt(0);
                return temp;
            }
        }
        else
            return Vector2.zero;
    }
    //получение текущих координат
    private Vector2 getCurrentPosition(Transform tempCoords)
    {
        return new Vector2((int)(tempCoords.position.x / LevelSettings.scale),(int)(tempCoords.position.y / LevelSettings.scale));
    }
    public Vector2 getCurrentPosition()
    {
       // Debug.Log("Текущая позиция X "+ currentPosition.x+"  y= "+currentPosition.y);
        return currentPosition;
    }
    public bool setPath(ArrayList p)
    {
        path = p;
        return true;
    }

    /*
    public bool MoveToPath(ArrayList path) //если путь закончился false
    {
        PassAnim.SetBool("isMove", MoveBool);
        if (MoveBool)// движение между точками , чтобы персонажи не зависали между клетками
        {
            MoveBool = MoveToNewPoint(nextPosition);
        }
        else
        {
            if (LevelSettings.isMoving)// можно ли начинать новый шаг
            {
                
                nextPosition = getNewStep();
                
                if (nextPosition != Vector2.zero)
                {
                    RotateTo(AngleForRotate(currentPosition, nextPosition, currentDirect));
                    MoveBool = true;
                    LevelSettings.grid[(int)currentPosition.x, (int)currentPosition.y].IsWall = false;
                    LevelSettings.grid[(int)nextPosition.x, (int)nextPosition.y].IsWall = true;
                }
                else
                    return true;
            }    
        }

        return false;
    }*/
    
            
    public bool MoveToRoute()
    {
        PassAnim.SetBool("isMove", MoveBool);
        if (MoveBool)// движение между точками , чтобы персонажи не зависали между клетками
        {
            return MoveBool = MoveToNewPoint(nextPosition);
        }
        else
        {
            if (isMoving)// можно ли начинать новый шаг
            {

                nextPosition = getNewStep();

                if (nextPosition != Vector2.zero)
                {
                    RotateTo(AngleForRotate(currentPosition, nextPosition, currentDirect));
                    MoveBool = true;
                    LevelSettings.grid[(int)currentPosition.x, (int)currentPosition.y].IsWall = false;
                    LevelSettings.grid[(int)nextPosition.x, (int)nextPosition.y].IsWall = true;
                }
                else
                {
                    
                    /*if (countOfStand > 1)//чтоб пассажиры не бежали сразу на выход
                    {
                        isGoal = true;
                        
                        path = LevelSettings.generateNewPath(currentPosition, LevelSettings.generateExitPoint());
                        countOfStand = 0;
                    }
                    else
                    {
                        path = LevelSettings.generateNewPath(currentPosition, LevelSettings.generatePosition());
                        countOfStand++;
                    }*/
                }
                
            }
            return false;
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
       // Debug.Log("speed*Time.deltatime" + speed * Time.deltaTime);
       // Debug.Log("speed*Time.deltatime*10" + 10*speed * Time.deltaTime);
        return speed * Time.deltaTime;
    }

    public bool inBus() // функция проверки каждого персонажа в автобусе
    {
        return currentPosition.x > 2;
    }
  
    
    //уничтожение объектов 
   private void checkAndDestroy()
    {
       float error = 0.01f;
       for (int i = 0; i < LevelSettings.exitPoints.Count;i++ )
       {
           //
           if ((Math.Abs(currentPosition.x - ((Vector2)(LevelSettings.exitPoints[i])).x) < error) & (Math.Abs(currentPosition.y - ((Vector2)(LevelSettings.exitPoints[i])).y) < error))
            {
                isDelete = true;
                //annigilation();
            }
        }
    }

    public void annigilation() // все операции связанные с удалением текущего объекта
    {
        Destroy(Coords.gameObject);
        Debug.Log("удаление пассажира");
        
    }
    
    
    private float AngleForRotate(Vector2 curPoint, Vector2 nextPoint, Vector2 curDirect)
    {
        Vector2 nextDirect = nextPoint - curPoint;
        return Mathf.Rad2Deg*(Mathf.Atan2(nextDirect.y, nextDirect.x) - Mathf.Atan2(curDirect.y, curDirect.x));
    }
    void RotateTo(float Angle)
    {
        transform.Rotate(Vector3.forward, Angle);
    }
  
    void Update()
    {
   
    }
    void OnMouseDown()
    {
        isActive = !isActive;

        if (isActive)
        {
            this.GetComponentsInChildren<SpriteRenderer>()[0].material.color = new Color(0.70f, 0.70f, 0.70f);
            this.GetComponentsInChildren<SpriteRenderer>()[1].material.color = new Color(0.70f, 0.70f, 0.70f);
            this.GetComponentsInChildren<SpriteRenderer>()[2].material.color = new Color(0.70f, 0.70f, 0.70f);
        }
        else
        {
            this.GetComponentsInChildren<SpriteRenderer>()[0].material.color = Color.white;
            this.GetComponentsInChildren<SpriteRenderer>()[1].material.color = Color.white;
            this.GetComponentsInChildren<SpriteRenderer>()[2].material.color = Color.white;
        }


        if (!isTicket)
        {
            LevelSettings.money++;
            isTicket = true;
        }else
        if (isTicket)
        {
            LevelSettings.money--;
        }
     
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveToRoute();
        checkAndDestroy();
        
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
   
}
