using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelSinc : MonoBehaviour
{
    public WheelCollider wheelCollider;
    [SerializeField] private Transform wtransform;
    [SerializeField] private GameObject driftParticle;
    public TrailRenderer trail;
    [SerializeField] private Color[] particleColors = new Color[3];

    private void Awake()
    {
       if (trail != null) trail.emitting = false;
    }
    void FixedUpdate()
    {
        WheelMovmentVisual();
    }
    void WheelMovmentVisual()
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wtransform.position = pos;
        wtransform.rotation = rot;
    }
    public void TrailEffect(bool isActive, float currentDriftAmount, float[] trailType)
    {
        if (isActive && trail != null && driftParticle != null)
        {
            trail.emitting = isActive;
            SetParticleEffect(currentDriftAmount, trailType);
            driftParticle.GetComponent<ParticleSystem>().Play();
            //var systems = driftParticle.GetComponents<ParticleSystem>();
            //foreach (ParticleSystem particle in systems) particle.Play();
        }
        else
        {
            trail.emitting = isActive;
            var systems = driftParticle.GetComponents<ParticleSystem>();
            foreach (ParticleSystem particle in systems) particle.Stop();
        }
    }
    void SetParticleEffect(float currentDriftAmount, float[] trailType)
    {
        var particles = driftParticle.GetComponentsInChildren<ParticleSystem>();
        switch (currentDriftAmount)
        {
            case float f when f < trailType[0]:
                foreach (ParticleSystem particle in particles) particle.startColor = particleColors[0];
                break;
            case float f when f < trailType[1]:
                foreach (ParticleSystem particle in particles) particle.startColor = particleColors[1];
                break;
            case float f when f < trailType[2]:
                foreach (ParticleSystem particle in particles) particle.startColor = particleColors[2];
                break;
        }
    }
}
