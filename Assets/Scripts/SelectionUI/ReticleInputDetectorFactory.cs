using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ReticleInputDetector : CombinedInputDetectionListener, LevelStateListener
{
    ReticleInputDetectorDataProvider DataProvider { get; set; }
}

public interface ReticleInputDetectorDataProvider
{
    List<ReticleListener> ReticleListeners { get; }

    RectTransform TheRectTransform { get; }

    Transform TheTransform { get; }
}

public static class ReticleInputDetectorFactory
{
    public static ReticleInputDetector CreateDetector()
    {
        if (DeviceDetector.TargetDevice == DeviceDetector.Device.PC)
        {
            return new PCReticleInputDetector();
        }
        else
        {
            return new MobileReticleInputDetector();
        }
    }
}
