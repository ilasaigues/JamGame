using System;
using System.Collections.Generic;
using AstralCore;
using UnityEngine;

public abstract class BaseStateMachine : TimeboundMonoBehaviour
{
}

public abstract class StateMachine<T> : BaseStateMachine where T : MonoBehaviour
{
    readonly Dictionary<Type, BaseState<T>> StateDict = new();

    private BaseState<T> _currentState = null;
    private StateChangeRequest _changeStateRequest;
    public T Agent { get; protected set; }
    public void SetAgent(T agent)
    {
        Agent = agent;
        var childStates = GetComponentsInChildren<BaseState<T>>();
        foreach (var state in childStates)
        {
            state.SetStateMachine(this);
            StateDict[state.GetType()] = state;
            state.SetAgent(agent);
        }
    }

    public void SetNextState(StateChangeRequest newRequest)
    {
        _changeStateRequest = newRequest;
    }

    void Update()
    {
        _currentState?.UpdateState(DeltaTime);

        if (_changeStateRequest != null)
        {
            ChangeToNextState();
            _changeStateRequest = null;
        }
    }

    void FixedUpdate()
    {
        _currentState?.FixedUpdateState(FixedDeltaTime);
    }

    private void ChangeToNextState()
    {
        _currentState?.ExitState();
        _currentState = StateDict[_changeStateRequest.StateType];
        _currentState.EnterState(_changeStateRequest.Configs);
    }

}