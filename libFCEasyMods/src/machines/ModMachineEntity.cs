using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace nialsorva.FCEEasyMods
{
    using StateCoroutine = IEnumerator<ModMachineEntity.StateReturn>;
    class CoroutineEntry
    {
        protected long nextRun = 0;
        private IEnumerator<int> coroutine;
        public bool IsRunning { get; private set; } = true;

        public CoroutineEntry(IEnumerator<int> coroutine)
        {
            this.coroutine = coroutine;
        }
        public CoroutineEntry(IEnumerable<int> coroutine)
        {
            this.coroutine = coroutine.GetEnumerator();
        }
        public void Step(long now)
        {
            if (IsRunning && nextRun <= now)
            {
                IsRunning = coroutine.MoveNext();
                nextRun = now + coroutine.Current;
            }
        }
    }
    public class ModMachineEntity : MachineEntity
    {
        //=======================================================
        // Key constants
        //=======================================================
        public const string BUTTON_EXTRACT = "Extract";  // Typically 'Q'
        public const string BUTTON_INTERACT = "Interact";  // Typically 'E'
        public const string BUTTON_STORE = "Store";  // Typically 'T'

        public const int MOD_NONE = 0;
        public const int MOD_SHIFT = 1;
        public const int MOD_CTRL = 2;
        public const int MOD_ALT = 4;

        //=======================================================
        // Static fields for timing
        //=======================================================
        protected static Stopwatch stopwatch = new Stopwatch();
        static ModMachineEntity()
        {
            stopwatch.Start();
        }

        //=======================================================
        // Members
        //=======================================================
        protected GameObject primaryGO = null;




        //=======================================================
        // Orientation
        //=======================================================
        public Vector3 Forward { get; protected set; }
        public Vector3 Left { get; protected set; }
        public Vector3 Up { get; protected set; }

        //=======================================================
        // State Management
        //=======================================================
        public class StateReturn
        {
            public long nextRun=0;
            public IEnumerator<StateReturn> nextState =null;
            public StateReturn(int n) { this.nextRun = n; }
            public StateReturn(IEnumerator<StateReturn> i) { nextState=i; }

            public static implicit operator StateReturn(int n) { return new StateReturn(n); }
        }

        public class State
        {
            protected long nextRun = 0;
            private IEnumerator<StateReturn> coroutine;
            public bool IsRunning { get; private set; } = true;

            public State(IEnumerator<StateReturn> coroutine)  {this.coroutine = coroutine; }
            public State(IEnumerable<StateReturn> coroutine)  {this.coroutine = coroutine.GetEnumerator(); }
            public void Step(long now)
            {
                if (IsRunning && nextRun <= now)
                {
                    IsRunning = coroutine.MoveNext();
                    StateReturn sr = coroutine.Current;
                    nextRun = now + (sr?.nextRun ?? 0);
                    if(sr?.nextState !=null)
                    {
                        coroutine = sr.nextState;
                        IsRunning = true;
                    }
                }
            }
        }
        protected static StateReturn NextState(IEnumerable<StateReturn> next) { return new StateReturn(next.GetEnumerator()); }
        protected static StateReturn NextState(IEnumerator<StateReturn> next) { return new StateReturn(next); }


        private State CurrentState;
        private State CurrentVisualState;

        protected void InitialState(IEnumerable<StateReturn> s)
        {
            CurrentState = new State(s);
        }

        protected void InitialVisualState(IEnumerable<StateReturn> s)
        {
            CurrentVisualState = new State(s);
        }

        //=======================================================
        // Coroutines for the timers
        //=======================================================
        List<CoroutineEntry> lowFreqCoroutines = new List<CoroutineEntry>();
        List<CoroutineEntry> animationCoroutines = new List<CoroutineEntry>();

        //=======================================================
        // Manually Tracked Event Listeners
        //=======================================================
        List<Tuple<string, int, Action>> buttonActions = new List<Tuple<string, int, Action>>();


        //=======================================================
        // Manually Tracked Event Listeners
        //=======================================================
        public delegate void RotationEventHandler(ModMachineEntity sender,Quaternion rotation);
        public event RotationEventHandler RotationEvent;



        //=======================================================
        // Constructors
        //=======================================================

        public ModMachineEntity(ModCreateSegmentEntityParameters parameters, SpawnableObjectEnum spawnable) : this(parameters,spawnable,eSegmentEntity.Mod) { }

        public ModMachineEntity(ModCreateSegmentEntityParameters parameters,SpawnableObjectEnum spawnable, eSegmentEntity segEntity) :
            base(segEntity, spawnable, parameters.X, parameters.Y, parameters.Z, 
                parameters.Cube, parameters.Flags, parameters.Value, parameters.Position, parameters.Segment)
        {
               
        }


        public void AddButtonAction(string button, Action action)  { AddButtonAction(button, MOD_NONE, action); }
        public void AddButtonAction(string button, int modMask, Action action)
        {
            buttonActions.Add(Tuple.Create(button, modMask, action));
        }


        protected void checkButtons()
        {
            var actions = buttonActions.Where(a => Input.GetButtonDown(a.Item1));
            if (actions.Count() == 0)
                return;

            int keymask = 0;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                keymask |= MOD_SHIFT;
            if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
                keymask |= MOD_ALT;
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                keymask |= MOD_CTRL;

            foreach (var v in actions.Where(a=> a.Item2==keymask))
            {
                v.Item3();
            }
        }

        

        protected void StartLowFrequencyCoroutine(IEnumerable<int> coroutine)
        {
            lowFreqCoroutines.Add(new CoroutineEntry(coroutine.GetEnumerator()));
        }
        protected void StartAnimationCoroutine(IEnumerable<int> coroutine)
        {
            animationCoroutines.Add(new CoroutineEntry(coroutine.GetEnumerator()));
        }

        public override void LowFrequencyUpdate()
        {
            base.LowFrequencyUpdate();
            long now = stopwatch.ElapsedMilliseconds;

            CurrentState?.Step(now);

            foreach (CoroutineEntry i in lowFreqCoroutines)
            {
                i.Step(now);
            }
            // clean up the coroutines that have ended
            lowFreqCoroutines.RemoveAll(e => e.IsRunning);

            // clean up the animation routines here, just to save time in the animation loop
            animationCoroutines.RemoveAll(e => e.IsRunning);

        }

        public override void UnityUpdate()
        {
            base.UnityUpdate();
            long now = stopwatch.ElapsedMilliseconds;

            CurrentVisualState?.Step(now);

            foreach (CoroutineEntry i in animationCoroutines)
            {
                i.Step(now);
            }

        }

        protected void UpdateOrientationVectors(Quaternion rotation)
        {
            Forward = rotation * Vector3.forward;
            Left = rotation * Vector3.left;
            Up = rotation * Vector3.up;
         }

        public override void OnUpdateRotation(byte newFlags)
        {
            base.OnUpdateRotation(newFlags);
            Quaternion rotation=SegmentCustomRenderer.GetRotationQuaternion(newFlags);

            UpdateOrientationVectors(rotation);

            RotationEvent?.Invoke(this,rotation);
        }

        public override string GetPopupText()
        {
            checkButtons();
            return "";
        }

        /* 
         * Gets a list of mesh renderers, animations, and scrollers.  Useful for putting in GetPopupText() when seeing what you can manipulate for a given GameObject
         */
        public string DebugComponentsText()
        {
            string msg = "";
            foreach (GameObject go in mWrapper.mGameObjectList)
            {
                msg += go.name + ": \n  Mesh: ";
                msg += String.Join(", ",
                    (go.GetComponentsInChildren<MeshRenderer>())
                        .Select(m => m.name)
                        .ToArray()
                ) + "\n";
                foreach (Animation a in go.GetComponentsInChildren<Animation>()) {
                    msg += "  Anim: " + a.name + "[";
                    foreach (AnimationState state in a)
                    {
                        msg += state.name + ",";
                    }
                    msg += "]\n";
                }
                var scrollers = go.GetComponentsInChildren<UVScroller_PG>();
                if (scrollers.Length > 0) {
                    msg += "  UVScroll_PG: " + String.Join(", ", scrollers.Select(m => m.name).ToArray());
                }
            }
            return msg;
        }
    }

}
