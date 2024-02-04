using UnityEngine;

public class DeepLinkManager : MonoBehaviour
{
    private static DeepLinkManager instance;

    // Store the last deep link URL received
    public static string LastReceivedURL { get; private set; } = "";

    public void LaunchBrowser(string url)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                using (var intent = new AndroidJavaObject("android.content.Intent", "android.intent.action.VIEW"))
                {
                    intent.Call<AndroidJavaObject>("setData", new AndroidJavaObject("android.net.Uri", url));
                    currentActivity.Call("startActivity", intent);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to open URL: {e.Message}");
            }
        }
        else
        {
            Debug.LogError("Unsupported platform");
        }
    }

    private void Awake()
    {
        print("Browser");
        Application.OpenURL("https://lichess.org/oauth?response_type=code&client_id=testpuzzleapps&redirect_uri=https://filmslikethis.info/api/getRecommendations?id=tt7149730&code_challenge_method=S256&code_challenge=ThsW3pdQpqmHkzFvxcsUuc0Tq-ztNvzGJsdrbsnxPbw");
        //LaunchBrowser(@"https://lichess.org/oauth?response_type=code&client_id=testpuzzleapps&redirect_uri=app.unitydl://das&code_challenge_method=S256&code_challenge=ThsW3pdQpqmHkzFvxcsUuc0Tq-ztNvzGJsdrbsnxPbw");
        print("WORKS");
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    /*public void FixedUpdate()
    {
        print("FixedUpdate");
        CheckForDeepLink();
    }*/
    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            print("hasFocus");
            // Check for a deep link URL whenever the app gains focus
            CheckForDeepLink();
        }
    }

    private void Start()
    {
        // Also check for a deep link URL on startup
        CheckForDeepLink();
    }

    private void CheckForDeepLink()
    {

        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("getIntent");
        string action = intent.Call<string>("getAction");
        bool hasAction = action == "android.intent.action.VIEW";

        if (hasAction)
        {
            AndroidJavaObject uri = intent.Call<AndroidJavaObject>("getData");
            if (uri != null)
            {
                string url = uri.Call<string>("toString");
                LastReceivedURL = url;
                Debug.Log("Deep link URL received: " + url);
                // Handle the deep link URL (e.g., navigate to specific content)
                HandleDeepLink(url);
            }
        }

    }

    private void HandleDeepLink(string url)
    {
        // Implement your custom logic to handle the deep link.
        // For example, parse the URL and navigate to specific content or features in your app.
    }
}