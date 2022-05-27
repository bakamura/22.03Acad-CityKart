using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelSinc : MonoBehaviour
{
    public WheelCollider wheelCollider;
    [SerializeField] private Transform wtransform;
    public ParticleSystem driftParticle;
    public TrailRenderer trail;
    [SerializeField] private Color[] particleColors = new Color[3];
    private List<Color> baseParticleColors;
    private List<ParticleSystem> allParticles;

    private void Awake()
    {
        if (trail != null) trail.emitting = false;
        if (driftParticle != null)
        {
            allParticles = new List<ParticleSystem>();
            baseParticleColors = new List<Color>();
            var particles = driftParticle.gameObject.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem particle in particles)
            {
                allParticles.Add(particle);
                baseParticleColors.Add(particle.startColor);
            }
        }
    }
    void FixedUpdate()
    {
        WheelMovmentVisual();
    }
    void WheelMovmentVisual()
    {
        //if (wheelCollider) {
            Vector3 pos;
            Quaternion rot;
            wheelCollider.GetWorldPose(out pos, out rot);
            wtransform.position = pos;
            wtransform.rotation = rot;
        //}
        //else transform.rotation = Quaternion.Euler(0, horzMov * data.TurningDegrees, 0);
    }
    public void TrailEffect(bool isActive, float currentDriftAmount, float[] trailType)
    {
        if (isActive && trail != null && driftParticle != null)
        {
            trail.emitting = isActive;
            SetParticleEffect(currentDriftAmount, trailType);
            driftParticle.gameObject.SetActive(true);
        }
        else
        {
            trail.emitting = isActive;
            driftParticle.gameObject.SetActive(false);
        }
    }
    void SetParticleEffect(float currentDriftAmount, float[] trailType)
    {
        if (currentDriftAmount >= trailType[0] / 2f)
        {
            switch (currentDriftAmount)
            {
                case float f when f < trailType[0]:
                    foreach (ParticleSystem particle in allParticles) particle.startColor = new Color(particleColors[0].r, particleColors[0].g, particleColors[0].b);
                    break;
                case float f when f < trailType[1]:
                    foreach (ParticleSystem particle in allParticles) particle.startColor = new Color(particleColors[1].r, particleColors[1].g, particleColors[1].b);
                    break;
                case float f when f < trailType[2]:
                    foreach (ParticleSystem particle in allParticles) particle.startColor = new Color(particleColors[2].r, particleColors[2].g, particleColors[2].b);
                    break;
            }
        }
        else
        {
            for (int i = 0; i < allParticles.Count; i++) allParticles[i].startColor = new Color(baseParticleColors[i].r, baseParticleColors[i].g, baseParticleColors[i].b);
        }
    }
}
