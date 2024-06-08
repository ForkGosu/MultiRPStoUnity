using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RPSGameController : MonoBehaviour
{
    public int m_roomSeq = -1;

    public Button rockButton;
    public Button sisserButton;
    public Button paperButton;
    public Button quitButton;

    public List<Sprite> rpsImage;


    public Image p1Image;
    public Image p2Image;



    public Button goButton;

    public TextMeshProUGUI player1Nick;
    public TextMeshProUGUI player2Nick;

    
    public TextMeshProUGUI progressText;

    void Start(){
        rockButton.onClick.AddListener(()=>{ OnClickPlaying(1); });
        sisserButton.onClick.AddListener(()=>{ OnClickPlaying(2); });
        paperButton.onClick.AddListener(()=>{ OnClickPlaying(3); });

        quitButton.onClick.AddListener(()=>{ OnClickQuit(); });

        goButton.onClick.AddListener(()=>{ OnClickGo(); });
    }
    public void SetCardsInfo(List<int> _cards, int _status)
    {
        if(_cards[0] > 0 && _cards[1] > 0){
            p1Image.sprite = rpsImage[_cards[0]-1];
            p2Image.sprite = rpsImage[_cards[1]-1];

            if(_cards[0] == 1){
                if(_cards[1] == 1){
                    progressText.text = "비김";
                } else if (_cards[1] == 2) {
                    progressText.text = "1p 이김";
                } else {
                    progressText.text = "1p 짐";
                }
            } else if (_cards[0] == 2) {
                if(_cards[1] == 1){
                    progressText.text = "1p 짐";
                } else if (_cards[1] == 2) {
                    progressText.text = "비김";
                } else {
                    progressText.text = "1p 이김";
                }
            } else {
                if(_cards[1] == 1){
                    progressText.text = "1p 이김";
                } else if (_cards[1] == 2) {
                    progressText.text = "1p 짐";
                } else {
                    progressText.text = "비김";
                }
            }
            Invoke("OnClickTableInfo", 2);
        } else {
            if(_status == 0){
                SetGo(false);
                progressText.text = "준비하세용";
            } else {
                progressText.text = "게임시작중?";
            }
            p1Image.sprite = rpsImage[3];
            p2Image.sprite = rpsImage[3];

        }
    }
    public void SetPlayerInfo(List<string> players)
    {
        player1Nick.text = players[0];
        player2Nick.text = players[1];
    }
    public void SetGo(bool _isGo)
    {
        goButton.gameObject.SetActive(!_isGo);
    }
    
    public void Init(int _roomSeq){
        m_roomSeq = _roomSeq;
        RPSGameManager.Instance.SendRPSGamePlayersInfo(_roomSeq);
        goButton.gameObject.SetActive(true);
        p1Image.sprite = rpsImage[3];
        p2Image.sprite = rpsImage[3];
    }

    public void OnClickQuit(){
        LobbyManager.Instance.SendRoomQuit(m_roomSeq);
    }
    public void OnClickGo(){
        RPSGameManager.Instance.SendRPSGameGo(m_roomSeq);
    }
    public void OnClickPlaying(int _rps){
        RPSGameManager.Instance.SendRPSGamePlaying(m_roomSeq, _rps);
    }
    
    public void OnClickTableInfo(){
        RPSGameManager.Instance.SendRPSGameTableInfo(m_roomSeq);
    }
    // public void OnClickQuit(){
    //     LobbyManager.Instance.SendRoomQuit(m_roomSeq);
    // }
}
