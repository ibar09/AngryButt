using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class TurnManager : MonoBehaviour
{
    // The ID of the current player's turn
    public int CurrentPlayer = 1;
    private PhotonView p;
    public TextMeshProUGUI t;

    private void Start()
    {
        p = GetComponent<PhotonView>();
    }
    // Next player turn 

    public void NextTurn()
    {

        if (PhotonNetwork.IsMasterClient)
        {
            CurrentPlayer = (CurrentPlayer + 1) % PhotonNetwork.PlayerList.Length;

            if (CurrentPlayer == 0)
            { CurrentPlayer = 2; }
            p.RPC("RPC_UpdateCurrentPlayer", RpcTarget.All, CurrentPlayer);
            t.text = "OPPONENT'S TURN";
            p.RPC("RPC_changeTurnText", RpcTarget.Others);
            Debug.Log("NextTurn() output" + CurrentPlayer);
        }
        else
        {
            t.text = "OPPONENT'S TURN";
            p.RPC("RPC_changeTurnText", RpcTarget.MasterClient);
            p.RPC("RPC_NextTurn", RpcTarget.MasterClient);
        }
    }

    // Check if it is my turn
    public bool IsMyTurn()
    {
        Debug.Log("isMyTurn() output" + CurrentPlayer);
        return CurrentPlayer == PhotonNetwork.LocalPlayer.ActorNumber;
    }

    [PunRPC]
    private void RPC_UpdateCurrentPlayer(int player)
    {
        CurrentPlayer = player;
    }

    [PunRPC]
    private void RPC_NextTurn()
    {
        CurrentPlayer = (CurrentPlayer + 1) % PhotonNetwork.PlayerList.Length;

        if (CurrentPlayer == 0)
        { CurrentPlayer = 2; }
        p.RPC("RPC_UpdateCurrentPlayer", RpcTarget.All, CurrentPlayer);
        /*  t.text = "OPPENENT'S TURN";
          p.RPC("RPC_changeTurnText", RpcTarget.MasterClient);*/
        Debug.Log("NextTurn() output" + CurrentPlayer);
    }

    [PunRPC]
    public void RPC_changeTurnText()
    {
        Debug.Log("NOOOTIIIIIIIIIIIIICEEEE MEEEEE      :   " + PhotonNetwork.IsMasterClient);
        if (t.text == "OPPONENT'S TURN")
            t.text = "YOUR TURN";

    }
}
