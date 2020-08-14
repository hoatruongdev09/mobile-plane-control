[System.Serializable]
public class GameStateManager {
    public GameController GameController { get; private set; }
    public StateMachine StateMachine { get; private set; }

    public GameInitState InitState { get; private set; }
    public GameStartedState StartedState { get { return startedState; } private set { startedState = value; } }
    public GamePauseState PauseState { get; private set; }
    public GameOverState OverState { get; private set; }

    public GameStartedState startedState;
    public GameStateManager (GameController gameController, StateMachine stateMachine) {
        this.GameController = gameController;
        this.StateMachine = stateMachine;
        Init ();
    }

    private void Init () {
        InitState = new GameInitState (this);
        StartedState = new GameStartedState (this);
        PauseState = new GamePauseState (this);
        OverState = new GameOverState (this);
    }
}