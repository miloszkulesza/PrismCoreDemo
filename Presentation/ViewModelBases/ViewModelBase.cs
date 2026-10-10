using Presentation.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Navigation.Regions;

namespace Presentation.ViewModelBases
{
    public class ViewModelBase : BindableBase, INavigationAware, IWorkspaceScreen
    {
        protected readonly IWorkspaceService workspace;
        public DelegateCommand CloseViewCommand { get; }

        public ViewModelBase(IWorkspaceService workspace)
        {
            this.workspace = workspace;
            CloseViewCommand = new DelegateCommand(OnCloseView);
        }

        protected virtual void OnCloseView() => workspace.Close(this);
        public virtual bool CanNavigateTo(NavigationParameters parameters) => true;
        public virtual void OnClosed() { }
        public virtual void OnNavigatedTo(NavigationContext navigationContext) { }
        public virtual bool IsNavigationTarget(NavigationContext navigationContext) => true;
        public virtual void OnNavigatedFrom(NavigationContext navigationContext) { }
    }
}