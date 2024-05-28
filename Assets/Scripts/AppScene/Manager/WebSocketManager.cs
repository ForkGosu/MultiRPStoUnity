using System;
using UnityEngine;
using NativeWebSocket;
using Google.Protobuf;

using ProtoBuf.ProtoBufPacket;
using System.Collections;

public class WebSocketManager : MonoSingleton<WebSocketManager>
{
#region 서버와 연동하는 데이터
    [Header("서버와 연동하는 데이터")]
    private WebSocket webSocket; // 유니티 패키지인 Native WebSocket을 이용

    public string webSocketDevUrl = "ws://127.0.0.1:48055";
    public string webSocketProdUrl = "ws://54.180.79.84:48055";
    public bool isDev = true;
    
    private Action sendAction; // 보낸 데이터를 받았을 때 잘 받기 위함
    private Action<PayloadClass> receiveAction; // 보낸 데이터를 받았을 때 잘 받기 위함

#endregion

#region 클라이언트에서 저장하고 있을 데이터
    [Header("클라이언트에서 저장하고 있을 데이터")]
    private bool isAppRun = false;

    private bool isConnect = false;
    public bool IsConnect { get { return isConnect; } }
    private bool isReConnecting = false;


#endregion

    void Awake()
    {
        isAppRun = true;

        isConnect = false;
        isReConnecting = false;
        InitWebSocket();
    }
    
    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        webSocket.DispatchMessageQueue();
#endif
    }

    async void OnApplicationQuit()
    {
        isAppRun = false;
        await webSocket.Close(); // Task로 열려서 직접 닫아줘야 함
    }

    void OnApplicationPause(bool pause){
        if(pause) {
            //SendReConnection(PayloadType.Disconnect);
            // Debug.Log("백그라운드로 나감");
        } else {
            //SendReConnection(PayloadType.Reconnect);
            // Debug.Log("어플로 돌아옴");
        }
    }


#region 웹소켓 초기화 함수
    void InitWebSocket(){
        string url = isDev ? webSocketDevUrl : webSocketProdUrl;
        webSocket = new WebSocket(url);

        webSocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
            
            isConnect = true; // 연결 확인

            if(!isAppRun){ WebSocketClose(); } // 연결 되었을 때 앱이 종료됬다면 웹소켓 닫기

            // 저장된 sendAction실행 후 다 제거
            sendAction?.Invoke();
            sendAction = null;
        };

        webSocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        webSocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
            
            isConnect = false; // 연결 확인

            // 버전 확인하고 인게임에 들어온 상태 일 경우 재연결 시도
            if(AppManager.Instance.IsAppVersion && !isReConnecting){ StartCoroutine(ServerReConnection()); };
        };

        // 메세지 받았을 때
        webSocket.OnMessage += (bytes) =>
        {
            PayloadClass payload = PayloadClass.Parser.ParseFrom(bytes);
            
            Debug.Log("데이터 Receive : " + payload.ToString());

            // 5초 이상인 패킷은 다 버리기
            
            // receiveAction에 저장된Class들에게 payload전달
            receiveAction?.Invoke(payload);
        };
    }
#endregion


#region 웹소켓 연결 및 닫기 함수

    // 서버 접속 되었는지 확인
    private IEnumerator ServerReConnection(int _maxConnecting = 5)
    {
        isReConnecting = true;

        int connectingCount = 0;
        
        while(!IsConnect && connectingCount < _maxConnecting){
            connectingCount++;
            yield return WebSocketConnect();
            // TODO : 재접속 될 때 까지 로딩 이미지 붙혀 넣을 것
            // descriptionString = $"서버 접속 확인중(재 접속 {++connectingCount}회)";
            LoadingScreenManager.Instance.AddLoadingText("ReConnection");
        }

        isReConnecting = false;
        LoadingScreenManager.Instance.RemoveLoadingText("ReConnection");

        // 만약 연결되지 못했다면 서버 종료
        if(!isConnect){
            AlertWindowManager.Instance.Alert("서버에 접속되지 못했습니다.\n앱을 종료합니다.", () => AppManager.Instance.AppQuit());
            isReConnecting = true;
        }
    }
    // 코루틴을 활용
    public Coroutine WebSocketConnect()
    {
        return StartCoroutine(WebSocketConnectCoroutine());
    }
    private IEnumerator WebSocketConnectCoroutine(){
        // 연결이 안되어 있을 경우만 연결
        if(webSocket.State != WebSocketState.Open && isAppRun)
        {
            bool isTaskDone = false;
            
            webSocket.Connect().ContinueWith((completedTask) => {
                isTaskDone = true;
            });

            // Task의 완료를 코루틴에서 기다림
            yield return new WaitUntil(() => isTaskDone | isConnect);
        }
    }
    
    public async void WebSocketClose()
    {
        // isReConnecting을 true로 만들어서 웹소켓 Close시 재연결 하는 것을 막는다
        isReConnecting = true;

        await webSocket.Close();
    }

#endregion


#region 웹소켓을 Receive 해서 Class들에게 데이터 전달하기 위해 필요한 함수
    public void AddWebSocketReceiveMessage(Action<PayloadClass> _receiveAction){
        this.receiveAction += _receiveAction;
    }
    
    public void DeleteWebSocketReceiveMessage(Action<PayloadClass> _receiveAction){
        this.receiveAction -= _receiveAction;
    }

#endregion


#region 웹소켓을 Send 해주기 위해 필요한 함수
    // 연결 되었을 때 보내는 함수 실행하기 위함
    private void SendActionWebSocketMessage(Action action){
        if(webSocket.State == WebSocketState.Open){
            action.Invoke();
        } else {
            sendAction += action;
        }
    }

    // 웹소켓을 사용할 클래스들이 데이터를 전송 하기 위한 함수
    public void SendWebSocket(PayloadType _requestCode, IMessage _requestData = null, int _broadCastGroup = -1){
        SendActionWebSocketMessage(()=>SendWebSocketAction(_requestCode, _requestData, _broadCastGroup));
    }

    // 연결된 웹소켓에 직접적으로 데이터를 정리해서 보내는 함수
    private async void SendWebSocketAction(PayloadType _requestCode, IMessage _requestData = null, int _broadCastGroup = -1){
        if (webSocket.State == WebSocketState.Open)
        {
            ByteString requestData = ByteString.Empty;
            if(_requestData != null){
                requestData = ByteString.CopyFrom(_requestData.ToByteArray());
            }
            
            if(AppManager.Instance.userUUID == null){
                AppManager.Instance.userUUID = "";
            }
            
            // ProtoBuf타입으로 서버로 전송시킬 PayloadClass를 만듬
            PayloadClass payload = new PayloadClass
            {
                // App에 관한 정보
                AppRunOS = (int)AppManager.Instance.appRunOS,
                AppName = AppManager.Instance.appName,
                AppVersion = AppManager.Instance.appVersion,
                // Time에 관한 정보
                CurrentUTC = ClientTimer.GetServerUTC(),
                // User에 관한 정보
                UserUUID = AppManager.Instance.userUUID,
                // 주고 받을 데이터에 대한 정보
                RequestCode = _requestCode,
                RequestData = requestData, // 직렬화하여 데이터 넣기
                ResultMessage = "Send",
                BroadCastGroup = _broadCastGroup
            };
            
            Debug.Log("데이터 Send : "+ payload.ToString());

            await webSocket.Send(payload.ToByteArray());
        }
    }

#endregion


}