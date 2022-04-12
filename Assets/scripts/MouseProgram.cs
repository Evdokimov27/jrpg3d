using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using System.Runtime.InteropServices;
using System.Windows;

namespace MouseSpeedSwitcher
{
    public class MouseProgram : MonoBehaviour
    {
        public string Speed;
        public const UInt32 SPI_SETMOUSESPEED = 0x0071;

        [DllImport("User32.dll")]
        static extern Boolean SystemParametersInfo(
            UInt32 uiAction,
            UInt32 uiParam,
            UInt32 pvParam,
            UInt32 fWinIni);

        public void SpeedMouse(string MouseSpeed)
        {
            SystemParametersInfo(
                SPI_SETMOUSESPEED,
                0,
                uint.Parse(MouseSpeed),
                0);

            Speed = MouseSpeed;
        }
    }
}
