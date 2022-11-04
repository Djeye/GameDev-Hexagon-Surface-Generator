using System;
using UnityEngine;

namespace ProceduralAnimation
{
    public class Mech : MonoBehaviour
    {
        [Header("Legs Data")]
        [SerializeField] private LegData[] legs;

        [Space]
        [Header("Legs parameters")]
        [SerializeField] private float stepSpeed = 0.5f;
        [SerializeField] private float stepLength = 0.75f;
        [SerializeField] private float stepHeight = 0.25f;
        [SerializeField] private AnimationCurve stepCurve;

        [Serializable]
        private struct LegData
        {
            public LegTarget leg;
            public LegRaycast raycast;

            public void Init(float stepSpeed, float stepHeight, AnimationCurve stepCurve, Transform parentTransform)
            {
                leg.Init(stepSpeed, stepHeight, stepCurve);
                raycast.Init(parentTransform);
            }
        }


        private void Start()
        {
            foreach (var legData in legs)
            {
                legData.Init(stepSpeed, stepHeight, stepCurve, transform);
            }
        }

        private void Update()
        {
            for (int index = 0; index < legs.Length; index++)
            {
                var legData = legs[index];
                if (!CanMove(index))
                {
                    continue;
                }

                if (!legData.leg.IsMoving &&
                    !(Vector3.Distance(legData.leg.Position, legData.raycast.Position) > stepLength))
                {
                    continue;
                }

                legData.leg.MoveTo(legData.raycast.Position);
            }
        }

        private bool CanMove(int legIndex)
        {
            int legsCount = legs.Length;
            int prevLegIndex = (legIndex + legsCount - 1) % legsCount;
            int nextLegIndex = (legIndex + 1) % legsCount;

            LegData prevLeg = legs[prevLegIndex];
            LegData nextLeg = legs[nextLegIndex];

            return !prevLeg.leg.IsMoving && !nextLeg.leg.IsMoving;
        }
    }
}