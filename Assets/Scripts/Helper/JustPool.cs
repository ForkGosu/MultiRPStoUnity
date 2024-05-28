using UnityEngine;

using System;
using System.Collections.Generic;
public class JustPool<T> where T : class
{
    // 비활성화 되어있는 리스트
    internal readonly List<T> m_InactiveList;
    // 모든 리스트
    internal readonly List<T> m_AllList;

    private readonly Func<T> m_CreateFunc;

    private readonly Action<T> m_ActionOnGet;

    private readonly Action<T> m_ActionOnRelease;

    public int CountActive => m_AllList.Count - CountInactive;

    public int CountInactive => m_InactiveList.Count;

    public JustPool(Func<T> createFunc, Action<T> actionOnGet = null, Action<T> actionOnRelease = null, int initSize = 0)
    {
        if (createFunc == null)
        {
            throw new ArgumentNullException("createFunc");
        }
        if (actionOnGet == null)
        {
            throw new ArgumentNullException("actionOnGet");
        }
        if (actionOnRelease == null)
        {
            throw new ArgumentNullException("actionOnRelease");
        }

        m_InactiveList = new List<T>();
        m_AllList = new List<T>();

        m_CreateFunc = createFunc;
        m_ActionOnGet = actionOnGet;
        m_ActionOnRelease = actionOnRelease;

        // 처음에 오브젝트 풀 만들때 initSize만큼 초기화
        for (int i = 0; i < initSize; i++)
        {
            Get();
        }
        // 만든 오브젝트들 모두 비활성화 하기
        Clear();
    }

    public T Get()
    {
        T val;
        // 비활성화 되어있는게 없다면 새로 생성
        if (m_InactiveList.Count == 0)
        {
            val = m_CreateFunc();
            m_AllList.Add(val);
        }
        // 비활성화 된 오브젝트 사용
        else
        {
            int index = m_InactiveList.Count - 1;
            val = m_InactiveList[index];
            m_InactiveList.RemoveAt(index);
        }

        m_ActionOnGet(val);
        return val;
    }

    // 지정된 오브젝트 비활성화
    public void Release(T element)
    {
        // 지정된 함수 실행
        m_ActionOnRelease(element);

        // 비활성화 된 오브젝트 리스트에 넣기
        if(!m_InactiveList.Contains(element)){
            m_InactiveList.Add(element);
        }
    }

    // 활성화된 오브젝트 모두 비활성화
    public void Clear()
    {
        foreach (T item in m_AllList)
        {
            // 비활성화 되어 있는것은 Release필요 없음
            if (!m_InactiveList.Contains(item)){
                Release(item);
            }
        }
    }
    
    // 활성화된 오브젝트 모두 건네기 왜안됨?
    public List<T> GetList()
    {
        return m_AllList;
    }
}
