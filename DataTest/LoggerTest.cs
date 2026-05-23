using Data;

namespace DataTest;

[TestClass]
public sealed class LoggerTests
{
    private record class TestLog(string Details);

    [TestMethod]
    public async Task LoggerTest()
    {
        const string testFilePath = "testLog.txt";
        // write dummy data to file
        await File.WriteAllTextAsync(testFilePath, "tEs123t");

        var loggerCancellationSource = new CancellationTokenSource();
        var logger = new Logger(testFilePath);
        _ = Task.Run(() => logger.LoggingThread(loggerCancellationSource.Token), loggerCancellationSource.Token);


        logger.Log(new TestLog("very important"));

        await Task.Delay(200);
        await loggerCancellationSource.CancelAsync();

        string expectedText = "[TestLog] {\"Details\":\"very important\"}\n";

        // logger should have cleared the file and wrote the singular log
        Assert.DoesNotContain("tEs123t", File.ReadAllText(testFilePath));
        Assert.EndsWith(expectedText, File.ReadAllText(testFilePath));

        // logger Task have been cancelled by now, the new log shouldn't end up in the file
        logger.Log(new TestLog("not as important"));
        await Task.Delay(200);
        Assert.EndsWith(expectedText, File.ReadAllText(testFilePath));
    }
}
