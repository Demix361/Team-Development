using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class MyPlayerMovement : NetworkBehaviour
{
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _jumpForce;
    [Range(0, .3f)] [SerializeField] private float _movementSmoothing = .05f;
    [SerializeField] private bool _airControl;
    [SerializeField] private LayerMask _whatIsGround; // A mask determining what is ground to the character
    [SerializeField] private Transform _groundCheck; // A position marking where to check if the player is grounded.
    [SerializeField] private Transform _ceilingCheck; // A position marking where to check for ceilings

    [Header("Events")]
    public UnityEvent OnLandEvent;

    //[System.Serializable]
    //public class BoolEvent : UnityEvent<bool> { }

    [Header("Player Components")]
    [SerializeField] private PlayerProperties _playerProperties;
    [SerializeField] private Animator _animator;

    private float _horizontalMove = 0f;
    private bool _jump = false;
    private const float _groundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool _grounded;            // Whether or not the player is grounded.
    private Rigidbody2D _rigidbody;
    private bool _facingRight = true;  // For determining which way the player is currently facing.
    private Vector3 _velocity = Vector3.zero;


    [Client]
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();
    }

    void Update()
    {
        if (!hasAuthority)
        {
            return;
        }

        if (_playerProperties.allowInput)
        {
            _horizontalMove = Input.GetAxisRaw("Horizontal") * _runSpeed;
        }
        else
        {
            _horizontalMove = 0;
        }
        
        _animator.SetFloat("Speed", Mathf.Abs(_horizontalMove));
        

        if (_playerProperties.allowInput && Input.GetButtonDown("Jump"))
        {
            _jump = true;
            _animator.SetBool("IsJumping", true);
        }
    }

    public void OnLanding()
    {
        _animator.SetBool("IsJumping", false);
    }

    void FixedUpdate()
    {
        bool wasGrounded = _grounded;
        _grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_groundCheck.position, _groundedRadius, _whatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (!colliders[i].isTrigger && colliders[i].gameObject != gameObject)
            {
                _grounded = true;
                if (!wasGrounded)
                    OnLandEvent.Invoke();
            }
        }

        Move(_horizontalMove * Time.fixedDeltaTime, _jump);
        _jump = false;
    }

    public void Move(float move, bool jump)
    {
        //only control the player if grounded or airControl is turned on
        if (_grounded || _airControl)
        {
            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(move * 10f, _rigidbody.velocity.y);
            // And then smoothing it out and applying it to the character
            _rigidbody.velocity = Vector3.SmoothDamp(_rigidbody.velocity, targetVelocity, ref _velocity, _movementSmoothing);

            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !_facingRight)
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && _facingRight)
            {
                // ... flip the player.
                Flip();
            }
        }
        // If the player should jump...
        if (_grounded && jump)
        {
            // Add a vertical force to the player.
            _grounded = false;
            _rigidbody.AddForce(new Vector2(0f, _jumpForce));
        }
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        _facingRight = !_facingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
