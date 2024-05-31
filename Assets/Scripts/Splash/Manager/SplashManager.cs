using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoBuf.ProtoBufPacket;

public class SplashManager : MonoSingleton<SplashManager>
{

    void Awake(){
        // 오브젝트 생성되면 서버에서 전달 된 데이터 받기
        WebSocketManager.Instance.AddWebSocketReceiveMessage(ReceiveAction);
    }

    void OnDestroy(){
        // 오브젝트 파괴되면 더이상 받지 않기
        WebSocketManager.Instance.DeleteWebSocketReceiveMessage(ReceiveAction);
    }

//* 웹소켓에 대해 주고 받는 함수들

#region 웹소켓에서 데이터 받는 함수
    public void ReceiveAction(PayloadClass _payload)
    {
        // 버전 확인
        if(_payload.RequestCode == PayloadType.AppVersionCheck){
            if(_payload.ResultMessage == "Complete"){
                AppManager.Instance.IsAppVersion = true;
                
                ClientTimer.SetServerUTC(_payload.CurrentUTC);
                AppManager.Instance.LoadSceneAsync("Login");
            } else {
                AppManager.Instance.IsAppVersion = false;
                WebSocketManager.Instance.WebSocketClose();
                AlertWindowManager.Instance.Alert("버전이 다릅니다.\n앱을 업데이트 해주세요.", () => AppManager.Instance.AppQuit());
            }
        }
    }

#endregion

#region 웹소켓에 데이터 보내는 함수
    public void SendAppVersionCheck()
    {
        WebSocketManager.Instance.SendWebSocket(PayloadType.AppVersionCheck);
    }

#endregion
}
