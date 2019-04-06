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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using EmployeeWPF.Model;

namespace EmployeeWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitData();
        }

        /// <summary>
        /// инициализация списков
        /// </summary>
        private void InitData()
        {
            //привязка к представлению
            departmentListBox.ItemsSource = DataController.DepartmentList;
            dgEmployee.ItemsSource = DataController.EmployeeList;
        }

        /// <summary>
        /// Изменение подразделения сотрудника
        /// </summary>
        /// <param name="sender">Объект, который вызвал событие</param>
        /// <param name="e">Параметры вызова</param>
        private void BtChangeDepartment_Click(object sender, RoutedEventArgs e)
        {
            //должен быть выбран сотрудник
            if (dgEmployee.SelectedItem is Employee)
            {
                var editWin = new EditEmpDepartWindow
                {
                    Owner = this,
                    SelectedEmployee = dgEmployee.SelectedItem as Employee
                };
                editWin.Show();
            }
        }

        /// <summary>
        /// Обработка нажатия клавиши добавления нового подразделения
        /// </summary>
        /// <param name="sender">Объект, который вызвал событие</param>
        /// <param name="e">Параметры вызова</param>
        private void BtAddDepartment_Click(object sender, RoutedEventArgs e)
        {
            var newDepartWin = new AddNewDepartWindow
            {
                Owner = this
            };
            newDepartWin.Show();
        }

        /// <summary>
        /// Обработка нажатия клавиши добавления нового солтрудника
        /// </summary>
        /// <param name="sender">Объект, который вызвал событие</param>
        /// <param name="e">Параметры вызова</param>
        private void BtAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            var newEmpWin = new AddNewEmployeeWindow
            {
                Owner = this
            };
            newEmpWin.Show();
        }

        /// <summary>
        /// Удаление выбранной записи подразделения
        /// </summary>
        /// <param name="sender">Объект, который вызвал событие</param>
        /// <param name="e">Параметры вызова</param>
        private void BtDeleteDepart_Click(object sender, RoutedEventArgs e)
        {
            if (departmentListBox.SelectedItem is Department)
            {
                DataController.DeleteRecord(departmentListBox.SelectedItem as Department);
            }
        }

        /// <summary>
        /// Удаление выбранной записи сотрудника
        /// </summary>
        /// <param name="sender">Объект, который вызвал событие</param>
        /// <param name="e">Параметры вызова</param>
        private void BtDeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (dgEmployee.SelectedItem is Employee)
            {
                DataController.DeleteRecord(dgEmployee.SelectedItem as Employee);
            }
        }

        /// <summary>
        /// Закрытие главного окна и всего приложения
        /// </summary>
        /// <param name="sender">Объект, который вызвал событие</param>
        /// <param name="e">Параметры вызова</param>
        private void MainWin_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //закрываем подключение с базой
            DataController.CloseDBConnection();
        }

        /// <summary>
        /// Сохраняет все изменения в БД
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtSaveData_Click(object sender, RoutedEventArgs e)
        {
            DataController.UpdateAllData();
        }
    }
}
