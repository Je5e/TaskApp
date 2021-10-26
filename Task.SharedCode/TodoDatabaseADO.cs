using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Task.SharedCode.Bl;

namespace Task.SharedCode.Dl
{
    public class TodoDatabase
    {
        static readonly object locker = new object();
        public SqliteConnection Connection;
        public string Path;

        public TodoDatabase(string path)
        {
            this.Path = path;

            // Crear las tablas
            bool exists = File.Exists(path);

            if (!exists)
            {
                Connection = new SqliteConnection($"Data Source={path}");
                // Abrir la coneccion
                Connection.Open();
                var Commands = new[]
                {
                    "CREATE TABLE [Items] (_id INTEGER PRIMARY KEY ASC, Name NTEXT, Notes NTEXT, Done INTEGER);"
                };
                foreach (var command in Commands)
                {
                    using var c = Connection.CreateCommand();
                    c.CommandText = command;
                    c.ExecuteNonQuery();
                }
            }
            else
            {
                // La base bd existe, hacemos nada.
            }
        }


        // CRUD
        public IEnumerable<TodoItem> GetItems()
        {
            List<TodoItem> Result = new List<TodoItem>();

            lock (locker)
            {
                Connection = new SqliteConnection($"Data Source= {Path}");
                Connection.Open();
                using var contents = Connection.CreateCommand();
                contents.CommandText = "SELECT [_Id], [Name],[Notes], [Done] FROM [Items]";
                var R = contents.ExecuteReader();
                while (R.Read())
                {
                    Result.Add(FromReaderToItem(R));
                }
                Connection.Close();
            }

            return Result;
        }

        public TodoItem GetItem(int id)
        {
            var t = new TodoItem();
            lock (locker)
            {
                Connection = new SqliteConnection("Data Source=" + Path);
                Connection.Open();
                using (var command = Connection.CreateCommand())
                {
                    command.CommandText = "SELECT [_id], [Name], [Notes], [Done] from [Items] WHERE [_id] = $id";

                    command.Parameters.AddWithValue("$id", id);
                    var r = command.ExecuteReader();
                    while (r.Read())
                    {
                        t = FromReaderToItem(r);
                        break;
                    }
                }
                Connection.Close();
            }
            return t;
        }

        public int SaveItem(TodoItem item)
        {
            int r;
            lock (locker)
            {
                // Vamos a editar una tarea.
                if (item.Id != 0)
                {
                    Connection = new SqliteConnection("Data Source=" + Path);
                    Connection.Open();
                    using (var command = Connection.CreateCommand())
                    {
                        command.CommandText = "UPDATE [Items] SET [Name] = $name, [Notes] = $notes, [Done] = $done WHERE [_id] = $id;";
                        command.Parameters.AddWithValue("$name", item.Name);
                        command.Parameters.AddWithValue("$notes", item.Notes);
                        command.Parameters.AddWithValue("$done", item.Done);
                        command.Parameters.AddWithValue("$id", item.Id);
                        r = command.ExecuteNonQuery();
                    }
                    Connection.Close();
                    return r;
                }
                // Registramos un nuevo TodoItem
                else
                {
                    Connection = new SqliteConnection("Data Source=" + Path);
                    Connection.Open();
                    using (var command = Connection.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO [Items] ([Name], [Notes], [Done]) VALUES ($name ,$notes,$done)";
                        command.Parameters.AddWithValue("$name", item.Name);
                        command.Parameters.AddWithValue("$notes", item.Notes);
                        command.Parameters.AddWithValue("$done", item.Done);
                        r = command.ExecuteNonQuery();
                    }
                    Connection.Close();
                    return r;
                }

            }
        }

        public int DeleteItem(int id)
        {
            lock (locker)
            {
                int r;
                Connection = new SqliteConnection("Data Source=" + Path);
                Connection.Open();
                using (var command = Connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM [Items] WHERE [_id] = $id;";
                    command.Parameters.AddWithValue("$id", id);
                    r = command.ExecuteNonQuery();
                }
                Connection.Close();
                return r;
            }
        }

        private TodoItem FromReaderToItem(SqliteDataReader r)
        {
            var t = new TodoItem
            {
                Id = Convert.ToInt32(r["_id"]),
                Name = r["Name"].ToString(),
                Notes = r["Notes"].ToString(),
                Done = Convert.ToInt32(r["Done"]) == 1
            };
            return t;
        }
    }
}
