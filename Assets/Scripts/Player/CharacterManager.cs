
public class CharacterManager : Singleton<CharacterManager>
{
    private Player _player;
    public Player Player
    {
        get { return _player; }
        set { _player = value; }
    }
    protected override void Awake()
    {
        base.Awake();
    }
}
