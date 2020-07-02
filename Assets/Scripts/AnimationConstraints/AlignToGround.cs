using Unity.Mathematics;

namespace UnityEngine.Animations.Rigging {
    public class AlignToGround : RigConstraint<AlignToGroundJob, AlignToGroundData, AlignToGroundBinder<AlignToGroundData>> { }

    [Unity.Burst.BurstCompile]
    public struct AlignToGroundJob : IWeightedAnimationJob {
        public FloatProperty jobWeight { get; set; }

        public ReadOnlyTransformHandle source;
        public ReadWriteTransformHandle constrained;

        public Vector3Property groundPoint { get; set; }
        public Vector3Property groundNormal { get; set; }

        public FloatProperty offsetFromGround { get; set; }

        public void ProcessAnimation(AnimationStream stream) {
            float w = jobWeight.Get(stream);

            if (w > 0f) {
                //We only need to process if the weight is greater than 0
                float minOffset = offsetFromGround.Get(stream);
                Vector3 normal = groundNormal.Get(stream);

                float angle = Vector3.Angle(Vector3.up, normal);

                float offset = minOffset / math.sin(math.radians(90 - angle));

                Vector3 targetPosition = groundPoint.Get(stream) + Vector3.up * offset;
                Vector3 sourcePosition = source.GetPosition(stream);

                if (targetPosition.y > sourcePosition.y) {
                    constrained.SetPosition(stream, math.lerp(sourcePosition, targetPosition, w));

                    Quaternion rotation = quaternion.LookRotation(source.GetRotation(stream) * Vector3.forward, normal);
                    constrained.SetRotation(stream, rotation);
                } else {
                    constrained.SetPosition(stream, sourcePosition);
                    constrained.SetRotation(stream, source.GetRotation(stream));
                }
            }
        }

        public void ProcessRootMotion(AnimationStream stream) { }
    }

    [System.Serializable]
    public struct AlignToGroundData : IAnimationJobData, IAlignToGroundData {
        [SerializeField] private Transform m_constrainedObject;
        [SyncSceneToStream, SerializeField] private Transform m_sourceObject;

        [SyncSceneToStream, SerializeField, HideInInspector] private Vector3 m_groundPoint;
        [SyncSceneToStream, SerializeField, HideInInspector] private Vector3 m_groundNormal;

        [SerializeField] private LayerMask m_ignoreMask;

        [SerializeField] private float m_startPointOffset;
        [SerializeField] private float m_maxDistance;
        [SerializeField, SyncSceneToStream] private float m_offsetFromGround;


        public Transform constrainedObject { get => m_constrainedObject; set => m_constrainedObject = value; }
        public Transform sourceObject { get => m_sourceObject; set => m_sourceObject = value; }

        public Vector3 groundPoint { get => m_groundPoint; set => m_groundPoint = value; }
        public Vector3 groundNormal { get => m_groundNormal; set => m_groundNormal = value; }

        public LayerMask ignoreMask => m_ignoreMask;

        public float startPointOffset => m_startPointOffset;
        public float maxDistance => m_maxDistance;
        public float offsetFromGround => m_offsetFromGround;


        string IAlignToGroundData.groundPointVector3Property => PropertyUtils.ConstructConstraintDataPropertyName(nameof(m_groundPoint));

        string IAlignToGroundData.groundNormalVector3Property => PropertyUtils.ConstructConstraintDataPropertyName(nameof(m_groundNormal));

        string IAlignToGroundData.offsetFromGroundFloatProperty => PropertyUtils.ConstructConstraintDataPropertyName(nameof(m_offsetFromGround));

        public bool IsValid() => !(m_constrainedObject == null || m_sourceObject == null || m_groundPoint == null || m_groundNormal == null);

        public void SetDefaultValues() {
            m_constrainedObject = null;
            m_sourceObject = null;

            m_ignoreMask = 0;

            m_groundPoint = Vector3.zero;
            m_groundNormal = Vector3.up;

            m_startPointOffset = 0.6f;
            m_maxDistance = 1f;
            m_offsetFromGround = 0.5f;
        }
    }

    public interface IAlignToGroundData {
        Transform constrainedObject { get; }
        Transform sourceObject { get; }

        Vector3 groundPoint { get; set; }
        string groundPointVector3Property { get; }

        Vector3 groundNormal { get; set; }
        string groundNormalVector3Property { get; }

        float startPointOffset { get; }

        float maxDistance { get; }

        float offsetFromGround { get; }
        string offsetFromGroundFloatProperty { get; }

        LayerMask ignoreMask { get; }
    }


    public class AlignToGroundBinder<T> : AnimationJobBinder<AlignToGroundJob, T>
        where T : struct, IAnimationJobData, IAlignToGroundData {
        public override AlignToGroundJob Create(Animator animator, ref T data, Component component) {
            return new AlignToGroundJob() {
                constrained = ReadWriteTransformHandle.Bind(animator, data.constrainedObject),
                source = ReadOnlyTransformHandle.Bind(animator, data.sourceObject),
                groundPoint = Vector3Property.Bind(animator, component, data.groundPointVector3Property),
                groundNormal = Vector3Property.Bind(animator, component, data.groundNormalVector3Property),
                offsetFromGround = FloatProperty.Bind(animator, component, data.offsetFromGroundFloatProperty)
            };
        }

        public override void Update(AlignToGroundJob job, ref T data) {
            Ray ray = new Ray(data.sourceObject.position + Vector3.up * data.startPointOffset, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, data.maxDistance, ~data.ignoreMask)) {
                data.groundPoint = hit.point;
                data.groundNormal = hit.normal;
            }

            base.Update(job, ref data);
        }

        public override void Destroy(AlignToGroundJob job) { }
    }
}
