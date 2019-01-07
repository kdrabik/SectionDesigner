/*
 * Created by SharpDevelop.
 * User: TS040198
 * Date: 12/11/2018
 * Time: 14:59
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SectionDesigner;

namespace Majstersztyk.TS_materials
{
	/// <summary>
	/// Description of TS_material.
	/// </summary>
	public abstract class TS_material : INotifyParametersChanged
    {
        private double _E;

        public double E {
            get { return _E; }
            set {
                _E = value;
                OnPropertyChanged();
				OnParametersChanged();
            } }

		public string Name;
		
		public override string ToString()
		{
			return Name + " E = " + String.Format("{0:e3}", E);
		}

        #region InotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "") {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
        
        #region InotifyParametersChanged Members
        public event EventHandler ParametersChanged;

        private void OnParametersChanged() {
            EventHandler handler = ParametersChanged;

            if (handler != null) {
                handler(this, new EventArgs());
            }
        }
        #endregion
        
        public void OnContainedElementChanged(object sender, EventArgs e){
        	
        }
    }


}
