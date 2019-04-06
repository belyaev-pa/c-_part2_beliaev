using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeWPF.Model
{
    /// <summary>
    /// Класс подразделений
    /// </summary>
    class Department : INotifyPropertyChanged, IDB
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int id;
        private string name;

        public int Id
        {
            get
            {
                return id;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }                
            set
            {
                name = value;
                NotifyPropertyChanged(nameof(this.Name));
            }
        }

        /// <summary>
        /// Инициализация нового подразделения
        /// </summary>
        /// <param name="name">Название</param>
        public Department(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Инициализация нового подразделения
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="name">Название</param>
        public Department(int id, string name)
        {
            this.id = id;
            this.name = name;
        }

        /// <summary>
        /// Переопределение метода ToString()
        /// </summary>
        /// <returns>Название подразделения</returns>
        public override string ToString()
        {
            return Name;
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
            return $"UPDATE [{tableName}] SET Name = '{name}' WHERE Id = {id};";
        }

        /// <summary>
        /// Генерация строки вставки данных
        /// </summary>
        /// <param name="tableName">Название таблицы</param>
        /// <returns>Возвращает итоговую строку запроса</returns>
        public string InsertString(string tableName)
        {
            return $"INSERT INTO [{tableName}] (Name) VALUES ('{name}');";
        }

        /// <summary>
        /// Генерация строки удаления данных
        /// </summary>
        /// <param name="tableName">Название таблицы</param>
        /// <returns>Возвращает итоговую строку запроса</returns>
        public string DeleteString(string tableName)
        {
            return $"DELETE FROM [{tableName}] WHERE Id = {id};";
        }
    }
}
