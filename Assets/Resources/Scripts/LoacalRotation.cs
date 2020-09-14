using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class LoacalRotation : MonoBehaviour
{
    public float revolutionSpeed;
    public float rotationSpeed;
    public Vector3 forwardDir = new Vector3(1,0,0);
    public Axis revolutionAxis;
    private Vector3 rotationDelta;

    // Start is called before the first frame update
    void Start()
    {
        rotationDelta = forwardDir.normalized*rotationSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotationDelta);

        try
        {
            switch (revolutionAxis) {
                case Axis.X:
                    transform.RotateAround(transform.parent.position, transform.parent.right, revolutionSpeed);
                    break;
                case Axis.Y:
                    transform.RotateAround(transform.parent.position, transform.parent.up, revolutionSpeed);
                    break;
                case Axis.Z:
                    transform.RotateAround(transform.parent.position, transform.parent.forward, revolutionSpeed);
                    break;
            }
        }
        catch { }


    }
}
