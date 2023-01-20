using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
public class AngryFace : MonoBehaviour
{
    TouchPhase touchPhase = TouchPhase.Ended;
    public TextMeshProUGUI t;
    private PhotonView photonView;
    TurnManager turnManager;
    public GameManager gameManager;
    private void Start()
    {
        turnManager = FindObjectOfType<TurnManager>();
        photonView = GetComponent<PhotonView>();
        t = FindObjectOfType<TextMeshProUGUI>();
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
                    {
                        if (turnManager.IsMyTurn())
                        {

                            if (PhotonNetwork.IsMasterClient)
                            {
                                photonView.RPC("endGame", RpcTarget.All);
                                PhotonNetwork.Destroy(gameObject);
                            }
                            else
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
        photonView.RPC("endGame", RpcTarget.All);
        if (gameObject != null)
            PhotonNetwork.Destroy(gameObject);

    }

    [PunRPC]
    public void endGame()
    {
        gameManager.endGame();
    }

    public void changeTurnText()
    {
        if (t.text == "OPPENENT'S TURN")
            t.text = "YOUR TURN";
    }


}
