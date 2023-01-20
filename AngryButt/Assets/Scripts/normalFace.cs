using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
public class normalFace : MonoBehaviour
{
    TouchPhase touchPhase = TouchPhase.Ended;
    private PhotonView photonView;
    public TurnManager turnManager;
    public GameManager gameManager;
    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        turnManager = FindObjectOfType<TurnManager>();
        gameManager = FindObjectOfType<GameManager>();
    }
    private void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == touchPhase)
        {

            RaycastHit2D raycastHit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position), Vector2.zero);
            if (raycastHit2D.collider != null)
            {
                if (raycastHit2D.transform.gameObject == gameObject && gameObject != null)
                {

                    if (turnManager.IsMyTurn())
                    {
                        if (PhotonNetwork.IsMasterClient)
                            FindObjectOfType<SoundManager>().Play("Click1");
                        else
                            FindObjectOfType<SoundManager>().Play("Click2");
                        if (photonView.IsMine)
                        {
                            gameManager.faces.Remove(this);
                            turnManager.NextTurn();
                            PhotonNetwork.Destroy(gameObject);

                        }
                        else
                        {
                            turnManager.NextTurn();
                            photonView.RPC("DestroyObject", RpcTarget.MasterClient);
                        }
                    }
                }
            }
        }
    }


    [PunRPC]
    public void DestroyObject()
    {
        if (gameObject != null)
        {
            gameManager.faces.Remove(this);
            PhotonNetwork.Destroy(gameObject);
        }
    }


}
