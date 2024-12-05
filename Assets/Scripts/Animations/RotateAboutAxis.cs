using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Rotates a game object about a predetermined axis.
public class Rotate : MonoBehaviour
{

    public enum AxisToRotateAbout
    {
        xAxis,
        yAxis,
        zAxis,
    }
    [SerializeField] public AxisToRotateAbout axisOfRotation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.axisOfRotation == AxisToRotateAbout.zAxis) transform.Rotate(new Vector3(0f, 0f, 1f));

    }
}
