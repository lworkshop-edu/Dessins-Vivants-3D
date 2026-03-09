using PathCreation;
using PathCreation.Examples;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FolowHand : MonoBehaviour
{
     List<BoxCollider> boundaryColliders = new List<BoxCollider>();
    // Array of BoxColliders defining the boundaries
    public bool conected = false;

    private Vector3[] minBounds; // Minimum bounds of each boundary collider
    private Vector3[] maxBounds; // Maximum bounds of each boundary collider

    Vector3 lastposetion;
    public bool link = false;
    PathFollower folow;
    Vector3 originesapwn;
    private void Awake()
    {
        GameObject[] boxes = GameObject.FindGameObjectsWithTag("box");
        foreach (GameObject box in boxes)
        {
            boundaryColliders.Add(box.GetComponent<BoxCollider>());
        }

    }
    private void Start()
    {
        // Initialize arrays for minBounds and maxBounds
        minBounds = new Vector3[boundaryColliders.Count];
        maxBounds = new Vector3[boundaryColliders.Count];

        // Calculate the minimum and maximum bounds for each collider
        CalculateBounds();
        folow = GetComponent<PathFollower>();
        //folow.enabled = false;
        lastposetion = transform.position;
        originesapwn = transform.position;
    }

    private void CalculateBounds()
    {
        for (int i = 0; i < boundaryColliders.Count; i++)
        {
            BoxCollider collider = boundaryColliders[i];
            Bounds bounds = collider.bounds;
            minBounds[i] = bounds.min;
            maxBounds[i] = bounds.max;
        }
    }


    private void FixedUpdate()
    {
        if (folow && link == false && transform.position == lastposetion && folow.enabled == false)
        {
            folow.enabled = true;
            //lastposetion = transform.position;
        }
        else if (folow &&  link == false && transform.position != lastposetion && folow.enabled == false)
        {
            transform.position = Vector3.MoveTowards(transform.position, lastposetion, 200 * Time.deltaTime); ;
        }
        if (folow &&  folow.enabled == true)
            lastposetion = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "boxhole")
        {
            transform.gameObject.GetComponent<PathFollower>().enabled = false;
            transform.gameObject.GetComponent<PathFollower>().pathCreator = null;
            transform.position = originesapwn;
            lastposetion = originesapwn;
            GameObject spawner = GameObject.FindWithTag("spawner");
            List<String> playerNames = spawner.GetComponent<PathSpawner>().playerNames;
            for (int i = 0; i < playerNames.Count; i++)
            {

                if (playerNames[i].Equals(transform.name))
                {
                    int count = spawner.GetComponent<PathSpawner>().playerNames.Count;
                    spawner.GetComponent<PathSpawner>().playerNames.RemoveAt(i);
                    spawner.GetComponent<PathSpawner>().playerObjects.RemoveAt(i);
                    PathCreator pathctemp = spawner.GetComponent<PathSpawner>().pathPrefab[i];
                    Destroy(spawner.GetComponent<PathSpawner>().spawnPoints[i].GetChild(0).gameObject);
                    Transform spantemp = spawner.GetComponent<PathSpawner>().spawnPoints[i];
                    spawner.GetComponent<PathSpawner>().spawnPoints.RemoveAt(i);
                    spawner.GetComponent<PathSpawner>().spawnPoints.Insert(count - 1,spantemp);
                    spawner.GetComponent<PathSpawner>().followers[i].gameObject.GetComponent<PathFollower>().distanceTravelled = 0;
                    spawner.GetComponent<PathSpawner>().followers.RemoveAt(i);
                    spawner.GetComponent<PathSpawner>().pathPrefab.RemoveAt(i);
                    spawner.GetComponent<PathSpawner>().pathPrefab.Insert(count - 1,pathctemp);
                    spawner.GetComponent<PathSpawner>().i -= 1;
                }
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {

        if (other.transform.tag == "Plane" && conected)
        {
           
            link = true;
            other.transform.gameObject.GetComponent<alreadytaking>().taken = true;
            folow.enabled = false;
            // Clamp the position of the object within the bounds of the boundary collider
            int nearestColliderIndex = 0;
            float minDistance = Mathf.Infinity;
            for (int i = 0; i < boundaryColliders.Count; i++)
            {
                Vector3 closestPoint = boundaryColliders[i].ClosestPoint(other.gameObject.transform.position);
                float distance = Vector3.Distance(other.gameObject.transform.position, closestPoint);
                if (distance < minDistance)
                {
                    nearestColliderIndex = i;
                    minDistance = distance;
                }
            }

            // Clamp the position of the object within the bounds of the nearest collider
            Vector3 clampedPosition = new Vector3(
                Mathf.Clamp(other.gameObject.transform.position.x, minBounds[nearestColliderIndex].x, maxBounds[nearestColliderIndex].x),
                Mathf.Clamp(other.gameObject.transform.position.y, minBounds[nearestColliderIndex].y, maxBounds[nearestColliderIndex].y),
                 transform.position.z // Maintain the object's original z position
            );




            // Update the position of the object
            alreadytaking tak = other.gameObject.GetComponent<alreadytaking>();
            transform.position = Vector3.MoveTowards(transform.position, clampedPosition, 200 * Time.deltaTime);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Plane")
        {
            link = false;
        }
    }

}
