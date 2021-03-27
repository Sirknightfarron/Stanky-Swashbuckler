using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floter : MonoBehaviour
{
    public Rigidbody _rigidbody;
    public float _depthBeforeSubmerged = 1f;
    public float _displacementAmount = 3f;
    public float _floaterCount = 4f;
    public float _waterplanehight= 9f; // sollte eigentlich null sein die plan ist aber aufgrund des meshes angehoben worden
    public float _gravityFaktor;
    private void FixedUpdate()
    {
        float _waveHieght = WaveManager.instance.GetWaveHight(transform.position.x)+9f;
        _rigidbody.AddForceAtPosition((Physics.gravity/_floaterCount) * _gravityFaktor, transform.position, ForceMode.Acceleration);
        Debug.Log("Waterlevel");
        Debug.Log(_waveHieght);
        Debug.Log("Schiffywert");

        Debug.Log( transform.position.y );
        if (transform.position.y  < _waveHieght)
        {
            float _dislplacmentMultiplier = (Mathf.Clamp01((_waveHieght - transform.position.y) / _depthBeforeSubmerged)) *_displacementAmount;
            Debug.Log("auftrib");

            _rigidbody.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * _dislplacmentMultiplier, 0f),transform.position, ForceMode.Force);

        }
    }

}
