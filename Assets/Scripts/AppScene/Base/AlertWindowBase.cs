using System;
using UnityEngine;
// using UnityEngine.Localization.Components; // 현재 Localization 비활성
using TMPro;
using UnityEngine.UI;

public class AlertWindowBase : MonoBehaviour
{
    // public LocalizeStringEvent m_localizedStringContent;
    public TextMeshProUGUI m_alertContext;
    public Button m_cancelButton;

    public Action m_confirmAction = null;

    public void Alert(string _context, Action _action = null, AlertWindowType _type = AlertWindowType.confirm)
    {
        // m_localizedStringContent.SetEntry(null);

        m_alertContext.text = _context;

        if(_type == AlertWindowType.choice) m_cancelButton.gameObject.SetActive(true);
        else m_cancelButton.gameObject.SetActive(false);

        m_confirmAction += _action;
    }

    public void LocalizationAlert(string _context, Action _action = null, AlertWindowType _type = AlertWindowType.confirm)
    {
        // m_localizedStringContent.SetEntry(_context);

        if(_type == AlertWindowType.choice) m_cancelButton.gameObject.SetActive(true);
        else m_cancelButton.gameObject.SetActive(false);

        m_confirmAction += _action;
    }

    public void OnClickConfirm()
    {
        AlertWindowManager.Instance.objectPool.Release(this);

        m_cancelButton.gameObject.SetActive(false);

        m_confirmAction?.Invoke();
        m_confirmAction = null;
    }

    public void OnClickCancel()
    {
        AlertWindowManager.Instance.objectPool.Release(this);
        
        m_cancelButton.gameObject.SetActive(false);
    }
}