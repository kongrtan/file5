using System;
using System.Collections.Concurrent;
using TIBCO.Rendezvous;

public class RvMultiTransportManager
{
    // 멀티 스레드 환경(Dispatcher 등)에서의 안전한 관리를 위해 ConcurrentDictionary 사용
    private readonly ConcurrentDictionary<string, TransportContext> _transports = new ConcurrentDictionary<string, TransportContext>();
    private Queue _queue;
    private Dispatcher _dispatcher;

    public class TransportContext
    {
        public string Key { get; set; }
        public NetTransport Transport { get; set; }
        public Subscriber DisconnectSub { get; set; }
    }

    public void Initialize()
    {
        TIBCO.Rendezvous.Environment.Open();
        _queue = new Queue();
        _dispatcher = new Dispatcher(_queue);
    }

    // Transport 추가 및 disconnect 감지 등록
    public void AddTransport(string key, string service, string network, string daemon)
    {
        var transport = new NetTransport(service, network, daemon);

        var context = new TransportContext
        {
            Key = key,
            Transport = transport
        };

        // Subscriber 생성 시 마지막 closure 인자에 context(또는 key)를 전달
        context.DisconnectSub = new Subscriber(
            _queue,
            transport,
            "_RV.WARN.SYSTEM.RVD.DISCONNECTED",
            context
        );

        context.DisconnectSub.MessageReceived += OnRvdDisconnected;

        _transports.TryAdd(key, context);
    }

    private void OnRvdDisconnected(object listener, MessageReceivedEventArgs e)
    {
        // closure 객체를 가져와 어느 Transport에서 발생했는지 식별
        if (e.Message.Closure is TransportContext context)
        {
            Console.WriteLine($"[연결 끊김] Key: {context.Key}의 rvd 연결이 끊어졌습니다.");

            RemoveAndDestroyTransport(context.Key);
        }
    }

    private void RemoveAndDestroyTransport(string key)
    {
        if (_transports.TryRemove(key, out var context))
        {
            try
            {
                // 핸들러 해제 및 RV 리소스 정리
                context.DisconnectSub.MessageReceived -= OnRvdDisconnected;
                context.DisconnectSub.Destroy();
                context.Transport.Destroy();

                Console.WriteLine($"[정리 완료] Key: {key}의 Transport 및 Subscriber 리소스가 제거되었습니다.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[오류] Key: {key} 리소스 정리 중 예외 발생: {ex.Message}");
            }
        }
    }

    public void CloseAll()
    {
        foreach (var key in _transports.Keys)
        {
            RemoveAndDestroyTransport(key);
        }

        _dispatcher?.Destroy();
        _queue?.Destroy();
        TIBCO.Rendezvous.Environment.Close();
    }
}
