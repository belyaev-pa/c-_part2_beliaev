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
    /// Логика взаимодействия для AddNewEmployeeWindow.xaml
    /// </summary>
    public partial class AddNewEmployeeWindow : Window
    {
        /// <summary>
        /// Инициализация нового окна
        /// </summary>
        public AddNewEmployeeWindow()
        {
            InitializeComponent();
            InitData();
        }

        /// <summary>
        /// Инициализация данных
        /// </summary>
        private void InitData()
        {
            deprtmentListBox.ItemsSource = DataController.DepartmentList;
        }

        /// <summary>
        /// Добавление нового подразделения
        /// </summary>
        /// <param name="sender">Объект, который вызвал событие</param>
        /// <param name="e">Параметры вызова</param>
        private void BtAddNewEmployee_Click(object sender, RoutedEventArgs e)
        {
            var firstName = tbFirstName.Text;
            var lastName = tbLastName.Text;
            Department department = deprtmentListBox.SelectedItem as Department;

            if (String.IsNullOrEmpty(firstName) || String.IsNullOrEmpty(lastName))
            {
                MessageBox.Show("Основные данные о сотрдунике должны быть все заполнены!");
                return;
            }

            if (department == null)
            {
                MessageBox.Show("Укажите подразделение для нового сотрудника!");
                return;
            }

            DataController.InsertRecord(new Employee(firstName, lastName, department));

            Close();
        }
    }
}
