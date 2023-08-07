namespace GemMatch {
    public enum ColorIndex {
        None = -1, 
        Red = 0, 
        Orange = 1, 
        Yellow = 2, 
        Green = 3, 
        Blue = 4, 
        Purple = 5, 
        Brown = 6, 
        Pink = 7, 
        Cyan = 8, 
        Random = 1000, // 이 컬러로 지정되었다면 게임 시작시 랜덤으로 컬러가 계산되어 지정된다.
        Sole = 2000, // 시뮬레이션 용도로 사용되는 더미 컬러로, 픽할 수 있다.
    }
}