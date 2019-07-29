
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace _02_LeonceEcrit
{

    public sealed class MyTextBox : TextBox
    {
        public MyTextBox()
        {
            this.DefaultStyleKey = typeof(TextBox);
        }

        public MyCaseType MyCharacterCasing
        {
            get { return (MyCaseType)GetValue(myCharacterCasingProperty); }
            set { SetValue(myCharacterCasingProperty, value); }
        }

        public  static readonly DependencyProperty myCharacterCasingProperty = DependencyProperty.Register("MyCharacterCasing", typeof(MyCaseType), typeof(MyTextBox), new PropertyMetadata(MyCaseType.Normal, (s, e) =>
        {
            TextBox myTextBox = (TextBox)s;
            if ((MyCaseType)e.NewValue != (MyCaseType)e.OldValue)
            {
                myTextBox.TextChanged += MyTextBox_TextChanged;
            }
            else
            {
                myTextBox.TextChanged -= MyTextBox_TextChanged;
            }
        }));

        private static void MyTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            MyTextBox myTextBox = sender as MyTextBox;
            switch (myTextBox.MyCharacterCasing)
            {
                case MyCaseType.UpperCase:
                    myTextBox.Text = myTextBox.Text.ToUpper();
                    break;
                case MyCaseType.LowerCase:
                    myTextBox.Text = myTextBox.Text.ToLower();
                    break;
                default:
                    break;
            }
            myTextBox.SelectionStart = myTextBox.Text.Length;
        }

    }
}
