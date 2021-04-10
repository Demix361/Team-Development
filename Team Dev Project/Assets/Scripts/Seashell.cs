using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Seashell : NetworkBehaviour
{
    [SerializeField] private Transform _overlapPoint1;
    [SerializeField] private Transform _overlapPoint2;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _shootInterval;

    private float _counter;

    private void Update()
    {
        if (isServer)
        {
            _counter += Time.deltaTime;

            Collider2D[] colliders = Physics2D.OverlapAreaAll(_overlapPoint1.position, _overlapPoint2.position, _layerMask);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].CompareTag("Player"))
                {
                    if (_counter > _shootInterval)
                    {
                        _counter = 0;
                        Shoot();
                    }
                }
            }

                    
        }
    }

    private void Shoot()
    {
        _animator.SetTrigger("Fire");
    }
}
