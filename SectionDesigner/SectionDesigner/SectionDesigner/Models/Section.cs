namespace SectionDesigner.Models
{
using System;
using System.ComponentModel;



	public class Section : INotifyPropertyChanged
	{
		public Section(int secSize)
		{
			Size = secSize;
			SizeComputed = 2 * Size;
		}
		
		private int _Size;
		public int Size {
			get {
				return _Size;
			}
			set {
				_Size = value;
				OnPropertyChanged("Size");
				this.SizeComputed = 2 * value;
			}
		}
		
		private int _SizeComputed;
		public int SizeComputed {
			get {
				return _SizeComputed;
			}
			set {
				_SizeComputed = value;
				OnPropertyChanged("SizeComputed");
				//this.Size = Convert.ToInt32(0.5 * value);
			}
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