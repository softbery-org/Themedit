// Version: 1.0.0.153
// Copyright (c) 2024 Softbery by Paweł Tobis
using ThemeditLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Themedit.src;
using System.Windows.Controls.Primitives;
using MahApps.Metro.Controls;
using Themedit.Properties;

namespace Themedit
{
    /// <summary>
    /// Logika interakcji dla klasy SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : MetroWindow
    {
        public static Action OnLanguageChange;

        public SettingsWindow()
        {
            InitializeComponent();

            OnLanguageChange += LanguageChange;
            translate();
            /*var s = "";
            App.WriteSettings();
            foreach (var item in App.Config)
            {
                s += item.Key+$"={item.Value}" + "\n";
            }
            MessageBox.Show(s);*/
            getAppMultipleRun();
        }

        private void getAppMultipleRun()
        {
            if (App.Config["Settings:ApplicationMultipleRun"] == "allow")
            {
                _checkboxRunMultipleApplicationCheckBox.IsChecked = true;
            }
            else if(App.Config["Settings:ApplicationMultipleRun"] == "denny")
            {
                _checkboxRunMultipleApplicationCheckBox.IsChecked = false;
            }
        }

        private void setAppMultipleRun()
        {
            if (_checkboxRunMultipleApplicationCheckBox.IsChecked == true)
            {
                App.IniConfiguration.Write("Settings", "ApplicationMultipleRun", "allow");
                App.WriteSettings();
            }
            else if (_checkboxRunMultipleApplicationCheckBox.IsChecked == false)
            {
                App.IniConfiguration.Write("Settings", "ApplicationMultipleRun", "deny");
                App.WriteSettings();
            }
        }


        private void LanguageChange()
        {
            translate();
        }

        private void _comboboxLanguages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void comboboxLanguages() 
        {
            foreach (var item in Translation.Languages)
            {
                _comboboxLanguages.Text = item.Name;
                _comboboxLanguages.Items.Add(item);
            }

            foreach (ILanguage item in _comboboxLanguages.Items)
            {
                if (item.Code == App.Config["Application:Language"])
                {
                    _comboboxLanguages.SelectedItem = item;
                    break;
                }
            }
        }

        private void _btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_comboboxLanguages.SelectedItem!=null)
            {
                Translation.ChangeLanguage((_comboboxLanguages.SelectedItem as ILanguage).Code);
                App.IniConfiguration.Write("Application", "Language", (_comboboxLanguages.SelectedItem as ILanguage).Code);
                App.WriteSettings();
                MediaControls.OnLanguageChange();
                OnLanguageChange();
                PlaylistWindow.OnLanguageChange();
                setAppMultipleRun();
            }
        }

        private void _btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void translate()
        {
            var language = Translation.Current;
            this.Resources["groupboxLanguage.Header"] = language.Settings.groupboxLanguage;
            this.Resources["groupboxRunOptions.Header"] = language.Settings.groupboxRunOptions;
            this.Resources["btnSave.Content"] = language.Settings.btnSave;
            this.Resources["btnCancel.Content"] = language.Settings.btnCancel;
            this.Resources["checkboxRunMultipleApplicationText.Text"] = language.Settings.checkboxRunMultipleApplicationText;
            this.Resources["textBlockInformation.Text"] = $"{language.Settings.textBlockInformation}\nAbout {language.Name}\t{language.Version}\t{language.Description}\n";

            _comboboxLanguages.Items.Clear();
            comboboxLanguages();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Frame_DragEnter(object sender, DragEventArgs e)
        {

        }

        private void _btnOk_Click(object sender, RoutedEventArgs e)
        {
            App.IniConfiguration.Write("Settings", "ApplicationMultipleRun", "deny");
            App.WriteSettings();
        }
    }
}
