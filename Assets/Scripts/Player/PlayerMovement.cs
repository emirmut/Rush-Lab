using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Player _player;
    private Rigidbody _rb;
    private float _horizontalInput;
    private float _verticalInput;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private Transform _orientation;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (_player.IsDead == false)
        {
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _verticalInput = Input.GetAxisRaw("Vertical");
        }
    }

    private void FixedUpdate()
    {
        if (_player.IsDead == false)
        {
            MovePlayer(_horizontalInput, _verticalInput);
        }
    }

    private void MovePlayer(float horizontalInput, float verticalInput)
    {
        Vector3 moveDirection = _orientation.forward * verticalInput + _orientation.right * horizontalInput;
        _rb.linearVelocity = moveDirection * _moveSpeed;
    }
}
