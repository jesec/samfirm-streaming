using System;
using System.IO;
using System.Windows.Forms;

namespace SamFirm
{
    internal static class Logger
    {

        //현재 날짜와 시각을 알아내는 함수
        private static string GetTimeDate()
        {
            return DateTime.Now.ToString("yyyy/MM/dd") + " " + DateTime.Now.ToString("HH:mm:ss");
        }

        //로그를 파일로 저장하는 메소드
        public static void SaveLog()
        {

        }

        //로그 텍스트 박스에 문자열을 출력하는 메소드
        public static void WriteLine(string str)
        {
            Console.WriteLine(str);
        }
    }
}