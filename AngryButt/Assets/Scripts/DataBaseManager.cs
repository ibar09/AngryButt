using Firebase;
using Firebase.Database;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections;
using System;
using System.Collections.Generic;
using Photon.Pun;

public class DataBaseManager : MonoBehaviour
{
    private DatabaseReference databaseReference;
    private bool isFirebaseInitialized;
    public GameManager gameManager;
    private void Awake()
    {

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }
    public IEnumerator ReadTable(Action<string> onCallback)
    {

        Debug.Log("check 1");
        var task = databaseReference.Child("games").Child("gameCode").GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);
        Debug.Log("check 2");
        if (task.IsCompleted)
        {
            Debug.Log("check 3");
            DataSnapshot snapshot = task.Result;
            onCallback.Invoke((string)snapshot.Value);

        }
        else if (task.IsFaulted)
        {
            // Handle any errors that occurred
            Debug.LogError("An error occurred while reading data from the database: " + task.Exception);
        }


    }

    public void deleteGameCode()
    {
        databaseReference.Child("games").Child("gameCode").RemoveValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Value deleted successfully.");
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("Error deleting value: " + task.Exception);
            }
        });
    }

    public void ReadGameCode()
    {
        StartCoroutine(ReadTable((string c) =>
        {
            gameManager.roomCode = c;
            PhotonNetwork.JoinLobby();
        }));


    }

}