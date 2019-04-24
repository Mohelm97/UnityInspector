using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace UnityInspector.GUI.ViewModels
{
    public class NumericTextBox : TextBox
    {
        bool isControlling;
        Point lastMousePosition;
        public double doubleValue;
        BindingExpression be;
        public NumericTextBox () : base ()
        {
            Cursor = Cursors.SizeWE;
            
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            be = GetBindingExpression(TextProperty);
        }

        protected override void OnMouseMove (MouseEventArgs e)
        {
            if (isControlling)
            {
                Point currentMousePosition = e.GetPosition (this);
                float deltaMove = (float) (currentMousePosition.X - lastMousePosition.X) / 10;
                doubleValue += deltaMove;
                Text = doubleValue.ToString ();
                be.UpdateSource();
                lastMousePosition = currentMousePosition;
            }
        }
        protected override void OnMouseUp (MouseButtonEventArgs e)
        {
            base.OnMouseUp (e);
            isControlling = false;
        }
        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            base.OnTextInput(e);
            if (Text[Text.Length - 1] != '.')
                be.UpdateSource();
        }

        protected override void OnMouseDown (MouseButtonEventArgs e)
        {
            base.OnMouseDown (e);
            doubleValue = 0;
            double.TryParse (Text, out doubleValue);
            isControlling = true;
            lastMousePosition = e.GetPosition (this);
        }

    }
}
