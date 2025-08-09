using Timer = System.Threading.Timer;

namespace WiC64ChatInformation;

public static class Program
{
    private static readonly string apiUrl = "https://x.wic64.net/chat/api/usercount/";
    private static readonly int pollInterval = 10;

    private static bool isShuttingDown = false;
    private static bool isSoundEnabledGlobal = false;

    private static readonly string usersIconPath;
    private static NotifyIcon notifyIcon;
    private static Timer monitorTimer;
    private static int lastUserCount = -1;
    private static readonly CancellationTokenSource cts = new();

    private static bool? isApiConnected = null;

    static Program()
    {
        usersIconPath = ResourceHelper.ExtractResourceToTempFile("UsersIcon.ico", "UsersIconTemp.ico");
    }

    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        isSoundEnabledGlobal = Properties.Settings.Default.EnableSound;
                
        ToastManager.Initialize(usersIconPath);
        ToastManager.RegisterAppForToasts();

        notifyIcon = new NotifyIcon
        {
            Icon = ResourceHelper.GetEmbeddedIcon("WiC64Icon.ico"),
            Visible = true,
            Text = "WiC64 Chat Information"
        };

        var contextMenu = new ContextMenuStrip();

        var enableSoundMenuItem = new ToolStripMenuItem("Enable Sound")
        {
            CheckOnClick = true,
            Checked = isSoundEnabledGlobal
        };

        enableSoundMenuItem.CheckedChanged += (sender, e) =>
        {
            isSoundEnabledGlobal = enableSoundMenuItem.Checked;
            Properties.Settings.Default.EnableSound = isSoundEnabledGlobal;
            Properties.Settings.Default.Save();
        };

        contextMenu.Items.Add(enableSoundMenuItem);

        var quitItem = new ToolStripMenuItem(
            "Quit",
            ResourceHelper.GetEmbeddedImage("PowerIcon.ico"),
            (sender, e) => ShutdownApplication());

        contextMenu.Items.Add(quitItem);
        notifyIcon.ContextMenuStrip = contextMenu;

        monitorTimer = new Timer(async _ => await CheckApiAsync(apiUrl, cts.Token),
                                    null,
                                    TimeSpan.Zero,
                                    TimeSpan.FromSeconds(pollInterval));

        Application.Run();
    }



    private static async Task CheckApiAsync(string url, CancellationToken token)
    {
        try
        {
            if (token.IsCancellationRequested) return;

            using var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(url, token);

            if (int.TryParse(response, out int currentUserCount))
            {
                bool reconnected = false;

                if (isApiConnected != true)
                {
                    if (lastUserCount > -1) ToastManager.ShowConnectivityRestoredToast();
                    isApiConnected = true;
                    reconnected = true;
                }

                if (lastUserCount != currentUserCount || reconnected)
                {
                    ToastManager.ShowUserCountToast(lastUserCount, currentUserCount);
                    lastUserCount = currentUserCount;

                    notifyIcon.Text = BuildBaseText(currentUserCount);
                }
            }
            else
            {
                HandleApiError();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            HandleApiError();
        }
    }

    private static string BuildBaseText(int currentUserCount)
    {
        if (currentUserCount == 0)
            return "There is currently no one online.";

        string verb = currentUserCount == 1 ? "is" : "are";
        string plural = currentUserCount == 1 ? "" : "s";

        return $"There {verb} currently {currentUserCount} user{plural} online.";
    }
    private static void HandleApiError()
    {
        if (isApiConnected == true)
        {
            ToastManager.ShowConnectionLostToast();
            isApiConnected = false;

            notifyIcon.Text = "Connection to server is lost.";
        }
    }
    private static void ShutdownApplication()
    {
        if (isShuttingDown) return;
        isShuttingDown = true;

        monitorTimer?.Change(Timeout.Infinite, Timeout.Infinite);
        monitorTimer?.Dispose();

        notifyIcon.Visible = false;

        CleanupTempFiles();

        Application.Exit();
    }
    private static void CleanupTempFiles()
    {
        DeleteFileIfExists(usersIconPath);
    }
    private static void DeleteFileIfExists(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not delete file {path}: {ex.Message}");
        }
    }
}