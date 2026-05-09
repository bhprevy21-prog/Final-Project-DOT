public class Structs
{
    public struct Tags
    {
        public const string playerTag = "Player";
        public const string Enemy = "Enemy";
    }

    public struct AnimationParameters
    {
        public const string isWalking = "IsWalking";
    }

    public struct Input
    {
        public const string vertical = "Vertical";
        public const string horizontal = "Horizontal";
    }

    public struct GameObjects
    {
        public const string talkIcon = "TalkIcon";
        public const string talkText = "TalkText";
        public const string talkPanel = "TalkPanel";
    }
    
    public struct Enemy {
        public const string boundsTag = "Bounds";
        public const string bulletTag = "Bullet";
        public const string gameControllerComponent = "GameController";
    }
}
