public class GameStateManager {
    public GameController GameController { get; private set; }
    public StateMachine StateMachine { get; private set; }

    public GameInitState InitState { get; private set; }
    public GameStartedState StartedState { get; private set; }
    public GamePauseState PauseState { get; private set; }

    public GameStateManager (GameController gameController, StateMachine stateMachine) {
        this.GameController = gameController;
        this.StateMachine = stateMachine;
        Init ();
    }

    private void Init () {
        InitState = new GameInitState (this);
        StartedState = new GameStartedState (this);
        PauseState = new GamePauseState (this);
    }
}