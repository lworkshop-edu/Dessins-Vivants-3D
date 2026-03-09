using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class alreadytaking : MonoBehaviour
{
    public bool taken = false;

    string nameofconection = null;

    private Vector3 lastPosition;
    private float idleTimer = 0f;
    private float idleThreshold = 0.3f; // Adjust this value as needed


    // Start is called before the first frame update
    void Start()
    {

    }

    private void Update()
    {
        //Vector3(-351.62207, -172.293793, 794.450195)
             if (transform.position != lastPosition)
        {
            // Object is moving, reset idle timer
            idleTimer = 0f;
        }
        else
        {
            // Object is not moving, increment idle timer
            idleTimer += Time.deltaTime;
        }

        // Update lastPosition for the next frame
        lastPosition = transform.position;

        // If the object hasn't moved for idleThreshold seconds, reset its position
        if (idleTimer >= idleThreshold)
        {
            transform.position = new Vector3(-415.970001f, -167.509995f, 0);
        }
    }
    // Update is called once per frame

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !taken)
        {
            other.transform.gameObject.GetComponent<FolowHand>().conected = true;
            taken = true;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player" && !taken)
        {
            other.transform.gameObject.GetComponent<FolowHand>().conected = true;
            taken = true ;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            taken = false;
            other.transform.gameObject.GetComponent<FolowHand>().conected = false;
        }
    }

}
