# WiC64ChatInformation
Small systray tool showing information about how many people are online in the [WiC64](https://wic64.net/web/) chat

# Main Features
- As soon as someone joins the chat, a toast notification appears indicating that someone has logged in and showing how many people are currently in the chat.
- If someone logs out, a notification appears with the information that someone has logged out (and how many people remain in the chat).
- If no one is in the chat, this is also displayed in the toast.
- When you hover the mouse over the icon, a tooltip appears with information on how many people are currently in the chat.
- If you currently have no internet connection or the API endpoint for the WiC64 chat is unavailable, a toast appears for that as well — and of course, another one when the connection is restored.
- Currently, the endpoint is queried every 10 seconds.
- I recorded the “Pling!” sound that the chat plays when new messages arrive and integrated it into my widget.
- Each toast is accompanied by this pling sound.
- Right-clicking the icon opens a small context menu.
- Through this menu, you can exit the application and there’s also a checkbox to enable or disable the pling sound.

<img width="392" height="220" alt="image" src="https://github.com/user-attachments/assets/e9b2bfab-de50-4bd3-9cf4-959e25e2ed78" />

