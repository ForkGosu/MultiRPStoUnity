using System;
using System.Diagnostics;

public enum AlertWindowType{
    confirm,
    choice,
}

public class AlertWindowManager : MonoSingleton<AlertWindowManager>
{
    public JustPool<AlertWindowBase> objectPool;
    public AlertWindowBase basePrefab;

    void Awake()
    {
        objectPool = new JustPool<AlertWindowBase>(
            CreateAlertWindowBase,
            OnGet,
            OnRelease,
            initSize : 5
        );
    }
    
    private AlertWindowBase CreateAlertWindowBase(){
        AlertWindowBase objectBase = Instantiate(basePrefab).GetComponent<AlertWindowBase>();

        objectBase.transform.SetParent(this.transform, false);
        return objectBase;
    }
    //=> Get이 실행될 때 실행되는 함수 => element가 parameter가 됨
    private void OnGet(AlertWindowBase _objectBase)
    {
        _objectBase.gameObject.SetActive(true);
        _objectBase.transform.SetAsLastSibling(); // 마지막 번째로 이동해서 위로 보여줌
    }

    //=> Release가 실행될 때 실행되는 함수 element가 실행함.
    private void OnRelease(AlertWindowBase _objectBase)
    {
        _objectBase.gameObject.SetActive(false);
    }

    public void Alert(string _context, Action _action = null, AlertWindowType _type = AlertWindowType.confirm){
        AlertWindowBase objectBase = objectPool.Get();
        objectBase.Alert(_context,_action,_type);
        UnityEngine.Debug.Log("어디서?");
    }
    public void LocalizationAlert(string _context, Action _action = null, AlertWindowType _type = AlertWindowType.confirm){
        AlertWindowBase objectBase = objectPool.Get();
        objectBase.LocalizationAlert(_context,_action,_type);
    }
}
