using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace DisassemblerUI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadFileButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(FileTextBox.Text))
            {
                // Show the file picker
                var fileDialog = new OpenFileDialog {CheckFileExists = true, CheckPathExists = true};
                if (fileDialog.ShowDialog() == true)
                {
                    // Load the file
                }
            }
            else
            {
                if (File.Exists(FileTextBox.Text))
                {
                    // Do a check here with the disassemble to determine that this is a valid chip 8 file                       
                }
                else
                {   
                    // Show the error message box and clear the box
                    MessageBox.Show("File does not exit");
                    FileTextBox.Text = string.Empty;
                }

            }
        }

        private void ShowHexCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // Update the instruction data to convert all hexadecimal to decimal or vice versa.
            MessageBox.Show("Checkbox");
        }
    }
}
