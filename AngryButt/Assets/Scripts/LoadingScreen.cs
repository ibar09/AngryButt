using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LoadingScreen : MonoBehaviour
{
    public PhotonView photonView;



    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }
    public void disableScreen()
    {
        photonView.RPC("DS", RpcTarget.All);
    }
    [PunRPC]
    public void DS()
    {
        gameObject.SetActive(false);
    }
}
