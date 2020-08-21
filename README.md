# UnityTouchGesture

## 1. 개요
UnityTouchGesture는 손가락의 제스쳐를 인식하여 동작을 수행합니다!

 - ## What
   - 개발중인 게임에 제스쳐 인식을 위한 코드가 필요했다. 에셋으로 해결하는 방법도 있지만 개발 비용을 줄이기 위해 우리 게임에 필요한 기능만 따로 빼서 개발하기로 했다.
   - 게임에 Tap, Swipe, Rotate, Long 제스쳐가 필요했다
 - ## How
    - 제스쳐를 구분하기 위해 각각의 규칙을 세웠다
    ### 1. Tap
        터치 시간이 짧고 터치 시작점과 끝점의 간격이 짧다.
    ### 2. Swipe
        터치 시간이 짧고 터치 시작점과 끝점의 간격이 짧다
        또한 시작점과 끝점의 좌표를 비교해서 스와이프의 방향을 알 수 있다
    ### 3. Rotate
        각 프레임마다 터치위치의 차이를 각도로 표현하면 45도씩 나누어 8방향을 모두 지나침 
        (추후 입력의 편의성을 위해 7방향만 지나쳐도 Rotate로 인정하게 하였음)
    ### 4. Long
        터치 시간이 길다.
    - 제스쳐의 입력과정을 세단계로 나누었다.
      1. #### 터치 시작
         - Rotate 계산에 필요한 8방향 계산 초기화
         - 터치 시간 초기화
         - 터치 시작점 기록
         - 터치 시작시 입력된 손가락의 고유번호 기록
      2. #### 터치 중
         - 이전터치와 현재터치간의 거리를 각도를 구함
         - 구한 각도를 8방향으로 변환함
         - 8방향을 바탕으로 Rotate 여부를 확인함
         - 터치 시간 기록
      3. #### 터치 끝
         - 터치 시간이 길 경우 Long 제스쳐로 분류, 처리 종료
         - 
 - ## Problem
     이렇게 규칙을 세울경우 Long 제스쳐에 문제가 발생한다.\
     Long 제스쳐를 입력받을경우 즉시 입력이 되지 않고 '터치 시간이 길다'를 인지할때까지의 n초가 필요했다.\
     그렇다고 입력을 받자마자 Long으로 인식시키면 다른 제스쳐들과의 모호성이 생긴다.\
     그렇기에 입력의 유무만 처리하여 Long 제스쳐를 필요로하는 오브젝트에서 Long을 감지하게 하였다.\
     또한 Unity의 GetTouch로 터치 정보를 가져올 경우 먼저 들어온 터치를 땔 경우 GetTouch의 인자와 손가락의 고유번호가 일치하지 않는 문제가 발생하였다.\
     이를 해결하기 위해 '터치중' 과정에서 손가락 고유번호를 바탕으로 GetTouch의 인자를 자동으로 추적하는 과정을 추가하였음.
## 2. 사용법

#### 1. 해당 스크립트를 프로젝트에 추가
#### 2. GameObject에 해당 스크립트를 Component로 추가
#### 3. 해당 스크립트에 주석처리된 Do~~~ 줄에 동작 추가
#### 3-1. ex) DoTap, DoSwipe, DoRotate
#### 3-2 Long 제스쳐의 경우 Long을 감지해야하는 오브젝트에서 해당 스크립트의 isTouching의 여부를 확인하면 됨, Index는 첫번째 터치, 두번째 터치
