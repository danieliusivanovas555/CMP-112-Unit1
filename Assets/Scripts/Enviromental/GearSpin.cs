//Dan Ivanovas
using UnityEngine;

public class GearSpin : MonoBehaviour
{
    //Vector 3 lets you you control (x,y,z)
    public Vector3 rotationSpeed = new Vector3(0f, 90f, 0f);

    void Update()
    {
        //In This scenario we want to control rotation
        //This will rotate the gears at a fixed frame over time, Space.Self is used when we need to go it from the objects pivot point like the a center of the gear in this scenario
        transform.Rotate(rotationSpeed * Time.deltaTime, Space.Self);
    }
}