using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Transform obj1;


    Quaternion prev;
    Vector3 preve;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 direction = obj1.position - transform.position;
        prev = transform.rotation; //Quaternion.LookRotation(direction.normalized);
        preve = transform.forward;
    }


    // Update is called once per frame
    void Update()
    {
        
        //transform.position += direction.normalized * Time.deltaTime;
        //direction.x = Mathf.DeltaAngle(0, direction.x);
        //direction.y = Mathf.DeltaAngle(0, direction.y);
        //direction.z = Mathf.DeltaAngle(0, direction.z);
        Quaternion deltaRotation = transform.rotation * Quaternion.Inverse(prev);
        //Quaternion target = Quaternion.FromToRotation(preve, direction.normalized);
        prev = transform.rotation;
        preve = transform.forward;

        //transform.rotation = transform.rotation * deltaRotation;
        transform.rotation = Quaternion.Lerp(transform.rotation, deltaRotation, 1.5f * Time.deltaTime);
    }
}
