using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyHeaderController : MonoBehaviour
{
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI pointText;
    public Button quitButton;
    public Button alertButton;
    public Button settingButton;

    public LobbySettingController m_lobbySetting;

    void Start()
    {
        quitButton.onClick.AddListener(() => OnClickQuit());
        alertButton.onClick.AddListener(() => OnClickAlert());
        settingButton.onClick.AddListener(() => OnClickSetting());
    }
    public void UpdateUserInfo(){
        // coinText.text = UserManager.Instance.userCoin.ToString("N0");
        // pointText.text = UserManager.Instance.userPoint.ToString("N0");
    }
    
#region 버튼에 관한 함수들
    public void OnClickQuit(){
        AlertWindowManager.Instance.Alert("종료하시겠습니까?",() => {AppManager.Instance.AppQuit();}, AlertWindowType.choice);
    }
    // TODO : 알람 버튼
    public void OnClickAlert(){

    }
    // TODO : 설정 버튼
    public void OnClickSetting(){
        m_lobbySetting.gameObject.SetActive(true);
    }
#endregion
}
