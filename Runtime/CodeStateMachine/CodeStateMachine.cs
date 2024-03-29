using System;
using System.Collections;
using MTPS.Core.Attributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
#endif

namespace MTPS.Core.CodeStateMachine
{
    public class CodeStateMachine : MonoBehaviour
    {
        [SerializeField] protected bool startWhenResolverIsReady = true;
        public ReferenceResolver ReferenceResolver;
        protected IStateMachineVariables Variables;

#if UNITY_EDITOR
        [StatesAddButton]
        public
#else
    [SerializeField] private
#endif
            State[] states = Array.Empty<State>();
        
        public State CurrentState
        {
            get => states[CurrentStateIndex];
        }
        
        public int CurrentStateIndex { get; private set; }

        public UnityEvent onStateChanged;

        protected bool isStarted;


        
        protected virtual void Awake()
        {
            Variables = GetComponent<IStateMachineVariables>();

            if (startWhenResolverIsReady)
            {
                return;
            }

            foreach (var codeState in states)
            {
                codeState.CacheReferences(Variables,ReferenceResolver);
                foreach (var codeStateTransition in codeState.Transitions)
                {
                    codeStateTransition.Initialise(Variables,ReferenceResolver);
                }
            }

            CurrentStateIndex = 0;
        }
        

#if UNITY_EDITOR
        public void EDITORSetStateIndex(int i) => CurrentStateIndex = i;
        public event Action EDITOR_OnValidate;
        
        public void OnValidate()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode) return;
            bool isDirty = false;
            var statesProp = new SerializedObject(this).FindProperty("states");

            for (var s = 0; s < states.Length; s++)
            {
                var codeState = states[s];
                var features = statesProp.GetArrayElementAtIndex(s).FindPropertyRelative("features");


                for (var f = 0; f < codeState.features.Length; f++)
                {
                    try
                    {
                        codeState.features[f].path =
                            features.GetArrayElementAtIndex(f).propertyPath;
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }


                var Transitions = statesProp.GetArrayElementAtIndex(s).FindPropertyRelative("Transitions");
                for (var f = 0; f < codeState.Transitions.Length; f++)
                {
                    ref var codeStateTransition = ref codeState.Transitions[f];
                    if (codeStateTransition == null) continue;
                      
                    isDirty = codeStateTransition.ValidateTransition(ref states);
                    codeStateTransition.path = Transitions.GetArrayElementAtIndex(f).propertyPath;
                }
                
                foreach (var codeStateTransition in codeState.Transitions)
                {
                    if (codeStateTransition != null)
                        isDirty = codeStateTransition.ValidateTransition(ref states);
                }
            }

            statesProp.serializedObject.ApplyModifiedPropertiesWithoutUndo();

            if (isDirty) 
                EditorUtility.SetDirty(this);
            
            EDITOR_OnValidate?.Invoke();
        }

        /// <summary>
        /// sometime bugs happens and transitions and features across different states become
        /// one objects non unique,this function is to fix that bug after reason found
        /// </summary>
        [ContextMenu("Repair")]
        public void Repair()
        {
            var copy = states.Clone() as State[];
            states = new State[copy.Length];
            
            var statesProp = new SerializedObject(this).FindProperty("states"); 
            for (int i = 0; i < states.Length; i++)
            {
                states[i] = new State(copy[i]);
                statesProp.serializedObject.Update();
                var features = statesProp.GetArrayElementAtIndex(i).FindPropertyRelative("features");

                for (var f = 0; f < states[i].features.Length; f++)
                {
                    try
                    {
                        states[i].features[f].path =
                            features.GetArrayElementAtIndex(f).propertyPath;
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }

                var transitions = statesProp.GetArrayElementAtIndex(i).FindPropertyRelative("Transitions");
                for (var f = 0; f < states[i].Transitions.Length; f++)
                {
                    try
                    {
                        states[i].Transitions[f].path =
                            transitions.GetArrayElementAtIndex(f).propertyPath;
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }
            OnValidate();
        }
#endif

        protected virtual IEnumerator Start()
        {
            if (startWhenResolverIsReady)
            {
                if(!ReferenceResolver.isReady)
                    yield return new WaitUntil(() => ReferenceResolver.isReady);
                
                foreach (var codeState in states)
                {
                    codeState.CacheReferences(Variables,ReferenceResolver);
                    foreach (var codeStateTransition in codeState.Transitions)
                    {
                        codeStateTransition.Initialise(Variables,ReferenceResolver);
                    }
                }
        
                CurrentStateIndex = 0;
            }
           
            
            CurrentState.OnEnterState();
            isStarted = true;
        }

        protected virtual void Update()
        {
            if(!isStarted) return;
            CurrentState.OnUpdateState();
            foreach (var transition in CurrentState.Transitions)
            {
                if (transition.couldHaveTransition)
                {
                    CurrentState.OnExitState();
                    CurrentStateIndex = transition.GetNextStateIndex();
                    CurrentState.OnEnterState();
                    onStateChanged?.Invoke();
                    break;
                }
            }
        }

        protected virtual void FixedUpdate()
        {
            if(!isStarted) return;
            CurrentState.OnFixedUpdateState();
        }
    }
}