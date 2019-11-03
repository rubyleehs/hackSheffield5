using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class RippleEffect : MonoBehaviour
{

    public AnimationCurve waveform = new AnimationCurve(
        new Keyframe(0.00f, 0.50f, 0, 0),
        new Keyframe(0.05f, 1.00f, 0, 0),
        new Keyframe(0.15f, 0.10f, 0, 0),
        new Keyframe(0.25f, 0.80f, 0, 0),
        new Keyframe(0.35f, 0.30f, 0, 0),
        new Keyframe(0.45f, 0.60f, 0, 0),
        new Keyframe(0.55f, 0.40f, 0, 0),
        new Keyframe(0.65f, 0.55f, 0, 0),
        new Keyframe(0.75f, 0.46f, 0, 0),
        new Keyframe(0.85f, 0.52f, 0, 0),
        new Keyframe(0.99f, 0.50f, 0, 0)
    );

    [Range(0.01f, 1.0f)] public float refractionStrength = 0.5f;

    // ---

    protected class Ripple
    {

        public Vector2 position;
        public float progress;
        public float duration;
        public float oneOverScale;

    }

    List<Ripple> ripples = new List<Ripple>();

    MCoroutine currentRippleCoroutine;

    // ---

    public Shader shader;
    Texture2D gradTexture;
    Material material;

    // ---

    new Camera camera;
    public static RippleEffect instance;

    // ---

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);

        camera = GetComponent<Camera>();

        // --- generate wave/gradient(?) texture

        gradTexture = new Texture2D(2048, 1, TextureFormat.Alpha8, false)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear
        };

        for (var i = 0; i < gradTexture.width; i++)
        {
            var x = 1.0f / gradTexture.width * i;
            var a = waveform.Evaluate(x);
            gradTexture.SetPixel(i, 0, new Color(a, a, a, a));
        }

        gradTexture.Apply();

        // --- create material

        material = new Material(shader)
        {
            hideFlags = HideFlags.DontSave
        };
        material.SetTexture("_GradTex", gradTexture);
    }

    public void AddRipple(Vector2 _position, float _duration, float _size)
    {
        Ripple ripple;

        if (ripples.Count < rippleMaxCount || ripples.Count <= 0)
        {
            ripple = new Ripple();
            ripples.Add(ripple);
        }
        else ripple = ripples[0];

        ripple.position = _position;
        ripple.duration = _duration;
        ripple.progress = 0f;

        ripple.oneOverScale = 1f / Mathf.Clamp(_size, 0.2f, 1f);

        if (currentRippleCoroutine?.running != true)
        {
            currentRippleCoroutine = currentRippleCoroutine.StartCoroutine(this, UpdateRipplesCoroutine());
        }
    }

    const int rippleMaxCount = 5;
    Vector4[] rippleParamCache = new Vector4[rippleMaxCount];

    IEnumerator UpdateRipplesCoroutine()
    {
        while (ripples.Count > 0)
        {
            material.SetVector("_Params1", new Vector4(camera.aspect, 1, 0f, 0));
            material.SetVector("_Params2", new Vector4(1, 1 / camera.aspect, refractionStrength, 0f));

            for (int i = 0; i < rippleMaxCount; i++)
            {

                if (i >= ripples.Count)
                {
                    rippleParamCache[i] = new Vector4(0f, 0f, 0f, -1f);
                    break;
                }

                Ripple ripple = ripples[i];
                Vector2 viewportPosition = camera.WorldToViewportPoint(ripple.position);
                rippleParamCache[i] = new Vector4(viewportPosition.x * camera.aspect, viewportPosition.y,
                                                  ripple.progress, ripple.oneOverScale);

            }

            material.SetVectorArray("_Ripples", rippleParamCache);

            float dt = Time.deltaTime;

            for (int i = ripples.Count - 1; i >= 0; i--)
            {

                if (i >= ripples.Count)
                {
                    rippleParamCache[i] = new Vector4(0f, 0f, 0f, -1f);
                    break;
                }
                Ripple ripple = ripples[i];
                ripple.progress += dt / ripple.duration;
                if (ripple.progress >= 1f) ripples.RemoveAt(i);

            }
            yield return null;
        }
        rippleParamCache[0] = new Vector4(0f, 0f, 0f, -1f);
        material.SetVectorArray("_Ripples", rippleParamCache);
    }

    void OnRenderImage(RenderTexture _source, RenderTexture _destination)
    {

        if (currentRippleCoroutine?.running != true)
        {
            Graphics.Blit(_source, _destination);
            return;
        }
        Graphics.Blit(_source, _destination, material);
    }
}
