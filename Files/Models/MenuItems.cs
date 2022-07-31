using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Files.Models
{
    public interface IMenuItem { }

    public class Separator : IMenuItem { }

    public class MenuItem : IMenuItem
    {
        public string Header { get; private set; }
        public ICommand Command { get; private set; }
        public object CommandParameter { get; set; }
        //public ImageSource ImageSource { get; private set; }
        public IList<IMenuItem> Children { get; private set; }

        public MenuItem(string header, ICommand command)
        {
            Header = header;
            Command = command;

            Children = new List<IMenuItem>();
        }
    }

    public class MenuItemContainerTemplateSelector : ItemContainerTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, ItemsControl parentItemsControl)
        {
            var key = new DataTemplateKey(item.GetType());
            return (DataTemplate)parentItemsControl.FindResource(key);
        }
    }
}
