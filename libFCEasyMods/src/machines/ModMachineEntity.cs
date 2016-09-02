using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace nialscorva.FCEEasyMods
{
    using StateCoroutine = IEnumerator<ModMachineEntity.StateReturn>;
    using Coroutine = IEnumerator<int>;

    class CoroutineEntry
    {
        protected long nextRun = 0;
        private Coroutine coroutine;
        public bool IsRunning { get; private set; } = true;

        public CoroutineEntry(Coroutine coroutine)
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
        // Logging helpers
        //=======================================================
        protected string LOGGER_PREFIX;
        protected void log(String msg, params object[] args)
        {
            UnityEngine.Debug.Log(LOGGER_PREFIX + ": " + String.Format(msg, args));
        }

        //=======================================================
        // Static fields for timing
        //=======================================================
        protected static Stopwatch stopwatch = new Stopwatch();
        static ModMachineEntity()
        {
            stopwatch.Start();
        }
        public static long Now { get { return stopwatch.ElapsedMilliseconds; }}

        //=======================================================
        // Game Object members
        //=======================================================
        protected GameObject primaryGO = null;

        //=======================================================
        // Orientation
        //=======================================================
        public Vector3 Forward  { get; protected set; }
        public Vector3 Back     { get { return -Forward; } }
        public Vector3 Left     { get; protected set; }
        public Vector3 Right    { get { return -Left; } }
        public Vector3 Up       { get; protected set; }
        public Vector3 Down     { get { return -Up; } }

        //=======================================================
        // State Management
        //=======================================================
        public class StateReturn
        {
            public long NextRun = 0;
            public StateCoroutine NextState = null;
            public StateReturn(int n) { this.NextRun = n; }
            public StateReturn(StateCoroutine i) { NextState = i; }
            public static implicit operator StateReturn(int n) { return new StateReturn(n); }
        }

        public class State
        {
            protected long nextRun = 0;
            private StateCoroutine coroutine;
            public bool IsRunning { get; private set; } = true;

            public State(StateCoroutine coroutine) { this.coroutine = coroutine; }
            public void Step(long now)
            {
                if (IsRunning && nextRun <= now)
                {
                    IsRunning = coroutine.MoveNext();
                    StateReturn sr = coroutine.Current;
                    nextRun = now + (sr?.NextRun ?? 0);
                    if (sr?.NextState != null)
                    {
                        coroutine = sr.NextState;
                        IsRunning = true;
                    }
                }
            }
        }
        
        private State CurrentState;
        private State CurrentAnimationState;


        protected static StateReturn NextState(StateCoroutine next) { return new StateReturn(next); }

        //=======================================================
        // Coroutines for the timers
        //=======================================================
        List<CoroutineEntry> lowFreqCoroutines = new List<CoroutineEntry>();
        List<CoroutineEntry> animationCoroutines = new List<CoroutineEntry>();

        //=======================================================
        // Rotation Actions
        //=======================================================
        public delegate void RotationEventHandler(ModMachineEntity sender, Quaternion rotation);
        public event RotationEventHandler RotationEvent;

        public delegate void HoverEventHandler(ModMachineEntity sender);
        public event HoverEventHandler HoverEvent;
        
        //=======================================================
        // Constructors
        //=======================================================
        protected ModMachineEntity(ModCreateSegmentEntityParameters parameters) : base(parameters) {
            mbNeedsLowFrequencyUpdate = true;
            mbNeedsUnityUpdate = true;

            LOGGER_PREFIX = "[" + this.GetType().FullName + this.ToNormalizedCoordinates() + "]";

            StartLowFrequencyCoroutine(UpdateRotationOnce());
        }

        //=======================================================
        // Low Frequency Thread Handlers
        //=======================================================
        protected void StartLowFrequencyCoroutine(Coroutine coroutine)
        {
            lowFreqCoroutines.Add(new CoroutineEntry(coroutine));
        }
        protected void SetState(StateCoroutine s)
        {
            CurrentState = new State(s);
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
            lowFreqCoroutines.RemoveAll(e => !e.IsRunning);

            // clean up the animation routines here, just to save time in the animation loop
            animationCoroutines.RemoveAll(e => !e.IsRunning);

        }

        //=======================================================
        // Render Thread Handlers
        //=======================================================
        protected virtual void InitializeGameObjects() { }

        protected void StartAnimationCoroutine(Coroutine coroutine)
        {
            animationCoroutines.Add(new CoroutineEntry(coroutine));
        }
        protected void SetAnimationState(StateCoroutine s)
        {
            CurrentAnimationState = new State(s);
        }
        public override void UnityUpdate()
        {
            base.UnityUpdate();
            if (this?.mWrapper?.mbHasGameObject != true)
            {
                return;
            }
            long now = stopwatch.ElapsedMilliseconds;
            if (primaryGO == null)
            {
                primaryGO = this.mWrapper?.mGameObjectList?[0];
                if (primaryGO != null)
                {
                    InitializeGameObjects();
                }
            }
            else
            {
                CurrentAnimationState?.Step(now);

                foreach (CoroutineEntry i in animationCoroutines)
                {
                    i.Step(now);
                }
            }
        }
        //=======================================================
        // Rotation Handlers
        //=======================================================
        public override void OnUpdateRotation(byte newFlags)
        {
            base.OnUpdateRotation(newFlags);
            Quaternion rotation = SegmentCustomRenderer.GetRotationQuaternion(newFlags);

            Forward = rotation * Vector3.forward;
            Left = rotation * Vector3.left;
            Up = rotation * Vector3.up;

            RotationEvent?.Invoke(this, rotation);
        }

        private Coroutine UpdateRotationOnce()
        {
            OnUpdateRotation(this.mFlags);
            yield break;
        }


        //=======================================================
        // Spatial Queries
        //=======================================================

        public T GetEntityAt<T>(CubeCoord relativeLoc) where T : class { return GetEntityAt<T>((long)relativeLoc.x, (long)relativeLoc.y, (long)relativeLoc.z); }
        public T GetEntityAt<T>(Vector3 relativeLoc) where T : class  {return GetEntityAt<T>((long)relativeLoc.x, (long)relativeLoc.y, (long)relativeLoc.z); }
        public T GetEntityAt<T>(long x, long y, long z) where T : class
        {
            x += this.mnX;
            y += this.mnY;
            z += this.mnZ;

            object se = base.AttemptGetSegment(x, y, z)?.SearchEntity(x, y, z);
            if (se is T)
            {
                return (T)se;
            }
            return null;

        }

        protected void StartNeighborSearchCoroutine<T>(IEnumerable<CubeCoord> coords,Func<T,Coroutine> whenFound) where T: class
        {
            StartLowFrequencyCoroutine(SearchForNeighbor<T>(coords,whenFound));
        }
        protected Coroutine SearchForNeighbor<T>(IEnumerable<CubeCoord> coords, Func<T, Coroutine> whenFound) where T : class
        {
            T neighbor=null;
            while (true)
            {
                foreach (CubeCoord c in coords)
                {
                    neighbor = GetEntityAt<T>(c);
                    if (neighbor != null)
                    {
                        break;
                    }
                    yield return 0;
                }
                // if we fell through because we found a neighbor, then delegate to the whenFound state.
                // Otherwise, loop back through and scan some more
                if (neighbor != null) {
                    Coroutine it = whenFound(neighbor);

                    while ((neighbor as SegmentEntity)?.mbDelete != true && it.MoveNext())
                    {
                        yield return it.Current;
                    }
                    
                    // resume scanning
                    neighbor = null;
                }

            }
        }

        private string DebugText ="";
        protected virtual string PopupText {  get
            {
                if (DebugText.Equals(""))
                {
                    DebugText = DebugComponentsText();
                }
                return DebugText;
            }
        }

        public override string GetPopupText()
        {
            HoverEvent(this);
            return PopupText;
        }

        //=======================================================
        // Debugging Helpers
        //=======================================================

        /* 
         * Gets a list of mesh renderers, animations, and scrollers.  Useful for putting in GetPopupText() when seeing what you can manipulate for a given GameObject
         */
        public string DebugComponentsText()
        {
            string msg = "";
            if (primaryGO == null)
            {
                return "";
            }
            foreach (GameObject go in mWrapper.mGameObjectList)
            {
                msg += go.name + ": \n  Mesh: ";
                msg += String.Join(", ",
                    (go.GetComponentsInChildren<MeshRenderer>())
                        .Select(m => m.name)
                        .ToArray()
                ) + "\n";
                foreach (Animation a in go.GetComponentsInChildren<Animation>())
                {
                    msg += "  Anim: " + a.name + "[";
                    foreach (AnimationState state in a)
                    {
                        msg += state.name + ",";
                    }
                    msg += "]\n";
                }
                var scrollers = go.GetComponentsInChildren<UVScroller_PG>();
                if (scrollers.Length > 0)
                {
                    msg += "  UVScroll_PG: " + String.Join(", ", scrollers.Select(m => m.name).ToArray());
                }
            }
            return msg;
        }
        protected void RotateMesh(Mesh m, Quaternion rot)
        {
            Vector3[] newVertices = new Vector3[m.vertices.Length];

            for (int i = 0; i < m.vertices.Length; ++i)
            {
                newVertices[i] = rot * m.vertices[i];
            }
            m.vertices = newVertices;
        }
    }
}
