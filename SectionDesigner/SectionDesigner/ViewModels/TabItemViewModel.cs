namespace SectionDesigner.ViewModels
{
	using System;
	using System.Windows.Input;
	using SectionDesigner.Models;
	using SectionDesigner.Commands;
	using Majstersztyk;
	using System.ComponentModel;

public class TabItemViewModel
    {
        public string Name { get; set; }
        private bool isSelected;
        
        public TabItemViewModel(TS_part part) {
		}

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                DoSomethingWhenSelected();
            }
        }
        private void DoSomethingWhenSelected()
        {
            if(isSelected)
               Console.WriteLine("You selected " + Name);
        }
    }
}