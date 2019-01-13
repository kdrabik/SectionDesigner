using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SectionDesigner
{
    public interface INotifyParametersChanged : INotifyPropertyChanged
    {
        event EventHandler ParametersChanged;
        void ContainedElementParametersChanged(object sender, EventArgs e);
    }

    public class ObservableCollectionEx<T> : ObservableCollection<T> where T : INotifyParametersChanged
    {
        public ObservableCollectionEx() : base() { }

        public ObservableCollectionEx(IEnumerable<T> collection) : base() {
            this.AddRange(collection);
        }

        public new void Add(T item) {
            base.Add(item);
            item.ParametersChanged += ContainedElementParametersChanged;
            item.PropertyChanged += ContainedElementPropertyChanged;
            /*NotifyCollectionChangedEventArgs e =
                 new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item);

            OnCollectionChanged(e);*/
        }
        
        public void AddRange(IEnumerable<T> collection) {
            //base.AddRange(collection);
            foreach (var item in collection) {
                item.ParametersChanged += ContainedElementParametersChanged;
                item.PropertyChanged += ContainedElementPropertyChanged;
                this.Add(item);
            }
            NotifyCollectionChangedEventArgs e =
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<T>(collection));

            OnCollectionChanged(e);
        }

        public new void Clear() {
            foreach (var item in this) {
                item.ParametersChanged -= ContainedElementParametersChanged;
                item.PropertyChanged -= ContainedElementPropertyChanged;
            }
            base.Clear();
            /*NotifyCollectionChangedEventArgs e =
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);

            OnCollectionChanged(e); */
        }

        public new void Insert(int i, T item) {
            base.Insert(i, item);
            item.ParametersChanged += ContainedElementParametersChanged;
            item.PropertyChanged += ContainedElementPropertyChanged;
            /*NotifyCollectionChangedEventArgs e =
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item);
            OnCollectionChanged(e); */
        }
        
        public void InsertRange(int i, IEnumerable<T> collection) {
            //base.InsertRange(i, collection);
            List<T> tempList = new List<T>();

            for (int j = 0; j < i; j++) {
                tempList.Add(this[j]);
            }

            foreach (var item in collection) {
                item.ParametersChanged += ContainedElementParametersChanged;
                item.PropertyChanged += ContainedElementPropertyChanged;
                tempList.Add(item);
            }

            for (int j = i; j < Count; j++) {
                tempList.Add(this[j]);
            }

            this.Clear();
            this.AddRange(tempList);

            NotifyCollectionChangedEventArgs e =
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, collection);
            OnCollectionChanged(e);
        }

        public new void Remove(T item) {
            base.Remove(item);
            item.ParametersChanged -= ContainedElementParametersChanged;
            item.PropertyChanged -= ContainedElementPropertyChanged;
            /*NotifyCollectionChangedEventArgs e =
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item);
            OnCollectionChanged(e);*/
        }
        /*
        public new void RemoveAll(Predicate<T> match) {
            List<T> backup = FindAll(match);
            foreach (var item in backup) {
                item.ParametersChanged -= OnContainedElementParametersChanged;
                item.PropertyChanged -= ContainedElementChanged;
            }
            base.RemoveAll(match);
            NotifyCollectionChangedEventArgs e =
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, backup);
            OnCollectionChanged(e);
        }*/

        public new void RemoveAt(int i) {
            T backup = this[i];
            backup.ParametersChanged -= ContainedElementParametersChanged;
            backup.PropertyChanged -= ContainedElementPropertyChanged;
            base.RemoveAt(i);
            /*NotifyCollectionChangedEventArgs e =
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, backup);
            OnCollectionChanged(e);*/
        }
        /*
        public new void RemoveRange(int index, int count) {
            List<T> backup = GetRange(index, count);
            foreach (var item in backup) {
                item.ParametersChanged -= OnContainedElementParametersChanged;
                item.PropertyChanged -= ContainedElementChanged;
            }
            base.RemoveRange(index, count);
            NotifyCollectionChangedEventArgs e =
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, backup);
            OnCollectionChanged(e);
        }*/

        public new T this[int index] {
            get { return base[index]; }
            set {
                T oldValue = base[index];
                oldValue.ParametersChanged += ContainedElementParametersChanged;
                oldValue.PropertyChanged += ContainedElementPropertyChanged;
                NotifyCollectionChangedEventArgs e =
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldValue);
                OnCollectionChanged(e);
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
            Unsubscribe(e.OldItems);
            Subscribe(e.NewItems);
            base.OnCollectionChanged(e);
        }

        #region Subscriptions
        private void Subscribe(IList iList) {
            if (iList != null) {
                foreach (T element in iList) {
                    element.PropertyChanged += ContainedElementPropertyChanged;
                    element.ParametersChanged += ContainedElementParametersChanged;
                }
            }
        }

        private void Unsubscribe(IList iList) {
            if (iList != null) {
                foreach (T element in iList) {
                    element.PropertyChanged -= ContainedElementPropertyChanged;
                    element.ParametersChanged -= ContainedElementParametersChanged;
                }
            }
        }
        #endregion

        public void Reverse() {
            List<T> tempList = new List<T>();
            tempList.AddRange(this);
            tempList.Reverse();
            this.Clear();
            foreach (var item in tempList) {
                this.Add(item);
            }
        }
        
        #region INotifyPropertyChanged Members
        public new event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "") {

            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void ContainedElementPropertyChanged(object sender, PropertyChangedEventArgs e) {
            OnPropertyChanged(e.PropertyName);
        }
        #endregion
        
        #region INotifyParametersChanged Members
        public event EventHandler ParametersChanged;

        protected void OnParametersChanged() {

            if (ParametersChanged != null) {
                ParametersChanged(this, new EventArgs());
            }
        }

        private void ContainedElementParametersChanged(object sender, EventArgs e) {
            OnParametersChanged();
        }
        #endregion
    }

}
