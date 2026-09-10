# Grafana Business Text 패널 - 가변 행 테이블 2단 배치

행 개수가 고정이 아닌 가변적인 테이블을, 화면이 넓을 때 좌우 2단으로 나눠 한 화면에 보이게 하는 방법.

## 핵심 아이디어

- 테이블(`<table>`)은 행 단위 컬럼 분할이 불가능하므로 **div 기반 가짜 테이블**로 구성
- CSS **`column-count`** (다단 레이아웃)을 사용하면 행 개수에 상관없이 브라우저가 자동으로 좌/우 단에 나눠서 채워줌
- 데이터를 수동으로 반씩 자를 필요 없음 → 가변 행 개수에 적합

## 템플릿 코드

```html
<div class="grafana-split-table">
  {{#each data}}
    <div class="row">
      <span class="col">{{this.name}}</span>
      <span class="col">{{this.value}}</span>
    </div>
  {{/each}}
</div>

<style>
.grafana-split-table {
  column-count: 2;
  column-gap: 24px;
  column-fill: auto;
}
.grafana-split-table .row {
  display: flex;
  justify-content: space-between;
  break-inside: avoid;
  padding: 2px 6px;
  font-size: 12px;
}
</style>
```

## 옵션별 설명

| 속성 | 역할 | 비고 |
|---|---|---|
| `column-count: 2` | 2단으로 분할 | 3단 이상도 가능 |
| `column-fill: auto` | 왼쪽 컬럼부터 순서대로 채움 | 기본값 `balance`는 좌우 높이를 억지로 맞추려 함 |
| `break-inside: avoid` | 행이 컬럼 경계에서 잘리지 않게 함 | 없으면 행이 위아래로 짤림 |

## 헤더 처리

- `column-count`는 헤더를 각 단마다 자동 복제해주지 않음
- 해결 방법 두 가지
  1. 헤더는 데이터 흐름 밖에 두고 grid로 별도 배치
  2. 헤더 없이 각 행을 `이름: 값` 형태 인라인 라벨로 표시 (Business Text 패널에서 더 흔한 패턴)

## 반응형 (패널 폭에 따라 1단/2단 전환)

```css
@media (max-width: 400px) {
  .grafana-split-table { column-count: 1; }
}
```

패널(iframe) 폭이 좁아지면 자동으로 1단으로 전환됨.

## 참고: 행을 수동으로 반씩 나누는 대안 (행 개수가 고정일 때만)

가변 행에는 부적합하지만, 행 개수가 고정이라면 flex/grid로 두 테이블을 나란히 배치하는 방법도 있음.

```css
.table-wrap {
  display: flex;
  gap: 24px;
}
.split-table { flex: 1; }

@media (max-width: 900px) {
  .table-wrap { flex-direction: column; }
}
```
