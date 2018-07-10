using UnityEngine;
using UnityScript.Lang;

public class Passenger : MonoBehaviour
{
    public GameObject map;
    Level LevelSettings;
    public float speed = 1.0f;
    public Transform Coords; //текущие координаты
    public Vector2 CurrentCoordInMatrix; //текущие координаты относительно координат уровня
    bool move = true;
    public Array ExitPoint = new Array();//точки выхода, в которых объект уничтожается
    private float tempstep = 0;
    
   
    // Use this for initialization
    void Start()
    {
        map = GameObject.Find("Bus"); // получение ссылки на автобус, к которому прикреплен скрипт
        LevelSettings = map.GetComponent<Level>(); //получение ссылки на скрипт
        ExitPoint = LevelSettings.getEntryPoints(); //получение точек выхода

        Coords = gameObject.GetComponent<Transform>();// получение текущих координат
        CurrentCoordInMatrix = getCurrentPosition(Coords);
    }
    //получение текущих координат
    private Vector2 getCurrentPosition(Transform tempCoords)
    {
        return new Vector2((int)(tempCoords.position.x / LevelSettings.scale),(int)(tempCoords.position.y / LevelSettings.scale));
    }

    //перемещение к новой ячейке
    void MoveTo(Vector2 newDirect, int step = 1)
    {
        tempstep += MoveStep(newDirect, speed);
        if (tempstep >= step * LevelSettings.scale)
        {
            move = false;
            tempstep = 0;
        }
    }
       // checkAndDestroy();
    
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
    float MoveStep(Vector2 direction, float speed)
    {
        Coords.Translate(direction * speed * Time.deltaTime);
        return speed * Time.deltaTime;
    }
    void RotateTo(Vector2 newPosition)
    {
        transform.Rotate(Vector3.forward, 90);

     


     //   Vector2 temp = new Vector2();
      //  temp.x = (newPosition.x - getCurrentPosition().x);
       // temp.y = (newPosition.y - getCurrentPosition().y);

       
       // int tempint = 0;

        //Vector2 tempVector = new Vector2();
       // tempVector = Vector2.down - Vector2.left;
      

        //tempVector = Vector2.down - Vector2.right;
       
        /*
        if (temp.x == 0 && temp.y > 0)
        {
            tempint = 1;
        }
        if (temp.x > 0 && temp.y == 0)
        {
            tempint = 2;
        }
        if (temp.x < 0 && temp.y == 0)
        {
            tempint = 3;
        }
        if (temp.x == 0 && temp.y < 0)
        {
            tempint = 0;
        }
        */
       // transform.Rotate(Vector3.forward, 90 * tempint);
    }
    void RotateTo()
    {
     //   RotateTo(new Vector2(1,3));
        //transform.rotation.SetFromToRotation(transform.rotation.eulerAngles, Vector2.right);
        transform.Rotate(Vector3.forward, 90);
        //transform.Rotate(0, 0, 180);


       /* var curvector = new Vector2(0,-1);
        var newvector = Vector2.up;
        transform.Rotate(Vector3.forward,((curvector.x  - newvector.x)+(curvector.y-newvector.y)) * 90);
        */
        //transform.rotation = new Quaternion(0, 0, 1, 0);
        //Debug.Log("старое");
        //Debug.Log(transform.rotation);
        //transform.rotation.SetFromToRotation(new Vector3(0,0,0),new Vector3(0,0,90));
        //Debug.Log("новое");
        //Debug.Log(transform.rotation);
        //transform.Rotate(0,0,45);

        // Vector2(-1,0) = -1 ;  -90
        // Vector2(1,0) = 1 ;90
        // Vector2(0,1) = 2;180
        // Vector2(0,-1) = 0;-180
       // (V.X* 90 + Vector2.Y*180)

        //Set the Quaternion rotation from the GameObject's position to the next GameObject's position
      /*  Quaternion m_MyQuaternion = new Quaternion();
        m_MyQuaternion.SetFromToRotation(new Vector3(0,0,0), new Vector3(0,90,0));

        transform.rotation = transform.rotation * m_MyQuaternion;
        ///Move the GameObject towards the second GameObject
        //transform.position = Vector3.Lerp(transform.position, m_NextPoint.position, m_Speed * Time.deltaTime);
        //Rotate the GameObject towards the second GameObject
        //transform.rotation = m_MyQuaternion * transform.rotation;

        
        
        
        //transform.Rotate(Vector3.back,-90);
        
        /*int temp = Random.Range(0, 3);
        transform.Rotate(Vector3.forward, 90 * temp);// Vector3.forward);
        */


        //pass.transform.transform.Rotate(Vector3.forward, Vector3.Angle(Vector3.down, Vector3.right));
        //Vector3 vd = new Vector3();
        //Quaternion qa = new Quaternion();

        //pass.transform.Rotate(1,1,0);// Vector3.forward, Space.Self);
        //Debug.Log("НАЛЕВО");
        //pass.transform.rotation = new Quaternion(-1,0,0,0);
        /*Debug.Log("Угол" + Vector3.Angle(Vector3.down, Vector3.right));
        Debug.Log("Угол" + Vector3.Angle(Vector3.down, Vector3.up));
        Debug.Log("Угол" + Vector3. Angle(Vector3.left, Vector3.up));
        // pass.transform.Rotate(Vector3.left);
        */
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
        if (move)
        {
            MoveTo(Vector2.down);
        }
        else
        {
            RotateTo();
            Debug.Log("Поворот");
            move = !move;
        }
        Debug.Log("X:=" + getCurrentPosition(Coords).x);
        Debug.Log("Y:=" + getCurrentPosition(Coords).y);

        if (Input.GetMouseButtonDown(0))
            Debug.Log("Pressed left click.");

        if (Input.GetMouseButtonDown(1))
            Debug.Log("Pressed right click.");

        if (Input.GetMouseButtonDown(2))
            Debug.Log("Pressed middle click.");

        

        // .position -= pass.transform.forward * speed * Time.deltaTime;
        //rb.position -= rb.position * speed * Time.deltaTime;
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
