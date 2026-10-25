using System.IO;
using System.IO.Pipes;
using System.Text;

namespace Seeker.DevToolbox.App.Services;

/// <summary>
/// 单实例:named mutex 做互斥,named pipe 把第二次启动的参数转给主实例。
/// </summary>
public sealed class SingleInstance : IDisposable
{
    private const string MutexName = @"Local\Seeker.DevToolbox.SingleInstance";
    private static readonly string PipeName = $"Seeker.DevToolbox.Pipe.{Environment.UserName}";

    private readonly Mutex _mutex;
    private readonly CancellationTokenSource _cts = new();

    public SingleInstance()
    {
        _mutex = new Mutex(initiallyOwned: true, MutexName, out var createdNew);
        IsFirstInstance = createdNew;
    }

    public bool IsFirstInstance { get; }

    public event Action<string[]>? ArgumentsReceived;

    public void StartListening()
    {
        _ = Task.Run(ListenLoopAsync);
    }

    public void SendToPrimary(IEnumerable<string> arguments)
    {
        try
        {
            using var client = new NamedPipeClientStream(".", PipeName, PipeDirection.Out);
            client.Connect(1000);

            using var writer = new StreamWriter(client, new UTF8Encoding(false));
            writer.WriteLine(string.Join('\u001f', arguments));
            writer.Flush();
        }
        catch (Exception ex) when (ex is IOException or TimeoutException or UnauthorizedAccessException)
        {
            // 主实例可能正在退出,静默失败即可。
        }
    }

    private async Task ListenLoopAsync()
    {
        while (!_cts.IsCancellationRequested)
        {
            try
            {
                await using var server = new NamedPipeServerStream(
                    PipeName,
                    PipeDirection.In,
                    maxNumberOfServerInstances: 1,
                    PipeTransmissionMode.Byte,
                    PipeOptions.Asynchronous);

                await server.WaitForConnectionAsync(_cts.Token);

                using var reader = new StreamReader(server, Encoding.UTF8);
                var line = await reader.ReadLineAsync(_cts.Token);
                var args = string.IsNullOrEmpty(line) ? [] : line.Split('\u001f');

                ArgumentsReceived?.Invoke(args);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (IOException)
            {
                // 客户端提前断开,继续监听下一次。
            }
        }
    }

    public void Dispose()
    {
        _cts.Cancel();

        if (IsFirstInstance)
        {
            try
            {
                _mutex.ReleaseMutex();
            }
            catch (ApplicationException)
            {
                // 未持有互斥量时忽略。
            }
        }

        _mutex.Dispose();
        _cts.Dispose();
    }
}
