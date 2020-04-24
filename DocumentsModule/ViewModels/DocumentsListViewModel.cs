using DocumentsModule.Views;
using Infrastructure.ViewModelBases;
using Prism.Mvvm;
using Prism.Regions;

namespace DocumentsModule.ViewModels
{
    public class DocumentsListViewModel : ViewModelBase
    {
        public DocumentsListViewModel(IRegionManager regionManager): base(regionManager, typeof(DocumentsList), typeof(DocumentsListRibbonTab))
        {

        }

        public string Message { get; set; } = "Moduł dokumenty, widok lista dokumentów";
    }
}
