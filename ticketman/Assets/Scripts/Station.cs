using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station : MonoBehaviour {

    public GameObject passenger;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void landing(int num)
    {
        for (int i = 0; i < num; i++)
        {
            Instantiate(passenger);
            passenger.transform.position = new Vector2(12, -16);
            Quaternion angel = new Quaternion();
            angel.SetLookRotation(passenger.transform.position);
            passenger.transform.rotation = angel;
        }
    }
}
