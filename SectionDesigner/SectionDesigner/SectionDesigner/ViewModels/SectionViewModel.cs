namespace SectionDesigner.ViewModels
{
	using System;
	using System.Windows.Input;
	using SectionDesigner.Models;
	using SectionDesigner.Commands;
	using Majstersztyk;
	using System.ComponentModel;

	internal class SectionViewModel
	{
		public SectionViewModel() {
			_Section = Creator.ReadSection();
			UpdateCommand = new SectionUpdateCommand(this);
		}
		
		public bool CanUpdate {
			get {
				if (Section == null) {
					return false;
				}
				return true;
			}
			//set;
		}
		
		private TS_section _Section;
		public TS_section Section {
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
