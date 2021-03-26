using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floter : MonoBehaviour
{
    public Rigidbody _rigidbody;
    public float _depthBeforeSubmerged = 1f;
    public float _displacementAmount = 3f;
    public float _floaterCount = 4f;

    private void FixedUpdate()
    {
        float _waveHieght = WaveManager.instance.GetWaveHight(transform.position.x);
        _rigidbody.AddForceAtPosition(Physics.gravity/_floaterCount , transform.position, ForceMode.Acceleration);


        if (transform.position.y < _waveHieght)
        {
            float _dislplacmentMultiplier = (Mathf.Clamp01(_waveHieght -transform.position.y - _depthBeforeSubmerged)) *_displacementAmount;
            //Debug.Log(_dislplacmentMultiplier);

            _rigidbody.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * _dislplacmentMultiplier, 0f),transform.position, ForceMode.Acceleration);

        }
    }

}
