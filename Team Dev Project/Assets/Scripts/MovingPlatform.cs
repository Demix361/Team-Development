using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MovingPlatform : NetworkBehaviour
{
    [SerializeField] private Transform _leftBottomPoint;
    [SerializeField] private Transform _rightTopPoint;
    [SerializeField] private Rigidbody2D _rigidBody;
    [SerializeField] private bool _horizontalMovement;
    [SerializeField] private bool _verticalMovement;
    [SerializeField] private float _horizontalSpeed = 0;
    [SerializeField] private float _verticalSpeed = 0;
    [SerializeField] private Transform _leftBottomSpritePoint;
    [SerializeField] private Transform _rightTopSpritePoint;
    [SerializeField] private float m_MovementSmoothing = .05f;

    private Vector3 m_Velocity = Vector3.zero;
    private bool _positiveMovement = true;
    private bool _startMoving = false;
    private float _timeOffset = 3;
    private float _count = 0;


    private void Start()
    {
        if (_horizontalMovement)
            _verticalMovement = false;
        if (_verticalMovement)
            _horizontalMovement = false;
    }

    private void Update()
    {
        if (isServer && !_startMoving)
        {
            _count += Time.deltaTime;

            if (_count > _timeOffset)
                CmdStartMoving();
        }
    }

    private void FixedUpdate()
    {
        if (_startMoving)
        {
            if (_horizontalMovement)
            {
                if (_positiveMovement)
                {
                    if (_rightTopSpritePoint.position.x > _rightTopPoint.position.x)
                        _positiveMovement = false;
                    else
                    {
                        Vector3 targetVelocity = new Vector2(_horizontalSpeed * Time.fixedDeltaTime, 0);
                        _rigidBody.velocity = Vector3.SmoothDamp(_rigidBody.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
                    }
                }
                else
                {
                    if (_leftBottomSpritePoint.position.x < _leftBottomPoint.position.x)
                        _positiveMovement = true;
                    else
                    {
                        Vector3 targetVelocity = new Vector2(-_horizontalSpeed * Time.fixedDeltaTime, 0);
                        _rigidBody.velocity = Vector3.SmoothDamp(_rigidBody.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
                    }
                }
            }

            if (_verticalMovement)
            {
                if (_positiveMovement)
                {
                    if (_rightTopSpritePoint.position.y > _rightTopPoint.position.y)
                        _positiveMovement = false;
                    else
                    {
                        Vector3 targetVelocity = new Vector2(0, _verticalSpeed * Time.fixedDeltaTime);
                        _rigidBody.velocity = Vector3.SmoothDamp(_rigidBody.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
                    }
                }
                else
                {
                    if (_leftBottomSpritePoint.position.y < _leftBottomPoint.position.y)
                        _positiveMovement = true;
                    else
                    {
                        Vector3 targetVelocity = new Vector2(0, -_verticalSpeed * Time.fixedDeltaTime);
                        _rigidBody.velocity = Vector3.SmoothDamp(_rigidBody.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
                    }
                }
            }
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdStartMoving()
    {
        RpcStartMoving();
    }

    [ClientRpc]
    private void RpcStartMoving()
    {
        _startMoving = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        if (_horizontalMovement)
        {
            Vector3 pointLeft_1 = new Vector3(_leftBottomPoint.position.x, transform.position.y + 1, 0);
            Vector3 pointLeft_2 = new Vector3(_leftBottomPoint.position.x, transform.position.y - 1, 0);
            Vector3 pointRight_1 = new Vector3(_rightTopPoint.position.x, transform.position.y + 1, 0);
            Vector3 pointRight_2 = new Vector3(_rightTopPoint.position.x, transform.position.y - 1, 0);
            Gizmos.DrawLine(pointLeft_1, pointLeft_2);
            Gizmos.DrawLine(pointRight_1, pointRight_2);
        }
        else if (_verticalMovement)
        {
            Vector3 pointBottom_1 = new Vector3(transform.position.x + 1, _leftBottomPoint.position.y, 0);
            Vector3 pointBottom_2 = new Vector3(transform.position.x - 1, _leftBottomPoint.position.y, 0);
            Vector3 pointTop_1 = new Vector3(transform.position.x + 1, _rightTopPoint.position.y, 0);
            Vector3 pointTop_2 = new Vector3(transform.position.x - 1, _rightTopPoint.position.y, 0);
            Gizmos.DrawLine(pointBottom_1, pointBottom_2);
            Gizmos.DrawLine(pointTop_1, pointTop_2);
        }

    }

}
