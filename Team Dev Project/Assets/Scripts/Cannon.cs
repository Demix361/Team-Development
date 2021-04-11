using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Cannon : NetworkBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Animator _smokeAnimator;
    [SerializeField] private GameObject _ballPrefab;
    [SerializeField] private float _shootInterval;
    [SerializeField] private float _shootOffset;
    [SerializeField] private float _shootForce;
    [SerializeField] private Transform _ballSpawnPoint;

    private float count = 0;

    [Command(requiresAuthority = false)]
    private void CmdShoot()
    {
        RpcShoot();
    }

    [ClientRpc]
    private void RpcShoot()
    {
        GameObject ball = Instantiate(_ballPrefab, _ballSpawnPoint.position, Quaternion.identity);
        ball.GetComponent<Rigidbody2D>().AddForce(new Vector2(_shootForce * -transform.localScale.x, 0));
        //ball.GetComponent<CannonBall>().Move(_shootForce * -transform.localScale.x);
        _animator.SetTrigger("Fire");
        _smokeAnimator.SetTrigger("Fire");
    }

    private void Update()
    {
        if (isServer)
        {
            count += Time.deltaTime;
            if (count > _shootInterval)
            {
                count = 0;
                CmdShoot();
            }
        }
    }
}
