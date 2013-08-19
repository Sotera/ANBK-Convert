using System.Collections.Generic;
using JistBridge.Utilities.StateMachine;

namespace JistBridge.Interfaces
{
    public interface IFSMSystem
    {
        StateID CurrentStateID { get; }
        FSMState CurrentState { get; }

        /// <summary>
        /// This method places new states inside the FSM,
        /// or prints an ERROR message if the state was already inside the List.
        /// First state added is also the initial state.
        /// </summary>
        void AddState(FSMState s);

        /// <summary>
        /// This method delete a state from the FSM List if it exists, 
        ///   or prints an ERROR message if the state was not on the List.
        /// </summary>
        void DeleteState(StateID id);

        /// <summary>
        /// This method tries to change the state the FSM is in based on
        /// the current state and the transition passed. If current state
        ///  doesn't have a target state for the transition passed, 
        /// an ERROR message is printed.
        /// </summary>
        void PerformTransition(Transition trans);

        void Start(List<FSMState> fsmStates, FSMState initialState);
    }
}
