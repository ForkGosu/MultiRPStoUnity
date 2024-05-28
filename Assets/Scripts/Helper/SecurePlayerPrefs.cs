using System.Text;
using UnityEngine;

public static class SecurePlayerPrefs
{
    public static void SetString(string _key, string _value)
    {
        // utf-8 인코딩
        byte[] encodedBytes = Encoding.UTF8.GetBytes(_value);
        string encodedString = System.Convert.ToBase64String(encodedBytes);

        // 저장하기
        PlayerPrefs.SetString(_key, encodedString);
    }

    public static string GetString(string _key)
    {
        if (!PlayerPrefs.HasKey(_key))
            return null;

        // 불러오기
        string encodedString = PlayerPrefs.GetString(_key);
        
        // utf-8 디코딩
        byte[] decodedBytes = System.Convert.FromBase64String(encodedString);
        string decodedString = Encoding.UTF8.GetString(decodedBytes);

        return decodedString;
    }

    public static void SetBool(string key, bool _value)
    {
        string value = _value ? "1" : "0";

        SetString(key, value);
    }

    public static bool GetBool(string _key)
    {
        string value = GetString(_key);

        if(value != null && value == "1"){
            return true;
        } else {
            return false;
        }
    }

}