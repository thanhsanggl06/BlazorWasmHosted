namespace BlazorWasmHosted.Components
{
    public class MessageBoxModel
    {
        public string Title { get; set; } = "通知"; // Thông báo
        public string Message { get; set; } = "";
        public MessageBoxType Type { get; set; } = MessageBoxType.Info;
        public MessageBoxButtons Buttons { get; set; } = MessageBoxButtons.OK;
        public string OkButtonText { get; set; } = "OK";
        public string CancelButtonText { get; set; } = "キャンセル"; // Hủy
        public string YesButtonText { get; set; } = "はい"; // Có
        public string NoButtonText { get; set; } = "いいえ"; // Không
        public string DefaultValue { get; set; } = "";
    }

    public enum MessageBoxType
    {
        Info,
        Success,
        Warning,
        Error,
        Question,
        Prompt
    }

    public enum MessageBoxButtons
    {
        OK,
        OKCancel,
        YesNo,
        YesNoCancel
    }

    public enum MessageBoxResult
    {
        None,
        OK,
        Cancel,
        Yes,
        No
    }
}
