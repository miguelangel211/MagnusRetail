using System;
using System.Collections;
using System.Windows.Input;
using Xamarin.Forms;

namespace CheckstoresMagnusRetail.ViewModels
{
    public class InfiniteListView:ListView
    {
        public static readonly BindableProperty LoadMoreCommandProperty = BindableProperty.Create(nameof(LoadMoreCommandProperty), typeof(ICommand), typeof(InfiniteListView), default(ICommand));
        public ICommand LoadMoreCommand {
            get { return (ICommand)GetValue(LoadMoreCommandProperty); }
            set { SetValue(LoadMoreCommandProperty, value); }
        }


        public InfiniteListView(ListViewCachingStrategy cachingStrategy) : 
                base(cachingStrategy) {
            ItemAppearing += InfiniteListView_ItemAppearing;
        }

        void InfiniteListView_ItemAppearing(object sender,ItemVisibilityEventArgs e) {
            var items = ItemsSource as IList;
            if (LoadMoreCommand != null && LoadMoreCommand.CanExecute(null))
                LoadMoreCommand.Execute(null);


        }
    }
}
