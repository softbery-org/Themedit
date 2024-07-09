// Version: 1.0.0.102
// Copyright (c) 2024 Softbery by Paweł Tobis
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Themedit.src
{
    public class Database
    {
        private SqlConnection _connection;
        private SqlCommand _command;
        private string _server = "localhost";
        private string _user = "root";
        private string _password = "";
        private string _databaseName = "master";
        private string _connectionString = "";

        public Database(string user, string password, string server, string database) 
        {
            
        }

        public Database(string user, string password, string server, string[] database)
        {

        }

        public void CreateDatabase(string database_name, int size=10, int max_size=50, int file_growth=10, bool log = true)
        {
            string command_string;
            SqlConnection connection = new SqlConnection("Server=localhost;Integrated security=SSPI;database=master");

            double d_size = size / 2;
            double d_max_size = max_size / 2;

            command_string = $"CREATE DATABASE {database_name} ON PRIMARY " +
                  $"(NAME = {database_name}_Data, " +
                  $"FILENAME = 'database/{database_name}Data.mdf', " +
                  $"SIZE = {size}MB, MAXSIZE = {max_size}MB, FILEGROWTH = {file_growth}%)" +
                  $"LOG ON (NAME = {database_name}_Log, " +
                  $"FILENAME = 'database/{database_name}Log.ldf', " +
                  $"SIZE = {d_size}MB, " +
                  $"MAXSIZE = {d_max_size}MB, " +
                  $"FILEGROWTH = {file_growth}%)";

            SqlCommand command = new SqlCommand(command_string, connection);

            try
            {
                connection.Open();
                command.ExecuteNonQuery();
                MessageBox.Show("DataBase is created successfully", "Themedit info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Themedit info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public async void Connect()
        {
            _connectionString = $"Server={_server};Integrated security=SSPI;database={_databaseName};user={_user};password={_password};";
            SqlConnection connection = new SqlConnection(_connectionString);

            try
            {
                _connection = connection;
                await _connection.OpenAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Can't open connection with {_databaseName}. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                connection.Close();
            }
        }

        public void Close()
        {
            if (_connection != null)
            {
                _connection.Close();
            }
            else
            {
                MessageBox.Show($"No SQL connection instance. Connection has null value.\nFirst must open connection with database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class SqlServerConnection
    {
        private Dictionary<string, SqlConnection> servers = new Dictionary<string, SqlConnection>();

        public string Name { get; private set; }
        public string User { get; private set; }
        public string Password { get; private set; }
        public string Server { get; private set; }
        public string Database { get; private set; }
        public SqlConnection Connection { get; private set; }

        public SqlConnection this[string name]
        {
            get
            {
                return servers[name];
            }
            set
            {
                servers[name] = value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public SqlServerConnection(){}

        ~SqlServerConnection()
        {
            
        }

        public void Add(string name, string user, string password, string server, string database)
        {
            try
            {
                Name = name;
                User = user;
                Password = password;
                Server = server;
                Database = database;
                Connection = new SqlConnection($"Server={server};Integrated Security=true;Database={database};UserId={user};Password={password};");

                servers.Add(name, Connection);
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Can't add SQL server connection. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }
    }
}
