using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbySettingController : MonoBehaviour
{
    public Button m_logout;
    public Button m_appQuit;
    public Button m_close;
    // Start is called before the first frame update
    void Start()
    {
        m_logout.onClick.AddListener(()=>{ OnClickLogout(); });
        m_appQuit.onClick.AddListener(()=>{ AppManager.Instance.AppQuit(); });
        m_close.onClick.AddListener(()=>{ gameObject.SetActive(false); });
    }

    public void OnClickLogout(){
        // LobbyManager.Instance.SendUserLogout();
    }
}
