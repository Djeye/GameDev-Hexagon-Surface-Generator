using System;
using UnityEngine;

namespace ProceduralAnimation
{
    public class Mech : MonoBehaviour
    {
        [Header("Legs Data")]
        [SerializeField] private LegTarget[] legs;

        [Space] [Header("Legs parameters")]
        [SerializeField] private LegInfo legInfo;

        [Serializable]
        public struct LegInfo
        {
            public float stepSpeed;
            public float stepLength;
            public float stepHeight;
            public float moveImpact;
            public AnimationCurve stepCurve;
        }

        public void InitLegs(Transform transform)
        {
            foreach (LegTarget leg in legs)
            {
                leg.Init(legInfo, transform);
            }
        }

        public void UpdateLegsMovement(Vector3 moveDirection)
        {
            for (int index = 0; index < legs.Length; index++)
            {
                LegTarget leg = legs[index];
                
                if (!CanMoveByAdjLegs(index))
                {
                    continue;
                }

                leg.ApplyMoveDirection(moveDirection);
                
                if (!leg.IsPossibleToMove)
                {
                    continue;
                }

                leg.Move();
            }

            bool CanMoveByAdjLegs(int legIndex)
            {
                int legsCount = legs.Length;
                int prevLegIndex = (legIndex + legsCount - 1) % legsCount;
                int nextLegIndex = (legIndex + 1) % legsCount;

                LegTarget prevLeg = legs[prevLegIndex];
                LegTarget nextLeg = legs[nextLegIndex];

                return !prevLeg.IsMoving && !nextLeg.IsMoving;
            }
        }
    }
}