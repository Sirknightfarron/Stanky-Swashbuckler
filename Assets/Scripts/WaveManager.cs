using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public float _amplitude = 1f;
    public float _length = 2f;
    public float _speed = 1f;
    public float _offset = 1;

    public static WaveManager instance;
    private void Awake()
    {
        Debug.Log("Instance already exists, destroing object!");
        if (instance == null)
        {
            instance = this;
            
        }
        else if (instance != null)
        {
            Debug.Log("Instance already exists, destroing object!");
            Destroy(this);
        }
        
    }
    private void Update()
        {
        _offset += Time.deltaTime * _speed;


        }
    public float GetWaveHight(float _x)
    {

        return _amplitude * Mathf.Sin((_x / _length) + _offset);

    }
}
