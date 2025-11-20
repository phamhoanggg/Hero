using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class Glitch : SingletonMonoBehaviour<Glitch>
{
    [SerializeField] Material material;
    [SerializeField] float noiseAmount;
    [SerializeField] float glitchStrength;
    [SerializeField] float scanLinesStrength;

    int noiseAmountId;
    int glitchStrengthId;
    int scanLinesStrengthId;

    protected override void Awake()
    {
        base.Awake();
        noiseAmountId = Shader.PropertyToID("_NoiseAmount");
        glitchStrengthId = Shader.PropertyToID("_GlitchStrength");
        scanLinesStrengthId = Shader.PropertyToID("_ScanLinesStrength");
        Set(0, 0, 1);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Set(0, 0, 1);
    }
    public void Play()
    {
        Set(noiseAmount, glitchStrength, scanLinesStrength);
    }

    [Button(ButtonStyle.FoldoutButton)]
    public void Play(float duration)
    {
        Set(noiseAmount, glitchStrength, scanLinesStrength);

        Invoke(nameof(ResetNoise), duration);
    }

    public void ResetNoise()
    {
        Set(0, 0, 1);
    }

    void Set(float noiseAmount, float glitchStrength, float scanLinesStrength)
    {
        material.SetFloat(noiseAmountId, noiseAmount);
        material.SetFloat(glitchStrengthId, glitchStrength);
        material.SetFloat(scanLinesStrengthId, scanLinesStrength);
    }
}
