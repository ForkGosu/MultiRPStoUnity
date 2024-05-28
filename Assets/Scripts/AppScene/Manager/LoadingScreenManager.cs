using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// using UnityEngine.Localization.Components;
using TMPro;

public class LoadingScreenManager : MonoSingleton<LoadingScreenManager>
{
    private HashSet<string> m_explanationHashSet = new HashSet<string>();

    public GameObject m_loadingGameObject;

    // public LocalizeStringEvent m_localizedString;
    public TextMeshProUGUI m_explanationText;

    public void AddLoadingText(string _explanation){
        m_explanationHashSet.Add(_explanation);
        UpdateLoading();
    }
    
    public void RemoveLoadingText(string _explanation){
        m_explanationHashSet.Remove(_explanation);
        UpdateLoading();
    }

    private void UpdateLoading(){
        string firstLoading = m_explanationHashSet.FirstOrDefault();

        if(firstLoading != null){
            m_loadingGameObject.SetActive(true);
            // m_localizedString.SetEntry(firstLoading);
            m_explanationText.text = firstLoading;
        } 
        
        else {
            m_loadingGameObject.SetActive(false);
        }
    }

}
