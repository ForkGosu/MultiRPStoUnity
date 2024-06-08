using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class LoginController : MonoBehaviour
{
    public TMP_InputField idInputField;
    // public TMP_InputField pwInputField;

    public Button loginButton;
    // public Button gotoJoinPageButton;
    public Button quitButton;
    // public Toggle autoLoginToggle;

    void Start()
    {
        // IMEOffOnSelected(idInputField);
        // IMEOffOnSelected(pwInputField);

        // 일반 버튼 (로그인, 회원가입페이지로)
        loginButton.onClick.AddListener(() => OnClickUserLogin());
        // gotoJoinPageButton.onClick.AddListener(() => OnClickGotoJoinPage());
        quitButton.onClick.AddListener(() => OnClickAppQuit());
    }


#region IME를 관리하여 한영키(IME) 쓰지못하게 하는 함수
    // InputField선택 시 IME OFF하고 선택 해제하면 Auto로 다시 하기
    void IMEOffOnSelected(TMP_InputField _inputField){
        _inputField.onSelect.AddListener((_) => {StartCoroutine(ChangeIME());} );
        _inputField.onDeselect.AddListener((_) => {Input.imeCompositionMode = IMECompositionMode.Auto;});
    }

    // 1프레임 기다리고 IME를 OFF 해야 OFF가 됨
    IEnumerator ChangeIME(){
        yield return null;
        Input.imeCompositionMode = IMECompositionMode.Off;
    }

#endregion

#region 버튼에 관한 함수들
    public void OnClickUserLogin(){
        string id = idInputField.text;
        // string pw = pwInputField.text;

        if(id.Length < 4 || 12 < id.Length){
            AlertWindowManager.Instance.Alert("아이디는 4~12글자를 입력해주세요");
            return;
        }

        // if(pw.Length < 8 || 20 < pw.Length){
        //     AlertWindowManager.Instance.Alert("비밀번호는 8~20글자를 입력해주세요");
        //     return;
        // }

        LoginManager.Instance.SendUserEnter(id);
    }

    public void OnClickGotoJoinPage(){
        idInputField.text = "";
        // pwInputField.text = "";

        // LoginManager.Instance.joinController.gameObject.SetActive(true);
    }

    public void OnClickAppQuit(){
        AlertWindowManager.Instance.Alert("종료하시겠습니까?",() => {AppManager.Instance.AppQuit();}, AlertWindowType.choice);
    }

#endregion
}
