using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using UnityInspector.GUI.Models;
using UnityInspector.GUI.ViewModels;

namespace UnityInspector.GUI.Converters
{
    public class ObjectMemberTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate (object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;
            ObjectMemberViewModel member = item as ObjectMemberViewModel;
            if (member.Type == CommunicatorClient.Types.Bool)
            {
                return element.FindResource ("BoolMemberDataTemplate") as DataTemplate;
            }
            else if (member.Type > CommunicatorClient.Types.Bool && member.Type < CommunicatorClient.Types.String)
            {
                return element.FindResource ("NumericMemberDataTemplate") as DataTemplate;
            }
            else if (member.Type == CommunicatorClient.Types.Array)
            {
                return element.FindResource ("ArrayMemberDataTemplate") as DataTemplate;
            }
            else if (member.Type == CommunicatorClient.Types.Vector3)
            {
                return element.FindResource ("Vector3MemberDataTemplate") as DataTemplate;
            }
            else if (member.Type == CommunicatorClient.Types.Enum)
            {
                return element.FindResource ("EnumMemberDataTemplate") as DataTemplate;
            }
            else if (member.Type == CommunicatorClient.Types.Color)
            {
                return element.FindResource ("ColorMemberDataTemplate") as DataTemplate;
            }
            else if (member.Type == CommunicatorClient.Types.Vector2)
            {
                return element.FindResource ("Vector2MemberDataTemplate") as DataTemplate;
            }
            else if (member.Type == CommunicatorClient.Types.Rect)
            {
                return element.FindResource ("RectMemberDataTemplate") as DataTemplate;
            }
            else if(member.Type == CommunicatorClient.Types.GameObject)
            {
                return element.FindResource("GameObjectMemberDataTemplate") as DataTemplate;
            }
            else if(member.Type != CommunicatorClient.Types.Unknown)
            {
                return element.FindResource ("TextBoxMemberDataTemplate") as DataTemplate;
            }
            else
            {
                return element.FindResource ("UnknownMemberDataTemplate") as DataTemplate;
            }
        }
    }
}
