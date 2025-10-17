namespace BlazorWasmHosted.Components
{
    public interface IMessageBoxService
    {
        event Func<MessageBoxModel, Task<MessageBoxResult>> OnShow;
        event Func<string, Task<string>> OnPrompt;
        Task<MessageBoxResult> ShowAsync(MessageBoxModel model);
        Task<MessageBoxResult> ShowAsync(string message, string title = "通知", MessageBoxType type = MessageBoxType.Info);
        Task AlertAsync(string message, string title = "通知");
        Task<bool> ConfirmAsync(string message, string title = "確認");
        Task<string> PromptAsync(string message, string title = "入力", string defaultValue = "");
        Task SuccessAsync(string message, string title = "成功");
        Task ErrorAsync(string message, string title = "エラー");
        Task WarningAsync(string message, string title = "警告");
    }
}
