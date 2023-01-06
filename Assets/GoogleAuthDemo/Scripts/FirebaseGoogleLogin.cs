using Firebase.Auth;
using Firebase.Extensions;
using Google;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FirebaseGoogleLogin : MonoBehaviour
{
    private string GoogleWebAPI = "738023273775-j74bnsie9eoq47e0vevtafkm0hg7vkqh.apps.googleusercontent.com";
    private GoogleSignInConfiguration configuration;
    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
    FirebaseAuth auth;
    FirebaseUser user;

    public TextMeshProUGUI UsernameText, UserMailText;
    public Image UserProfileImage;

    public string imageURL;

    public GameObject LoginPanel, ProfilePanel;

    private void Awake()
    {
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = GoogleWebAPI,
            RequestIdToken = false
        };
    }


    private void Start()
    {
        InitFirebase();

        if (auth.CurrentUser!=null)
        {
            GetUser();


            StartCoroutine(LoadImage(CheckImageURL(user.PhotoUrl.ToString())));
        }
        else
        {
            LoginPanel.SetActive(true);
            ProfilePanel.SetActive(false);
        }
       
    }

    private void InitFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
    }

    public void GoogleSignInButton() 
    {
        GoogleSignIn.Configuration=configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        GoogleSignIn.Configuration.RequestEmail = true;

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnGoogleAuthenticationFinished);
    }
    public void LogOut()
    {
        auth.SignOut();
        LoginPanel.SetActive(true);
        ProfilePanel.SetActive(false);
    }
    private void OnGoogleAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            Debug.LogError("Fault : "+task.Result);
        }
        else if (task.IsCanceled)
        {
            Debug.LogError("Canceled : " + task.Result);
        }
        else
        {
            Credential credential = GoogleAuthProvider.GetCredential(task.Result.IdToken,null);

            auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task => {

                if (task.IsCanceled)
                {
                    Debug.LogError("Firebase Fault : " + task.Result);

                }
                else if (task.IsCanceled)
                {
                    Debug.LogError("Firebase Canceled : " + task.Result);
                }

                GetUser();

            });
        }
    }
    void GetUser() 
    {
        user = auth.CurrentUser;
        UsernameText.text = user.DisplayName;
        UserMailText.text = user.Email;

        LoginPanel.SetActive(false);
        ProfilePanel.SetActive(true);

        StartCoroutine(LoadImage(CheckImageURL(user.PhotoUrl.ToString())));
    }
    private string CheckImageURL(string url) 
    {
        if (!string.IsNullOrEmpty(url))
        {
            return url;
        }

        return "";
    }

    IEnumerator LoadImage(string imageUri) 
    {
        WWW www = new WWW(imageUri);
        yield return www;

        UserProfileImage.sprite = Sprite.Create(www.texture,new Rect(0,0,www.texture.width,www.texture.height),new Vector2(0,0));
    }
}
