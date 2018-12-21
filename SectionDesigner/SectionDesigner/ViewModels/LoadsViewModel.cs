namespace SectionDesigner.ViewModels
{
	using System;
	using System.Windows.Input;
	using SectionDesigner.Models;
	using SectionDesigner.Commands;
	using System.ComponentModel;

	internal class LoadsViewModel
	{
		public LoadsViewModel() {
			_Section = new Section(3);
			UpdateCommand = new LoadsUpdateCommand(this);
		}
		
		public bool CanUpdate {
			get {
				if (Section == null) {
					return false;
				}
				return !String.IsNullOrEmpty(Convert.ToString(Section.Size));
			}
			//set;
		}
		
		private Section _Section;
		public Section Section {
			get {
				return _Section;
			}
		}
		
		public ICommand UpdateCommand {
			get;
			private set;
		}
		
		public void SaveChanges() {

		}
	}
}
