using System;
using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;

namespace PathCreation.Examples {

    public class PathSpawner : MonoBehaviour {

        //public PathCreator pathPrefab;
        //public PathFollower followerPrefab;
        //public Transform[] spawnPoints;

        //private void Awake()
        //{
        //}
        //void Start () {

        //    foreach (Transform t in spawnPoints) {
        //        var path = Instantiate (pathPrefab, t.position, t.rotation);
        //        var follower = Instantiate (followerPrefab);
        //        follower.pathCreator = path;
        //        path.transform.parent = t;
                
        //    }
        //}

        public List<PathCreator> pathPrefab;
        public List<Transform> spawnPoints;
        public  List<PathFollower> followers = new List<PathFollower>();
        [SerializeField]
        private Transform[] waypoints;
        public List<GameObject> playerObjects = new List<GameObject>();
        public List<String> playerNames = new List<String>();
        public int maxpath = 3;
        int newone = 0;

        [SerializeField]
          float moveSpeed = 2f;

        // Index of current waypoint from which Enemy walks
        // to the next one
        private int waypointIndex = 0;
        public int i = 0;
        bool newimage = false;

        //void Start()
        //{
        //    Invoke("loading",1);
        //}
        private IEnumerator Start()
        {

            while (true)
            {
                yield return new WaitForSeconds(1f);
                loading();
            }
        }
        private void loading()
        {
            int count = 0;
            GameObject[] tmpobj = GameObject.FindGameObjectsWithTag("Player");
            HashSet<string> currentNames = new HashSet<string>();



            foreach (GameObject obj in tmpobj)
            {
               
                currentNames.Add(obj.name);
            }


            

            if (playerObjects.Count == 0)
            {
                foreach(GameObject obj in tmpobj)
                {
      playerObjects.Add(obj);
                        playerNames.Add(obj.name);
                    
                   
                }
                newimage = true;
            }
            else
            {
                count = playerObjects.Count;
                foreach (GameObject obj in tmpobj)
                {
                    if (!playerNames.Contains(obj.name) )
                    {
                        playerObjects.Add(obj);
                        playerNames.Add(obj.name);
                        newimage = true;
                    }
                }
            }
            if(newimage)
            {
                for(int j = count; j < playerObjects.Count ; j++)
                {
                    GameObject playerObject = playerObjects[j];
                    playerObject.transform.position = waypoints[0].transform.position;
                    PathFollower followerComponent = playerObject.GetComponent<PathFollower>();
                    if (followerComponent != null)
                    {
                        followers.Add(followerComponent);
                        newimage = false;
                    }
                }
                
                
            }

            for (int j = 0; j < playerNames.Count; j++)
            {
              

                if (!currentNames.Contains(playerNames[j]))
                {
                    //PathFollower followerComponent = playerObjects[j].GetComponent<PathFollower>();
                    followers.RemoveAt(j);
                    playerObjects.RemoveAt(j);
                    playerNames.Remove(playerNames[j]);
                    i--;

                }
            }

            //for (int j = 0; j < playerObjects.Count; j++)
            //{
            //    if (!currentNames.Contains(playerNames[j]))
            //    {
            //        Debug.Log("heeeeeeeeeeere");
            //        Destroy(playerObjects[j]);
            //        playerObjects.RemoveAt(j);
            //        followers.RemoveAt(j);
            //        playerNames.RemoveAt(j);
            //        i--;
            //    }
            //}


            foreach (GameObject obj in playerObjects)
            {
                if (!currentNames.Contains(obj.name))
                {
                    PathFollower followerComponent = obj.GetComponent<PathFollower>();
                    Destroy(obj);
                   
                    followers.Remove(followerComponent);
                

                    playerObjects.Remove(obj);
                    playerNames.Remove(obj.name);
                    i--;

                }
            }
        }


        private void Update()
        {
            if(playerObjects != null && playerObjects.Count > 0 &&  i < playerObjects.Count ) 
                 Move();
        }

      
        private void Move()
        {
            if (waypointIndex <= waypoints.Length - 1)
            {
                playerObjects[i].transform.position = Vector3.MoveTowards(playerObjects[i].transform.position,
                   waypoints[waypointIndex].transform.position,
                   moveSpeed * Time.deltaTime);
                if (playerObjects[i].transform.position == waypoints[waypointIndex].transform.position)
                {
                    waypointIndex++;
                }
            }
            else if (i < playerObjects.Count)
            {
                if (newone > spawnPoints.Count)
                    newone = 0;
                    if (i < spawnPoints.Count)
                {
                    var path = Instantiate(pathPrefab[i], spawnPoints[i].position, spawnPoints[i].rotation);
                    followers[i].pathCreator = path;
                    path.transform.parent = spawnPoints[i];
                    path.transform.localScale = Vector3.one;
                }
                else
                {
                    var path = Instantiate(pathPrefab[newone], spawnPoints[newone].position, spawnPoints[newone].rotation);
                    followers[i].pathCreator = path;
                    path.transform.parent = spawnPoints[newone];
                    path.transform.localScale = Vector3.one;
                    newone++;
                }
                i++;
                waypointIndex = 0;
            }
        }
    }

}