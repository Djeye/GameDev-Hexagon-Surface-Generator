using System;
using UnityEngine;

namespace ProceduralAnimation
{
    [RequireComponent(typeof(CharacterController))]
    public class Mech : MonoBehaviour
    {
        [Header("Legs Data")]
        [SerializeField] private LegData[] legs;
        
        [Space]
        [Header("Legs parameters")]
        [SerializeField] private float stepSpeed = 0.5f;
        [SerializeField] private float stepLength = 0.75f;
        [SerializeField] private float stepHeight = 0.25f;
        [SerializeField] private float stepMoveImpact = 0.5f;
        [SerializeField] private AnimationCurve stepCurve;

        [Serializable]
        private struct LegData
        {
            [field: SerializeField] public LegTarget Leg { get; private set; }
            [field: SerializeField] public LegRaycast Raycast { get; private set; }

            public void Init(float stepSpeed, float stepHeight, AnimationCurve stepCurve, Transform parentTransform, float moveImpact)
            {
                Leg.Init(stepSpeed, stepHeight, stepCurve);
                Raycast.Init(parentTransform, moveImpact);
            }
        }


        private void Start()
        {
            InitLegs();
        }

        private void InitLegs()
        {
            foreach (var legData in legs)
            {
                legData.Init(stepSpeed, stepHeight, stepCurve, transform, stepMoveImpact);
            }
        }

        public void UpdateLegs(Vector3 moveDirection)
        {
            for (int index = 0; index < legs.Length; index++)
            {
                var legData = legs[index];
                legData.Raycast.ApplyMoveDirection(moveDirection);
                
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