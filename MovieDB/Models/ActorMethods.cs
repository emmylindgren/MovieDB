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

        public int InsertActor(ActorDetail ad, out string errormsg)
        {
            //Create SqlConnection object
            SqlConnection dbConnection = new SqlConnection();

            //Connection towards SQL Server
            dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Movies;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            //Sqlstring and adding a movie to the db
            String sqlstring = null;
            if(ad.ProfilePicturePath != null)
            {
                sqlstring = "INSERT INTO Tbl_Actor (Ac_FirstName, Ac_Lastname, Ac_BirthYear, Ac_ProfilePic) " +
                "VALUES (@FirstName,@LastName,@BirthYear, @ProfilePicPath)"; 
            }
            else
            {
                sqlstring = "INSERT INTO Tbl_Actor (Ac_FirstName, Ac_Lastname, Ac_BirthYear) " +
                "VALUES (@FirstName,@LastName,@BirthYear)";
            }
            

            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            dbCommand.Parameters.Add("FirstName", SqlDbType.NVarChar, 50).Value = ad.FirstName;
            dbCommand.Parameters.Add("LastName", SqlDbType.NVarChar, 50).Value = ad.LastName;
            dbCommand.Parameters.Add("BirthYear", SqlDbType.Int).Value = Convert.ToInt32(ad.BirthYear);
            if(ad.ProfilePicturePath!=null)
            {
                dbCommand.Parameters.Add("ProfilePicPath", SqlDbType.NVarChar).Value = ad.ProfilePicturePath;
            }

            List<String> movies = ad.Movies;

            try
            {
                dbConnection.Open();
                int i = 0;
                //Adding the actor to the db. Returns integer representing how many rows has been 
                //updated in the db. If 1, success in adding one actor to the db. 
                i = dbCommand.ExecuteNonQuery();
                if (i == 1)
                {
                    errormsg = "";

                }
                else
                {
                    errormsg = "An actor was not created in the database";
                }
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
                if (movies != null)
                {
                    int movie_id = GetActorID(ad.FirstName, ad.LastName, ad.BirthYear, out errormsg);
                    addActorConnections(movie_id, ad.Movies, out errormsg);
                }
            }

        }

        private int addActorConnections(int actor_id, List<String> movies, out string errormsg)
        {

            //Create SqlConnection object
            SqlConnection dbConnection = new SqlConnection();

            //Connection towards SQL Server
            dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Movies;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            int count = movies.Count;
            int i = 0;

            try
            {
                dbConnection.Open();
                int update = 0;

                while (count > 0 && i < count)
                {
                    //Sqlstring adding the connections betweeen movie and actor. 
                    String sqlstringConnection = "INSERT INTO Tbl_MovieActor (MA_Actor, MA_Movie) " +
                            "VALUES (@actor,@movie)";
                    SqlCommand dbCommandConnection = new SqlCommand(sqlstringConnection, dbConnection);
                    dbCommandConnection.Parameters.Add("actor", SqlDbType.Int).Value = actor_id;
                    dbCommandConnection.Parameters.Add("movie", SqlDbType.Int).Value = Convert.ToInt32(movies[i]);
                    update = dbCommandConnection.ExecuteNonQuery();
                    i++;
                }
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

        public int GetActorID(string firstName,string lastName, int birthYear, out string errormsg)
        {
            SqlConnection dbConnection = new SqlConnection();

            dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Movies;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            String sqlstring = "SELECT Ac_Id FROM Tbl_Actor WHERE Ac_FirstName= @FirstName AND Ac_LastName = @LastName AND Ac_BirthYear= @BirthYear";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);
            dbCommand.Parameters.Add("FirstName", SqlDbType.NVarChar, 50).Value = firstName;
            dbCommand.Parameters.Add("LastName", SqlDbType.NVarChar, 50).Value = lastName;
            dbCommand.Parameters.Add("BirthYear", SqlDbType.Int).Value = birthYear;

            //Create adapter 
            SqlDataAdapter movieAdapter = new SqlDataAdapter(dbCommand);
            DataSet movieDS = new DataSet();

            try
            {
                dbConnection.Open();
                movieAdapter.Fill(movieDS, "Actor");

                int count = 0;

                count = movieDS.Tables["Actor"].Rows.Count;

                if (count > 1)
                {
                    errormsg = "There is two or more actors with that name and age in the database";
                    return 0;
                }
                else if (count == 1)
                {
                    int i = Convert.ToInt16(movieDS.Tables["Actor"].Rows[0]["Ac_Id"]);
                    errormsg = "";
                    return i;
                }
                else
                {
                    errormsg = "No actors was fetched";
                    return 0;
                }
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

        public List<ActorDetail> GetAllActors(out string errormsg)
        {

            SqlConnection dbConnection = new SqlConnection();

            dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Movies;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            String sqlstring = "SELECT * FROM Tbl_Actor";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            //Create adapter 
            SqlDataAdapter actorAdapter = new SqlDataAdapter(dbCommand);
            DataSet actorDS = new DataSet();

            List<ActorDetail> ActorList = new List<ActorDetail>();

            try
            {
                dbConnection.Open();

                //Fill the dataset with data in a table named Actor.
                actorAdapter.Fill(actorDS, "Actor");

                int count = 0;
                int i = 0;

                count = actorDS.Tables["Actor"].Rows.Count;

                if (count > 0)
                {
                    while (i < count)
                    {
                        //Read the data from the dataset. 
                        ActorDetail ad = new ActorDetail();

                        ad.Id = Convert.ToInt16(actorDS.Tables["Actor"].Rows[i]["Ac_Id"]);

                        ad.FirstName = actorDS.Tables["Actor"].Rows[i]["Ac_FirstName"].ToString();
                        ad.LastName = actorDS.Tables["Actor"].Rows[i]["Ac_LastName"].ToString();

                        ad.BirthYear = Convert.ToInt16(actorDS.Tables["Actor"].Rows[i]["Ac_BirthYear"]);

                        i++;
                        ActorList.Add(ad);
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

        public List<ActorDetail> SearchActors(out string errormsg, string searchfrase)
        {

            SqlConnection dbConnection = new SqlConnection();

            dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Movies;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            String[] name = searchfrase.Split(' ');

            String sqlstring = "SELECT * FROM Tbl_Actor WHERE Ac_FirstName LIKE @FirstName OR Ac_LastName LIKE @LastName";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            if (name.Length > 1)
            {
                dbCommand.Parameters.Add("FirstName", SqlDbType.NVarChar).Value = name[0] + "%";
                dbCommand.Parameters.Add("LastName", SqlDbType.NVarChar).Value = name[1] + "%";
            }
            else
            {
                dbCommand.Parameters.Add("FirstName", SqlDbType.NVarChar).Value = name[0] + "%";
                dbCommand.Parameters.Add("LastName", SqlDbType.NVarChar).Value = name[0] + "%";
            }
            
            //Create adapter 
            SqlDataAdapter actorAdapter = new SqlDataAdapter(dbCommand);
            DataSet actorDS = new DataSet();

            List<ActorDetail> ActorList = new List<ActorDetail>();

            try
            {
                dbConnection.Open();

                //Fill the dataset with data in a table named Actor.
                actorAdapter.Fill(actorDS, "Actor");

                int count = 0;
                int i = 0;

                count = actorDS.Tables["Actor"].Rows.Count;

                if (count > 0)
                {
                    while (i < count)
                    {
                        //Read the data from the dataset. 
                        ActorDetail ad = new ActorDetail();

                        ad.Id = Convert.ToInt16(actorDS.Tables["Actor"].Rows[i]["Ac_Id"]);

                        ad.FirstName = actorDS.Tables["Actor"].Rows[i]["Ac_FirstName"].ToString();
                        ad.LastName = actorDS.Tables["Actor"].Rows[i]["Ac_LastName"].ToString();

                        ad.BirthYear = Convert.ToInt16(actorDS.Tables["Actor"].Rows[i]["Ac_BirthYear"]);

                        i++;
                        ActorList.Add(ad);
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

        public ActorDetail GetOneActor(int actor_id, out string errormsg)
        {

            SqlConnection dbConnection = new SqlConnection();

            dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Movies;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            String sqlstring = "SELECT * FROM Tbl_Actor WHERE Ac_Id = @id";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            dbCommand.Parameters.Add("id", SqlDbType.Int).Value = actor_id;

            //Create adapter 
            SqlDataAdapter actorAdapter = new SqlDataAdapter(dbCommand);
            DataSet actorDS = new DataSet();

            ActorDetail ad = new ActorDetail();

            try
            {
                dbConnection.Open();

                //Fill the dataset with data in a table named Actor.
                actorAdapter.Fill(actorDS, "Actor");

                int count = 0;
                int i = 0;

                count = actorDS.Tables["Actor"].Rows.Count;

                if (count == 1)
                {
                    ad.Id = Convert.ToInt16(actorDS.Tables["Actor"].Rows[i]["Ac_Id"]);

                    ad.FirstName = actorDS.Tables["Actor"].Rows[i]["Ac_FirstName"].ToString();
                    ad.LastName = actorDS.Tables["Actor"].Rows[i]["Ac_LastName"].ToString();

                    ad.BirthYear = Convert.ToInt16(actorDS.Tables["Actor"].Rows[i]["Ac_BirthYear"]);
                
                    ad.ProfilePicturePath = actorDS.Tables["Actor"].Rows[i]["Ac_ProfilePic"].ToString();
                    
                    errormsg = "";
                    return ad;
                }
                else
                {
                    errormsg = "No actor was fetched";
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

        public int DeleteActor(int actor_id, out string errormsg)
        {

            SqlConnection dbConnection = new SqlConnection();

            dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Movies;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            String sqlstring = "DELETE FROM Tbl_Actor WHERE Ac_Id = @id";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            dbCommand.Parameters.Add("id", SqlDbType.Int).Value = actor_id;
            int id = actor_id;

            try
            {
                dbConnection.Open();
                int i = 0;
                //Removing the actor from the db. Returns integer representing how many rows has been 
                //updated in the db. If 1, success in removing one actor from the db. 
                i = dbCommand.ExecuteNonQuery();
                if (i == 1)
                {
                    errormsg = "";
                }
                else
                {
                    errormsg = "The actor was not deleted from the database";
                }
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

        public List<SelectListItem> GetActorandIDSelected(out string errormsg, int id)
        {

            SqlConnection dbConnection = new SqlConnection();

            dbConnection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Movies;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            String sqlstring = "SELECT Ac_FirstName, Ac_LastName, Ac_Id FROM Tbl_Actor";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            //Create adapter 
            SqlDataAdapter actorAdapter = new SqlDataAdapter(dbCommand);
            DataSet actorDS = new DataSet();

            List<String> SelectedActors = GetActorsCorrespondingTo(id,out errormsg);
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
                        String name = actorDS.Tables["Actor"].Rows[i]["Ac_FirstName"].ToString() + " " + actorDS.Tables["Actor"].Rows[i]["Ac_LastName"].ToString();
                        if(SelectedActors != null)
                        {
                            if (SelectedActors.Contains(name))
                            {
                                ActorList.Add(new SelectListItem
                                {
                                    Text = actorDS.Tables["Actor"].Rows[i]["Ac_FirstName"].ToString() + " " + actorDS.Tables["Actor"].Rows[i]["Ac_LastName"].ToString(),
                                    Value = actorDS.Tables["Actor"].Rows[i]["Ac_Id"].ToString(),
                                    Selected = true
                                });
                            }
                            else
                            {
                                ActorList.Add(new SelectListItem
                                {
                                    Text = actorDS.Tables["Actor"].Rows[i]["Ac_FirstName"].ToString() + " " + actorDS.Tables["Actor"].Rows[i]["Ac_LastName"].ToString(),
                                    Value = actorDS.Tables["Actor"].Rows[i]["Ac_Id"].ToString()
                                });

                            }
                        }
                        else
                        {
                            ActorList.Add(new SelectListItem
                            {
                                Text = actorDS.Tables["Actor"].Rows[i]["Ac_FirstName"].ToString() + " " + actorDS.Tables["Actor"].Rows[i]["Ac_LastName"].ToString(),
                                Value = actorDS.Tables["Actor"].Rows[i]["Ac_Id"].ToString()
                            });
                        }
                        
                        //Read the data from the dataset.
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
