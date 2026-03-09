using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MoveCarpets : MonoBehaviour
{
    List<KeyValuePair<GameObject, float>> carpets = new List<KeyValuePair<GameObject, float>>();
    public float spacing = 35;
    public float timetomove = 6;
    bool change = true;
    public LeanTweenType easeType;
    public float animationtime = 0.7f;
    public Create_Carpets creatCarpetsobj;
    bool isdone = false;
    void Start()
    {
        InvokeRepeating("FindCarpets", 0, 1f);
        
        InvokeRepeating("RollDown", 0, 0.1f);
    }
    public void RollDown()
    {
        if (creatCarpetsobj.mouvecarpet)
        {
            for (int i = 0; carpets.Count > i; i++)
            {
                //if (!carpets[i].Key.GetComponent<Cloth>().enabled && (carpets[i].Value == 0 || carpets[i].Value == -spacing || carpets[i].Value == spacing))
                //    carpets[i].Key.GetComponent<Cloth>().enabled = true;
                if (carpets[i].Key != null && !carpets[i].Key.GetComponent<Cloth>().enabled &&
    (carpets[i].Value == 0 || carpets[i].Value == -spacing || carpets[i].Value == spacing))
                {
                    carpets[i].Key.GetComponent<Cloth>().enabled = true;
                }
            }
        }
    }

    private void FixPositionsAfterDelete()
    {
       
        for (int i = carpets.Count - 1; i >= 0; i--)
        {
            if (carpets[i].Key == null)
            {
                carpets.RemoveAt(i);
                for (int t = 0; t < carpets.Count; t++)
                {
                    float correctX = spacing - (t * spacing); 

                    
                    carpets[t] = new KeyValuePair<GameObject, float>(carpets[t].Key, correctX);

                        float z = correctX == 0 ? 30f : 40f;
                        carpets[t].Key.transform.position = new Vector3(correctX, 17f, z);
                }
            }
        }

     
        
    }

    public void FindCarpets()
    {
        GameObject[] findCarpets = GameObject.FindGameObjectsWithTag("Player");
        FixPositionsAfterDelete();
        if (creatCarpetsobj.mouvecarpet)
        {
            foreach (GameObject carpet in findCarpets)
            {

                if (!carpets.Any(obj => obj.Key != null && obj.Key.name == carpet.name))
                {
                    float x = spacing;
                    if (carpets.Count == 0)
                        x = spacing;
                    else
                        x = carpets.Last().Value - spacing;
                    carpets.Add(new KeyValuePair<GameObject, float>(carpet, x));
                    if (carpets.Count == 1)
                        carpet.transform.position = new Vector3(spacing, 17, 40);
                    else
                        carpet.transform.position = new Vector3(x, 17, 40);
                    if (x == 0)
                    {
                        carpet.transform.position = new Vector3(x, 17, 30);

                    }
                }

            }
        }
    }
    private void moveCarpets()
    {
        if (carpets.Count != 0 && carpets[carpets.Count - 1].Value == 0)
                    change = false;
                else if (carpets.Count != 0 &&  carpets[0].Value == 0)
                    change = true;
                if (carpets.Count > 1)
                {
                    if (carpets.Count == 4 )
                        move4carpets();
                    if (carpets.Count >= 5)
                        movemorecarpets();
                }
    }

    private void move4carpets()
    {
        
        if (change)
        {
            for (int i = 0; carpets.Count > i; i++)
            {

                if (carpets[i].Value + spacing == 0)
                {
                    if (carpets[i].Key != null)
                        LeanTween.move(carpets[i].Key, new Vector3(carpets[i].Value + spacing, 17, 30), animationtime).setEase(easeType);
                }
                else
                {
                    if (carpets[i].Key != null)
                        LeanTween.move(carpets[i].Key, new Vector3(carpets[i].Value + spacing, 17, 40), animationtime).setEase(easeType);
                }
                carpets[i] = new KeyValuePair<GameObject, float>(carpets[i].Key, carpets[i].Value + spacing);
            }
        }
        else
        {
            for (int i = 0; carpets.Count > i; i++)
            {
                

                if (carpets[i].Value - spacing == 0)
                {
                    if (carpets[i].Key != null)
                        LeanTween.move(carpets[i].Key, new Vector3(carpets[i].Value - spacing, 17, 30), animationtime).setEase(easeType);
                }
                else
                {
                    if (carpets[i].Key != null)
                        LeanTween.move(carpets[i].Key, new Vector3(carpets[i].Value - spacing, 17, 40), animationtime).setEase(easeType);
                }
                carpets[i] = new KeyValuePair<GameObject, float>(carpets[i].Key, carpets[i].Value - spacing);
            }
        }
    }
    private void movemorecarpets()
    {
        if (carpets[0].Value == (spacing * 2))
        {
            if (carpets[0].Key != null)  
            {
                carpets.Add(new KeyValuePair<GameObject, float>(carpets[0].Key, carpets.Last().Value - spacing));
                carpets.RemoveAt(0);
                if (carpets.Last().Key != null)
                    carpets.Last().Key.transform.position = new Vector3(carpets.Last().Value, 17, 40);
            }
        }
        for (int i = 0; carpets.Count > i; i++)
            {
                if (carpets[i].Value + spacing == 0)
                {
                if (carpets[i].Key != null)
                    LeanTween.move(carpets[i].Key, new Vector3(carpets[i].Value + spacing, 17, 30), animationtime).setEase(easeType);
                }
                else
                {
                if (carpets[i].Key != null)
                    LeanTween.move(carpets[i].Key, new Vector3(carpets[i].Value + spacing, 17, 40),animationtime).setEase(easeType);
                }
                carpets[i] = new KeyValuePair<GameObject, float>(carpets[i].Key, carpets[i].Value + spacing);
            }

    }

    // Update is called once per frame
    void Update()
    {
        if (creatCarpetsobj.mouvecarpet && !isdone)
        {
            InvokeRepeating("moveCarpets", 0, timetomove);
            isdone = true;
        }
    }


}
