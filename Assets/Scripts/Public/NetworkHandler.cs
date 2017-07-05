using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class NetworkHandler : MonoBehaviour {

    public static NetworkHandler instance;

    private delegate DateTime myCallbackDelegate(bool convertToLocalTime);
    private static DateTime serverResponse;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public static void GetNetworkUTCTimeAsync()
    {
        myCallbackDelegate deleg = new myCallbackDelegate(new AsyncTasks().GetNISTDate);
        deleg.BeginInvoke(false, new AsyncCallback(OnServerResponse), null);
    }

    private static void OnServerResponse(IAsyncResult result)
    {
        AsyncResult res = (AsyncResult)result;
        myCallbackDelegate deleg = (myCallbackDelegate)res.AsyncDelegate;
        serverResponse = deleg.EndInvoke(result);
        Debug.Log("GOT RESPONSE FROM SERVER: " + serverResponse);
        GameDataManager.instance.NotifyAsync(serverResponse);
    }
}
