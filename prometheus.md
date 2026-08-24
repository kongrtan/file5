# RV Line 프로메테우스 매트릭 설계 및 C# 수집 가이드

## 1. 매트릭 명명 및 정의

프로메테우스 표준 매트릭 명명 규칙(`snake_case`)에 따른 RV 라인 관련 매트릭 정의입니다.

| 매트릭명 | 타입 | 설명 |
| :--- | :--- | :--- |
| `rv_line_registered_total` | **Gauge** | 관리자가 등록한 전체 RV 라인 총개수 (증감이 드물음) |
| `rv_line_pool_connections` | **Gauge** | 현재 TCP 커넥션이 맺어져 Pool에 등록된 RV 라인 수 |
| `rv_line_active_lines` | **Gauge** | 특정 시간 기준(`window` 레이블) 이내 실제 사용된 RV 라인 수 |

* **관계 크기 비교:** `rv_line_registered_total` $\ge$ `rv_line_pool_connections` \ge` `rv_line_active_lines`

---

## 2. 프로메테우스 익스포터 노출 예시

```text
# HELP rv_line_registered_total Total number of registered RV lines by admin.
# TYPE rv_line_registered_total gauge
rv_line_registered_total 100

# HELP rv_line_pool_connections Current number of TCP connected RV lines in the pool.
# TYPE rv_line_pool_connections gauge
rv_line_pool_connections 45

# HELP rv_line_active_lines Number of RV lines active within the specified time window.
# TYPE rv_line_active_lines gauge
rv_line_active_lines{window="10m"} 12
```

---

## 3. C# 구현 가이드 (`prometheus-net`)

### 수집 및 갱신 주기
* **수집 주기 (Scrape Interval):** **10초** 권장 (실시간성과 시스템 부하의 최적 균형)
* **갱신 방식:** 
  * 이벤트 발생 시(TCP 연결/해제) 즉시 업데이트
  * 주기적 집계(10분 내 사용량)는 백그라운드 Task에서 10초 간격으로 계산 및 `Set()` 수행

### C# 코드 예시

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using Prometheus;

public class RvLineMetricsCollector
{
    private static readonly Gauge RegisteredTotal = Metrics
        .CreateGauge("rv_line_registered_total", "Total registered RV lines by admin.");

    private static readonly Gauge PoolConnections = Metrics
        .CreateGauge("rv_line_pool_connections", "Current TCP connections in pool.");

    private static readonly Gauge ActiveLines = Metrics
        .CreateGauge("rv_line_active_lines", "Active lines in time window.", new GaugeConfiguration
        {
            LabelNames = new[] { "window" }
        });

    // 1. 이벤트 기반 갱신
    public void OnTcpConnected() => PoolConnections.Inc();
    public void OnTcpDisconnected() => PoolConnections.Dec();

    // 2. 백그라운드 10초 주기 갱신
    public async Task StartMonitoringAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            UpdateActiveLinesMetric("10m");

            await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
        }
    }

    private void UpdateActiveLinesMetric(string windowType)
    {
        switch (windowType)
        {
            case "10m":
                int count10m = GetActiveLineCount(TimeSpan.FromMinutes(10));
                ActiveLines.WithLabels("10m").Set(count10m);
                break;

            case "1h":
                int count1h = GetActiveLineCount(TimeSpan.FromHours(1));
                ActiveLines.WithLabels("1h").Set(count1h);
                break;

            default:
                break;
        }
    }

    private int GetActiveLineCount(TimeSpan timeWindow)
    {
        // 실사용 계산 로직 작성
        return 12;
    }
}
```

---

## 4. PromQL 활용 예시

* **Pool 점유율 (%):**
  ```promql
  (rv_line_pool_connections / rv_line_registered_total) * 100
  ```
* **Pool 대비 실사용률 (%):**
  ```promql
  (rv_line_active_lines{window="10m"} / rv_line_pool_connections) * 100
  ```
