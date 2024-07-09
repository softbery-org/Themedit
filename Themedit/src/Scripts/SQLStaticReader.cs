// Version: 1.0.0.102
// Copyright (c) 2024 Softbery by Paweł Tobis
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Xml;

namespace Themedit.src.Scripts
{
    public static class SQLStaticReader
    {
        public static Dictionary<string,string> GetData(this SqlCommand command, string column_name)
        {
            try
            {
                SqlDataReader reader = command.ExecuteReader();
                var result = new Dictionary<string, string>();

                while (reader.Read())
                {
                    SqlXml xmlData = reader.GetSqlXml(0);
                    XmlReader xmlReader = xmlData.CreateReader();

                    xmlReader.MoveToContent();

                    while (xmlReader.Read())
                    {
                        if (xmlReader.NodeType == XmlNodeType.Element)
                        {
                            string elementName = xmlReader.LocalName;
                            xmlReader.Read();

                            result.Add(elementName, xmlReader.Value);
                        }
                    }
                }
                return result;
            }
            catch(Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }   
        }

        /// <summary>
        /// Drop database 
        /// </summary>
        /// <param name="database_name"></param>
        public static void DropDatabase(this SqlConnection connection, string database_name) 
        {
            var command = $"USE tempdb;" +
                $"GO" +
                $"DECLARE @SQL nvarchar(1000);" +
                $"IF EXISTS(SELECT 1 FROM sys.databases WHERE[name] = N'{database_name}')" +
                $"BEGIN" +
                $"SET @SQL = N'USE [{database_name}];" +
                $"ALTER DATABASE Sales SET SINGLE_USER WITH ROLLBACK IMMEDIATE;" +
                $"USE[tempdb];" +
                $"DROP DATABASE {database_name}; ';" +
                $"EXEC(@SQL);" +
                $"END;";

            var execute =  new SqlCommand(command, connection).ExecuteNonQuery();

            if (execute==0)
            {
                MessageBox.Show("Database no exist.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Database was deleted.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    public struct WindowStruct
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Width { get; private set; }
        public double Height { get; private set; }

        public WindowStruct(double x, double y, double width, double heigh)
        {
            X = x;
            Y = y;
            Width = width;
            Height = heigh;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="window_postition">Window position in format (important - '(.,.,.,.)' data must be between brackets, and data must be separated by commas like neared example)</param>
        /// <example>
        /// (x,y,width,height)
        /// (11.2, 12.3, 100, 50)
        /// </example>
        public WindowStruct(string window_postition)
        {
            if (window_postition.StartsWith("(") && window_postition.EndsWith(")"))
            {
                var replaced = window_postition.Replace("(", "").Replace(")", "");
                var splited = replaced.Split(',');
                var result = new string[4];

                if (splited.Length != 4)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        string s = "";

                        if (splited[i] != null)
                        {
                            s = splited[i];
                        }
                        else
                        {
                            s = "0";
                        }

                        result[i] = s;
                    }
                }

                X = double.Parse(result[0]);
                Y = double.Parse(result[1]);
                Width = double.Parse(result[2]);
                Height = double.Parse(result[3]);
            }
            else
            {
                X = 0; 
                Y = 0; 
                Width = 0; 
                Height = 0;
            }
        }

        public (double,double,double,double) GetWindowPosition()
        {
            return (X, Y, Width, Height);
        }

        public double GetX() { return X; }
        public double GetY() { return Y; }
        public double GetWidth() { return Width; }
        public double GetHeight() { return Height; }
        public void SetX(double x) { X = x; }
        public void SetY(double y) { Y = y; }
        public void SetWidth(double width) { Width = width; }
        public void SetHeight(double height) { Height = height; }
    }
}
