using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Passenger : MonoBehaviour
{
    //старое
    public GameObject map;
    Level LevelSettings;
    public float speed = 2000.0f;
    public Transform Coords; //текущие координаты
    bool MoveBool = false;
   
    private float tempstep = 0;
    private ArrayList path = new ArrayList();
    private Vector2 currentPosition;
    private Vector2 nextPosition;
    private Vector2 currentDirect;
    private bool isActive = false;
    public bool inBus = false;
    int countOfStand = 0;

   
       
    void Start()
    {
        map = GameObject.Find("Bus"); // получение ссылки на автобус, к которому прикреплен скрипт
        LevelSettings = map.GetComponent<Level>(); //получение ссылки на скрипт
      
        Coords = gameObject.GetComponent<Transform>();// получение текущих координат
        Coords.transform.localScale=new Vector3(2,2,2);
               
        //новое
        currentPosition = getCurrentPosition(Coords); // сохраняем текущую позицию
        currentDirect = Vector2.down;
       // generatePath();//генерируем путь

        LevelSettings.grid[(int)currentPosition.x,(int)currentPosition.y].IsWall = true;

        path = LevelSettings.generateNewPath(currentPosition, LevelSettings.generatePosition());

        Debug.Log("Текущая позиция"+ currentPosition);
        foreach (Vector2 temp in path)
        {
            Debug.Log("ПУТЬ" + temp);
        }
     
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
    private void MoveToRoute()
    {
        if(MoveBool)// движение между точками , чтобы персонажи не зависали между клетками
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
                {
                    if (countOfStand >4)//чтоб пассажиры не бежали сразу на выход
                    {
                        path = LevelSettings.generateNewPath(currentPosition, LevelSettings.generateExitPoint());
                        countOfStand = 0;
                    }
                    else
                    {
                        path = LevelSettings.generateNewPath(currentPosition, LevelSettings.generatePosition());
                        countOfStand++;
                    }
                }
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
  
    
    //уничтожение объектов 
   private void checkAndDestroy()
    {
       float error = 0.01f;
       for (int i = 0; i < LevelSettings.exitPoints.Count;i++ )
       {
           if ((Math.Abs(currentPosition.x - ((Vector2)(LevelSettings.exitPoints[i])).x) < error) & (Math.Abs(currentPosition.y - ((Vector2)(LevelSettings.exitPoints[i])).y) < error))
            //if ((currentPosition.x == ((Vector2)(ExitPoint[0])).x) && (currentPosition.y == ((Vector2)(ExitPoint[0])).y))
            {
                Destroy(Coords.gameObject);
                LevelSettings.deletePass();
                Debug.Log("удаление пассажира");
                LevelSettings.grid[(int)currentPosition.x, (int)currentPosition.y].IsWall = false;

            }
        }
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
