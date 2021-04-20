using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EnemyHealth : NetworkBehaviour
{
    [SerializeField] private float _maxHealth;
    [SerializeField] private bool _invincible = false;
    [SyncVar] private float _currentHealth;

    public override void OnStartServer()
    {
        SetHealth(_maxHealth);
    }

    // Получение урона
    [Command(requiresAuthority = false)]
    public void CmdDealDamage(float damage)
    {
        if (_invincible)
            return;

        RpcHitAnimation();
        SetHealth(Mathf.Max(_currentHealth - damage, 0));
    }

    [ClientRpc]
    private void RpcHitAnimation()
    {
        GetComponent<Animator>().SetTrigger("Hit");
    }

    [Server]
    private void SetHealth(float value)
    {
        if (value == 0)
        {
            Destroy(gameObject);
        }
        _currentHealth = value;
    }

}
