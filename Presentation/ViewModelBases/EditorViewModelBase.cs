using Presentation.Services;
using Prism.Commands;
using Prism.Navigation;
using Prism.Navigation.Regions;
using System;
using System.ComponentModel;

namespace Presentation.ViewModelBases
{
    public abstract class EditorViewModelBase<T> : ViewModelBase where T : class, INotifyPropertyChanged
    {
        private readonly IUserDialogService dialogs;
        private T model;
        private bool loaded;
        private int recordId;
        public bool HasChanges { get; private set; }
        protected abstract string ParameterName { get; }
        protected T Model => model;
        public bool SaveButtonState => CanSave();
        public DelegateCommand SaveAndCloseCommand { get; }
        public DelegateCommand CancelAndCloseViewCommand { get; }

        protected EditorViewModelBase(IWorkspaceService workspace, IUserDialogService dialogs) : base(workspace)
        {
            this.dialogs = dialogs;
            SaveAndCloseCommand = new DelegateCommand(SaveAndClose, CanSave);
            CancelAndCloseViewCommand = new DelegateCommand(OnCloseView);
        }

        public override bool CanNavigateTo(NavigationParameters parameters) =>
            !loaded || parameters.GetValue<int>(ParameterName) == recordId ||
            !HasChanges || dialogs.ConfirmDiscardChanges();

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            var id = navigationContext.Parameters.GetValue<int>(ParameterName);
            if (loaded && id == recordId) return;
            var nextModel = Load(id);
            if (model != null) model.PropertyChanged -= ModelChanged;
            model = nextModel;
            recordId = id;
            loaded = true;
            OnModelLoaded();
            HasChanges = false;
            if (model != null) model.PropertyChanged += ModelChanged;
            NotifyValidation();
        }

        private void ModelChanged(object sender, PropertyChangedEventArgs e)
        {
            HasChanges = true;
            NotifyValidation();
        }

        protected void NotifyValidation()
        {
            RaisePropertyChanged(nameof(SaveButtonState));
            SaveAndCloseCommand.RaiseCanExecuteChanged();
        }

        private void SaveAndClose()
        {
            if (!CanSave()) return;
            try
            {
                Save();
                HasChanges = false;
                workspace.Close(this);
            }
            catch (Exception e) when (e is ArgumentException || e is InvalidOperationException)
            {
                dialogs.ShowError(e.Message);
            }
        }

        protected override void OnCloseView()
        {
            if (!HasChanges || dialogs.ConfirmDiscardChanges()) workspace.Close(this);
        }

        public override void OnClosed()
        {
            if (model != null) model.PropertyChanged -= ModelChanged;
            model = null;
            loaded = false;
            HasChanges = false;
            OnModelLoaded();
            NotifyValidation();
        }

        protected abstract T Load(int id);
        protected abstract void OnModelLoaded();
        protected abstract bool CanSave();
        protected abstract void Save();
    }
}
