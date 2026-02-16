using UnityEngine;
using System.Collections.Generic;

[DefaultExecutionOrder(1000)]
public class HandFollow : MonoBehaviour
{
    public Transform target;
    [Tooltip("When enabled, lag is applied in local space. Use this when both objects share CameraHolder parent.")]
    [SerializeField] bool useLocalSpace = true;

    [Header("Lag Settings")]
    [Min(0f)] public float positionDelay = 0.12f;
    [Min(0f)] public float rotationDelay = 0.10f;

    private struct PoseSample
    {
        public float time;
        public Vector3 position;
        public Quaternion rotation;
    }

    private readonly List<PoseSample> samples = new List<PoseSample>();

    void OnEnable()
    {
        samples.Clear();

        if (!target && Camera.main != null)
        {
            target = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
        if (!target) return;

        samples.Add(new PoseSample
        {
            time = Time.time,
            position = useLocalSpace ? target.localPosition : target.position,
            rotation = useLocalSpace ? target.localRotation : target.rotation
        });

        float delayedPosTime = Time.time - positionDelay;
        float delayedRotTime = Time.time - rotationDelay;

        if (useLocalSpace)
        {
            transform.localPosition = SamplePositionAt(delayedPosTime);
            transform.localRotation = SampleRotationAt(delayedRotTime);
        }
        else
        {
            transform.position = SamplePositionAt(delayedPosTime);
            transform.rotation = SampleRotationAt(delayedRotTime);
        }

        TrimOldSamples();
    }

    private Vector3 SamplePositionAt(float sampleTime)
    {
        if (samples.Count == 0) return useLocalSpace ? transform.localPosition : transform.position;
        if (sampleTime <= samples[0].time) return samples[0].position;

        for (int i = 1; i < samples.Count; i++)
        {
            if (samples[i].time >= sampleTime)
            {
                PoseSample a = samples[i - 1];
                PoseSample b = samples[i];
                float t = Mathf.InverseLerp(a.time, b.time, sampleTime);
                return Vector3.LerpUnclamped(a.position, b.position, t);
            }
        }

        return samples[samples.Count - 1].position;
    }

    private Quaternion SampleRotationAt(float sampleTime)
    {
        if (samples.Count == 0) return useLocalSpace ? transform.localRotation : transform.rotation;
        if (sampleTime <= samples[0].time) return samples[0].rotation;

        for (int i = 1; i < samples.Count; i++)
        {
            if (samples[i].time >= sampleTime)
            {
                PoseSample a = samples[i - 1];
                PoseSample b = samples[i];
                float t = Mathf.InverseLerp(a.time, b.time, sampleTime);
                return Quaternion.SlerpUnclamped(a.rotation, b.rotation, t);
            }
        }

        return samples[samples.Count - 1].rotation;
    }

    private void TrimOldSamples()
    {
        float keepDuration = Mathf.Max(positionDelay, rotationDelay) + 0.5f;
        float oldestTimeToKeep = Time.time - keepDuration;

        int removeCount = 0;
        for (int i = 0; i < samples.Count; i++)
        {
            if (samples[i].time < oldestTimeToKeep) removeCount++;
            else break;
        }

        if (removeCount > 0)
        {
            samples.RemoveRange(0, removeCount);
        }
    }
}
