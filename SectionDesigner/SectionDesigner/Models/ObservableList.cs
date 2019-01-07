/*
 * Created by SharpDevelop.
 * User: TS040198
 * Date: 04/01/2019
 * Time: 13:07
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SectionDesigner
{
    /// <summary>
    /// Description of ObservableList.
    /// </summary>
    /// <summary>
    /// Description of IObservableList.
    /// </summary>
    public interface IObservableList<T> : IList<T>, INotifyCollectionChanged {

    }
    
    public interface INotifyParametersChanged : INotifyPropertyChanged{
    	event EventHandler ParametersChanged;
		void OnContainedElementChanged(object sender, EventArgs e);
    }

    public class ObservableList<T> : List<T>, IObservableList<T>, INotifyPropertyChanged where T : INotifyParametersChanged
    {

        #region Constructors

        public ObservableList() {
            IsNotifying = true;

            // As a gimmick, I wanted to bind to the Count property, so I
            // use the OnPropertyChanged event from the INotifyPropertyChanged
            // interface to notify about Count changes.
            CollectionChanged += new NotifyCollectionChangedEventHandler(
                delegate (object sender, NotifyCollectionChangedEventArgs e) {
                    OnPropertyChanged("Count");
                }
            );
        }

        public ObservableList(IEnumerable<T> collection) : this() {
            this.AddRange(collection);
        }

        #endregion

        #region Properties

        public bool IsNotifying { get; set; }

        #endregion

        #region Adding and removing items

        public new void Add(T item) {
            base.Add(item);
            item.ParametersChanged += OnContainedElementChanged;
            item.PropertyChanged += ContainedElementChanged;
            /*NotifyCollectionChangedEventArgs e =
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item);

            OnCollectionChanged(e);*/
        }

        public new void AddRange(IEnumerable<T> collection) {
            base.AddRange(collection);
			foreach (var item in collection) {
				item.ParametersChanged += OnContainedElementChanged;
				item.PropertyChanged += ContainedElementChanged;
			}
            /*NotifyCollectionChangedEventArgs e =
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<T>(collection));

            OnCollectionChanged(e);*/
        }

        public new void Clear() {
			foreach (var item in this) {
				item.ParametersChanged -= OnContainedElementChanged;
            	item.PropertyChanged -= ContainedElementChanged;
			}
            base.Clear();
            /*NotifyCollectionChangedEventArgs e =
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);

            OnCollectionChanged(e);*/
        }

        public new void Insert(int i, T item) {
            base.Insert(i, item);
            item.ParametersChanged += OnContainedElementChanged;
            item.PropertyChanged += ContainedElementChanged;
            /*NotifyCollectionChangedEventArgs e =
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item);
            OnCollectionChanged(e);*/
        }

        public new void InsertRange(int i, IEnumerable<T> collection) {
            base.InsertRange(i, collection);
			foreach (var item in collection) {
				item.ParametersChanged += OnContainedElementChanged;
            item.PropertyChanged += ContainedElementChanged;
			}
            /*NotifyCollectionChangedEventArgs e =
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, collection);
            OnCollectionChanged(e);*/
        }

        public new void Remove(T item) {
            base.Remove(item);
            item.ParametersChanged -= OnContainedElementChanged;
            item.PropertyChanged -= ContainedElementChanged;
            /*NotifyCollectionChangedEventArgs e =
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item);
            OnCollectionChanged(e);*/
        }

        public new void RemoveAll(Predicate<T> match) {
            List<T> backup = FindAll(match);
			foreach (var item in backup) {
				item.ParametersChanged -= OnContainedElementChanged;
				item.PropertyChanged -= ContainedElementChanged;
			}
            base.RemoveAll(match);
            NotifyCollectionChangedEventArgs e =
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, backup);
            OnCollectionChanged(e);
        }

        public new void RemoveAt(int i) {
            T backup = this[i];
            backup.ParametersChanged -= OnContainedElementChanged;
            backup.PropertyChanged -= ContainedElementChanged;
            base.RemoveAt(i);
            /*NotifyCollectionChangedEventArgs e =
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, backup);
            OnCollectionChanged(e);*/
        }

        public new void RemoveRange(int index, int count) {
            List<T> backup = GetRange(index, count);
			foreach (var item in backup) {
				item.ParametersChanged -= OnContainedElementChanged;
				item.PropertyChanged -= ContainedElementChanged;
			}
            base.RemoveRange(index, count);
            /*NotifyCollectionChangedEventArgs e =
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, backup);
            OnCollectionChanged(e);*/
        }

        public new T this[int index] {
            get { return base[index]; }
            set {
                T oldValue = base[index];
                oldValue.ParametersChanged += OnContainedElementChanged;
				oldValue.PropertyChanged += ContainedElementChanged;
                /*NotifyCollectionChangedEventArgs e =
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldValue);
                OnCollectionChanged(e);*/
            }
        }

        #endregion

        #region INotifyCollectionChanged Members
        
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
            Unsubscribe(e.OldItems);
            Subscribe(e.NewItems);
            /*if (IsNotifying && CollectionChanged != null)
                try {
                    CollectionChanged(this, e);
                } catch (System.NotSupportedException) {
                    NotifyCollectionChangedEventArgs alternativeEventArgs =
                        new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                    OnCollectionChanged(alternativeEventArgs);
                }*/
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "") {

            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event EventHandler ParametersChanged;

        protected void OnParametersChanged() {

            if (ParametersChanged != null) {
                ParametersChanged(this, new EventArgs());
            }
        }
        
        private void ContainedElementChanged(object sender, PropertyChangedEventArgs e) {
            OnPropertyChanged(e.PropertyName);
        }
        
        private void OnContainedElementChanged(object sender, EventArgs e) {
            OnParametersChanged();
        }

        //public delegate void ItemPropertyChanged(object sender, PropertyChangedEventArgs e);

        #endregion

        #region Subsciptions

        private void Subscribe(IList iList) {
            if (iList != null) {
				foreach (T element in iList) {
					element.ParametersChanged += OnContainedElementChanged;
					element.PropertyChanged += ContainedElementChanged;
				}
            }
        }

        private void Unsubscribe(IList iList) {
            if (iList != null) {
				foreach (T element in iList) {
					element.ParametersChanged -= OnContainedElementChanged;
					element.PropertyChanged += ContainedElementChanged;
				}
            }
        }
        #endregion
    }
}
