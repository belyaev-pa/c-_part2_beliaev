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
using EmployeeWPF.Model;

namespace EmployeeWPF
{
    /// <summary>
    /// Логика взаимодействия для AddNewDepartWindow.xaml
    /// </summary>
    public partial class AddNewDepartWindow : Window
    {
        public AddNewDepartWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Добавление нового подразделения
        /// </summary>
        /// <param name="sender">Объект, который вызвал событие</param>
        /// <param name="e">Параметры вызова</param>
        private void BtAddDepartment_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(tbDepartName.Text)) MessageBox.Show("Заполните название подразделения!");
            else
            {
                DataController.InsertRecord(new Department(tbDepartName.Text));
                Close();
            }
        }
    }
}
