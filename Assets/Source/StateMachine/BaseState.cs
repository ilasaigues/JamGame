
using System;
using UnityEngine;
public interface IState
{
    public bool Active { get; set; }
    void EnterState(params StateConfig.IBaseStateConfig[] configs);
    void FixedUpdateState(float delta);
    void ExitState();
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

public abstract class BaseState : MonoBehaviour, IState
{
    public bool Active { get; set; }

    public BaseStateMachine baseStateMachine;

    public virtual void EnterState(params StateConfig.IBaseStateConfig[] configs)
    {
        if (baseStateMachine == null)
        {
            throw new UnassignedReferenceException(nameof(baseStateMachine));
        }
        Active = true;
        EnterStateInternal(configs);
    }

    protected abstract void EnterStateInternal(params StateConfig.IBaseStateConfig[] configs);
    public void FixedUpdateState(float delta)
    {
        if (baseStateMachine == null)
        {
            throw new UnassignedReferenceException(nameof(baseStateMachine));
        }
        FixedUpdateStateInternal(delta);
    }
    protected abstract void FixedUpdateStateInternal(double delta);

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

    protected virtual void ChangeState<newStateType>(params StateConfig.IBaseStateConfig[] configs) where newStateType : BaseState<newStateType>
    {
        if (StateMachine == null)
        {
            throw new UnassignedReferenceException(nameof(baseStateMachine));
        }
        StateMachine.SetNextState(new StateChangeRequest(typeof(newStateType), configs));
    }
}