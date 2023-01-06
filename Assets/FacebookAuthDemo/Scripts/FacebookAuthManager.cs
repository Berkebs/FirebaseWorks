using Facebook.Unity;
using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FacebookAuthManager : MonoBehaviour
{

    FirebaseAuth auth;
    FirebaseUser user;
    public GameObject Status;
    public TextMeshProUGUI UsernameText;

    private void Awake()
    {
    }
    private void Start()
    {
        InitFirebase();

        if (auth.CurrentUser!=null)
        {
            FB.Init(SetInit);
            Debug.Log("User is null");
        }
        else
        {
            GetUser(auth.CurrentUser.DisplayName);
        }

    }
    private void InitFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
    }
    void SetInit() 
    {
        if (FB.IsLoggedIn)
        {
            Debug.Log("Logged in Successfuly");
        }
        else
        {
            Debug.Log("FB is not logged in");

        }
    }

    public void FacebookLogin() 
    {
        var permission = new List<string>() { "public_profile" };
        FB.LogInWithReadPermissions(permission,AuthCallback);
    }

    private void AuthCallback(ILoginResult result) 
    {
        var aToken = AccessToken.CurrentAccessToken;
        OnFacebookAuthenticationFinished(aToken.TokenString);
        GetUser(aToken.UserId);
        Debug.Log(result.RawResult);
    }
    void OnFacebookAuthenticationFinished(string accessToken) 
    {

        Credential credential =
        FacebookAuthProvider.GetCredential(accessToken);
        auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);

            Status.SetActive(true);
            GetUser(newUser.DisplayName);
        });
    }

    void GetUser(string Name) 
    {
        UsernameText.text = name;

    }
}
