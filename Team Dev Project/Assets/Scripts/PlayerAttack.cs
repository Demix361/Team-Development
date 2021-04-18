using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerAttack : NetworkBehaviour
{
    [SerializeField] private PlayerProperties _playerProperties;
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _attackColliderPoint1;
    [SerializeField] private Transform _attackColliderPoint2;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _damage;

    private void Update()
    {
        if (!hasAuthority)
            return;

        if (_playerProperties.allowInput && Input.GetButtonDown("Attack"))
        {
            _animator.SetTrigger("Attack");
            Debug.Log("Attack");

            Collider2D[] colliders = Physics2D.OverlapAreaAll(_attackColliderPoint1.position, _attackColliderPoint2.position, _layerMask);
        }
    }
    
    private void OnDrawGizmos()
    {
        DrawRect(_attackColliderPoint1, _attackColliderPoint2, Color.red);
    }

    private void DrawRect(Transform point1, Transform point2, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawLine(point1.position, new Vector2(point1.position.x, point2.position.y));
        Gizmos.DrawLine(point1.position, new Vector2(point2.position.x, point1.position.y));
        Gizmos.DrawLine(point2.position, new Vector2(point1.position.x, point2.position.y));
        Gizmos.DrawLine(point2.position, new Vector2(point2.position.x, point1.position.y));
    }
}
