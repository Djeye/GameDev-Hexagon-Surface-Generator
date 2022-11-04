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
            public LegTarget Leg { get; private set; }
            public LegRaycast Raycast  { get; private set; }

            public void Init(float stepSpeed, float stepHeight, AnimationCurve stepCurve, Transform parentTransform)
            {
                Leg.Init(stepSpeed, stepHeight, stepCurve);
                Raycast.Init(parentTransform);
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

                if (!legData.Leg.IsMoving &&
                    !(Vector3.Distance(legData.Leg.Position, legData.Raycast.Position) > stepLength))
                {
                    continue;
                }

                legData.Leg.MoveTo(legData.Raycast.Position);
            }
        }

        private bool CanMove(int legIndex)
        {
            int legsCount = legs.Length;
            int prevLegIndex = (legIndex + legsCount - 1) % legsCount;
            int nextLegIndex = (legIndex + 1) % legsCount;

            LegData prevLeg = legs[prevLegIndex];
            LegData nextLeg = legs[nextLegIndex];

            return !prevLeg.Leg.IsMoving && !nextLeg.Leg.IsMoving;
        }
    }
}