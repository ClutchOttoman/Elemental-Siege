using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class FlamethrowerPlay : MonoBehaviour
{
    [SerializeField] VisualEffect flamethrower;

    // Start is called before the first frame update
    void Start()
    {
        // Do not apply upon start.
        flamethrower.Stop();
        
    }

    public void playFlamethrower(){
        flamethrower.Play();
    }

    public void stopFlamethrower(){
        flamethrower.Stop();
    }

}
