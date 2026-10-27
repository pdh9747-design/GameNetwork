
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Concurrent;

class Program
{
    static string peerId = "";
    static int sequence = 0;
    static int x = 0, y = 0;

    static readonly ConcurrentDictionary<string, int> lastSequences = new();
    static readonly ConcurrentDictionary<string, string> peerPositions = new();

    static UdpClient? udp;
    static IPEndPoint remote = new(IPAddress.Loopback, 9001);

    static long receivedCount = 0;
    static long invalidCount = 0;
    static long ignoredCount = 0;

    static async Task Main()
    {
        Console.Write("내 피어 ID (A 또는 B): ");
        peerId = (Console.ReadLine() ?? "A").Trim().ToUpper();

        if (peerId != "A" && peerId != "B")
        {
            Console.WriteLine("피어 ID는 A 또는 B만 가능합니다.");
            return;
        }

        int localPort = peerId == "A" ? 9000 : 9001;
        int remotePort = peerId == "A" ? 9001 : 9000;

        remote = new IPEndPoint(IPAddress.Loopback, remotePort);

        try
        {
            udp = new UdpClient(localPort);
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"포트를 열 수 없습니다: {ex.Message}");
            return;
        }

        _ = Task.Run(ReceiveLoop);

        Console.WriteLine($"내 포트: {localPort}, 상대 포트: {remotePort}");
        Console.WriteLine("좌표 입력: x y (예: 3 5)");
        Console.WriteLine("명령어: test-invalid, test-old, test-duplicate, test-load100, test-load1000, stats, exit");

        while (true)
        {
            Console.Write("> ");
            string? input = Console.ReadLine();

            if (input == null || input.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
                break;

            input = input.Trim();

            if (input == "test-invalid")
            {
                await SendRaw("잘못된|메시지");
                Console.WriteLine("잘못된 형식의 메시지 전송 완료");
                continue;
            }

            if (input == "test-old")
            {
                await SendRaw($"{peerId}|0|99|99");
                Console.WriteLine("순서 번호 0인 메시지 전송 완료");
                continue;
            }

            if (input == "test-duplicate")
            {
                int testSeq = sequence + 1;
                string msg = $"{peerId}|{testSeq}|77|88";
                await SendRaw(msg);
                await Task.Delay(200);
                await SendRaw(msg);
                Console.WriteLine($"같은 메시지 두 번 전송: 순서 번호 {testSeq}");
                continue;
            }

            if (input == "test-load100" || input == "test-load1000")
            {
                int count = input == "test-load100" ? 100 : 1000;
                var watch = Stopwatch.StartNew();

                for (int i = 0; i < count; i++)
                {
                    sequence++;
                    await SendRaw($"{peerId}|{sequence}|{x + i}|{y + i}");
                }

                watch.Stop();
                Console.WriteLine($"{count}개 전송 시도 완료, 송신 시간: {watch.ElapsedMilliseconds} ms");
                continue;
            }

            if (input == "stats")
            {
                Console.WriteLine($"수신 적용: {receivedCount}, 잘못된 메시지: {invalidCount}, 오래됨/중복: {ignoredCount}");
                foreach (var entry in peerPositions)
                    Console.WriteLine($"상대 {entry.Key}: {entry.Value}");
                continue;
            }

            string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2 ||
                !int.TryParse(parts[0], out int nx) ||
                !int.TryParse(parts[1], out int ny))
            {
                Console.WriteLine("잘못된 입력입니다. 예: 3 5");
                continue;
            }

            x = nx;
            y = ny;
            sequence++;

            await SendRaw($"{peerId}|{sequence}|{x}|{y}");

            Console.WriteLine($"내 위치: ({x}, {y}), 전송 순서: {sequence}");
            PrintPeers();
        }

        udp.Close();
    }

    static async Task SendRaw(string message)
    {
        if (udp == null)
            return;

        byte[] data = Encoding.UTF8.GetBytes(message);
        await udp.SendAsync(data, data.Length, remote);
    }

    static async Task ReceiveLoop()
    {
        while (udp != null)
        {
            try
            {
                UdpReceiveResult result = await udp.ReceiveAsync();
                string message = Encoding.UTF8.GetString(result.Buffer);
                string[] parts = message.Split('|');

                if (parts.Length != 4 ||
                    string.IsNullOrWhiteSpace(parts[0]) ||
                    !int.TryParse(parts[1], out int seq) ||
                    !int.TryParse(parts[2], out int rx) ||
                    !int.TryParse(parts[3], out int ry))
                {
                    invalidCount++;
                    Console.WriteLine("\n잘못된 메시지 무시");
                    Console.Write("> ");
                    continue;
                }

                string sender = parts[0];

                if (sender == peerId)
                    continue;

                if (lastSequences.TryGetValue(sender, out int last) && seq <= last)
                {
                    ignoredCount++;
                    Console.WriteLine($"\n오래된/중복 메시지 무시: 피어 {sender}, 순서 {seq}");
                    Console.Write("> ");
                    continue;
                }

                lastSequences[sender] = seq;
                peerPositions[sender] = $"({rx}, {ry}), 순서 {seq}";
                receivedCount++;

                Console.WriteLine($"\n상대 {sender} 위치: ({rx}, {ry}), 순서: {seq}");
                Console.Write("> ");
            }
            catch (ObjectDisposedException)
            {
                break;
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"\nUDP 오류: {ex.Message}");
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n수신 처리 오류: {ex.Message}");
            }
        }
    }

    static void PrintPeers()
    {
        if (peerPositions.IsEmpty)
        {
            Console.WriteLine("상대 위치: 미수신");
            return;
        }

        foreach (var entry in peerPositions)
            Console.WriteLine($"상대 {entry.Key} 위치: {entry.Value}");
    }
}
