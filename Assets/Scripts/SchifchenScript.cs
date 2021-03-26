using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SchifchenScript : MonoBehaviour
{
    // Start is called before the first frame update
    public InputManager _inputSystem;
    private Vector2 _Movement;
    Rigidbody _Body;
    Object _Bullet;
    Camera _Camera;
    [SerializeField] 
    float _speed = 1000f;
    void Awake()
    {
        _Body = GetComponent<Rigidbody>();

        _inputSystem = new InputManager();
        //_inputSystem.Player.Move.performed += ctx => Move(ctx);
        _inputSystem.Player.Move.Enable();

        //_inputSystem.See.Move.canceled += ctx => Move(new Vector2(0, 0));
    }
    private void OnEnable()
    {
        _inputSystem.Player.Move.Enable();
    }
    private void OnDisable()
    {
        _inputSystem.Player.Move.Disable();

    }
    private void FixedUpdate()

    {

        Debug.Log(_Movement);

        _Body.AddForce(((transform.up) * _speed * _Movement.y));

        if (_Movement.x != 0)
        {
            _Body.AddForce(transform.right * _Movement.y * _Body.velocity.magnitude);
        }
        _Body.AddTorque(new Vector3(0, _Movement.x  , 0) *_speed);


       
    }
    public void Move(InputAction.CallbackContext context)
    {
        //Debug.Log( context.ReadValue<Vector2>() );
        _Movement = context.ReadValue<Vector2>();
         
       
    }
    public void Fire(InputAction.CallbackContext context)
    {

        Instantiate(_Bullet, transform.position, _Camera.transform.rotation);
        
    }
}


