using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public abstract class SlowUpdater : MonoBehaviour
    {
        protected readonly List<Action> slowActions = new List<Action>();
        
        private readonly WaitForSeconds _waiter = new WaitForSeconds(0.1f);

        private void Start()
        {
            PreStart();
            StartCoroutine(SlowUpdate());
        }

        protected virtual void PreStart() { }
        
        private IEnumerator SlowUpdate()
        {
            while (true)
            {
                foreach (var action in slowActions)
                {
                    action.Invoke();
                }
                
                yield return _waiter;
            }
            // ReSharper disable once IteratorNeverReturns
        }
    }
}