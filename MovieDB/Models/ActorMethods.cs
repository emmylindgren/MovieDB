using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace MovieDB.Models
{
    public class ActorMethods
    {

        public ActorMethods() { }

        public List<SelectListItem> GetActorandID(out string errormsg)
        {

            SqlConnection dbConnection = new SqlConnection();

            dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Movies;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            String sqlstring = "SELECT Ac_FirstName, Ac_LastName, Ac_Id FROM Tbl_Actor";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            //Create adapter 
            SqlDataAdapter actorAdapter = new SqlDataAdapter(dbCommand);
            DataSet actorDS = new DataSet();

            List<SelectListItem> ActorList = new List<SelectListItem>();

            try
            {
                dbConnection.Open();

                actorAdapter.Fill(actorDS, "Actor");

                int count = 0;
                int i = 0;

                count = actorDS.Tables["Actor"].Rows.Count;

                if (count > 0)
                {
                    while (i < count)
                    {
                        //Read the data from the dataset.
                        ActorList.Add(new SelectListItem
                        {
                            Text = actorDS.Tables["Actor"].Rows[i]["Ac_FirstName"].ToString() +" "+ actorDS.Tables["Actor"].Rows[i]["Ac_LastName"].ToString(),
                            Value = actorDS.Tables["Actor"].Rows[i]["Ac_Id"].ToString()
                        });
                        i++;
                    }
                    errormsg = "";
                    return ActorList;
                }
                else
                {
                    errormsg = "No actors was fetched";
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

        public List<String> GetActorsCorrespondingTo(int movie_id, out string errormsg)
        {

            SqlConnection dbConnection = new SqlConnection();

            dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Movies;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            String sqlstring = "SELECT Ac_FirstName, Ac_LastName " +
                "FROM Tbl_Actor, Tbl_MovieActor WHERE MA_Movie = @Id AND Ac_Id = MA_Actor";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            dbCommand.Parameters.Add("Id", SqlDbType.Int).Value = movie_id;

            //Create adapter 
            SqlDataAdapter actorAdapter = new SqlDataAdapter(dbCommand);
            DataSet actorDS = new DataSet();

            List<String> ActorList = new List<String>();

            try
            {
                dbConnection.Open();

                actorAdapter.Fill(actorDS, "Actor");

                int count = 0;
                int i = 0;

                count = actorDS.Tables["Actor"].Rows.Count;

                if (count > 0)
                {
                    while (i < count)
                    {
                        //Read the data from the dataset.
                        ActorList.Add(actorDS.Tables["Actor"].Rows[i]["Ac_FirstName"].ToString() + " " 
                                + actorDS.Tables["Actor"].Rows[i]["Ac_LastName"].ToString());
                        i++;
                    }
                    errormsg = "";
                    return ActorList;
                }
                else
                {
                    errormsg = "No actors was fetched";
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
