
using UnityEngine;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform target;
    [SerializeField]
    public float _offset= 35;
    

    public float _smoothspeed = 0.00125f;
    // LateUpdate wird nach update gecalled. Alternative ist der FixedUpdate der Physikengin(bei ruekeliger Ausgabe)
    private void LateUpdate()
    {
       Vector3 _desierdPosition = transform.position = target.position + target.transform.up*-1*_offset+new Vector3(0,_offset,0);
       
       Vector3 _smoothedPositon = Vector3.Lerp(transform.position, _desierdPosition, _smoothspeed);
        transform.LookAt(target.position + target.up * 100);
       
       
    }

}
