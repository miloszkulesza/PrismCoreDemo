namespace Presentation.Services
{
    public interface IUserDialogService
    {
        bool ConfirmDiscardChanges();
        void ShowError(string message);
    }
}
