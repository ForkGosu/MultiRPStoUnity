using UnityEngine.Pool;

public class AlertOnceManager : MonoSingleton<AlertOnceManager>
{
    public JustPool<AlertOnceBase> m_objectPool;
    public AlertOnceBase m_basePrefab;


    void Awake()
    {
        m_objectPool = new JustPool<AlertOnceBase>(
            CreateAlertWindowBase,
            OnGet,
            OnRelease,
            initSize : 5
        );
    }
    
    private AlertOnceBase CreateAlertWindowBase(){
        AlertOnceBase objectBase = Instantiate(m_basePrefab).GetComponent<AlertOnceBase>();
        objectBase.transform.SetParent(this.transform, false);

        return objectBase;
    }
    //=> Get이 실행될 때 실행되는 함수 => element가 parameter가 됨
    private void OnGet(AlertOnceBase _objectBase)
    {
        _objectBase.gameObject.SetActive(true);
        _objectBase.transform.SetAsLastSibling(); // 마지막 번째로 이동해서 위로 보여줌
    }

    //=> Release가 실행될 때 실행되는 함수 element가 실행함.
    private void OnRelease(AlertOnceBase _objectBase)
    {
        _objectBase.gameObject.SetActive(false);
    }

    // private void OnDestory(AlertOnceBase _objectBase)
    // {
    //     Destroy(_objectBase.gameObject);
    // }

    public void Alert(string _context){
        AlertOnceBase objectBase = m_objectPool.Get();
        objectBase.Alert(_context);
    }
    public void LocalizationAlert(string _entry){
        AlertOnceBase objectBase = m_objectPool.Get();
        objectBase.LocalizationAlert(_entry);
    }
}
