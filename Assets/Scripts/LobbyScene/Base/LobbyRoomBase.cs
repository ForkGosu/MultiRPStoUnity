using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyRoomBase : MonoBehaviour
{
    //public CanvasGroup canvasGroup;
    public TextMeshProUGUI roomSeqText;
    public TextMeshProUGUI roomTypeText;
    public TextMeshProUGUI roomPersonnelText;

    public Button roomJoinButton;

    public int roomSeq = 0;
    private LobbyRoomType roomType = 0;

    private bool isFull = false;

    public void Awake(){
        roomJoinButton.onClick.AddListener(()=>{ OnClickRoomJoin(); });
    }

    public void Init(int _seq, LobbyRoomType _type, int _nowPersonnel, int _maxPersonnel){
        roomSeq = _seq;
        roomType = _type;
        roomSeqText.text = _seq.ToString();
        roomTypeText.text = _type.ToString();
        roomPersonnelText.text = $"{_nowPersonnel} / {_maxPersonnel}";

        isFull = _nowPersonnel == _maxPersonnel ? true : false;
        Full(isFull);
    }

    private void Full(bool _full){
        if(_full){
            //canvasGroup.alpha = 0.2f;
            roomJoinButton.enabled = false;
        } else {
            //canvasGroup.alpha = 1f;
            roomJoinButton.enabled = true;
        }
    }

    private void OnClickRoomJoin(){
        LobbyManager.Instance.SendRoomJoin(roomSeq, roomType);
    }
}
