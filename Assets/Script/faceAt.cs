using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class faceAt : MonoBehaviour
{
    public Transform camera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.LookAt(camera);
    }
}
