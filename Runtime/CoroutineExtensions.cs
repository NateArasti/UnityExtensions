using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UnityExtensions
{
    public static class CoroutineExtensions
    {
        #region Wait Classes Cached

        private static readonly Dictionary<float, WaitForSeconds> WaitSecondsDictionary =
            new Dictionary<float, WaitForSeconds>();
        ///<summary>
        /// Non-allocated WaitForSeconds
        ///</summary>
        public static WaitForSeconds WaitSeconds(float waitTime)
        {
            if (WaitSecondsDictionary.TryGetValue(waitTime, out var waitForSeconds))
            {
                return waitForSeconds;
            }

            WaitSecondsDictionary[waitTime] = new WaitForSeconds(waitTime);
            return WaitSecondsDictionary[waitTime];
        }

        private static readonly Dictionary<float, WaitForSecondsRealtime> WaitSecondsRealtimeDictionary =
            new Dictionary<float, WaitForSecondsRealtime>();

        ///<summary>
        /// Non-allocated WaitForSecondsRealtime
        ///</summary>
        public static WaitForSecondsRealtime WaitSecondsRealtime(float waitTime)
        {
            if (WaitSecondsRealtimeDictionary.TryGetValue(waitTime, out var waitForSeconds))
                return waitForSeconds;

            WaitSecondsRealtimeDictionary[waitTime] = new WaitForSecondsRealtime(waitTime);
            return WaitSecondsRealtimeDictionary[waitTime];
        }

        /// <summary>
        /// Non-allocated WaitForFixedUpdate
        /// </summary>
        public static WaitForFixedUpdate WaitForFixedUpdate { get; } = new WaitForFixedUpdate();

        /// <summary>
        /// Non-allocated WaitForEndOfFrame
        /// </summary>
        public static WaitForEndOfFrame WaitForEndOfFrame { get; } = new WaitForEndOfFrame();

        #endregion

        #region Coroutines Invoking

        private static CoroutineBehaviour _separateCoroutineBehaviour;
        /// <summary>
        /// This is behaviour that is running all coroutines that were invoked separately
        /// </summary>
        public static CoroutineBehaviour SeparateCoroutineBehaviour
        {
            get
            {
                if (_separateCoroutineBehaviour == null)
                {
                    _separateCoroutineBehaviour = new GameObject(
                        "---- Separate Behaviour For Coroutine ----",
                        typeof(CoroutineBehaviour)).GetComponent<CoroutineBehaviour>();
                }
                return _separateCoroutineBehaviour;
            }
        }

        /// <summary>
        /// Invoking action after predicate returns true on a separate behaviour
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="action"></param>
        public static MonoBehaviour InvokeAfter(
            Func<bool> predicate,
            UnityAction action
            )
        {
            return StartCoroutine(
                SeparateCoroutineBehaviour,
                AfterDelay(action, predicate));
        }

        /// <summary>
        /// Invoking action after predicate returns true on a separate behaviour
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="action"></param>
        /// <param name="coroutine"></param>
        public static MonoBehaviour InvokeAfter(
            Func<bool> predicate,
            UnityAction action,
            out Coroutine coroutine
            )
        {
            return StartCoroutine(
                SeparateCoroutineBehaviour,
                AfterDelay(action, predicate),
                out coroutine);
        }

        /// <summary>
        /// Invoking action after specified frames delay on a separate behaviour
        /// </summary>
        /// <param name="action">Action to invoke</param>
        /// <param name="framesCountDelay">Count of frames that will be </param>
        public static MonoBehaviour InvokeFramesDelayed(
            UnityAction action,
            int framesCountDelay
            )
        {
            return StartCoroutine(
                SeparateCoroutineBehaviour,
                FramesDelay(action, framesCountDelay));
        }

        /// <summary>
        /// Invoking action after specified frames delay on a separate behaviour
        /// </summary>
        /// <param name="action">Action to invoke</param>
        /// <param name="framesCountDelay">Count of frames that will be </param>
        /// <param name="coroutine"></param>
        public static MonoBehaviour InvokeFramesDelayed(
            UnityAction action,
            int framesCountDelay,
            out Coroutine coroutine
            )
        {
            return StartCoroutine(
                SeparateCoroutineBehaviour,
                FramesDelay(action, framesCountDelay),
                out coroutine);
        }

        /// <summary>
        /// Invoking action after specified seconds delay on a separate behaviour
        /// </summary>
        /// <param name="action">Action to invoke</param>
        /// <param name="delay">Timed delay in seconds</param>
        public static MonoBehaviour InvokeSecondsDelayed(
            UnityAction action,
            float delay
            )
        {
            return StartCoroutine(
                SeparateCoroutineBehaviour,
                SecondsDelay(action, delay));
        }

        /// <summary>
        /// Invoking action after specified seconds delay on a separate behaviour
        /// </summary>
        /// <param name="action">Action to invoke</param>
        /// <param name="delay">Timed delay in seconds</param>
        /// <param name="coroutine"></param>
        public static MonoBehaviour InvokeSecondsDelayed(
            UnityAction action,
            float delay,
            out Coroutine coroutine
            )
        {
            return StartCoroutine(
                SeparateCoroutineBehaviour,
                SecondsDelay(action, delay),
                out coroutine);
        }

        /// <summary>
        /// Invoking action after specified seconds delay in realtime on a separate behaviour
        /// </summary>
        /// <param name="action">Action to invoke</param>
        /// <param name="delay">Timed delay in realtime seconds</param>
        public static MonoBehaviour InvokeRealtimeSecondsDelayed(
            UnityAction action,
            float delay
            )
        {
            return StartCoroutine(
                SeparateCoroutineBehaviour,
                RealtimeSecondsDelay(action, delay));
        }

        /// <summary>
        /// Invoking action after specified seconds delay in realtime on a separate behaviour
        /// </summary>
        /// <param name="action">Action to invoke</param>
        /// <param name="delay">Timed delay in realtime seconds</param>
        /// <param name="coroutine"></param>
        public static MonoBehaviour InvokeRealtimeSecondsDelayed(
            UnityAction action,
            float delay,
            out Coroutine coroutine
            )
        {
            return StartCoroutine(
                SeparateCoroutineBehaviour,
                RealtimeSecondsDelay(action, delay),
                out coroutine);
        }

        /// <summary>
        /// Invoking action after predicate returns true
        /// </summary>
        /// <param name="behaviour"></param>
        /// <param name="predicate"></param>
        /// <param name="action"></param>
        public static MonoBehaviour InvokeAfter(
            this MonoBehaviour behaviour,
            Func<bool> predicate,
            UnityAction action
            )
        {
            return StartCoroutine(
                behaviour,
                AfterDelay(action, predicate));
        }

        /// <summary>
        /// Invoking action after specified frames delay
        /// </summary>
        /// <param name="behaviour">Which behaviour will start delay coroutine</param>
        /// <param name="action">Action to invoke</param>
        /// <param name="framesCountDelay">Count of frames that will be </param>
        public static MonoBehaviour InvokeFramesDelayed(
            this MonoBehaviour behaviour,
            UnityAction action,
            int framesCountDelay
            )
        {
            return StartCoroutine(
                behaviour,
                FramesDelay(action, framesCountDelay));
        }

        /// <summary>
        /// Invoking action after specified seconds delay
        /// </summary>
        /// <param name="behaviour">Which behaviour will start delay coroutine</param>
        /// <param name="action">Action to invoke</param>
        /// <param name="delay">Timed delay in seconds</param>
        public static MonoBehaviour InvokeSecondsDelayed(
            this MonoBehaviour behaviour,
            UnityAction action,
            float delay
            )
        {
            return StartCoroutine(
                behaviour,
                SecondsDelay(action, delay));
        }

        /// <summary>
        /// Invoking action after specified seconds delay in realtime
        /// </summary>
        /// <param name="behaviour">Which behaviour will start delay coroutine</param>
        /// <param name="action">Action to invoke</param>
        /// <param name="delay">Timed delay in realtime seconds</param>
        public static MonoBehaviour InvokeRealtimeSecondsDelayed(
            this MonoBehaviour behaviour,
            UnityAction action,
            float delay
            )
        {
            return StartCoroutine(
                behaviour,
                RealtimeSecondsDelay(action, delay));
        }

        private static IEnumerator AfterDelay(UnityAction action, Func<bool> predicate)
        {
            yield return new WaitUntil(predicate);
            action.Invoke();
        }

        private static IEnumerator FramesDelay(UnityAction action, int framesCountDelay)
        {
            for (var i = 0; i < framesCountDelay; ++i)
                yield return null;
            action.Invoke();
        }

        private static IEnumerator SecondsDelay(UnityAction action, float delay)
        {
            yield return WaitSeconds(delay);
            action.Invoke();
        }

        private static IEnumerator RealtimeSecondsDelay(UnityAction action, float delay)
        {
            yield return WaitSecondsRealtime(delay);
            action.Invoke();
        }

        private static MonoBehaviour StartCoroutine(
            MonoBehaviour behaviour,
            IEnumerator enumerator)
        {
            behaviour.StartCoroutine(enumerator);
            return behaviour;
        }

        private static MonoBehaviour StartCoroutine(
            MonoBehaviour behaviour,
            IEnumerator enumerator,
            out Coroutine coroutine)
        {
            coroutine = behaviour.StartCoroutine(enumerator);
            return behaviour;
        }
    }

    public class CoroutineBehaviour : MonoBehaviour
    {
    }

    #endregion
}