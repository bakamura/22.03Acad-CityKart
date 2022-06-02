using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelSinc : MonoBehaviour {
    public WheelCollider wheelCollider;
    [SerializeField] private Transform wtransform;
    public ParticleSystem driftParticle;
    public TrailRenderer trail;
    [SerializeField] private Color[] particleColors = new Color[3];
    private List<Color> baseParticleColors;
    private List<ParticleSystem> allParticles;
    [SerializeField] private PlayerData playerData;

    private void Awake() {
        if (trail != null) trail.emitting = false;
        if (driftParticle != null) {
            allParticles = new List<ParticleSystem>();
            baseParticleColors = new List<Color>();
            var particles = driftParticle.gameObject.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem particle in particles) {
                allParticles.Add(particle);
                baseParticleColors.Add(particle.startColor);
            }
        }
    }
    //void FixedUpdate()
    //{
    //    WheelMovmentVisual();
    //}
    public void WheelMovmentVisual() {
        if (wheelCollider) {
            Vector3 pos;
            Quaternion rot;
            wheelCollider.GetWorldPose(out pos, out rot);
            wtransform.position = pos;
            wtransform.rotation = rot;
        }
        else wtransform.localRotation = Quaternion.Euler(0, playerData.inputManager.HorzMov() * playerData.TurningDegrees, 0);
    }
    public void TrailEffect(bool isActive, float currentDriftAmount, float[] trailType) {
        if (isActive && trail != null && driftParticle != null) {
            trail.emitting = isActive;
            SetParticleEffect(currentDriftAmount, trailType);
            driftParticle.gameObject.SetActive(true);
        }
        else {
            trail.emitting = isActive;
            driftParticle.gameObject.SetActive(false);
        }
    }
    void SetParticleEffect(float currentDriftAmount, float[] trailType) {
        //if (currentDriftAmount >= trailType[0] / 2f)
        //{
        switch (currentDriftAmount) {
            default:
                ChangeParticleColors(true);
                break;
            case float f when f >= trailType[0] && f < trailType[1]:
                ChangeParticleColors(false, 0);
                break;
            case float f when f >= trailType[1] && f < trailType[2]:
                ChangeParticleColors(false, 1);
                break;
            case float f when f >= trailType[2]:
                ChangeParticleColors(false, 2);
                break;
        }
        //}
        //else
        //{
        //    for (int i = 0; i < allParticles.Count; i++) allParticles[i].startColor = new Color(baseParticleColors[i].r, baseParticleColors[i].g, baseParticleColors[i].b);
        //}
    }
    void ChangeParticleColors(bool returToBaseColor, int colorType = 0) {
        for (int i = 0; i < allParticles.Count; i++) {
            if (returToBaseColor) allParticles[i].startColor = new Color(baseParticleColors[i].r, baseParticleColors[i].g, baseParticleColors[i].b);
            else allParticles[i].startColor = new Color(particleColors[colorType].r, particleColors[colorType].g, particleColors[colorType].b);
        }
    }
}
