using Mirror;
using Cinemachine;
using UnityEngine;

public class PlayerCameraFollow : NetworkBehaviour
{
    private ICinemachineCamera iVcam;

    [Client]
    void Start()
    {
        iVcam = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera;

        if (hasAuthority)
        {
            if (iVcam.Follow == null)
            {
                iVcam.Follow = transform;
            }
        }
    }
}
