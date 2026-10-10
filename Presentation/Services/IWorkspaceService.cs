using Prism.Navigation;

namespace Presentation.Services
{
    public interface IWorkspaceService
    {
        void Open(string viewName, string ribbonName, NavigationParameters parameters = null);
        void Activate(object screen);
        void Close(object screen);
    }

    public interface IWorkspaceScreen
    {
        bool CanNavigateTo(NavigationParameters parameters);
        void OnClosed();
    }
}
