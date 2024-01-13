namespace Kuchinashi
{
    public interface IState
    {
        public abstract bool OnCondition();
        
        public abstract void OnEnter();

        public virtual void OnUpdate() {}

        void OnLateUpdate() {}

        void OnFixedUpdate() {}

        public abstract void OnExit();
    }
}