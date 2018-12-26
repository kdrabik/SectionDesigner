namespace SectionDesigner.ViewModels
{
	using System;
	using System.Windows.Input;
	using SectionDesigner.Models;
	using SectionDesigner.Commands;
	using Majstersztyk;
	using System.ComponentModel;
	using System.Collections.ObjectModel;
	using System.Windows.Controls;

	internal class SectionViewModel : INotifyPropertyChanged
	{
        public OxyPlotViewModel OxyPreview { get; set; }

        public SectionViewModel() {
			_Section = Creator.ReadSection();
			//_Selected = SectionProp.SelectedContent as TS_part;
			UpdateCommand = new SectionUpdateCommand(this);
            OxyPreview = new OxyPlotViewModel();
            //OxyPreview.AddPart(_Section.Parts[0]);
            OxyPreview.DrawSection(_Section);
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
		
		private TS_part _Selected;
		public TS_part Selected {
			get {
				return _Selected;
			}
			set
            {
                _Selected = value;
            }
		}
		
		
		public ICommand UpdateCommand {
			get;
			private set;
		}
		
		public void SaveChanges() {

		}
		
		#region InotifyPropertyChanged Members
	
		public event PropertyChangedEventHandler PropertyChanged;
	
		private void OnPropertyChanged(string propertyName) {
			PropertyChangedEventHandler handler = PropertyChanged;
			
			if (handler != null) {
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		#endregion
	}
}
