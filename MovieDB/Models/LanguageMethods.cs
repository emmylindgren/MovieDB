using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace MovieDB.Models
{
    public class LanguageMethods
    {
        public LanguageMethods() { }

        public int getIDOf(String language, out string errormsg)
        {
            //Create SqlConnection object
            SqlConnection dbConnection = new SqlConnection();

            //Connection towards SQL Server
            dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Movies;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            //SQL for getting ID of the language
            String sqlstring = "SELECT La_Id FROM Tbl_Languages WHERE La_Language = '@language'";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            dbCommand.Parameters.Add("language", SqlDbType.Int).Value = language;

            try
            {
                dbConnection.Open();
                int i = dbCommand.ExecuteNonQuery();
                errormsg = "";
                return (i);
            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return 0;
            }
            finally
            {
                dbConnection.Close();
            }

        }

        public List<SelectListItem> GetAllLanguages(out string errormsg)
        {

            SqlConnection dbConnection = new SqlConnection();

            dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Movies;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            String sqlstring = "SELECT * FROM Tbl_Languages";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            //Create adapter 
            SqlDataAdapter languageAdapter = new SqlDataAdapter(dbCommand);
            DataSet languageDS = new DataSet();

            List<SelectListItem> LanguageList = new List<SelectListItem>();

            try
            {
                dbConnection.Open();

                languageAdapter.Fill(languageDS, "Language");

                int count = 0;
                int i = 0;

                count = languageDS.Tables["Language"].Rows.Count;

                if (count > 0)
                {
                    while (i < count)
                    {
                        //Read the data from the dataset.
                        LanguageList.Add(new SelectListItem
                        {
                            Text = languageDS.Tables["Language"].Rows[i]["La_Language"].ToString(),
                            Value = languageDS.Tables["Language"].Rows[i]["La_Id"].ToString()
                        });
                        i++;
                    }
                    errormsg = "";
                    return LanguageList;
                }
                else
                {
                    errormsg = "No languages was fetched";
                    return (null);
                }
            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return (null);
            }
            finally
            {
                dbConnection.Close();
            }

        }
    }
}
