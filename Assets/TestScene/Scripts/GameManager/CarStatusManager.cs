using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CarStatusManager
{
    public const float MaxVelocity = 1500f;
    public const float MinVelocity = 100f;
    //in degrees
    public const float MaxHandling = 90f;
    public const float MinHandling = 10f;
    //in degrees
    public const float MaxDrift = 45f;
    public const float MinDrift = 15f;

    public static void CheckCarStatus(CarControler status)
    {
        if (status.Velocity > MaxVelocity) status.Velocity = MaxVelocity;
        else if (status.Velocity < MinVelocity) status.Velocity = MinVelocity;

        if (status.TurningDegrees > MaxHandling) status.TurningDegrees = MaxHandling;
        else if (status.TurningDegrees < MinHandling) status.TurningDegrees = MinHandling;

        if (status.DriftAngle > MaxDrift) status.DriftAngle = MaxDrift;
        else if (status.DriftAngle < MinDrift) status.DriftAngle = MinDrift;

    }
}
