using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

[DisallowMultipleComponent, AddComponentMenu("Animation Rigging/Custom/Hand")]
public class HandConstraint : RigConstraint<HandConstraintJob, HandConstraintData, HandConstraintBinder> { }

[BurstCompile]
public struct HandConstraintJob : IWeightedAnimationJob {

    public ReadWriteTransformHandle ProximalThumb;
    public ReadWriteTransformHandle IntermediateThumb;
    public ReadWriteTransformHandle DistalThumb;

    public ReadWriteTransformHandle ProximalIndex;
    public ReadWriteTransformHandle IntermediateIndex;
    public ReadWriteTransformHandle DistalIndex;

    public ReadWriteTransformHandle ProximalMiddle;
    public ReadWriteTransformHandle IntermediateMiddle;
    public ReadWriteTransformHandle DistalMiddle;

    public ReadWriteTransformHandle ProximalRing;
    public ReadWriteTransformHandle IntermediateRing;
    public ReadWriteTransformHandle DistalRing;

    public ReadWriteTransformHandle ProximalLittle;
    public ReadWriteTransformHandle IntermediateLittle;
    public ReadWriteTransformHandle DistalLittle;

    public Vector2 ProximalThumbXRotationRange;
    public Vector2 ProximalThumbZRotationRange;
    public Vector2 IntermediateThumbXRotationRange;
    public Vector2 DistalThumbXRotationRange;

    public Vector2 ProximalIndexZRotationRange;
    public Vector2 ProximalIndexYRotationRange;
    public Vector2 IntermediateIndexZRotationRange;
    public Vector2 DistalIndexZRotationRange;

    public Vector2 ProximalMiddleZRotationRange;
    public Vector2 ProximalMiddleYRotationRange;
    public Vector2 IntermediateMiddleZRotationRange;
    public Vector2 DistalMiddleZRotationRange;

    public Vector2 ProximalRingZRotationRange;
    public Vector2 ProximalRingYRotationRange;
    public Vector2 IntermediateRingZRotationRange;
    public Vector2 DistalRingZRotationRange;

    public Vector2 ProximalLittleZRotationRange;
    public Vector2 ProximalLittleYRotationRange;
    public Vector2 IntermediateLittleZRotationRange;
    public Vector2 DistalLittleZRotationRange;

    public FloatProperty thumbAmount;
    public FloatProperty indexAmount;
    public FloatProperty middleAmount;
    public FloatProperty ringAmount;
    public FloatProperty littleAmount;
    public FloatProperty allAmount;
    public FloatProperty spreadAmount;

    public FloatProperty jobWeight { get; set; }

    public void ProcessRootMotion(AnimationStream stream) { }

    public void ProcessAnimation(AnimationStream stream) {
        var w = jobWeight.Get(stream);

        float thumbT = 1 - thumbAmount.Get(stream);
        float indexT = 1 - indexAmount.Get(stream);
        float middleT = 1 - middleAmount.Get(stream);
        float ringT = 1 - ringAmount.Get(stream);
        float littleT = 1 - littleAmount.Get(stream);
        float allT = 1 - allAmount.Get(stream);
        float spreadT = 1 - spreadAmount.Get(stream);

        if (w > 0) {
            // Thumb
            var proximalThumbHandleRot = Quaternion.Euler(
              math.lerp(ProximalThumbXRotationRange.y, ProximalThumbXRotationRange.x, thumbT * allT),
              0,
              math.lerp(ProximalThumbZRotationRange.y, ProximalThumbZRotationRange.x, spreadT)
            );
            var proximalThumbRot = ProximalThumb.GetLocalRotation(stream);
            ProximalThumb.SetLocalRotation(stream, Quaternion.Lerp(
              proximalThumbRot,
              proximalThumbHandleRot,
              w
            ));

            var intermediateThumbHandleRot = Quaternion.Euler(
              math.lerp(IntermediateThumbXRotationRange.y, IntermediateThumbXRotationRange.x, thumbT * allT),
              0,
              0
            );
            var intermediateThumbRot = IntermediateThumb.GetLocalRotation(stream);
            IntermediateThumb.SetLocalRotation(stream, Quaternion.Lerp(
              intermediateThumbRot,
              intermediateThumbHandleRot,
              w
            ));

            var distalThumbHandleRot = Quaternion.Euler(
              math.lerp(DistalThumbXRotationRange.y, DistalThumbXRotationRange.x, thumbT * allT),
              0,
              0
            );
            var distalThumbRot = DistalThumb.GetLocalRotation(stream);
            DistalThumb.SetLocalRotation(stream, Quaternion.Lerp(
              distalThumbRot,
              distalThumbHandleRot,
              w
            ));

            // Index
            var proximalIndexHandleRot = Quaternion.Euler(
              0,
              math.lerp(ProximalIndexYRotationRange.y, ProximalIndexYRotationRange.x, spreadT),
              math.lerp(ProximalIndexZRotationRange.y, ProximalIndexZRotationRange.x, indexT * allT)
            );
            var proximalIndexRot = ProximalIndex.GetLocalRotation(stream);
            ProximalIndex.SetLocalRotation(stream, Quaternion.Lerp(
              proximalIndexRot,
              proximalIndexHandleRot,
              w
            ));

            var intermediateIndexHandleRot = Quaternion.Euler(
              0,
              0,
              math.lerp(IntermediateIndexZRotationRange.y, IntermediateIndexZRotationRange.x, indexT * allT)
            );
            var intermediateIndexRot = IntermediateIndex.GetLocalRotation(stream);
            IntermediateIndex.SetLocalRotation(stream, Quaternion.Lerp(
              intermediateIndexRot,
              intermediateIndexHandleRot,
              w
            ));

            var distalIndexHandleRot = Quaternion.Euler(
              0,
              0,
              math.lerp(DistalIndexZRotationRange.y, DistalIndexZRotationRange.x, indexT * allT)
            );
            var distalIndexRot = DistalIndex.GetLocalRotation(stream);
            DistalIndex.SetLocalRotation(stream, Quaternion.Lerp(
              distalIndexRot,
              distalIndexHandleRot,
              w
            ));

            // Middle
            var proximalMiddleHandleRot = Quaternion.Euler(
              0,
              math.lerp(ProximalMiddleYRotationRange.y, ProximalMiddleYRotationRange.x, spreadT),
              math.lerp(ProximalMiddleZRotationRange.y, ProximalMiddleZRotationRange.x, middleT * allT)
            );
            var proximalMiddleRot = ProximalMiddle.GetLocalRotation(stream);
            ProximalMiddle.SetLocalRotation(stream, Quaternion.Lerp(
              proximalMiddleRot,
              proximalMiddleHandleRot,
              w
            ));

            var intermediateMiddleHandleRot = Quaternion.Euler(
              0,
              0,
              math.lerp(IntermediateMiddleZRotationRange.y, IntermediateMiddleZRotationRange.x, middleT * allT)
            );
            var intermediateMiddleRot = IntermediateMiddle.GetLocalRotation(stream);
            IntermediateMiddle.SetLocalRotation(stream, Quaternion.Lerp(
              intermediateMiddleRot,
              intermediateMiddleHandleRot,
              w
            ));

            var distalMiddleHandleRot = Quaternion.Euler(
              0,
              0,
              math.lerp(DistalMiddleZRotationRange.y, DistalMiddleZRotationRange.x, middleT * allT)
            );
            var distalMiddleRot = DistalMiddle.GetLocalRotation(stream);
            DistalMiddle.SetLocalRotation(stream, Quaternion.Lerp(
              distalMiddleRot,
              distalMiddleHandleRot,
              w
            ));

            // Ring
            var proximalRingHandleRot = Quaternion.Euler(
              0,
              math.lerp(ProximalRingYRotationRange.y, ProximalRingYRotationRange.x, spreadT),
              math.lerp(ProximalRingZRotationRange.y, ProximalRingZRotationRange.x, ringT * allT)
            );
            var proximalRingRot = ProximalRing.GetLocalRotation(stream);
            ProximalRing.SetLocalRotation(stream, Quaternion.Lerp(
              proximalRingRot,
              proximalRingHandleRot,
              w
            ));

            var intermediateRingHandleRot = Quaternion.Euler(
              0,
              0,
              math.lerp(IntermediateRingZRotationRange.y, IntermediateRingZRotationRange.x, ringT * allT)
            );
            var intermediateRingRot = IntermediateRing.GetLocalRotation(stream);
            IntermediateRing.SetLocalRotation(stream, Quaternion.Lerp(
              intermediateRingRot,
              intermediateRingHandleRot,
              w
            ));

            var distalRingHandleRot = Quaternion.Euler(
              0,
              0,
              math.lerp(DistalRingZRotationRange.y, DistalRingZRotationRange.x, ringT * allT)
            );
            var distalRingRot = DistalRing.GetLocalRotation(stream);
            DistalRing.SetLocalRotation(stream, Quaternion.Lerp(
              distalRingRot,
              distalRingHandleRot,
              w
            ));

            // Little
            var proximalLittleHandleRot = Quaternion.Euler(
              0,
              math.lerp(ProximalLittleYRotationRange.y, ProximalLittleYRotationRange.x, spreadT),
              math.lerp(ProximalLittleZRotationRange.y, ProximalLittleZRotationRange.x, littleT * allT)
            );
            var proximalLittleRot = ProximalLittle.GetLocalRotation(stream);
            ProximalLittle.SetLocalRotation(stream, Quaternion.Lerp(
              proximalLittleRot,
              proximalLittleHandleRot,
              w
            ));

            var intermediateLittleHandleRot = Quaternion.Euler(
              0,
              0,
              math.lerp(IntermediateLittleZRotationRange.y, IntermediateLittleZRotationRange.x, littleT * allT)
            );
            var intermediateLittleRot = IntermediateLittle.GetLocalRotation(stream);
            IntermediateLittle.SetLocalRotation(stream, Quaternion.Lerp(
              intermediateLittleRot,
              intermediateLittleHandleRot,
              w
            ));

            var distalLittleHandleRot = Quaternion.Euler(
              0,
              0,
              math.lerp(DistalLittleZRotationRange.y, DistalLittleZRotationRange.x, littleT * allT)
            );
            var distalLittleRot = DistalLittle.GetLocalRotation(stream);
            DistalLittle.SetLocalRotation(stream, Quaternion.Lerp(
              distalLittleRot,
              distalLittleHandleRot,
              w
            ));
        }
    }

}

[System.Serializable]
public struct HandConstraintData : IAnimationJobData {

    [SyncSceneToStream, Range(0, 1)] public float thumbAmount;
    [SyncSceneToStream, Range(0, 1)] public float indexAmount;
    [SyncSceneToStream, Range(0, 1)] public float middleAmount;
    [SyncSceneToStream, Range(0, 1)] public float ringAmount;
    [SyncSceneToStream, Range(0, 1)] public float littleAmount;

    [SyncSceneToStream, Range(0, 1)] public float allAmount;
    [SyncSceneToStream, Range(0, 1)] public float spreadAmount;

    public Transform ProximalThumb;
    public Transform IntermediateThumb;
    public Transform DistalThumb;

    public Transform ProximalIndex;
    public Transform IntermediateIndex;
    public Transform DistalIndex;

    public Transform ProximalMiddle;
    public Transform IntermediateMiddle;
    public Transform DistalMiddle;

    public Transform ProximalRing;
    public Transform IntermediateRing;
    public Transform DistalRing;

    public Transform ProximalLittle;
    public Transform IntermediateLittle;
    public Transform DistalLittle;

    public Vector2 ProximalThumbXRotationRange;
    public Vector2 ProximalThumbZRotationRange;
    public Vector2 IntermediateThumbXRotationRange;
    public Vector2 DistalThumbXRotationRange;

    public Vector2 ProximalIndexZRotationRange;
    public Vector2 ProximalIndexYRotationRange;
    public Vector2 IntermediateIndexZRotationRange;
    public Vector2 DistalIndexZRotationRange;

    public Vector2 ProximalMiddleZRotationRange;
    public Vector2 ProximalMiddleYRotationRange;
    public Vector2 IntermediateMiddleZRotationRange;
    public Vector2 DistalMiddleZRotationRange;

    public Vector2 ProximalRingZRotationRange;
    public Vector2 ProximalRingYRotationRange;
    public Vector2 IntermediateRingZRotationRange;
    public Vector2 DistalRingZRotationRange;

    public Vector2 ProximalLittleZRotationRange;
    public Vector2 ProximalLittleYRotationRange;
    public Vector2 IntermediateLittleZRotationRange;
    public Vector2 DistalLittleZRotationRange;

    public string thumbAmountProperty => PropertyUtils.ConstructConstraintDataPropertyName(nameof(thumbAmount));
    public string indexAmountProperty => PropertyUtils.ConstructConstraintDataPropertyName(nameof(indexAmount));
    public string middleAmountProperty => PropertyUtils.ConstructConstraintDataPropertyName(nameof(middleAmount));
    public string ringAmountProperty => PropertyUtils.ConstructConstraintDataPropertyName(nameof(ringAmount));
    public string littleAmountProperty => PropertyUtils.ConstructConstraintDataPropertyName(nameof(littleAmount));

    public string allAmountProperty => PropertyUtils.ConstructConstraintDataPropertyName(nameof(allAmount));
    public string spreadAmountProperty => PropertyUtils.ConstructConstraintDataPropertyName(nameof(spreadAmount));

    public bool IsValid() => !(
      ProximalThumb == null ||
      IntermediateThumb == null ||
      DistalThumb == null ||

      ProximalIndex == null ||
      IntermediateIndex == null ||
      DistalIndex == null ||

      ProximalMiddle == null ||
      IntermediateMiddle == null ||
      DistalMiddle == null ||

      ProximalRing == null ||
      IntermediateRing == null ||
      DistalRing == null ||

      ProximalLittle == null ||
      IntermediateLittle == null ||
      DistalLittle == null ||

      ProximalThumbXRotationRange == null ||
      ProximalThumbZRotationRange == null ||
      IntermediateThumbXRotationRange == null ||
      DistalThumbXRotationRange == null ||

      ProximalIndexZRotationRange == null ||
      ProximalIndexYRotationRange == null ||
      IntermediateIndexZRotationRange == null ||
      DistalIndexZRotationRange == null ||

      ProximalMiddleZRotationRange == null ||
      ProximalMiddleYRotationRange == null ||
      IntermediateMiddleZRotationRange == null ||
      DistalMiddleZRotationRange == null ||

      ProximalRingZRotationRange == null ||
      ProximalRingYRotationRange == null ||
      IntermediateRingZRotationRange == null ||
      DistalRingZRotationRange == null ||

      ProximalLittleZRotationRange == null ||
      ProximalLittleYRotationRange == null ||
      IntermediateLittleZRotationRange == null ||
      DistalLittleZRotationRange == null);

    public void SetDefaultValues() {
        ProximalThumb = null;
        IntermediateThumb = null;
        DistalThumb = null;

        ProximalIndex = null;
        IntermediateIndex = null;
        DistalIndex = null;

        ProximalMiddle = null;
        IntermediateMiddle = null;
        DistalMiddle = null;

        ProximalRing = null;
        IntermediateRing = null;
        DistalRing = null;

        ProximalLittle = null;
        IntermediateLittle = null;
        DistalLittle = null;

        ProximalThumbXRotationRange = Vector2.zero;
        ProximalThumbZRotationRange = Vector2.zero;
        IntermediateThumbXRotationRange = Vector2.zero;
        DistalThumbXRotationRange = Vector2.zero;

        ProximalIndexZRotationRange = Vector2.zero;
        ProximalIndexYRotationRange = Vector2.zero;
        IntermediateIndexZRotationRange = Vector2.zero;
        DistalIndexZRotationRange = Vector2.zero;

        ProximalMiddleZRotationRange = Vector2.zero;
        ProximalMiddleYRotationRange = Vector2.zero;
        IntermediateMiddleZRotationRange = Vector2.zero;
        DistalMiddleZRotationRange = Vector2.zero;

        ProximalRingZRotationRange = Vector2.zero;
        ProximalRingYRotationRange = Vector2.zero;
        IntermediateRingZRotationRange = Vector2.zero;
        DistalRingZRotationRange = Vector2.zero;

        ProximalLittleZRotationRange = Vector2.zero;
        ProximalLittleYRotationRange = Vector2.zero;
        IntermediateLittleZRotationRange = Vector2.zero;
        DistalLittleZRotationRange = Vector2.zero;

        thumbAmount = 0;
        indexAmount = 0;
        middleAmount = 0;
        ringAmount = 0;
        littleAmount = 0;

        allAmount = 0;
        spreadAmount = 0;
    }

}

public class HandConstraintBinder : AnimationJobBinder<HandConstraintJob, HandConstraintData> {

    public override HandConstraintJob Create(Animator animator, ref HandConstraintData data, Component component) {
        var job = new HandConstraintJob();

        job.ProximalThumb = ReadWriteTransformHandle.Bind(animator, data.ProximalThumb);
        job.IntermediateThumb = ReadWriteTransformHandle.Bind(animator, data.IntermediateThumb);
        job.DistalThumb = ReadWriteTransformHandle.Bind(animator, data.DistalThumb);

        job.ProximalIndex = ReadWriteTransformHandle.Bind(animator, data.ProximalIndex);
        job.IntermediateIndex = ReadWriteTransformHandle.Bind(animator, data.IntermediateIndex);
        job.DistalIndex = ReadWriteTransformHandle.Bind(animator, data.DistalIndex);

        job.ProximalMiddle = ReadWriteTransformHandle.Bind(animator, data.ProximalMiddle);
        job.IntermediateMiddle = ReadWriteTransformHandle.Bind(animator, data.IntermediateMiddle);
        job.DistalMiddle = ReadWriteTransformHandle.Bind(animator, data.DistalMiddle);

        job.ProximalRing = ReadWriteTransformHandle.Bind(animator, data.ProximalRing);
        job.IntermediateRing = ReadWriteTransformHandle.Bind(animator, data.IntermediateRing);
        job.DistalRing = ReadWriteTransformHandle.Bind(animator, data.DistalRing);

        job.ProximalLittle = ReadWriteTransformHandle.Bind(animator, data.ProximalLittle);
        job.IntermediateLittle = ReadWriteTransformHandle.Bind(animator, data.IntermediateLittle);
        job.DistalLittle = ReadWriteTransformHandle.Bind(animator, data.DistalLittle);

        job.ProximalThumbXRotationRange = data.ProximalThumbXRotationRange;
        job.ProximalThumbZRotationRange = data.ProximalThumbZRotationRange;
        job.IntermediateThumbXRotationRange = data.IntermediateThumbXRotationRange;
        job.DistalThumbXRotationRange = data.DistalThumbXRotationRange;

        job.ProximalIndexZRotationRange = data.ProximalIndexZRotationRange;
        job.ProximalIndexYRotationRange = data.ProximalIndexYRotationRange;
        job.IntermediateIndexZRotationRange = data.IntermediateIndexZRotationRange;
        job.DistalIndexZRotationRange = data.DistalIndexZRotationRange;

        job.ProximalMiddleZRotationRange = data.ProximalMiddleZRotationRange;
        job.ProximalMiddleYRotationRange = data.ProximalMiddleYRotationRange;
        job.IntermediateMiddleZRotationRange = data.IntermediateMiddleZRotationRange;
        job.DistalMiddleZRotationRange = data.DistalMiddleZRotationRange;

        job.ProximalRingZRotationRange = data.ProximalRingZRotationRange;
        job.ProximalRingYRotationRange = data.ProximalRingYRotationRange;
        job.IntermediateRingZRotationRange = data.IntermediateRingZRotationRange;
        job.DistalRingZRotationRange = data.DistalRingZRotationRange;

        job.ProximalLittleZRotationRange = data.ProximalLittleZRotationRange;
        job.ProximalLittleYRotationRange = data.ProximalLittleYRotationRange;
        job.IntermediateLittleZRotationRange = data.IntermediateLittleZRotationRange;
        job.DistalLittleZRotationRange = data.DistalLittleZRotationRange;

        job.thumbAmount = FloatProperty.Bind(animator, component, data.thumbAmountProperty);
        job.indexAmount = FloatProperty.Bind(animator, component, data.indexAmountProperty);
        job.middleAmount = FloatProperty.Bind(animator, component, data.middleAmountProperty);
        job.ringAmount = FloatProperty.Bind(animator, component, data.ringAmountProperty);
        job.littleAmount = FloatProperty.Bind(animator, component, data.littleAmountProperty);

        job.allAmount = FloatProperty.Bind(animator, component, data.allAmountProperty);
        job.spreadAmount = FloatProperty.Bind(animator, component, data.spreadAmountProperty);

        return job;
    }

    public override void Destroy(HandConstraintJob job) { }

}
