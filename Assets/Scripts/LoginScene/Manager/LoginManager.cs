using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoBuf.ProtoBufPacket;

public class LoginManager : MonoSingleton<LoginManager>
{
    public LoginController loginController;
    public JoinController joinController;

    private bool isAutoLogin = false;

    public void Awake()
    {
        // isAutoLogin = SecurePlayerPrefs.GetBool("AutoLogin");

        // 오브젝트 생성되면 서버에서 전달 된 데이터 받기
        WebSocketManager.Instance.AddWebSocketReceiveMessage(ReceiveAction);

        // 자동로그인이 켜져 있었으면 바로 자동로그인
        // if(isAutoLogin) SendUserAutoLogin();
    }

    private void OnDestroy(){
        // 오브젝트 파괴되면 더이상 받지 않기
        WebSocketManager.Instance.DeleteWebSocketReceiveMessage(ReceiveAction);
    }


//* 웹소켓에 대해 주고 받는 함수들

#region 웹소켓에서 데이터 받는 함수
    public void ReceiveAction(PayloadClass _payload)
    {
        // * 회원가입
        if(_payload.RequestCode == PayloadType.UserEnter){
            LoadingScreenManager.Instance.RemoveLoadingText("UserEnter");
            if(_payload.ResultMessage == "Complete"){
                PayloadClassUserEnter userEnter = PayloadClassUserEnter.Parser.ParseFrom(_payload.RequestData);
                // 로그인 시에는 새로 할당 받음
                AppManager.Instance.userUUID = _payload.UserUUID;
                
                // 토글에 따라서 다음에 자동로그인 할지 말지 활성화
                SecurePlayerPrefs.SetString("UUID", AppManager.Instance.userUUID);

                // if (loginController.autoLoginToggle.isOn) SecurePlayerPrefs.SetBool("AutoLogin", true);
                // else SecurePlayerPrefs.SetBool("AutoLogin", false);

                // 로그인 되었다면 로비로 이동
                AppManager.Instance.LoadSceneAsync("Lobby");
            } else if(_payload.ResultMessage == "AlreadyLogin") {
                AlertWindowManager.Instance.Alert("중복 로그인은 할 수 없습니다.\n만약 계속 같은 문제가 발생한다면 관리자에게 문의해주세요.");
            } else {
                AlertWindowManager.Instance.Alert("로그인 중 알수없는 오류가 발생했습니다.\n로그인을 다시 시도해 주세요.\n만약 계속 같은 문제가 발생한다면 관리자에게 문의해주세요.");
            }
        }
    }

#endregion

#region 웹소켓에 데이터 보내는 함수
    public void SendUserEnter(string _userName)
    {
        LoadingScreenManager.Instance.AddLoadingText("UserEnter");

        PayloadClassUserEnter userEnter = new () {
            UserName = _userName
        };

        WebSocketManager.Instance.SendWebSocket(PayloadType.UserEnter, userEnter);
    }

#endregion
}
