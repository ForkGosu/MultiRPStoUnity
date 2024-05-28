using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum AppRunOS
{
    none,
    window,
    android,
    ios,
    web,
}

public class AppManager : MonoSingleton<AppManager>
{
#region 서버와 연동하는 데이터
    [Header("서버와 연동하는 데이터")]
    public AppRunOS appRunOS;
    public string appName;
    public string appVersion;
    public string userUUID;

#endregion

#region 클라이언트에서 저장하고 있을 데이터
    [Header("클라이언트에서 저장하고 있을 데이터")]
    [SerializeField]
    private string appCurrentScene;

    private bool isAppVersion = false;
    public bool IsAppVersion { get{ return isAppVersion; } set { isAppVersion = value; } }

#endregion

    void Awake(){
        // 저장되어 있는 UUID가 있다면 불러오기
        userUUID = SecurePlayerPrefs.GetString("UUID");

        // 처음엔 되도록 "Splash"으로 씬 로딩 할 것
        LoadSceneAdditiveAsync(appCurrentScene);
    }

//* 클라이언트 종료 함수

    public void AppQuit(){
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit(); // 어플리케이션 종료
    #endif
    }


#region 다른 곳에서 싱글턴으로 씬 로딩을 도와 줄 함수
    public void LoadSceneAsync(string _sceneName){
        StartCoroutine(LoadSceneAsyncCoroutine(_sceneName));
    }
    private IEnumerator LoadSceneAsyncCoroutine(string _sceneName){
        yield return UnloadSceneAsync(appCurrentScene);
        appCurrentScene = _sceneName;
        yield return LoadSceneAdditiveAsync(_sceneName);
    }

#endregion

#region 비동기로 씬을 추가로 로딩하는 함수
    private Coroutine LoadSceneAdditiveAsync(string _sceneName)
    {
        return StartCoroutine(LoadSceneAdditiveAsyncCoroutine(_sceneName));
    }
    private IEnumerator LoadSceneAdditiveAsyncCoroutine(string _sceneName)
    {
        // 비동기로 씬을 추가로 로드하고 로딩 상태를 추적하는 AsyncOperation 객체를 얻습니다.
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Additive);

        // 씬이 완전히 로드될 때까지 대기합니다.
        while (!asyncOperation.isDone)
        {
            // 로딩 진행 상황을 출력하거나 다른 작업을 수행할 수 있습니다.
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);

            yield return null; // 다음 프레임까지 대기합니다.
        }

        // 로드된 씬을 활성화 된 씬으로 설정
        SetActiveScene(_sceneName);
    }
#endregion

#region 비동기로 로딩된 씬을 언로딩하는 함수
    private Coroutine UnloadSceneAsync(string _sceneName)
    {
        return StartCoroutine(UnloadSceneAsyncCoroutine(_sceneName));
    }

    private IEnumerator UnloadSceneAsyncCoroutine(string _sceneName)
    {
        // 추가로 로딩한 씬을 언로딩하고 로딩 상태를 추적하는 AsyncOperation 객체를 얻습니다.
        AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(_sceneName);

        // 씬이 완전히 언로딩될 때까지 대기합니다.
        while (!asyncOperation.isDone)
        {
            // 언로딩 진행 상황을 출력하거나 다른 작업을 수행할 수 있습니다.
            float progress = Mathf.Clamp01(asyncOperation.progress);

            yield return null;
        }

        // 자동으로 SetActiveScene가 App 씬으로 인식함
    }
#endregion

#region 씬 활성화 함수
    private void SetActiveScene(string _sceneName){
        
        // 로드된 씬에 접근
        Scene loadedScene = SceneManager.GetSceneByName(_sceneName);
        if (loadedScene.IsValid())
        {
            // 로드된 씬을 활성 씬으로 설정
            SceneManager.SetActiveScene(loadedScene);
        }
    }
#endregion

}
