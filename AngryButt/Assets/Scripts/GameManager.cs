using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    public int width, height;
    public normalFace normalFace;
    public AngryFace angryFace;
    public Transform startPos;
    public Transform endPopup;
    public Transform winPopup;
    public Transform newGameLoser;
    public Transform newGameWinner;
    public TextMeshProUGUI scoreText1;
    public TextMeshProUGUI scoreText2;
    public TextMeshProUGUI t;
    public Transform loadingScreen;
    public Transform discPopUp;
    public DataBaseManager db;
    public List<normalFace> faces;
    public TurnManager turnManager;
    [NonSerialized]
    public string roomCode = "";
    public int[] score;

    void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();
        score[0] = 0;
        score[0] = 0;


    }
    public override void OnConnectedToMaster()
    {
        db.ReadGameCode();

    }
    public override void OnJoinedLobby()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.JoinOrCreateRoom(roomCode, roomOptions, TypedLobby.Default);

    }
    public override void OnJoinedRoom()
    {
        Debug.Log("I joined the " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("My ID Is " + PhotonNetwork.LocalPlayer.ActorNumber);
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                t.text = "YOUR TURN";
                int angryFaceX = UnityEngine.Random.Range(0, width);
                int angryFaceY = UnityEngine.Random.Range(0, height);
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (x == angryFaceX && y == angryFaceY)
                        {
                            GameObject newAngryFace = PhotonNetwork.Instantiate("angryFace", new Vector3(startPos.position.x + x, startPos.position.y + y, 0), Quaternion.identity);
                            newAngryFace.name = "Face " + x.ToString() + "," + y.ToString();
                        }
                        else
                        {
                            GameObject newFace = PhotonNetwork.Instantiate("normalFace", new Vector3(startPos.position.x + x, startPos.position.y + y, 0), Quaternion.identity);
                            newFace.name = "Face " + x.ToString() + "," + y.ToString();
                            faces.Add(newFace.GetComponent<normalFace>());
                            Debug.Log(faces.Count);
                        }
                    }
                }
                // db.deleteGameCode();
                roomCode = null;
                loadingScreen.GetComponent<LoadingScreen>().disableScreen();
            }
            else
            {
                photonView.RPC("RPC_generateFaces", RpcTarget.MasterClient);
                t.text = "OPPONENT'S TURN";
            }
        }
        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
    }
    public void endGame()
    {
        FindObjectOfType<SoundManager>().Play("Lose");

        if (!turnManager.IsMyTurn())
        {
            if (PhotonNetwork.IsMasterClient)
            {
                score[0] += 1;
                photonView.RPC("RPC_UpdateScore1", RpcTarget.All, score[0]);
            }
            else
            {
                score[1] += 1;
                photonView.RPC("RPC_UpdateScore2", RpcTarget.All, score[1]);
            }
            photonView.RPC("checkScore", RpcTarget.All);

        }
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (normalFace face in faces)
            {

                PhotonNetwork.Destroy(face.gameObject);
            }
            faces.Clear();
            Debug.Log("faces deleted");

        }
        else
            photonView.RPC("RPC_DeleteFaces", RpcTarget.MasterClient);


    }

    [PunRPC]
    public void checkScore()
    {
        bool isScore2 = false;
        if (score[0] == 2 || score[1] == 2)
        {
            isScore2 = true;
            t.gameObject.SetActive(false);
        }

        if (isScore2)
        {
            if (turnManager.IsMyTurn())
                endPopup.gameObject.SetActive(true);
            else
                winPopup.gameObject.SetActive(true);

        }

        else
        {
            if (turnManager.IsMyTurn())
                newGameLoser.gameObject.SetActive(true);
            else
                newGameWinner.gameObject.SetActive(true);
        }



    }



    [PunRPC]
    private void RPC_DeleteFaces()
    {
        foreach (normalFace face in faces)
        {
            PhotonNetwork.Destroy(face.gameObject);
        }
        faces.Clear();
        Debug.Log("faces deleted");
    }


    [PunRPC]
    private void RPC_UpdateScore1(int score1)
    {
        score[0] = score1;
        scoreText1.text = score[0].ToString();
    }
    [PunRPC]
    private void RPC_UpdateScore2(int score1)
    {
        score[1] = score1;
        scoreText2.text = score[1].ToString();
    }

    public void QuitGmae()
    {
        Application.Quit();
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if (!otherPlayer.IsLocal)
        {
            discPopUp.gameObject.SetActive(true);

        }
    }


    [PunRPC]
    public void RPC_generateFaces()
    {
        t.text = "YOUR TURN";
        int angryFaceX = UnityEngine.Random.Range(0, width);
        int angryFaceY = UnityEngine.Random.Range(0, height);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == angryFaceX && y == angryFaceY)
                {
                    GameObject newAngryFace = PhotonNetwork.Instantiate("angryFace", new Vector3(startPos.position.x + x, startPos.position.y + y, 0), Quaternion.identity);
                    newAngryFace.name = "Face " + x.ToString() + "," + y.ToString();
                }
                else
                {
                    GameObject newFace = PhotonNetwork.Instantiate("normalFace", new Vector3(startPos.position.x + x, startPos.position.y + y, 0), Quaternion.identity);
                    newFace.name = "Face " + x.ToString() + "," + y.ToString();
                    faces.Add(newFace.GetComponent<normalFace>());
                    Debug.Log(faces.Count);
                }
            }

        }
        // db.deleteGameCode();
        roomCode = null;
        loadingScreen.GetComponent<LoadingScreen>().disableScreen();
    }


    [PunRPC]
    public void RPC_newGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            int angryFaceX = UnityEngine.Random.Range(0, width);
            int angryFaceY = UnityEngine.Random.Range(0, height);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x == angryFaceX && y == angryFaceY)
                    {
                        GameObject newAngryFace = PhotonNetwork.Instantiate("angryFace", new Vector3(startPos.position.x + x, startPos.position.y + y, 0), Quaternion.identity);
                        newAngryFace.name = "Face " + x.ToString() + "," + y.ToString();
                    }
                    else
                    {
                        GameObject newFace = PhotonNetwork.Instantiate("normalFace", new Vector3(startPos.position.x + x, startPos.position.y + y, 0), Quaternion.identity);
                        newFace.name = "Face " + x.ToString() + "," + y.ToString();
                        faces.Add(newFace.GetComponent<normalFace>());
                        Debug.Log(faces.Count);
                    }
                }

            }
        }

        if (newGameLoser.gameObject.activeSelf)
            newGameLoser.gameObject.SetActive(false);
        else
            newGameWinner.gameObject.SetActive(false);

    }
    public void newGame()
    {
        photonView.RPC("RPC_newGame", RpcTarget.All);
    }

}
