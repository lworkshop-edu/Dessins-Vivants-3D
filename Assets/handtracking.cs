//using Intel.RealSense;
using System.Linq;
using UnityEngine;

public class HandTracking : MonoBehaviour
{
    //private Context rsContext;
    //private Pipeline pipeline;
    //private HandModule handModule;
    //private HandData handData;

    void Start()
    {
        // Create a Context object
        //rsContext = new Context();

        //// Create a pipeline to configure, start and stop the camera
        //pipeline = new Pipeline(rsContext);

        //// Start the pipeline with default settings
        //pipeline.Start();

        //// Create and configure the hand module
        //handModule = HandModule.Activate(pipeline);
        //handModule.SetTrackingMode(HandTrackingMode.FullHand);
        //handModule.EnableAllGestures();
        //handModule.MaxNumHands = 2;

        //// Create an object to store hand data
        //handData = new HandData();
    }

    void Update()
    {
        // Wait for the next set of frames from the camera
        //using (var frames = pipeline.WaitForFrames())
        //{
        //    // Update the hand module with the latest depth frame
        //    handModule.ProcessDepth(frames.DepthFrame);

        //    // Retrieve the latest hand data
        //    handModule.QueryOutput(out handData);

        //    // Iterate through detected hands
        //    foreach (var hand in handData.Hands)
        //    {
        //        Debug.Log($"Hand ID: {hand.Id}, Tracking Status: {hand.TrackingState}");

        //        // Access hand joints
        //        foreach (var joint in hand.Joints)
        //        {
        //            Debug.Log($"Joint {joint.Type}: Position ({joint.PositionImage.x}, {joint.PositionImage.y}, {joint.PositionWorld.z})");
        //        }
        //    }
        //}
    }

    void OnDestroy()
    {
        // Cleanup
        //handData.Dispose();
        //handModule.Dispose();
        //pipeline.Stop();
    }
}