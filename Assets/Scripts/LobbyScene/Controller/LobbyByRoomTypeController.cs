using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyByRoomTypeController : MonoBehaviour
{
    public LobbyRoomType roomType;
    public JustPool<LobbyRoomBase> roomPool;
    public Transform roomPoolParent;
    public LobbyRoomBase basePrefab;
    public Button roomBackButton;
    public Button roomReloadButton;
    public Button roomCreateButton;
    
    void Awake()
    {
        roomBackButton.onClick.AddListener(()=>{OnClickBack();});
        roomReloadButton.onClick.AddListener(()=>{OnClickRoomReload();});
        roomCreateButton.onClick.AddListener(()=>{OnClickRoomCreate();});

        roomPool = new JustPool<LobbyRoomBase>(
            PoolCreate,
            PoolGet,
            PoolRelease,
            initSize:10
        );
    }
    
    public void OnEnable(){
        // BGame방들의 정보를 받기 위함
        RoomSearch();
    }

#region 오브젝트 풀링
    private LobbyRoomBase PoolCreate(){
        LobbyRoomBase objectBase = Instantiate(basePrefab,roomPoolParent).GetComponent<LobbyRoomBase>();
        return objectBase;
    }
    //=> Get이 실행될 때 실행되는 함수 => element가 parameter가 됨
    private void PoolGet(LobbyRoomBase _objectBase)
    {
        _objectBase.gameObject.SetActive(true);
        _objectBase.transform.SetAsLastSibling(); // 마지막 번째로 이동해서 위로 보여줌
    }

    //=> Release가 실행될 때 실행되는 함수 element가 실행함.
    private void PoolRelease(LobbyRoomBase _objectBase)
    {
        _objectBase.gameObject.SetActive(false);
    }
#endregion

    public void RoomSearch(){
        roomPool.Clear();
        LobbyManager.Instance.SendRoomsSearch(roomType);
    }

    public void RoomInfoCreate(int _seq, LobbyRoomType _type, int _nowPersonnel, int _maxPersonnel){
        switch(roomType){
        case LobbyRoomType.RPSGame:
            LobbyRoomBase roomBase = (LobbyRoomBase)roomPool.Get();
            roomBase.Init(_seq,_type,_nowPersonnel,_maxPersonnel);  
            break;
        }
    }


    public void OnClickBack(){
        // switch(roomType){
        // case LobbyRoomType.BGame:
        //     LobbyManager.Instance.CloseLobbyByBGame();
        //     break;
        
        // case LobbyRoomType.KGame:
        //     LobbyManager.Instance.CloseLobbyByKGame();
        //     break;
        // }
        // gameObject.SetActive(false);
    }

    public void OnClickRoomReload(){
        RoomSearch();
    }


    public void OnClickRoomCreate(){
        LobbyManager.Instance.SendRoomCreate(roomType);
    }


}
