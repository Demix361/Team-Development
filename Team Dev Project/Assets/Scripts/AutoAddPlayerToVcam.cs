using UnityEngine;
using Cinemachine;
using Mirror;


public class AutoAddPlayerToVcam : NetworkBehaviour
{
    void Update()
    {
        /*
        var vcam = GetComponent<CinemachineVirtualCameraBase>();

        GameObject localPlayer = ClientScene.localPlayer.gameObject;

        if (vcam != null && Tag.Length > 0)
        {
            var targets = GameObject.FindGameObjectsWithTag(Tag);
            if (targets.Length > 0)
                vcam.LookAt = vcam.Follow = localPlayer.transform;//targets[0].transform;
        }
        */
        var vcam = GetComponent<CinemachineVirtualCameraBase>();
        GameObject localPlayer = ClientScene.localPlayer.gameObject;

        if (vcam != null && localPlayer != null)
        {
            vcam.LookAt = vcam.Follow = localPlayer.transform;
        }
    }
    

    /*
    public override void OnStartAuthority()
    {
        var vcam = GetComponent<CinemachineVirtualCameraBase>();
        if (vcam != null && Tag.Length > 0)
        {
            var targets = GameObject.FindGameObjectsWithTag(Tag);
            if (targets.Length > 0)
                vcam.LookAt = vcam.Follow = targets[0].transform;
        }
    }
    */
}
