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

        path = LevelSettings.generateNewPath(currentPosition,new Vector2(1, Level.Height - 6));

        Debug.Log("Текущая позиция"+ currentPosition);
        foreach (Vector2 temp in path)
        {
            Debug.Log("ПУТЬ" + temp);
        }
        //Debug.Log("ПУТЬ"+path);
   
     //   this.GetComponent<Renderer>().material.color = Color.white;
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
                }
                else
                {
                    path = LevelSettings.generateNewPath(currentPosition, (Vector2)LevelSettings.ExitPoint[0]);
                    Debug.Log("Новая Текущая позиция" + currentPosition);
                    foreach (Vector2 temp in path)
                    {
                        Debug.Log("Новый " + temp);
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
     //   Debug.Log("удаление пассажира"+Math.Abs(currentPosition.x - ((Vector2)(LevelSettings.ExitPoint[0])).x));
        
       if ((Math.Abs(currentPosition.x - ((Vector2)(LevelSettings.ExitPoint[0])).x) < 0.01) & (Math.Abs(currentPosition.y - ((Vector2)(LevelSettings.ExitPoint[0])).y) < 0.01))
       //if ((currentPosition.x == ((Vector2)(ExitPoint[0])).x) && (currentPosition.y == ((Vector2)(ExitPoint[0])).y))
       {
                Destroy(Coords.gameObject);
                LevelSettings.deletePass();
                Debug.Log("удаление пассажира");
       }
        /*for (var i = 0; i < ExitPoint.length; i++)
        {
            if ((((Vector2)ExitPoint[i]).x == getCurrentPosition().x) && (((Vector2)ExitPoint[i]).y == getCurrentPosition().y))
            {
                Destroy(Coords.gameObject);
                LevelSettings.deletePass();
            }
        }*/
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
    /*void ChangeHead()
    {
        SpriteRenderer sp = new SpriteRenderer();
        sp.material.SetTexture("Left", new Texture());

    }*/
    void Update()
    {
   
    }
    void OnMouseDown()
    {
        isActive = !isActive;

        if (isActive)
        {
            this.GetComponentsInChildren<SpriteRenderer>()[0].material.color = Color.blue;
            this.GetComponentsInChildren<SpriteRenderer>()[1].material.color = Color.blue;
            this.GetComponentsInChildren<SpriteRenderer>()[2].material.color = Color.blue;
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
