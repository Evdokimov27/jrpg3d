using UnityEngine;
using System;
using System.Runtime.InteropServices;
using MouseSpeedSwitcher; 
using System.Windows;
using System.Linq;

public class MinimizeWindows : MonoBehaviour
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();
    IntPtr selectedWindow = GetForegroundWindow();

    [DllImport("User32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsIconic(IntPtr hWnd);

    [DllImport("User32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsZoomed(IntPtr hWnd);
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
    [DllImport("user32.dll")]

    private static extern IntPtr GetActiveWindow();
    public IntPtr hWnd = GetActiveWindow();
    
    public void Update()
    {

        if(selectedWindow != GetForegroundWindow())
        {
            GetComponent<MouseProgram>().SpeedMouse("10");
        }
        else GetComponent<MouseProgram>().SpeedMouse(GetComponent<AbilityBook>().mouseSpeed);
    }


    void OnApplicationQuit()
    {
        GetComponent<MouseProgram>().SpeedMouse("10");
        Debug.Log("Application ending after " + Time.time + " seconds");
    }
}







 