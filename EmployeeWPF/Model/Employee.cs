using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeWPF.Model
{
    /// <summary>
    /// Класс сотрудников
    /// </summary>
    class Employee : INotifyPropertyChanged, IDB
    {
        private int id;
        private string firstName;
        private string lastName;
        private Department department;

        public event PropertyChangedEventHandler PropertyChanged;

        public string FullName
        {
            get
            {
                return $"{lastName} {firstName}";
            }
        }


        public string FirstName
        {
            get
            {
                return firstName;
            }
            set
            {
                firstName = value;
                NotifyPropertyChanged(nameof(this.FirstName));
            }
        }

        public string LastName
        {
            get
            {
                return lastName;
            }
            set
            {
                lastName = value;
                NotifyPropertyChanged(nameof(this.LastName));
            }
        }

        public Department Department
        {
            get
            {
                return department;
            }
            set
            {
                department = value;
                NotifyPropertyChanged(nameof(this.Department));
            }
        }

        /// <summary>
        /// Инициализация нового сотрудника
        /// </summary>
        /// <param name="firstName">Имя</param>
        /// <param name="lastName">Фамилия</param>
        /// <param name="department">Подразделение</param>
        public Employee(string firstName, string lastName, Department department)
        {
            this.firstName = firstName;
            this.lastName = lastName;
            this.department = department;
        }

        /// <summary>
        /// Инициализация нового сотрудника
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="firstName">Имя</param>
        /// <param name="lastName">Фамилия</param>
        /// <param name="department">Подразделение</param>
        public Employee(int id, string firstName, string lastName, Department department)
        {
            this.id = id;
            this.firstName = firstName;
            this.lastName = lastName;
            this.department = department;
        }

        /// <summary>
        /// Переопределение метода ToString()
        /// </summary>
        /// <returns>Полное имя сотрудника</returns>
        public override string ToString()
        {
            return $"{FullName} - {Department.Name}";
        }

        /// <summary>
        /// Уведомление об изменении свойства объекта
        /// </summary>
        /// <param name="propName">Название свойства</param>
        public void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        /// <summary>
        /// Генерация строки обновления данных
        /// </summary>
        /// <param name="tableName">Название таблицы</param>
        /// <returns>Возвращает итоговую строку запроса</returns>
        public string UpdateString(string tableName)
        {
            return $"UPDATE [{tableName}] SET FirstName = '{FirstName}', LastName = '{LastName}', DepartId = {Department.Id} WHERE Id = {id};";
        }

        /// <summary>
        /// Генерация строки вставки данных
        /// </summary>
        /// <param name="tableName">Название таблицы</param>
        /// <returns>Возвращает итоговую строку запроса</returns>
        public string InsertString(string tableName)
        {
            return $"INSERT INTO [{tableName}] (FirstName, LastName, DepartId) VALUES ('{FirstName}', '{LastName}', {Department.Id});";
        }

        /// <summary>
        /// Генерация строки удаления данных
        /// </summary>
        /// <param name="tableName">Название таблицы</param>
        /// <returns>Возвращает итоговую строку запроса</returns>
        public string DeleteString(string tableName)
        {
            return $"DELETE FROM  [{tableName}] WHERE Id = {id};";
        }
    }
}
