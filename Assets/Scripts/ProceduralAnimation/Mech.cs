using System;
using UnityEngine;

namespace ProceduralAnimation
{
    public class Mech : MonoBehaviour
    {
        [SerializeField] private LegData[] legs;

        [SerializeField] private float stepLength = 0.75f;

        [Serializable]
        private struct LegData
        {
            public LegTarget leg;
            public LegRaycast raycast;
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
            var legcount = legs.Length;
            var n1 = legs[(legIndex + legcount - 1) % legcount];
            var n2 = legs[(legIndex + 1) % legcount];
            return !n1.leg.IsMoving && !n2.leg.IsMoving;
        }
    }
}