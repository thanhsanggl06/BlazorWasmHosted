namespace BlazorWasmHosted.Components
{
    public class MessageBoxService : IMessageBoxService
    {
        public event Func<MessageBoxModel, Task<MessageBoxResult>> OnShow;
        public event Func<string, Task<string>> OnPrompt;

        public async Task<MessageBoxResult> ShowAsync(MessageBoxModel model)
        {
            if (OnShow != null)
            {
                return await OnShow.Invoke(model);
            }
            return MessageBoxResult.None;
        }

        public Task<MessageBoxResult> ShowAsync(string message, string title = "通知", MessageBoxType type = MessageBoxType.Info)
        {
            var model = new MessageBoxModel
            {
                Title = title,
                Message = message,
                Type = type,
                Buttons = type == MessageBoxType.Question ? MessageBoxButtons.YesNo : MessageBoxButtons.OK
            };
            return ShowAsync(model);
        }

        public async Task AlertAsync(string message, string title = "通知")
        {
            await ShowAsync(new MessageBoxModel
            {
                Title = title,
                Message = message,
                Type = MessageBoxType.Info,
                Buttons = MessageBoxButtons.OK
            });
        }

        public async Task<bool> ConfirmAsync(string message, string title = "確認")
        {
            var result = await ShowAsync(new MessageBoxModel
            {
                Title = title,
                Message = message,
                Type = MessageBoxType.Question,
                Buttons = MessageBoxButtons.YesNo,
                YesButtonText = "はい",
                NoButtonText = "いいえ"
            });
            return result == MessageBoxResult.Yes || result == MessageBoxResult.OK;
        }

        public async Task<string> PromptAsync(string message, string title = "入力", string defaultValue = "")
        {
            if (OnPrompt != null)
            {
                var model = new MessageBoxModel
                {
                    Title = title,
                    Message = message,
                    Type = MessageBoxType.Prompt,
                    Buttons = MessageBoxButtons.OKCancel,
                    DefaultValue = defaultValue,
                    OkButtonText = "OK",
                    CancelButtonText = "キャンセル"
                };

                return await OnPrompt.Invoke(System.Text.Json.JsonSerializer.Serialize(model));
            }
            return null;
        }

        public Task SuccessAsync(string message, string title = "成功")
        {
            return ShowAsync(new MessageBoxModel
            {
                Title = title,
                Message = message,
                Type = MessageBoxType.Success,
                Buttons = MessageBoxButtons.OK,
                OkButtonText = "閉じる" // Đóng
            });
        }

        public Task ErrorAsync(string message, string title = "エラー")
        {
            return ShowAsync(new MessageBoxModel
            {
                Title = title,
                Message = message,
                Type = MessageBoxType.Error,
                Buttons = MessageBoxButtons.OK,
                OkButtonText = "閉じる"
            });
        }

        public Task WarningAsync(string message, string title = "警告")
        {
            return ShowAsync(new MessageBoxModel
            {
                Title = title,
                Message = message,
                Type = MessageBoxType.Warning,
                Buttons = MessageBoxButtons.OK,
                OkButtonText = "閉じる"
            });
        }
    }
}
