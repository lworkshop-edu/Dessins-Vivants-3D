using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gotoo : MonoBehaviour
{
    public float posx = 0,posy=0;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (posx != 0)
        {
            other.transform.position = new Vector3(posx, other.transform.position.y + posy, other.transform.position.z);
        }
        else
        {
            other.transform.position = new Vector3(other.transform.position.x, other.transform.position.y +posy, other.transform.position.z);
        }
      
        
    }
}
