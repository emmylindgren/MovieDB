using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace MovieDB.Models
{
    public class GenreMethods
    {

        public GenreMethods() { }

        public int GetIDOf(string genre, out string errormsg)
        {
            SqlConnection dbConnection = new SqlConnection();

            dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Movies;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            String sqlstring = "SELECT Ge_Id FROM Tbl_Genre WHERE Ge_Title = @genre";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            dbCommand.Parameters.Add("genre", SqlDbType.Char).Value = genre;
            //Create adapter 
            SqlDataAdapter genreAdapter = new SqlDataAdapter(dbCommand);
            DataSet genreDS = new DataSet();

            int id = 0;
            try
            {
                dbConnection.Open();

                genreAdapter.Fill(genreDS, "Genre");

                int count = 0;

                count = genreDS.Tables["Genre"].Rows.Count;

                if (count > 0)
                {
                    id = Convert.ToInt32(genreDS.Tables["Genre"].Rows[0]);
                    errormsg = "";
                    return id;
                }
                else
                {
                    errormsg = "No genres was fetched";
                    return (0);
                }
            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return (0);
            }
            finally
            {
                dbConnection.Close();
            }

        }
        public List<SelectListItem> GetAllGenres(out string errormsg)
        {

            SqlConnection dbConnection = new SqlConnection();

            dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Movies;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            String sqlstring = "SELECT * FROM Tbl_Genre";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            //Create adapter 
            SqlDataAdapter genreAdapter = new SqlDataAdapter(dbCommand);
            DataSet genreDS = new DataSet();

            List<SelectListItem> GenreList = new List<SelectListItem>();

            try
            {
                dbConnection.Open();

                genreAdapter.Fill(genreDS, "Genre");

                int count = 0;
                int i = 0;

                count = genreDS.Tables["Genre"].Rows.Count;

                if (count > 0)
                {
                    while (i < count)
                    {
                        //Read the data from the dataset.
                        GenreList.Add(new SelectListItem
                        {
                            Text = genreDS.Tables["Genre"].Rows[i]["Ge_Title"].ToString(),
                            Value = genreDS.Tables["Genre"].Rows[i]["Ge_Id"].ToString()
                        });
                        i++;
                    }
                    errormsg = "";
                    return GenreList;
                }
                else
                {
                    errormsg = "No genres was fetched";
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
