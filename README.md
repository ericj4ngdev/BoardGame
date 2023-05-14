# 게임소개
- 제목 : The aMAZEing Labyrinth

<div align="center">
    <img src= "https://github.com/ericj4ngdev/BoardGame/assets/108036322/36f05065-da6d-410e-87a3-4f2940c2f08c" />
</div>
<div align="center">
    <img src= "https://github.com/ericj4ngdev/BoardGame/assets/108036322/24afd4df-301c-4fdc-99ec-3eb481c88811" />
</div>

- 장르 : 보드게임, 턴제
- 플랫폼 : PC
- 제작 기간 : 2023-03-25 ~ 2023-05-13
- 플레이 방식 : 턴제 전략 게임
- 게임 흐름
    - 각 플레이어에게 서로다른 보물 4개가 주어지고 미로 타일이 랜덤으로 채워진다.
    - 타일 종류는 ’ㅣ‘,’ㅏ‘,’ㄱ‘ 총 3가지로 매턴마다 플레이어는 게임판 가장자리의 화살표가 있는 부분 중에서 한곳을 골라 남은 미로 타일을 밀어넣어 미로를 움직인다. 밀어넣고 나온 타일이 다음 사람이 밀어넣을 타일이 된다.
        
        ![image](https://github.com/ericj4ngdev/BoardGame/assets/108036322/e28480bf-ca50-4092-9f7c-ab85f7b92105)
        
    - 주어진 보물(=목표물)을 모두 찾아낸 후, 자신의 위치로 돌아오는 사람이 승리한다.

- 규칙
    - 자신의 차례에 미로를 반드시 한번 움직여야 한다.
    - 밀 수 있는 구역이 정해져 있고 고정된 타일도 있다.
    - 만약 아이템이 보드 밖으로 밀리면 해당 타일과 같이 이동한다.
    - 미로 타일을 밀어넣었는데 만약 반대편 맨끝에 게임말이 놓여있었다면 게임말은 반대편 타일(방금 밀어넣은 미로타일 위)로 순간이동한다.
    - 이전 게임자가 밀어넣어 나온 미로 타일로 상대가 원상복구하는 것을 방지하기 위해 밀어낸 구역의 반대편 구역이 다음 턴에 비활성화된다.
- 플레이 영상
https://youtu.be/EwcdFb7qusA
- 주요 개발 내용
https://www.notion.so/The-aMAZEing-Labyrinth-07779d5265b74b5bbfaeba23f104522c?pvs=4
- 게임 다운로드 링크
https://drive.google.com/drive/folders/1ImwF2jKaGJd5_aeAMr6Tevqhnto978Qo?usp=sharing

# 개발 환경
Visual Studio 2022 Community

Unity3D 20.3.25f1

# 개요 및 개발목적

본 게임은 보드게임 The aMAZEing Labyrinth을 모작한 게임으로 규칙기반의 AI와 플레이를 할 수 있는 싱글플레이를 구현하였다. 매 턴마다 타일을 움직여 미로를 바꾸는 것이 게임의 핵심 요소이며 길찾기 알고리즘과 규칙기반의 AI를 상대로 싱글플레이가 가능하게 하는 것이 개발의 주요 목적이다.

<div align="center">
    <img src= "https://github.com/ericj4ngdev/BoardGame/assets/108036322/9ff95f56-e2fe-4dc3-a7aa-4be0c52e6aca" />
</div>
    
- 자료구조 큐를 활용해 16개의 목표물을 2명의 플레이어에게 4개씩 겹치지 않게 랜덤으로 주어진다.
- AI의 난이도(상,중,하)를 조정할 수 있으며 AI와 1:1 싱글플레이가 가능하다.
- 매턴마다 변하는 미로속에서 AI는 dfs 알고리즘을 적용하고 규칙기반의 AI가 작동한다.
