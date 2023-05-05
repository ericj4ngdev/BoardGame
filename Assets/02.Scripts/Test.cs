using UnityEngine;
using System.IO;

public class Test : MonoBehaviour
{
    private FileStream test;
    private StreamWriter testStreamWriter;
    private string str;

    private void Awake()
    {
        test = new FileStream("Assets/Resources/test.txt", FileMode.OpenOrCreate);
        testStreamWriter = new StreamWriter(test, System.Text.Encoding.UTF8, 1024, true);
        str = "";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Func1();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Func2();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            str = "";
            test = new FileStream("Assets/Resources/test.txt", FileMode.Create);
            testStreamWriter = new StreamWriter(test);
            testStreamWriter.Write(str);
            testStreamWriter.Close();
        }
    }

    private void Func1()
    {
        str = "Hello ";
        testStreamWriter.Write(str);
        testStreamWriter.Flush(); // StreamWriter 버퍼를 비웁니다.
    }

    private void Func2()
    {
        str = "World!";
        testStreamWriter.Write(str);
        testStreamWriter.Flush(); // StreamWriter 버퍼를 비웁니다.
    }


    private void OnApplicationQuit()
    {
        testStreamWriter.Write(str);
        testStreamWriter.Close();
    }
}
