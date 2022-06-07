using System.Collections;
using UnityEngine;

public interface IObjectPollingManager {
    public bool IsActive { get; set; }
    public void Activate(bool state, /*float delay,*/ float[] initialLocation = null, Transform playerRef = null);
}