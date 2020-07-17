public class PlaneStateManager {
    public PlaneControl Controller { get; private set; }
    public StateMachine Machine { get; private set; }
    public FolllowPath StateFollowPath { get; private set; }
    public Landing StateLanding { get; private set; }
    public FreeFly StateFreeFly { get; private set; }
    public PlaneStateManager (PlaneControl controller, StateMachine stateMachine) {
        Controller = controller;
        Machine = stateMachine;
        Init ();
    }

    private void Init () {
        StateFollowPath = new FolllowPath (this);
        StateFreeFly = new FreeFly (this);
        StateLanding = new Landing (this);
    }
}