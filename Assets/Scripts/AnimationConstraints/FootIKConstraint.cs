using System;
using Unity.Burst;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

public class FootIKConstraint : RigConstraint<FootIKConstraintJob, FootIKConstraintData, FootIKConstraintBinder> { }

[BurstCompile]
public struct FootIKConstraintJob : IWeightedAnimationJob {
    public FloatProperty jobWeight { get; set; }

    public Vector2Property ikOffset;
    public Vector3Property normalLeftFoot;
    public Vector3Property normalRightFoot;

    public ReadOnlyTransformHandle leftToe;
    public ReadOnlyTransformHandle leftAnkle;
    public ReadOnlyTransformHandle rightToe;
    public ReadOnlyTransformHandle rightAnkle;

    public ReadWriteTransformHandle leftEffector;
    public ReadWriteTransformHandle rightEffector;

    public ReadWriteTransformHandle hips;

    public FloatProperty weightShiftHorizontal;
    public FloatProperty weightShiftVertical;
    public FloatProperty weightShiftAngle;

    public FloatProperty maxFootRotationOffset;

    public void ProcessRootMotion(AnimationStream stream) { }

    public void ProcessAnimation(AnimationStream stream) {
        var w = jobWeight.Get(stream);

        if (w == 0f)
            return;

        var offset = ikOffset.Get(stream);

        var leftToePos = leftToe.GetPosition(stream);
        var rightToePos = rightToe.GetPosition(stream);

        var leftIkOffset = offset.x * w;
        var rightIkOffset = offset.y * w;

        leftToePos += new Vector3(0f, leftIkOffset, 0f);
        rightToePos += new Vector3(0f, rightIkOffset, 0f);


        var leftAnklePos = leftAnkle.GetPosition(stream);
        var rightAnklePos = rightAnkle.GetPosition(stream);
        var leftAnkleRot = leftAnkle.GetRotation(stream);
        var rightAnkleRot = rightAnkle.GetRotation(stream);

        var leftAnkleIkPos = new Vector3(leftAnklePos.x, leftAnklePos.y + leftIkOffset, leftAnklePos.z);
        var rightAnkleIkPos = new Vector3(rightAnklePos.x, rightAnklePos.y + rightIkOffset, rightAnklePos.z);

        var hipHeightOffset = (leftIkOffset + rightIkOffset) * 0.5f;
        var forwardBackBias = (leftIkOffset - rightIkOffset) * weightShiftHorizontal.Get(stream);

        float maxRotationOffset = maxFootRotationOffset.Get(stream);

        // TODO: (sunek) Rework weight shift to move towards actual lower foot?
        hipHeightOffset += Mathf.Abs(leftIkOffset - rightIkOffset) * weightShiftVertical.Get(stream);
        var standAngle = Quaternion.AngleAxis(weightShiftAngle.Get(stream), Vector3.up) * Vector3.forward;
        hips.SetPosition(stream, hips.GetPosition(stream) + new Vector3(standAngle.x * forwardBackBias, hipHeightOffset, standAngle.z * forwardBackBias));

        // Figure out the normal rotation
        var leftNormalRot = Quaternion.LookRotation(Vector3.forward, normalLeftFoot.Get(stream));
        var rightNormalRot = Quaternion.LookRotation(Vector3.forward, normalRightFoot.Get(stream));

        // Clamp normal rotation
        var leftAngle = Quaternion.Angle(Quaternion.identity, leftNormalRot);
        var rightAngle = Quaternion.Angle(Quaternion.identity, rightNormalRot);

        if (leftAngle > maxRotationOffset && maxRotationOffset > 0f) {
            var fraction = maxRotationOffset / leftAngle;
            leftNormalRot = Quaternion.Lerp(Quaternion.identity, leftNormalRot, fraction);
        }

        if (rightAngle > maxRotationOffset && maxRotationOffset > 0f) {
            var fraction = maxRotationOffset / rightAngle;
            rightNormalRot = Quaternion.Lerp(Quaternion.identity, rightNormalRot, fraction);
        }

        // Apply rotation to ankle                
        var leftToesMatrix = Matrix4x4.TRS(leftToePos, Quaternion.identity, Vector3.one);
        var rightToesMatrix = Matrix4x4.TRS(rightToePos, Quaternion.identity, Vector3.one);

        var leftToesNormalDeltaMatrix = Matrix4x4.TRS(leftToePos, leftNormalRot, Vector3.one) * leftToesMatrix.inverse;
        var rightToesNormalDeltaMatrix = Matrix4x4.TRS(rightToePos, rightNormalRot, Vector3.one) * rightToesMatrix.inverse;

        var leftAnkleMatrix = Matrix4x4.TRS(leftAnkleIkPos, leftAnkleRot, Vector3.one) * leftToesMatrix.inverse;
        var rightAnkleMatrix = Matrix4x4.TRS(rightAnkleIkPos, rightAnkleRot, Vector3.one) * rightToesMatrix.inverse;

        leftAnkleMatrix = leftToesNormalDeltaMatrix * leftAnkleMatrix * leftToesMatrix;
        rightAnkleMatrix = rightToesNormalDeltaMatrix * rightAnkleMatrix * rightToesMatrix;

        leftAnkleIkPos = leftAnkleMatrix.GetColumn(3);
        rightAnkleIkPos = rightAnkleMatrix.GetColumn(3);

        leftAnkleRot = Quaternion.Lerp(leftAnkleRot, leftAnkleMatrix.rotation, w);
        rightAnkleRot = Quaternion.Lerp(rightAnkleRot, rightAnkleMatrix.rotation, w);

        // Update ik position   
        // TODO: (sunek) Consider combating leg overstretch
        var leftPosition = Vector3.Lerp(leftAnklePos, leftAnkleIkPos, w);
        var rightPosition = Vector3.Lerp(rightAnklePos, rightAnkleIkPos, w);

        leftEffector.SetPosition(stream, leftPosition);
        rightEffector.SetPosition(stream, rightPosition);
        leftEffector.SetRotation(stream, leftAnkleRot);
        rightEffector.SetRotation(stream, rightAnkleRot);
    }
}

[Serializable]
public struct FootIKConstraintData : IAnimationJobData {

    [SyncSceneToStream] public Vector2 ikOffset;
    [SyncSceneToStream] public Vector3 normalLeftFoot;
    [SyncSceneToStream] public Vector3 normalRightFoot;

    [SyncSceneToStream] public Transform leftAnkle;
    [SyncSceneToStream] public Transform leftToe;
    [SyncSceneToStream] public Transform rightAnkle;
    [SyncSceneToStream] public Transform rightToe;

    [SyncSceneToStream] public Transform leftEffector;
    [SyncSceneToStream] public Transform rightEffector;

    [SyncSceneToStream] public Transform hips;

    [SyncSceneToStream] public float weightShiftHorizontal;
    [SyncSceneToStream] public float weightShiftVertical;
    [SyncSceneToStream] public float weightShiftAngle;

    [SyncSceneToStream] public float maxFootRotationOffset;

    public string ikOffsetProperty => PropertyUtils.ConstructConstraintDataPropertyName(nameof(ikOffset));
    public string normalLeftFootProperty => PropertyUtils.ConstructConstraintDataPropertyName(nameof(normalLeftFoot));
    public string normalRightFootProperty => PropertyUtils.ConstructConstraintDataPropertyName(nameof(normalRightFoot));

    public string weightShiftHorizontalProperty => PropertyUtils.ConstructConstraintDataPropertyName(nameof(weightShiftHorizontal));
    public string weightShiftVerticalProperty => PropertyUtils.ConstructConstraintDataPropertyName(nameof(weightShiftVertical));
    public string weightShiftAngleProperty => PropertyUtils.ConstructConstraintDataPropertyName(nameof(weightShiftAngle));
    public string maxFootRotationOffsetProperty => PropertyUtils.ConstructConstraintDataPropertyName(nameof(maxFootRotationOffset));

    public bool IsValid() {
        return leftToe != null
            && rightToe != null
            && leftAnkle != null
            && rightAnkle != null
            && leftEffector != null
            && rightEffector != null
            && hips != null;
    }

    public void SetDefaultValues() {
        ikOffset = Vector2.zero;
        normalLeftFoot = Vector3.zero;
        normalRightFoot = Vector3.zero;

        leftAnkle = null;
        rightAnkle = null;

        leftToe = null;
        rightToe = null;

        leftEffector = null;
        rightEffector = null;

        hips = null;

        weightShiftVertical = 0;
        weightShiftHorizontal = 0;
        weightShiftAngle = 0;
    }
}

public class FootIKConstraintBinder : AnimationJobBinder<FootIKConstraintJob, FootIKConstraintData> {
    public override FootIKConstraintJob Create(Animator animator, ref FootIKConstraintData data, Component component) {
        var job = new FootIKConstraintJob();

        job.leftToe = ReadOnlyTransformHandle.Bind(animator, data.leftToe);
        job.leftAnkle = ReadOnlyTransformHandle.Bind(animator, data.leftAnkle);
        job.rightToe = ReadOnlyTransformHandle.Bind(animator, data.rightToe);
        job.rightAnkle = ReadOnlyTransformHandle.Bind(animator, data.rightAnkle);

        job.leftEffector = ReadWriteTransformHandle.Bind(animator, data.leftEffector);
        job.rightEffector = ReadWriteTransformHandle.Bind(animator, data.rightEffector);

        job.hips = ReadWriteTransformHandle.Bind(animator, data.hips);

        job.weightShiftAngle = FloatProperty.Bind(animator, component, data.weightShiftAngleProperty);
        job.weightShiftHorizontal = FloatProperty.Bind(animator, component, data.weightShiftHorizontalProperty);
        job.weightShiftVertical = FloatProperty.Bind(animator, component, data.weightShiftVerticalProperty);

        job.maxFootRotationOffset = FloatProperty.Bind(animator, component, data.maxFootRotationOffsetProperty);

        job.ikOffset = Vector2Property.Bind(animator, component, data.ikOffsetProperty);
        job.normalLeftFoot = Vector3Property.Bind(animator, component, data.normalLeftFootProperty);
        job.normalRightFoot = Vector3Property.Bind(animator, component, data.normalRightFootProperty);

        return job;
    }

    public override void Update(FootIKConstraintJob job, ref FootIKConstraintData data) {
        base.Update(job, ref data);
    }

    public override void Destroy(FootIKConstraintJob job) {
    }
}