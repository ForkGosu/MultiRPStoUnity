using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf.ProtoBufPacket;
using UnityEngine;

public class RPSGameManager : MonoSingleton<RPSGameManager>
{
    public RPSGameController active_GameController;

    public void Awake()
    {
        // 오브젝트 생성되면 서버에서 전달 된 데이터 받기
        WebSocketManager.Instance.AddWebSocketReceiveMessage(ReceiveAction);
    }

    private void OnDestroy(){
        // 오브젝트 파괴되면 더이상 받지 않기
        WebSocketManager.Instance.DeleteWebSocketReceiveMessage(ReceiveAction);
    }

//* 웹소켓에 대해 주고 받는 함수들

#region 웹소켓에서 데이터 받는 함수
    public void ReceiveAction(PayloadClass _payload)
    {
        // * 게임 정보
        if(_payload.RequestCode == PayloadType.RpsgamePlayersInfo){
            LoadingScreenManager.Instance.RemoveLoadingText("RpsgamePlayersInfo");
            if(_payload.ResultMessage == "Complete"){
                PayloadClassRPSGamePlayersInfo playerInfo = PayloadClassRPSGamePlayersInfo.Parser.ParseFrom(_payload.RequestData);

                // TODO 나중에 해줘야 할 것
                if (!CheckActiveRoom(_payload.BroadCastGroup)){
                    return;
                }
                Debug.Log(playerInfo.ToString());
                active_GameController.SetPlayerInfo(playerInfo.RpsGamePlayerNames.ToList());
            } else {
                AlertWindowManager.Instance.Alert("");
            }
        }
        // * 게임 정보
        if(_payload.RequestCode == PayloadType.RpsgameTableInfo){
            LoadingScreenManager.Instance.RemoveLoadingText("RpsgameTableInfo");
            if(_payload.ResultMessage == "Complete"){
                PayloadClassRPSGameTableInfo tableInfo = PayloadClassRPSGameTableInfo.Parser.ParseFrom(_payload.RequestData);

                // TODO 나중에 해줘야 할 것
                if (!CheckActiveRoom(_payload.BroadCastGroup)){
                    return;
                }
                Debug.Log(tableInfo.ToString());
                active_GameController.SetCardsInfo(tableInfo.RpsGamePlays.ToList(), tableInfo.RpsTableStatus);
                // active_GameController.SetPlayerInfo(playerInfo.RpsGamePlayerNames.ToList());
            } else {
                AlertWindowManager.Instance.Alert("");
            }
        }
        // * 게임 정보
        if(_payload.RequestCode == PayloadType.RpsgamePlaying){
            LoadingScreenManager.Instance.RemoveLoadingText("RpsgameTableInfo");
            if(_payload.ResultMessage == "Complete"){
                PayloadClassRPSGamePlaying playing = PayloadClassRPSGamePlaying.Parser.ParseFrom(_payload.RequestData);

                // TODO 나중에 해줘야 할 것
                if (!CheckActiveRoom(_payload.BroadCastGroup)){
                    return;
                }
                Debug.Log(playing.ToString());
                // active_GameController.SetPlayerInfo(playerInfo.RpsGamePlayerNames.ToList());
            } else if(_payload.ResultMessage == "Already") {
                AlertWindowManager.Instance.Alert("이미 가위바위보를 시작 했을터");
            } else {
                AlertWindowManager.Instance.Alert("레디를 먼저 해야 할 터");
            }
        }
        // * 게임 정보
        if(_payload.RequestCode == PayloadType.RpsgameGo){
            LoadingScreenManager.Instance.RemoveLoadingText("RpsgameTableInfo");
            if(_payload.ResultMessage == "Complete"){
                // PayloadClassRPSGamePlaying playing = PayloadClassRPSGamePlaying.Parser.ParseFrom(_payload.RequestData);

                // TODO 나중에 해줘야 할 것
                if (!CheckActiveRoom(_payload.BroadCastGroup)){
                    return;
                }
                // Debug.Log(playing.ToString());
                active_GameController.SetGo(true);
            } else {
                AlertWindowManager.Instance.Alert("");
            }
        }
        
        
    }
#endregion


#region 웹소켓에 데이터 보내는 함수
    public void SendRPSGamePlayersInfo(int _roomSeq)
    {
        LoadingScreenManager.Instance.AddLoadingText("RpsgamePlayersInfo");
        WebSocketManager.Instance.SendWebSocket(PayloadType.RpsgamePlayersInfo, null, _roomSeq);
    }
    public void SendRPSGameTableInfo(int _roomSeq)
    {
        WebSocketManager.Instance.SendWebSocket(PayloadType.RpsgameTableInfo, null, _roomSeq);
    }
    public void SendRPSGameGo(int _roomSeq)
    {
        WebSocketManager.Instance.SendWebSocket(PayloadType.RpsgameGo, null, _roomSeq);
    }
    public void SendRPSGamePlaying(int _roomSeq, int _rps)
    {
        PayloadClassRPSGamePlaying rpsGamePlaying = new () {
            RpsGamePlay = _rps
        };
        WebSocketManager.Instance.SendWebSocket(PayloadType.RpsgamePlaying, rpsGamePlaying, _roomSeq);
    }
    // public void SendRPSGameProgress(int _roomSeq)
    // {
    //     LoadingScreenManager.Instance.AddLoadingText("RpsgamePlayersInfo");
    //     WebSocketManager.Instance.SendWebSocket(PayloadType.RpsgameGo, null, _roomSeq);
    // }
#endregion



#region 일반 함수
    public bool CheckActiveRoom(int _roomSeq){
        if(active_GameController != null && active_GameController.m_roomSeq == _roomSeq){
            return true;
        } else {
            return false;
        }
    }

    public void JoinBGame(int _roomSeq){
        active_GameController.gameObject.SetActive(true);
        active_GameController.Init(_roomSeq);
    }
    
    public void QuitBGame(int _roomSeq){
        active_GameController.gameObject.SetActive(false);
        active_GameController.m_roomSeq = -1;
    }
#endregion

}
