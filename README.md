# BoardGame
제작 일지 : https://www.notion.so/Labyrinth-78a18ad22e9d42e18ad065fa6b2de361?pvs=4

## 제작 일지
### 5.4

- ScanBoard() 함수 생성
- start하자마자 Scan하면 아이템과 플레이어가 자식 오브젝트로 들어가기 전이라 맵정보가 아이템, 플레이어 없이 스캔되어 무의미한 정보 스캔
- 따라서 지금은 num0, num1 입력으로 text파일에 보드를 갱신할수 있도록 만듬.
- 아이템 먹을 시 setActive(false)를 Destroy로 바꿈. 이유는 GetChild는 비활성화된 것도 포함하여 플레이어랑 아이템 같이 있을 시 아이템만 표시. 임시방편으로 Destroy를 썼다.
