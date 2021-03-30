using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Longship : MonoBehaviour
{
    public Transform target;
    [SerializeField]
    public float _offset = 35;
    public float _smoothspeed = 0.9f;
   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion Targetratation = target.rotation * Quaternion.Euler(0,90,0)*Quaternion.Euler(0,0,-90);
        
        Vector3 _desierdPosition = target.position;
        transform.SetPositionAndRotation( Vector3.Lerp(transform.position, _desierdPosition, _smoothspeed), Targetratation  ) ;

    }
}
