using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeWPF.Model
{
    interface IDB
    {
        string UpdateString(string tableName);
        string InsertString(string tableName);
        string DeleteString(string tableName);
    }
}
