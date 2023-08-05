namespace GemMatch {
    public enum GameResult { None, Clear, Fail, Error }
    public enum SimulationResult { OnProgress, Error, Clear, Fail }

    public enum ColorIndex {
        None = -1, 
        Red, Orange, Yellow, Green, Blue, Purple, Brown, Pink, Cyan, 
        Random, // 이 컬러로 지정되었다면 게임 시작시 랜덤으로 컬러가 계산되어 지정된다.
        Dummy, // 시뮬레이션 용도로 사용되는 더미 컬러로, 픽할 수 있다.
    }
}