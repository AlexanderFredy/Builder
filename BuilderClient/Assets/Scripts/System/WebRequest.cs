using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class WebRequest : Singleton<WebRequest>
{
    public void Post(string uri, Dictionary<string, string> data, Action<string> success, Action<string> error = null) 
        => StartCoroutine(PostAsync(uri, data, success, error));
    private IEnumerator PostAsync(string uri, Dictionary<string, string> data, Action<string> success, Action<string> error = null)
    {
        using (UnityWebRequest www = UnityWebRequest.Post(uri, data))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success) error?.Invoke(www.error);
            else success?.Invoke(www.downloadHandler.text);
        }
    }
}
