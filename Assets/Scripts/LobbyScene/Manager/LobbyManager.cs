using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoBuf.ProtoBufPacket;
using System.Linq;

public enum LobbyRoomType {
    RPSGame = 0,
}

public class LobbyManager : MonoSingleton<LobbyManager>
{
    // TODO : 게임 활성화 시 로비  꺼주기
    public int roomType = -1;

    public GameObject lobbyGameObject;
    public LobbyHeaderController lobbyHeaderController;
    public LobbyByRoomTypeController lobbyByRPSGameController;
    
    public void Awake()
    {
        // 오브젝트 생성되면 서버에서 전달 된 데이터 받기
        WebSocketManager.Instance.AddWebSocketReceiveMessage(ReceiveAction);
        
        // SendUserInfo();
    }

    private void OnDestroy(){
        // 오브젝트 파괴되면 더이상 받지 않기
        WebSocketManager.Instance.DeleteWebSocketReceiveMessage(ReceiveAction);
    }


//* 웹소켓에 대해 주고 받는 함수들

#region 웹소켓에서 데이터 받는 함수
    public void ReceiveAction(PayloadClass _payload)
    {
        // * 방정보
        if(_payload.RequestCode == PayloadType.RoomsSearch){
            if(_payload.ResultMessage == "Complete"){
                PayloadClassRoomsSearch roomInfo = PayloadClassRoomsSearch.Parser.ParseFrom(_payload.RequestData);
                Debug.Log(roomInfo.ToString());

                LobbyRoomType roomType = (LobbyRoomType)roomInfo.RoomType;

                switch(roomType){
                case LobbyRoomType.RPSGame:
                    lobbyByRPSGameController.RoomInfoCreate(roomInfo.RoomSeq,roomType,roomInfo.RoomNowPersonnel, roomInfo.RoomMaxPersonnel);
                    break;
                default:
                    break;
                }
            } else {
                // AlertWindowManager.Instance.Alert("방 정보를 불러내는 중 오류가 발생했습니다.\n만약 계속 같은 문제가 발생한다면 관리자에게 문의해주세요.");
            }
        }
        
        // * 방 입장
        else if(_payload.RequestCode == PayloadType.RoomJoin){
            LoadingScreenManager.Instance.RemoveLoadingText("RoomJoin");
            if(_payload.ResultMessage == "Complete"){
                // TODO : 방 입장
                // lobbyByBGameController.RoomCreate(roomInfo.RoomSeq,roomType,roomInfo.RoomNowPersonnel, roomInfo.RoomMaxPersonnel, roomInfo.RoomMinBuyin, roomInfo.RoomMaxBuyin, roomInfo.RoomStake);
                
                PayloadClassRoomJoin roomJoin = PayloadClassRoomJoin.Parser.ParseFrom(_payload.RequestData);
                JoinGameType(roomJoin.RoomSeq, (LobbyRoomType)roomJoin.RoomType);
                SetLobbyType(roomJoin.RoomType);
            } else if(_payload.ResultMessage == "Already") {
                AlertWindowManager.Instance.Alert("이미 해당 방에 입장 중 입니다.");
            } else if(_payload.ResultMessage == "Full") {
                AlertWindowManager.Instance.Alert("이미 인원이 가득 찬 방입니다.");
            } else {
                AlertWindowManager.Instance.Alert("방 입장 중 오류가 발생했습니다.\n만약 계속 같은 문제가 발생한다면 관리자에게 문의해주세요.");
            }
        }

        // * 방 나가기
        else if(_payload.RequestCode == PayloadType.RoomQuit){
            LoadingScreenManager.Instance.RemoveLoadingText("RoomQuit");
            if(_payload.ResultMessage == "Complete"){
                // TODO : 방 나가기
                PayloadClassRoomQuit roomQuit = PayloadClassRoomQuit.Parser.ParseFrom(_payload.RequestData);
                Debug.Log(roomQuit.ToString());
                
                LobbyRoomType roomType = (LobbyRoomType)roomQuit.RoomType;
                switch(roomType){
                case LobbyRoomType.RPSGame:
                    RPSGameManager.Instance.QuitBGame(roomQuit.RoomSeq);
                    SetLobbyType(-1);
                    // if(setType){
                    //     SetLobbyType((int)LobbyRoomType.BGame);
                    // } else {
                    //     SetLobbyType(-1);
                    // }
                    break;
                default:
                    break;
                }
                // Quit 후 유저의 Coin업데이트
                // SendUserInfo();

            } else if (_payload.ResultMessage == "PlayingUser"){
                AlertWindowManager.Instance.Alert("배팅 시 방을 나갈 수 없습니다.\n 배팅을 취소 하고 나가야 합니다");
            } else {
                AlertWindowManager.Instance.Alert("방 나가는 중 오류가 발생했습니다.\n만약 계속 같은 문제가 발생한다면 관리자에게 문의해주세요.");
            }
        }
    }
#endregion

    

#region 웹소켓에 데이터 보내는 함수
    public void SendRoomCreate(LobbyRoomType _roomType)
    {
        PayloadClassRoomCreate roomCreate = new () {
            RoomType = (int)_roomType,
        };

        WebSocketManager.Instance.SendWebSocket(PayloadType.RoomCreate, roomCreate);
    }


    public void SendRoomsSearch(LobbyRoomType _roomType)
    {
        PayloadClassRoomsSearch roomsSearch = new () {
            RoomType = (int)_roomType,
        };

        WebSocketManager.Instance.SendWebSocket(PayloadType.RoomsSearch, roomsSearch);
    }
    public void SendRoomJoin(int _roomSeq,LobbyRoomType _roomType)
    {
        LoadingScreenManager.Instance.AddLoadingText("RoomJoin");

        PayloadClassRoomJoin roomJoin = new () {
            RoomSeq = _roomSeq,
            RoomType = (int)_roomType,
        };

        WebSocketManager.Instance.SendWebSocket(PayloadType.RoomJoin, roomJoin);
    }
    public void SendRoomQuit(int _roomSeq)
    {
        LoadingScreenManager.Instance.AddLoadingText("RoomQuit");

        PayloadClassRoomQuit roomQuit = new () {
            RoomSeq = _roomSeq,
        };

        WebSocketManager.Instance.SendWebSocket(PayloadType.RoomQuit, roomQuit);
    }

#endregion

#region 일반 함수

    public void GotoLobbyByBGame()
    {
        lobbyGameObject.SetActive(true);
        // lobbyHeaderController.gameObject.SetActive(true);
        // lobbyByBGameController.gameObject.SetActive(true);
        // SendRoomsSearch(RoomType.BGame);
    }
    public void CloseLobbyByBGame(){
        // if (BGameManager.Instance.active_GameController != null){
        //     lobbyGameObject.SetActive(false);
        //     // lobbyHeaderController.gameObject.SetActive(false);
        // } else {
        //     lobbyByBGameController.gameObject.SetActive(false);
        // }
    }

    public void GotoLobbyByKGame()
    {
        lobbyGameObject.SetActive(true);
        // lobbyHeaderController.gameObject.SetActive(true);
        // lobbyByKGameController.gameObject.SetActive(true);
        // SendRoomsSearch(RoomType.BGame);
    }
    
    public void CloseLobbyByKGame(){
        // if (BGameManager.Instance.active_GameController != null){
        //     lobbyGameObject.SetActive(false);
        //     // lobbyHeaderController.gameObject.SetActive(false);
        // } else {
        //     lobbyByKGameController.gameObject.SetActive(false);
        // }
    }

    public void GotoLobbyByHGame()
    {
        // LoadingScreenManager.Instance.AddLoadingText("UserInfo");
        // WebSocketManager.Instance.SendWebSocket(PayloadType.UserInfo);
    }

    public void GotoLobbyByMiniGame()
    {
        // LoadingScreenManager.Instance.AddLoadingText("UserInfo");
        // WebSocketManager.Instance.SendWebSocket(PayloadType.UserInfo);
    }
    
    public void JoinGameType(int _roomSeq, LobbyRoomType _roomType){
        switch(_roomType){
        case LobbyRoomType.RPSGame:
            RPSGameManager.Instance.JoinBGame(_roomSeq);
            break;
        default:
            break;
        }
    }

    public void SetLobbyType(int _roomType){
        roomType = _roomType;

        if(roomType == -1){
            // 원상복귀
            lobbyGameObject.SetActive(true);
            // lobbyMainController.gameObject.SetActive(true);
            lobbyHeaderController.gameObject.SetActive(true);

            return;
        } 

        // switch(roomType){
        //     // case (int)LobbyRoomType.BGame:
        //     // // B게임에 대한 것만 활성화
        //     // lobbyByBGameController.gameObject.SetActive(true);
        //     // break;
        // }
        
        lobbyGameObject.SetActive(false);
        // lobbyMainController.gameObject.SetActive(false);
        lobbyHeaderController.gameObject.SetActive(false);
    }


#endregion

}
