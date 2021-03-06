using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   

public interface ReticleDrawer: CombinedInputDetectionListener
{
    ReticleDrawerDataProvider DataProvider { get; set; }

    void DrawInitialState();
}

public interface ReticleDrawerDataProvider
{
    Image TheImage { get; }

    Color TranslucentReticleColor { get; }
}

public static class ReticleDrawerFactory
{
    public static ReticleDrawer CreateDrawer()
    {
        if (DeviceDetector.TargetDevice == DeviceDetector.Device.PC)
        {
            return new PCReticleDrawer();
        }
        else
        {
            return new MobileReticleDrawer();
        }
    }
}
