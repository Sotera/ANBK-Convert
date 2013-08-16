using System.Collections.Generic;
using System.Diagnostics;

namespace JistBridge.Utilities.StateMachine
{
    /// <summary>
    /// This class represents the States in the Finite State System.
    /// Each state has a Dictionary with pairs (transition-state) showing
    /// which state the FSM should be if a transition is fired while this state
    /// is the current state.
    /// Method Reason is used to determine which transition should be fired .
    /// Method Act has the code to perform the actions the NPC is supposed do if it's on this state.
    /// </summary>
    public abstract class FSMState
    {
        protected Dictionary<Transition, StateID> map = new Dictionary<Transition, StateID>();
        protected StateID stateID;
        public StateID ID { get { return stateID; } }
        public bool IsActive
        {
            get; protected set;
        }

        public void AddTransition(Transition trans, StateID id)
        {
            // Check if anyone of the args is invalid
            if (trans == Transition.NullTransition)
            {
                Debug.WriteLine("FSMState ERROR: NullTransition is not allowed for a real transition");
                return;
            }

            if (id == StateID.NullStateID)
            {
                Debug.WriteLine("FSMState ERROR: NullStateID is not allowed for a real ID");
                return;
            }

            // Since this is a Deterministic FSM,
            //   check if the current transition was already inside the map
            if (map.ContainsKey(trans))
            {
                Debug.WriteLine("FSMState ERROR: State " + stateID.ToString() + " already has transition " + trans.ToString() +
                                "Impossible to assign to another state");
                return;
            }

            map.Add(trans, id);
        }

        /// <summary>
        /// This method deletes a pair transition-state from this state's map.
        /// If the transition was not inside the state's map, an ERROR message is printed.
        /// </summary>
        public void DeleteTransition(Transition trans)
        {
            // Check for NullTransition
            if (trans == Transition.NullTransition)
            {
                Debug.WriteLine("FSMState ERROR: NullTransition is not allowed");
                return;
            }

            // Check if the pair is inside the map before deleting
            if (map.ContainsKey(trans))
            {
                map.Remove(trans);
                return;
            }
            Debug.WriteLine("FSMState ERROR: Transition " + trans.ToString() + " passed to " + stateID.ToString() +
                            " was not on the state's transition list");
        }

        /// <summary>
        /// This method returns the new state the FSM should be if
        ///    this state receives a transition and 
        /// </summary>
        public StateID GetOutputState(Transition trans)
        {
            // Check if the map has this transition
            if (map.ContainsKey(trans))
            {
                return map[trans];
            }
            return StateID.NullStateID;
        }

        /// <summary>
        /// This method is used to set up the State condition before entering it.
        /// It is called automatically by the FSMSystem class before assigning it
        /// to the current state.
        /// </summary>
        public virtual void DoBeforeEntering()
        {
            IsActive = true;
        }

        /// <summary>
        /// This method is used to make anything necessary, as reseting variables
        /// before the FSMSystem changes to another one. It is called automatically
        /// by the FSMSystem before changing to a new state.
        /// </summary>
        public virtual void DoBeforeLeaving()
        {
            IsActive = false;
        }
    }
}