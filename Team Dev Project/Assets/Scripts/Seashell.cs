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
    [SerializeField] private float _shootInterval;
    [SerializeField] private float _biteInterval;

    [Header("Pearl")]
    [SerializeField] private GameObject _pearlPrefab;
    [SerializeField] private Transform _pearlSpawnPoint;
    [SerializeField] private float _pearlShootForce;

    private float _counter;
    private bool _haveBitten;

    private void Update()
    {
        if (isServer)
        {
            _counter += Time.deltaTime;
            _haveBitten = false;

            Collider2D[] colliders = Physics2D.OverlapAreaAll(_closeColliderPoint1.position, _closeColliderPoint2.position, _layerMask);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].CompareTag("Player"))
                {
                    if (transform.localScale.x > 0 && colliders[i].transform.position.x > transform.position.x)
                    {
                        Flip();
                    }
                    else if (transform.localScale.x < 0 && colliders[i].transform.position.x < transform.position.x)
                    {
                        Flip();
                    }

                    if (_counter > _biteInterval)
                    {
                        _counter = 0;
                        Bite();
                        _haveBitten = true;
                    }
                }
            }

            colliders = Physics2D.OverlapAreaAll(_rangeColliderPoint1.position, _rangeColliderPoint2.position, _layerMask);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].CompareTag("Player"))
                {
                    if (_counter > _shootInterval && _haveBitten == false)
                    {
                        _counter = 0;
                        CmdShoot();
                    }
                }
            }

            


        }
    }

    private void Bite()
    {
        _animator.SetTrigger("Bite");
    }

    private void Flip()
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
