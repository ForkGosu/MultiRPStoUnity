using System;

public class ClientTimer
{
    public static long ServerCurrentUTC = 0;

    // 리눅스 시간 차이
    public static void SetServerUTC(long _currentTime) {
        ServerCurrentUTC = GetElapsedUTC(_currentTime);
    }
    
    // 리눅스 시간으로 변환
    public static long GetServerUTC() {
        // 0.xxx 초 까지 표시됨
        return GetCurrentUTC() - ServerCurrentUTC;
    }

    // 현재시간을 구하는 함수
    public static long GetCurrentUTC() {
        // 0.xxx 초 까지 표시됨
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
    
    // 현재시간과 받은 시간의 차이를 구하는 함수
    public static long GetElapsedUTC(long _currentTime){
        long elapsedTime = GetCurrentUTC() - _currentTime;
        return elapsedTime; // 1000 = 1초
    }
}
