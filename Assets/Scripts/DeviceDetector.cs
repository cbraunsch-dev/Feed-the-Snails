using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceDetector
{
    public static Device TargetDevice {
        get
        {
            return Device.PC;
        }
    }

    public enum Device
    {
        Mobile,
        PC
    }
}
