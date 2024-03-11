using UnityEngine;
using UnityEngine.SpatialTracking;

public class AmplifiedPoseDriver : TrackedPoseDriver
{
    public float gain = 1f;

    Pose localPose = new Pose();
    override protected void PerformUpdate()
    {
        PoseDataSource.TryGetDataFromSource(poseSource, out localPose);

        Vector3 euler = localPose.rotation.eulerAngles;

        if (euler.x >= 180f)
        {
            euler.x = (euler.x - 360f) * (1f + gain);
        }
        else
        {
            euler.x *= 1f + gain;
        }

        if (euler.y >= 180f)
        {
            euler.y = (euler.y - 360f) * (1f + gain);
        }
        else
        {
            euler.y *= 1f + gain;
        }
        
        localPose.rotation.eulerAngles = euler;

        SetLocalTransform(localPose.position, localPose.rotation,
          PoseDataFlags.Position | PoseDataFlags.Rotation);
    }
}
