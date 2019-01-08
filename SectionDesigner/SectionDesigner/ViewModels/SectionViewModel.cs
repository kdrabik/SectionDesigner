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
    using System.Runtime.CompilerServices;

    internal class SectionViewModel : INotifyPropertyChanged
	{
        public OxyPlotViewModel OxyPreview { get; set; }

        public SectionViewModel() {
			Section = Creator.ReadSection();
			//Section = Creator.CreateInitiateSection();
			//_Selected = SectionProp.SelectedContent as TS_part;
			UpdateCommand = new SectionUpdateCommand(this);
            OxyPreview = new OxyPlotViewModel();
            //OxyPreview.AddPart(_Section.Parts[0]);
            OxyPreview.Section = Section;
			OxyPreview.SelectedContour = Section.Parts[0].Voids[1];
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
            set {
                _Section = value;
                OnPropertyChanged();
            }
			get {
                return _Section;
			}
		}
        /*  NAJPRAWDOPODOBNIEJ NIEPOTRZEBNE 
		private TS_part _Selected_Part;
		public TS_part Selected_Part {
			get {
				return _Selected_Part;
			}
			set
            {
                _Selected_Part = value;
            }
		}
        
        private TS_contour _Selected_Contour;
        public TS_contour Selected_Contour {
            get {
                return _Selected_Contour;
            }
            set {
                _Selected_Contour = value;
            }
        }*/

        public ICommand UpdateCommand {
			get;
			private set;
		}
		
		public void SaveChanges() {

		}
		
		#region InotifyPropertyChanged Members
	
		public event PropertyChangedEventHandler PropertyChanged;
	
		private void OnPropertyChanged([CallerMemberName] string propertyName="") {
			PropertyChangedEventHandler handler = PropertyChanged;
			
			if (handler != null) {
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		#endregion
	}
}
