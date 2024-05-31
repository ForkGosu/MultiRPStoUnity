using System.Collections;
using UnityEngine;
using TMPro;

public class SplashController : MonoBehaviour
{
    private WaitForSeconds waitOne = new WaitForSeconds(1f);
    private WaitForSeconds waitZeroPointTwo = new WaitForSeconds(0.2f);

    [SerializeField]
    private CanvasGroup splashObject;
    
    [SerializeField]
    private TextMeshProUGUI descriptionText;
    [SerializeField]
    private string descriptionString;

    void Start()
    {
        // 처음엔 안보이게 초기화
        splashObject.alpha = 0f;
        descriptionText.alpha = 0f;

        // Splash 일련의 과정을 실행
        StartCoroutine(SplashAction());
    }

    private IEnumerator SplashAction()
    {
        // Splash Image가 점점 보이게 됨
        yield return StartCoroutine(ShowSplash());

        // 로딩중 설명을 위해 텍스트를 보이게 하고 ...을 붙힘
        StartCoroutine(LoadingTextAnimation());

        // 서버 접속 되었는지 확인
        yield return StartCoroutine(CheckServerConnection());

        // 서버 연결 안된다하고 끊기
        if(!WebSocketManager.Instance.IsConnect){
            AlertWindowManager.Instance.Alert("서버에 접속이 되지 않습니다.\n앱을 종료합니다.", () => AppManager.Instance.AppQuit());
            yield break;
        }

        // 앱 버전 맞는지 확인
        yield return StartCoroutine(CheckAppVersion());
    }

    private IEnumerator ShowSplash()
    {
        while(splashObject.alpha < 1f){
            splashObject.alpha += Time.deltaTime;
            yield return null;
        }
    }
    
    // 텍스트 처리
    private IEnumerator LoadingTextAnimation()
    {
        descriptionText.alpha = 1f;

        while(true){
            for (int i = 0; i <= 3; i++)
            {
                // 텍스트에 점 추가
                descriptionText.text = descriptionString + new string('.', i);
                yield return waitZeroPointTwo;
            }
        }
    }

    // 서버 접속 되었는지 확인
    private IEnumerator CheckServerConnection()
    {
        descriptionString = "서버 접속 확인중";
        int connectingCount = 0;
        
        while(!WebSocketManager.Instance.IsConnect && connectingCount < 5){
            yield return WebSocketManager.Instance.WebSocketConnect();
            descriptionString = $"서버 접속 확인중(재 접속 {++connectingCount}회)";
        }
    }
    
    // 앱 버전 맞는지 확인
    private IEnumerator CheckAppVersion()
    {
        descriptionString = "앱 버전 확인중";

        SplashManager.Instance.SendAppVersionCheck();

        // 버전이 맞을때 까지 대기
        yield return new WaitUntil(() => AppManager.Instance.IsAppVersion);
    }

}
