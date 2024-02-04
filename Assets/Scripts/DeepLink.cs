using UnityEngine;

public class DeepLinkManager : MonoBehaviour
{
    private static DeepLinkManager instance;

    // Store the last deep link URL received
    public static string LastReceivedURL { get; private set; } = "";

  
    private void Awake()
    {

        //Application.OpenURL("https://lichess.org/oauth?response_type=code&client_id=testpuzzleapps&redirect_uri=https://filmslikethis.info/api/getRecommendations?id=tt7149730&code_challenge_method=S256&code_challenge=ThsW3pdQpqmHkzFvxcsUuc0Tq-ztNvzGJsdrbsnxPbw");
        //LaunchBrowser(@"https://lichess.org/oauth?response_type=code&client_id=testpuzzleapps&redirect_uri=app.unitydl://das&code_challenge_method=S256&code_challenge=ThsW3pdQpqmHkzFvxcsUuc0Tq-ztNvzGJsdrbsnxPbw");
    }
}