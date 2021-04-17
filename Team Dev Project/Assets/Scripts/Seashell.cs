using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Seashell : NetworkBehaviour
{
    [SerializeField] private Transform _rangeColliderPoint1;
    [SerializeField] private Transform _rangeColliderPoint2;
    [SerializeField] private Transform _closeColliderPoint1;
    [SerializeField] private Transform _closeColliderPoint2;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Animator _animator;
    
    [Header("Bite")]
    [SerializeField] private float _biteInterval;
    [SerializeField] private float _biteDamage;
    [SerializeField] private float _biteAnimationDuration;

    [Header("Pearl")]
    [SerializeField] private float _shootInterval;
    [SerializeField] private GameObject _pearlPrefab;
    [SerializeField] private Transform _pearlSpawnPoint;
    [SerializeField] private float _pearlShootForce;
    [SerializeField] private float _shootOffset;

    private float _shootCounter;
    private float _biteCounter;
    private bool _haveBitten;
    private bool _animationStarted;
    private bool _delayedShot;

    private void Update()
    {
        if (isServer)
        {
            _biteCounter += Time.deltaTime;
            _shootCounter += Time.deltaTime;
            _haveBitten = false;

            Collider2D[] colliders = Physics2D.OverlapAreaAll(_closeColliderPoint1.position, _closeColliderPoint2.position, _layerMask);
            if (colliders.Length > 0)
            {
                Collider2D closestCollider = colliders[0];
                foreach (var c in colliders)
                {
                    if (Mathf.Abs(c.transform.position.x - transform.position.x) < Mathf.Abs(closestCollider.transform.position.x - transform.position.x))
                    {
                        closestCollider = c;
                    }
                }

                // Turning around if closest player is behind
                if (transform.localScale.x > 0 && closestCollider.transform.position.x > transform.position.x)
                    CmdFlip();
                else if (transform.localScale.x < 0 && closestCollider.transform.position.x < transform.position.x)
                    CmdFlip();

                // Starting bite animation
                if (_biteCounter > _biteInterval && !_animationStarted)
                {
                    _biteCounter = 0;
                    _animationStarted = true;

                    for (int i = 0; i < colliders.Length; i++)
                    {
                        if (transform.localScale.x > 0 && colliders[i].transform.position.x < transform.position.x ||
                            transform.localScale.x < 0 && colliders[i].transform.position.x > transform.position.x)
                        {
                            CmdBite();
                        }
                    }
                }

                // Biting player at the end of animation if he is still in hitbox
                if (_biteCounter > _biteAnimationDuration && _animationStarted)
                {
                    if (_biteCounter < _biteAnimationDuration + 0.1)
                    {
                        for (int i = 0; i < colliders.Length; i++)
                        {
                            if (transform.localScale.x > 0 && colliders[i].transform.position.x < transform.position.x ||
                                transform.localScale.x < 0 && colliders[i].transform.position.x > transform.position.x)
                            {
                                colliders[i].GetComponent<Health>().CmdDealDamage(0);
                            }
                        }
                    }

                    _biteCounter = 0;
                    _animationStarted = false;
                }

                _haveBitten = true;
            }

            colliders = Physics2D.OverlapAreaAll(_rangeColliderPoint1.position, _rangeColliderPoint2.position, _layerMask);
            
            // Shooting
            if (colliders.Length > 0)
            {
                if (_shootCounter > _shootInterval && !_haveBitten)
                {
                    _shootCounter = 0;
                    //CmdShoot();
                    _delayedShot = true;
                }

                if (_shootCounter  > _shootOffset && !_haveBitten &&_delayedShot)
                {
                    if (_shootCounter < _shootOffset + 0.1)
                        CmdShoot();
                    _shootCounter = 0;
                    _delayedShot = false;
                }
            }
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdBite()
    {
        RpcBite();
    }

    [ClientRpc]
    private void RpcBite()
    {
        _animator.SetTrigger("Bite");
    }

    [Command(requiresAuthority = false)]
    private void CmdFlip()
    {
        RpcFlip();
    }

    [ClientRpc]
    private void RpcFlip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1);
    }

    [Command(requiresAuthority = false)]
    private void CmdShoot()
    {
        RpcShoot();
    }

    [ClientRpc]
    private void RpcShoot()
    {
        GameObject pearl = Instantiate(_pearlPrefab, _pearlSpawnPoint.position, Quaternion.identity);
        pearl.GetComponent<Rigidbody2D>().AddForce(new Vector2(_pearlShootForce * -transform.localScale.x, 0));
        _animator.SetTrigger("Fire");
    }

    private void OnDrawGizmos()
    {
        DrawRect(_rangeColliderPoint1, _rangeColliderPoint2, Color.red);
        DrawRect(_closeColliderPoint1, _closeColliderPoint2, Color.blue);
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
