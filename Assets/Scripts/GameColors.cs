using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameColor
{
    Red,
    Blue,
    Green,
    Yellow
};

public static class Extensions
{
    public static Color? PrimaryColorRepresentation(this GameColor gameColor)
    {
        switch (gameColor)
        {
            case GameColor.Blue:
                return Color.blue;
            case GameColor.Red:
                return Color.red;
            case GameColor.Green:
                return Color.green;
            case GameColor.Yellow:
                return Color.yellow;
            default:
                return null;
        }
    }

    public static Color? SecondaryColorRepresentation(this GameColor gameColor)
    {
        switch (gameColor)
        {
            case GameColor.Blue:
                return Color.cyan;
            case GameColor.Red:
                return new Color(245.0f / 255, 179.0f / 255, 66.0f / 255);
            case GameColor.Green:
                return new Color(188.0f / 255, 250.0f / 255, 171.0f / 255);
            case GameColor.Yellow:
                return Color.yellow;
            default:
                return null;
        }
    }

    public static Color? TertiaryColorRepresentation(this GameColor gameColor)
    {
        switch (gameColor)
        {
            case GameColor.Blue:
                return new Color(0.0f, 0.0f, 1.0f, 0.6f);
            case GameColor.Red:
                return new Color(1.0f, 0.0f, 0.0f, 0.6f);
            case GameColor.Green:
                return new Color(0.0f, 1.0f, 0.0f, 0.6f);
            case GameColor.Yellow:
                return new Color(1.0f, 1.0f, 0.0f, 0.6f);
            default:
                return null;
        }
    }

    public static Color? QuaternaryColorRepresentation(this GameColor gameColor)
    {
        switch (gameColor)
        {
            case GameColor.Blue:
                return new Color(0.0f, 0.0f, 1.0f, 0.3f);
            case GameColor.Red:
                return new Color(1.0f, 0.0f, 0.0f, 0.3f);
            case GameColor.Green:
                return new Color(0.0f, 1.0f, 0.0f, 0.3f);
            case GameColor.Yellow:
                return new Color(1.0f, 1.0f, 0.0f, 0.3f);
            default:
                return null;
        }
    }
}
