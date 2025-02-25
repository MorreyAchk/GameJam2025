using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TriggerTransition : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if(IsClient)
            StartCoroutine(GlobalBehaviour.Instance.LoadOutLevel(null));
    }
}
