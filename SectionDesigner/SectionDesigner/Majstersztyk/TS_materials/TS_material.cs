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

namespace Majstersztyk.TS_materials
{
	/// <summary>
	/// Description of TS_material.
	/// </summary>
	public abstract class TS_material:INotifyPropertyChanged
    {
        private double _E;

        public double E {
            get { return _E; }
            set {
                _E = value;
                OnPropertyChanged();
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

    }


}
