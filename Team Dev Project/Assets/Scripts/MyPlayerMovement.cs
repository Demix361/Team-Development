using UnityEngine;
using UnityEngine.Events;
using Mirror;

/// <summary>
/// Класс передвижения игрока.
/// </summary>
/// <remarks>
/// Игрок перемещается с заданой скоростью, прыгает на заданную высоту.
/// </remarks>
public class MyPlayerMovement : NetworkBehaviour
{

    /// <summary>
    /// Скорость бега.
    /// </summary>
    [SerializeField] private float _runSpeed;
    /// <summary>
    /// Высота прыжка.
    /// </summary>
    [SerializeField] private float _jumpForce;
    /// <summary>
    /// Параметр камеры.
    /// </summary>
    [Range(0, .3f)] [SerializeField] private float _movementSmoothing = .05f;
    /// <summary>
    /// Контроль полета.
    /// </summary>
    [SerializeField] private bool _airControl;
    /// <summary>
    /// Маска определяющая точку опоры для персонажа.
    /// </summary>
    [SerializeField] private LayerMask _whatIsGround;
    /// <summary>
    /// Триггер, показывающий если персонаж на земле.
    /// </summary>
    [SerializeField] private Transform _groundCheck;
    /// <summary>
    /// Триггер, показывающий где проверять наличие потолка.
    /// </summary>
    [SerializeField] private Transform _ceilingCheck;

    /// <summary>
    /// Событие в случае контакта с земле.
    /// </summary>
    [Header("Events")]

    public UnityEvent OnLandEvent;

    //[System.Serializable]
    //public class BoolEvent : UnityEvent<bool> { }

    /// <summary>
    /// Свойства игрока.
    /// </summary>
    [Header("Player Components")]
    [SerializeField] private PlayerProperties _playerProperties;
    /// <summary>
    /// Аниматор.
    /// </summary>
    [SerializeField] private Animator _animator;

    /// <summary>
    /// Значение перемещения по горизонтали.
    /// </summary>
    private float _horizontalMove = 0f;
    /// <summary>
    /// Проверка на прыжок.
    /// </summary>
    private bool _jump = false;
    /// <summary>
    /// Радиус окружности для рассчета, если на земле.
    /// </summary>
    private const float _groundedRadius = .2f; 
    /// <summary>
    /// Проверка на нахождение на земле.
    /// </summary>
    private bool _grounded;
    /// <summary>
    /// Объект rigidbody.
    /// </summary>
    private Rigidbody2D _rigidbody;
    /// <summary>
    /// Проверка на направление персонажа игрока.
    /// </summary>
    private bool _facingRight = true;
    /// <summary>
    /// Приведение вектора к нулевому.
    /// </summary>
    private Vector3 _velocity = Vector3.zero;
    /// <summary>
    /// Проверка на нахождения на земле.
    /// </summary>
    [Client]
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();
    }


    /// <summary>
    /// Ввод игрока и его перемещение в пространстве.
    /// </summary>
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

    /// <summary>
    /// Аниматор бега по земле.
    /// </summary>
    public void OnLanding()
    {
        _animator.SetBool("IsJumping", false);
    }

    /// <summary>
    /// Проверка на приземление.
    /// </summary>
    void FixedUpdate()
    {
        bool wasGrounded = _grounded;
        _grounded = false;

        
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

    /// <summary>
    /// Оперемещение игрока.
    /// </summary>
    /// <param name="move">значение передвижения</param>
    /// <param name="jump">состояния прыжка</param>
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

    /// <summary>
    /// Смена направления персонажа игрока.
    /// </summary>
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
