
using System;
using UnityEngine;
public interface IState
{
    public bool Active { get; set; }
    void EnterState(params StateConfig.IBaseStateConfig[] configs);
    void FixedUpdateState(float delta);
    void ExitState();
}


public abstract class BaseState : MonoBehaviour, IState
{
    public bool Active { get; set; }

    public BaseStateMachine baseStateMachine { get; protected set; }

    public virtual void EnterState(params StateConfig.IBaseStateConfig[] configs)
    {
        CheckForStateMachine();
        Active = true;
        EnterStateInternal(configs);
    }

    protected abstract void EnterStateInternal(params StateConfig.IBaseStateConfig[] configs);
    public void UpdateState(float delta)
    {
        CheckForStateMachine();
        UpdateStateInternal(delta);
    }
    public void FixedUpdateState(float delta)
    {
        CheckForStateMachine();
        FixedUpdateStateInternal(delta);
    }
    protected abstract void UpdateStateInternal(float delta);
    protected abstract void FixedUpdateStateInternal(float delta);

    void CheckForStateMachine()
    {
        if (baseStateMachine == null)
        {
            throw new UnassignedReferenceException(nameof(baseStateMachine));
        }
    }

    public void ExitState()
    {
        if (!Active)
        {
            return;
        }
        Active = false;
        ExitStateInternal();
    }
    protected abstract void ExitStateInternal();

}

public abstract class BaseState<T> : BaseState where T : MonoBehaviour
{

    public T Agent { get; set; }

    public StateMachine<T> StateMachine { get; private set; }

    public virtual void SetAgent(T newAgent)
    {
        Agent = newAgent;
    }
    public virtual void SetStateMachine(StateMachine<T> newStateMachine)
    {
        baseStateMachine = StateMachine = newStateMachine;
    }

    protected virtual void ChangeState<newStateType>(params StateConfig.IBaseStateConfig[] configs) where newStateType : BaseState<T>
    {
        if (StateMachine == null)
        {
            throw new UnassignedReferenceException(nameof(baseStateMachine));
        }
        StateMachine.SetNextState(new StateChangeRequest(typeof(newStateType), configs));
    }
}
public class StateChangeRequest
{
    public Type StateType;
    public StateConfig.IBaseStateConfig[] Configs;

    public StateChangeRequest(Type newStateType, params StateConfig.IBaseStateConfig[] newConfigs)
    {
        this.StateType = newStateType;
        this.Configs = newConfigs;
    }
}