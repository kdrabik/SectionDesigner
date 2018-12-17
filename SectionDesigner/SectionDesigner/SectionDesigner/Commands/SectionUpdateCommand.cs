namespace SectionDesigner.Commands
{
	using System;
	using System.Windows.Input;
	using SectionDesigner.ViewModels;

	internal class SectionUpdateCommand : ICommand
	{
		public SectionUpdateCommand(SectionViewModel viewModel)
		{
			_ViewModel = viewModel;
		}
		
		private SectionViewModel _ViewModel;
//		
		#region ICommand Members
		
		public event System.EventHandler CanExecuteChanged {
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value;}
		}
		
		public bool CanExecute(object parameter) {
			return _ViewModel.CanUpdate;
		}
		
		public void Execute(object parameter) {
			_ViewModel.SaveChanges();
		}
		#endregion
	}
}
