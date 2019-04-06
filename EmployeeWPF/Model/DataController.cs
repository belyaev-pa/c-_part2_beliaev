using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
//using System.Data.SqlClient;
using System.Data.Common;
using System.Data.SQLite;


namespace EmployeeWPF.Model
{
    /// <summary>
    /// Класс для упраления списками сотрудников и подразделений
    /// </summary>
    class DataController
    {
        private static ObservableCollection<Employee> employeeList;
        private static ObservableCollection<Department> departmentList;
        private static string dbName = "EmployeeDB.db3";        
        private static SQLiteConnection connection;
        private static bool needFillData = false;
        private static SQLiteCommand command;
        private static string selectEmployee = @"SELECT * FROM [Employee]";
        private static string selectDepartment = @"SELECT * FROM [Department]";

        public static ObservableCollection<Employee> EmployeeList
        {
            get
            {
                if (employeeList == null)
                    InitData();
                return employeeList;
            }
        }

        public static ObservableCollection<Department> DepartmentList
        {
            get
            {
                if (departmentList == null)
                    InitData();
                return departmentList;
            }
        }

        private DataController() { }

        /// <summary>
        /// Инициализация данных
        /// </summary>
        private static void InitData()
        {            
            employeeList = new ObservableCollection<Employee>();
            departmentList = new ObservableCollection<Department>();

            //try
            //{                
                InitDBConnection();                
                GenerateDBSchema();
                FillTestData();
                FillLists();
            //}
            //catch (Exception e)
           // {
            //    Console.WriteLine(e.ToString());
           // }
        }

        /// <summary>
        /// Инициализация подключения к БД
        /// </summary>
        private static void InitDBConnection()
        {
            if (connection == null)
            {
                SQLiteConnection.CreateFile(dbName);
                SQLiteFactory factory = (SQLiteFactory)DbProviderFactories.GetFactory("System.Data.SQLite");
                connection = (SQLiteConnection)factory.CreateConnection();
                connection.ConnectionString = @"Data Source=" + dbName;
                connection.Open();
            }
                
        }

        /// <summary>
        /// Генерация схемы данных (таблицы сотрдуников и подразделений)
        /// </summary>
        private static void GenerateDBSchema()
        {
            string createEmpTable = @"
                    CREATE TABLE [Employee] (
                        [Id]        INTEGER   PRIMARY KEY AUTOINCREMENT    NOT NULL,
                        [FirstName] VARCHAR   NOT NULL,
                        [LastName]  VARCHAR   NOT NULL,
                        [DepartId]  INTEGER   NOT NULL            
                    );";
            string createDepartTable = @"
                    CREATE TABLE [Department] (
                        [Id]   INTEGER PRIMARY KEY AUTOINCREMENT    NOT NULL,
                        [Name] VARCHAR  NOT NULL                        
                    );";

            try
            {
                //пробуем сделать выборку сотрудников
                command = new SQLiteCommand(connection);
                command.CommandText = selectEmployee;
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                //если таблицы не существует, создаём её
                command = new SQLiteCommand(connection);
                command.CommandText = createEmpTable;
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
                needFillData = true;
            }
            
            try
            {
                //пробуем сделать выборку предприятий
                command = new SQLiteCommand(connection);
                command.CommandText = selectDepartment;
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                //если таблицы не существует, создаём её
                command = new SQLiteCommand(connection);
                command.CommandText = createDepartTable;
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
                needFillData = true;
            }
        }

        /// <summary>
        /// Заполнение списков тестовыми данными (временная функция)
        /// </summary>
        private static void FillTestData()
        {
            //заполняем таблицы тестовыми данными
            if (needFillData)
            {
                string deleteAllEmployee = @"DELETE FROM [Employee];";
                string deleteAllDepartment = @"DELETE FROM [Department];";

                //на всякий случай очищаем данные из таблиц (вдруг одна из них заполнена)
                command = new SQLiteCommand(connection);
                command.CommandText = deleteAllDepartment;
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
                command = new SQLiteCommand(connection);
                command.CommandText = deleteAllEmployee;
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();

                var insertDepart = new string[]
                {
                    @"INSERT INTO [Department] (Name) VALUES ('Рога и копы');",
                    @"INSERT INTO [Department] (Name) VALUES ('Хрю-Хрю');",
                    @"INSERT INTO [Department] (Name) VALUES ('Адиос-Амиго');",
                    @"INSERT INTO [Department] (Name) VALUES ('Крутые перцы');",
                    @"INSERT INTO [Department] (Name) VALUES ('Щекотка');",
                    @"INSERT INTO [Department] (Name) VALUES ('Вжух');"
                };

                foreach (var item in insertDepart)
                {
                    command = new SQLiteCommand(connection);
                    command.CommandText = item;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }

                var inserEmployee = new string[]
                {
                     @"INSERT INTO [Employee] (FirstName, LastName, DepartId) VALUES ('Иван', 'Перчиков', 1);",
                     @"INSERT INTO [Employee] (FirstName, LastName, DepartId) VALUES ('Павел', 'Жуков', 3);",
                     @"INSERT INTO [Employee] (FirstName, LastName, DepartId) VALUES ('Ярослав', 'Выборг', 4);",
                     @"INSERT INTO [Employee] (FirstName, LastName, DepartId) VALUES ('Александр', 'Кукловод', 1);",
                     @"INSERT INTO [Employee] (FirstName, LastName, DepartId) VALUES ('Алексей', 'Минаев', 2);",
                     @"INSERT INTO [Employee] (FirstName, LastName, DepartId) VALUES ('Роман', 'Егоров', 5);",
                     @"INSERT INTO [Employee] (FirstName, LastName, DepartId) VALUES ('Максим', 'Кукловод', 6);",
                     @"INSERT INTO [Employee] (FirstName, LastName, DepartId) VALUES ('Андрей', 'Петров', 1);",
                     @"INSERT INTO [Employee] (FirstName, LastName, DepartId) VALUES ('Дмитрий', 'Беляев', 2);",
                };

                foreach (var item in inserEmployee)
                {
                    command = new SQLiteCommand(connection);
                    command.CommandText = item;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Наполение списков сотрудников и подразделений данными из БД
        /// </summary>
        private static void FillLists()
        {
            SQLiteDataReader reader;

            //наполняем подразделения
            command = new SQLiteCommand(connection);
            command.CommandText = selectDepartment;
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();
            reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while(reader.Read())
                {
                    departmentList.Add(new Department(reader.GetInt32(reader.GetOrdinal("Id")),
                                                      reader.GetString(reader.GetOrdinal("Name")) ));
                }
            }
            reader.Close();

            //наполняем сотрудников
            command = new SQLiteCommand(connection);
            command.CommandText = selectEmployee;
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();
            reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    employeeList.Add(new Employee(reader.GetInt32(reader.GetOrdinal("Id")),
                                                  reader.GetString(reader.GetOrdinal("FirstName")),
                                                  reader.GetString(reader.GetOrdinal("LastName")),
                                                  GetDepartmentById(reader.GetInt32(reader.GetOrdinal("DepartId"))) ));
                }
            }
            reader.Close();
        }

        /// <summary>
        /// Получение подразделения по идентификатору
        /// </summary>
        /// <returns></returns>
        private static Department GetDepartmentById(int id)
        {
            foreach (var item in departmentList)
            {
                if (item.Id == id)
                    return item;
            }
            return null;
        }

        /// <summary>
        /// Закрытие соединения
        /// </summary>
        public static void CloseDBConnection()
        {
            if (connection != null)
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Обновление записи
        /// </summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <param name="updateObject">Объект</param>
        public static void UpdateRecord(IDB updateObject)
        {
            string tableName = String.Empty;
            if (updateObject is Employee) tableName = "Employee";
            if (updateObject is Department) tableName = "Department";

            if (String.IsNullOrEmpty(tableName)) return;

            command = new SQLiteCommand(connection);
            command.CommandText = updateObject.UpdateString(tableName);
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();       
        }

        /// <summary>
        /// Обновление всех данных
        /// </summary>
        public static void UpdateAllData()
        {
            foreach (var item in employeeList)
            {
                UpdateRecord(item);
            }

            foreach (var item in departmentList)
            {
                UpdateRecord(item);
            }
        }

        /// <summary>
        /// Вставка записи
        /// </summary>
        public static void InsertRecord(IDB insertObject)
        {
            string tableName = String.Empty;
            if (insertObject is Employee)
            {
                employeeList.Add(insertObject as Employee);
                tableName = "Employee";
            }
            if (insertObject is Department)
            {
                departmentList.Add(insertObject as Department);
                tableName = "Department";
            }

            if (String.IsNullOrEmpty(tableName)) return;

            command = new SQLiteCommand(connection);
            command.CommandText = insertObject.InsertString(tableName);
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();            
                        
            departmentList.Clear();
            employeeList.Clear();
            FillLists();
        }

        public static void DeleteRecord(IDB deleteObject)
        {
            string tableName = String.Empty;
            if (deleteObject is Employee)
            {
                employeeList.Remove(deleteObject as Employee);
                tableName = "Employee";
                command = new SQLiteCommand(connection);
                command.CommandText = deleteObject.DeleteString(tableName);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            if (deleteObject is Department)
            {
                Department dep = deleteObject as Department;
                departmentList.Remove(dep);
                tableName = "Department";
                // будет костыльно!
                command = new SQLiteCommand(connection);
                command.CommandText = $"DELETE FROM [Employee] WHERE DepartId = {dep.Id};";
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
                command = new SQLiteCommand(connection);
                command.CommandText = deleteObject.DeleteString(tableName);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }

            if (String.IsNullOrEmpty(tableName)) return;

            

            departmentList.Clear();
            employeeList.Clear();
            FillLists();

        }
    }
}
