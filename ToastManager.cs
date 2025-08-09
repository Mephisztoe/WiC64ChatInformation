using Microsoft.Toolkit.Uwp.Notifications;
using System;

namespace WiC64ChatInformation
{
    public static class ToastManager
    {
        private static string usersIconPath;

        public static void Initialize(string usersIconPath)
        {
            ToastManager.usersIconPath = usersIconPath;
        }

        public static void RegisterAppForToasts()
        {
            ToastNotificationManagerCompat.OnActivated += ToastNotificationManagerCompat_OnActivated;
        }

        private static void ToastNotificationManagerCompat_OnActivated(ToastNotificationActivatedEventArgsCompat args)
        {
            Console.WriteLine($"Toast clicked with arguments: {args.Argument}");
        }

        public static void ShowConnectionLostToast()
        {
            PlayNotificationSound("Pling.wav");

            new ToastContentBuilder()
                .AddText("Connection Lost")
                .AddText("Unable to retrieve user count from server.")
                .AddAudio(new ToastAudio() { Silent = true })
                .Show();
        }

        public static void ShowConnectivityRestoredToast()
        {
            PlayNotificationSound("Pling.wav");

            new ToastContentBuilder()
                .AddText("Connection Restored")
                .AddText("We can talk to the server again.")
                .AddAudio(new ToastAudio() { Silent = true })
                .Show();
        }

        public static void ShowUserCountToast(int lastUserCount, int currentUserCount)
        {
            PlayNotificationSound("Pling.wav");

            string finalText = BuildNotificationText(lastUserCount, currentUserCount);

            new ToastContentBuilder()
                .AddAppLogoOverride(new Uri($"file:///{usersIconPath.Replace("\\", "/")}"))
                .AddText("User Count")
                .AddText(finalText)
                .AddAudio(new ToastAudio() { Silent = true })
                .Show();
        }
        private static string BuildNotificationText(int lastUserCount, int currentUserCount)
        {
            if (lastUserCount == -1)
                return BuildBaseText(currentUserCount);

            string changeText = BuildChangeText(currentUserCount - lastUserCount);
            string baseText = BuildBaseText(currentUserCount);

            return string.IsNullOrWhiteSpace(changeText)
                ? baseText
                : $"{changeText} {baseText}";
        }

        private static string BuildBaseText(int currentUserCount)
        {
            if (currentUserCount == 0)
                return "There is currently no one online.";

            string verb = currentUserCount == 1 ? "is" : "are";
            string plural = currentUserCount == 1 ? "" : "s";

            return $"There {verb} currently {currentUserCount} user{plural} online.";
        }

        private static string BuildChangeText(int difference)
        {
            int absDiff = Math.Abs(difference);
            string verb = absDiff == 1 ? "has" : "s have";

            if (difference > 0)
            {
                return $"{absDiff} new user{(absDiff == 1 ? "" : "s")} {verb} joined the chat.";
            }
            else if (difference < 0)
            {
                return $"{absDiff} user{(absDiff == 1 ? "" : "s")} {verb} left the chat.";
            }

            return string.Empty;
        }
        private static void PlayNotificationSound(string soundResourceName)
        {
            if (Properties.Settings.Default.EnableSound)
            {
                var player = ResourceHelper.GetEmbeddedSoundPlayer(soundResourceName);
                player.Play();
            }
        }
    }
}