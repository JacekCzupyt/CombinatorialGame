namespace CombinatorialGameLibrary.GameState {
    public struct VictoryState {
        public bool GameEnded;
        public int? Winner;
        public int? SequenceStart;
        public int? SequenceJump;

        public static VictoryState None => new VictoryState {
            GameEnded = false,
            Winner = null,
            SequenceStart = null,
            SequenceJump = null
        };
        
        public static VictoryState Tie => new VictoryState {
            GameEnded = true,
            Winner = 0,
            SequenceStart = null,
            SequenceJump = null
        };
    }
}
