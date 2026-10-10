using System.Windows;

namespace Presentation.Services
{
    public class UserDialogService : IUserDialogService
    {
        public bool ConfirmDiscardChanges() => MessageBox.Show(
            "Czy odrzucić niezapisane zmiany?", "Niezapisane zmiany",
            MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;

        public void ShowError(string message) => MessageBox.Show(
            message, "Nie można wykonać operacji", MessageBoxButton.OK, MessageBoxImage.Warning);
    }
}
