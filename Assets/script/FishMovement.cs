using UnityEngine;
using PathCreation;

[ExecuteInEditMode]
public class FishMovement : MonoBehaviour
{
    public PathCreator pathCreator;
    public float Speed = 2;
    float distenceTravelled;

    void Update()
    {
        distenceTravelled += Speed * Time.deltaTime;
        transform.position = pathCreator.path.GetPointAtDistance(distenceTravelled);
        Quaternion currentRotation = pathCreator.path.GetRotationAtDistance(distenceTravelled);

        //transform.rotation = pathCreator.path.GetRotationAtDistance(distenceTravelled);
        transform.rotation = currentRotation * Quaternion.Euler(0, -90, 0);
    }

  

   
}
