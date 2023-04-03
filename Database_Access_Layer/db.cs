using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using FYP_Blood.Models;
using System.Web.Script.Serialization;
using System.Web.Mvc;
using FYP_Blood.Controllers;

namespace FYP_Blood.Database_Access_Layer
{
    public class db
    {
        private SqlConnection myConnection = null;
        private SqlCommand cmd = null;

        string SQLconnection = ConfigurationManager.ConnectionStrings["con"].ConnectionString;

        AuthorityController authCon = new AuthorityController();

        public void OpenConnection()
        {
            try
            {
                myConnection = new SqlConnection(SQLconnection);
                myConnection.Open();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void CloseConnection()
        {
            try
            {
                myConnection.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /*-----------------------------------------------USER MODULE------------------------------------------ */
        public User userlogin(User user)
        {
            try
            {
                OpenConnection();

                cmd = new SqlCommand("", myConnection);
                cmd.CommandText = @"SELECT * FROM [User] WHERE [Email] = @Email and [Password] = @Password";
                cmd.Parameters.AddWithValue("@Email", user.email);
                cmd.Parameters.AddWithValue("@Password", authCon.Encrypt(user.password));
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        user.userId = (int)reader["UserID"];
                        user.name = reader["Name"].ToString();
                        //user.contactNo = reader["ContactNo"].ToString();
                        user.contactNo = authCon.Decrypt(reader["ContactNo"].ToString());
                        user.gender = reader["Gender"].ToString();
                        user.email = reader["Email"].ToString();
                        user.password = authCon.Decrypt(reader["Password"].ToString());
                        user.department = reader["Department"].ToString();
                        user.role = reader["Role"].ToString();
                        user.status = reader["Status"].ToString();
                        user.nricNo = authCon.Decrypt(reader["nricNo"].ToString());
                        user.dob = Convert.ToDateTime(reader["dob"]);
                        user.bloodType = reader["bloodType"].ToString();
                        user.address = authCon.Decrypt(reader["address"].ToString());
                        user.message = "Login Successful";
                    }
                }
                CloseConnection();
                return user;
            }
            catch (Exception ex)
            {
                CloseConnection();
                user.message = "Invalid.";
                throw new Exception("Error On UserLogin" + ex.Message);
            }
        }

        public int validateuser(string _email)
        {
            try
            {
                OpenConnection();

                int userexist = 0;

                cmd = new SqlCommand("", myConnection);
                cmd.CommandText = @"SELECT * FROM [dbo].[User] WHERE [Email] = @Email";
                cmd.Parameters.AddWithValue("@Email", _email);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    userexist = 1;
                }
                else
                {
                    userexist = 0;
                }
                CloseConnection();
                return userexist;
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw new Exception("Error on Validate Existing User" + ex.Message);
            }
        }

        public void UserSignUp(User user)
        {
            try
            {
                OpenConnection();

                cmd = new SqlCommand("", myConnection);
                cmd.CommandText = @"INSERT INTO [dbo].[User] (Name, Email, Password, Role, Status) VALUES (@Name, @Email, @Password, @Role, @Status)";
                //cmd.Parameters.AddWithValue("@UserID", "12345" DBNull.Value);
                cmd.Parameters.AddWithValue("@Name", user.name);
                cmd.Parameters.AddWithValue("@Email", user.email);
                cmd.Parameters.AddWithValue("@Password", authCon.Encrypt(user.password));
                cmd.Parameters.AddWithValue("@Role", "User");
                cmd.Parameters.AddWithValue("@Status", "Active");

                cmd.ExecuteNonQuery();
                CloseConnection();
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw new Exception("Error on signing user" + ex.Message);
            }
        }

        public void OTPCode(string _email, string OTPCode)
        {
            try
            {
                OpenConnection();

                cmd = new SqlCommand("", myConnection);
                cmd.CommandText = @"UPDATE [dbo].[User] SET [ResetPasswordCode] = @OTPCode WHERE [Email] = @Email";
                cmd.Parameters.AddWithValue("@OTPCode", OTPCode);
                cmd.Parameters.AddWithValue("@Email", _email);

                cmd.ExecuteNonQuery();
                CloseConnection();
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw new Exception("Error on insert reset password" + ex.Message);
            }
        }

        public int verifyOTPCode(string OTPCode)
        {
            try
            {
                OpenConnection();
                cmd = new SqlCommand("", myConnection);
                cmd.CommandText = @"SELECT * FROM [User] WHERE [ResetPasswordCode] = @OTPCode";
                cmd.Parameters.AddWithValue("@OTPCode", OTPCode);
                SqlDataReader reader = cmd.ExecuteReader();

                int OTPvalid = 1;

                if (reader.HasRows)
                {
                    OTPvalid = 1;

                }
                else
                {
                    OTPvalid = 0;
                }

                CloseConnection();
                return OTPvalid;
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw new Exception("Error On UserLogin" + ex.Message);
            }
        }

        public void updateResetPassword(string resetcode, string password)
        {
            try
            {
                OpenConnection();

                cmd = new SqlCommand("", myConnection);
                cmd.CommandText = @"UPDATE [dbo].[User] SET [Password] = @Password, [ResetPasswordCode] = @ResetCode WHERE [ResetPasswordCode] = @OTPCode";
                cmd.Parameters.AddWithValue("@Password", authCon.Encrypt(password));
                cmd.Parameters.AddWithValue("@ResetCode", DBNull.Value);
                cmd.Parameters.AddWithValue("@OTPCode", resetcode);

                cmd.ExecuteNonQuery();
                CloseConnection();
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw new Exception("Error on update reset password" + ex.Message);
            }
        }

        /*-----------------------------------------------ADMIN MODULE------------------------------------------ */
        public List<User> GetAuthorityAccount()
        {
            try
            {
                OpenConnection();
                cmd = new SqlCommand("", myConnection);
                cmd.CommandText = @"SELECT UserID, Name, Department, Status FROM [User] WHERE Role='Hospital Authority';";
                SqlDataReader reader = cmd.ExecuteReader();

                List<User> userObjList = new List<User>();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        User userData = new User();
                        userData.userId = (int)reader["UserID"];
                        userData.name = reader["Name"].ToString();
                        userData.department = reader["Department"].ToString();
                        userData.status = reader["Status"].ToString();

                        userObjList.Add(userData);
                    }
                }

                CloseConnection();
                return userObjList;
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw new Exception("Error on Getting Authority Listing" + ex.Message);
            }
        }

        public void RegisterHospitalAuthority(string name, string username, string dept, string password)
        {
            try
            {
                OpenConnection();

                AdminController adminCon = new AdminController();

                string secretKey = adminCon.GenSecretKey();

                cmd = new SqlCommand("", myConnection);
                cmd.CommandText = @"INSERT INTO [dbo].[User] (Name, Email, Password, Role, Department, Status, dob, seKey) VALUES (@Name, @Email, @Password, @Role, @Department, @Status, @dob, @seKey)";
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Email", username);
                cmd.Parameters.AddWithValue("@Password", authCon.Encrypt(password));
                cmd.Parameters.AddWithValue("@Role", "Hospital Authority");
                cmd.Parameters.AddWithValue("@Department", dept);
                cmd.Parameters.AddWithValue("@Status", "Active");
                cmd.Parameters.AddWithValue("@dob", "01-01-2001");
                cmd.Parameters.AddWithValue("@seKey", secretKey);

                cmd.ExecuteNonQuery();
                CloseConnection();
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw new Exception("Error on Registering Hospital Authority" + ex.Message);
            }
        }

        public void UpdateAuthorityAccount(string name, string department, string status, string id)
        {
            try
            {
                OpenConnection();
                cmd = new SqlCommand("", myConnection);
                cmd.CommandText = @"UPDATE [dbo].[User] SET [Name] = @Name, [Department] = @Department, [Status] = @Status WHERE [UserID] = @ID;";
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Department", department);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@ID", id);

                cmd.ExecuteNonQuery();
                CloseConnection();
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw new Exception("Error on update authority account" + ex.Message);
            }
        }

        //Validate User Password - update password
        public int validatePass(string _password, int _currentid)
       {
            try
            {
                int passwordexist = 0;

                OpenConnection();
                cmd = new SqlCommand("", myConnection);
                cmd.CommandText = @"SELECT Password FROM [dbo].[User] WHERE UserID = @userid ";
                cmd.Parameters.AddWithValue("@userid", _currentid);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string currentpass = authCon.Decrypt(reader["Password"].ToString());

                        if(currentpass == _password)
                        {
                            passwordexist = 1;
                        }
                        else
                        {
                            passwordexist = 0;
                        }
                    }
               
                }
                else
                {
                    passwordexist = 0;
                }
                CloseConnection();
                return passwordexist;
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw new Exception("Error on validating current password " + ex.Message);
            }
        }

        public void UpdateHosPassword(int userid, string password)
        {
            try
            {
                OpenConnection();
                cmd = new SqlCommand("", myConnection);
                cmd.CommandText = @"UPDATE [dbo].[User] SET [Password] = @Pass WHERE [UserID] = @ID;";
                cmd.Parameters.AddWithValue("@Pass", authCon.Encrypt(password));
                cmd.Parameters.AddWithValue("@ID", userid);
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw new Exception("Error on update authority account" + ex.Message);
            }
          

            cmd.ExecuteNonQuery();
            CloseConnection();
        }

        public int validateAdminPass(string _password, string _currentid)
        {
            try
            {
                int passwordexist = 0;

                OpenConnection();
                cmd = new SqlCommand("", myConnection);
                cmd.CommandText = @"SELECT Password FROM [dbo].[User] WHERE UserID = @userid ";
                cmd.Parameters.AddWithValue("@userid", _currentid);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string currentpass = authCon.Decrypt(reader["Password"].ToString());

                        if (currentpass == _password)
                        {
                            passwordexist = 1;
                        }
                        else
                        {
                            passwordexist = 0;
                        }
                    }

                }
                else
                {
                    passwordexist = 0;
                }
                CloseConnection();
                return passwordexist;
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw new Exception("Error on validating current password " + ex.Message);
            }
        }

        public int validateSK(int _sk, int _currentid)
        {
            try
            {
                int skexist = 0;

                OpenConnection();
                cmd = new SqlCommand("", myConnection);
                cmd.CommandText = @"SELECT seKey FROM [dbo].[User] WHERE UserID = @userid ";
                cmd.Parameters.AddWithValue("@userid", _currentid);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int currentsk = Convert.ToInt32(reader["seKey"]);
                        if (currentsk == _sk)
                        {
                            skexist = 1;
                        }
                        else
                        {
                            skexist = 0;
                        }
                    }

                }
                else
                {
                    skexist = 0;
                }
                CloseConnection();
                return skexist;
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw new Exception("Error on validating secret key" + ex.Message);
            }
        }


        public int CountPartner()
        {
            try
            {
                OpenConnection();
                cmd = new SqlCommand("", myConnection);
                cmd.CommandText = @"SELECT COUNT(DISTINCT Name) FROM [dbo].[User] WHERE Role = 'Hospital Authority';";
                int partnercount = (int)cmd.ExecuteScalar();
                CloseConnection();

                return partnercount;
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw new Exception("Error on Getting Authority Listing" + ex.Message);
            }
        }
        /*-----------------------------------------------BLOOD LEVEL MODULE------------------------------------------ */

        public List<BloodLevel> GetBloodLevel(string _currentname)
        {
            try
            {
                
                OpenConnection();

                cmd = new SqlCommand("", myConnection);
                cmd.CommandText = @"SELECT BloodLvlID,a.Name, b.UserID, DateCollected, BloodStockQty, Department FROM [BloodLevel] AS b, [User] AS a WHERE a.UserID = b.UserID AND a.Name = @hosname;";
                cmd.Parameters.AddWithValue("@hosname", _currentname);
                SqlDataReader reader = cmd.ExecuteReader();

                List<BloodLevel> bloodLvlObjList = new List<BloodLevel>();

                if (reader.HasRows)
                {
                    BloodLevel bloodLvlData;

                    while (reader.Read())
                    {
                        bloodLvlData = new BloodLevel();
                        bloodLvlData.bloodlvlid = reader["BloodLvlID"].ToString();
                        bloodLvlData.userid = reader["UserID"].ToString();
                        bloodLvlData.bloodqty = (int)reader["BloodStockQty"];
                        bloodLvlData.datecollected = Convert.ToDateTime(reader["DateCollected"]).ToString("MMMM dd, yyyy");
                        bloodLvlData.department = reader["Department"].ToString();

                        bloodLvlObjList.Add(bloodLvlData);
                    }
                }
                CloseConnection();
                return bloodLvlObjList;

            }
            catch (Exception ex)
            {
                CloseConnection();
                throw new Exception("Error on loading blood level listing..." + ex.Message);
            }
        }

        public void UpdateBloodLevel(string bloodlvldate, int bloodqty, string userid)
        {

            try
            {
                OpenConnection();

                cmd = new SqlCommand("", myConnection);
                cmd.CommandText = @"INSERT INTO [dbo].[BloodLevel] (UserID, DateCollected, BloodStockQty) VALUES (@AuthorityID, @DateCollected, @StockQty)";
                cmd.Parameters.AddWithValue("@AuthorityID", userid);
                cmd.Parameters.AddWithValue("@DateCollected", bloodlvldate);
                cmd.Parameters.AddWithValue("@StockQty", bloodqty);

                cmd.ExecuteNonQuery();
                CloseConnection();
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw new Exception("Error on updating blood level" + ex.Message);
            }
        }

        //Blood Level - HomePage
        public List<BloodLevel> GetBloodLevelRecord()
        {
            try
            {
                OpenConnection();

                cmd = new SqlCommand("", myConnection);
                cmd.CommandText = @"SELECT [User].Name, BloodStockQty FROM [User], [BloodLevel] t
                                    INNER JOIN(SELECT [UserID], MAX([DateCollected]) AS MaxDate
                                    FROM [dbo].[BloodLevel] GROUP BY UserID) tm ON t.UserID = tm.UserID AND t.DateCollected = tm.MaxDate
                                    WHERE[User].UserID = t.UserID";
                SqlDataReader reader = cmd.ExecuteReader();

                List<BloodLevel> bloodLvlRecObjList = new List<BloodLevel>();

                if (reader.HasRows)
                {
                    BloodLevel bloodLvlRec;

                    while (reader.Read())
                    {
                        bloodLvlRec = new BloodLevel();
                        bloodLvlRec.hosname = reader["Name"].ToString();
                        bloodLvlRec.bloodqty = (int)reader["BloodStockQty"];

                        bloodLvlRecObjList.Add(bloodLvlRec);
                    }
                }
                CloseConnection();
                return bloodLvlRecObjList;

            }
            catch (Exception ex)
            {
                CloseConnection();
                throw new Exception("Error on loading blood level listing..." + ex.Message);
            }
        }

        /*-----------------------------------------------BLOOD QUALITY MODULE------------------------------------------ */
        public int ValidateAptId(int _aptid)
        {
            try
            {
                OpenConnection();
                cmd = new SqlCommand("", myConnection);
                cmd.CommandText = @"SELECT * FROM [dbo].[Appointment] WHERE apptId = @aptid AND status = 'Completed' AND Result IS NULL";
                cmd.Parameters.AddWithValue("@aptid", _aptid);
                SqlDataReader reader = cmd.ExecuteReader();

                int AptIDValid = 1;

                if (reader.HasRows)
                {
                    AptIDValid = 1;
                }
                else
                {
                    AptIDValid = 0;
                }

                CloseConnection();
                return AptIDValid;
            }
            catch(Exception ex)
            {
                CloseConnection();
                throw new Exception("Error on Validating Appointment ID " + ex.Message);
            }
        }

        public void UpdateAptResult(int _aptid)
        {
            try
            {
                OpenConnection();
                cmd = new SqlCommand("", myConnection);
                cmd.CommandText = @"UPDATE [Appointment] SET[Result] = @result WHERE apptId = @apptid;";
                cmd.Parameters.AddWithValue("@result", "Updated");
                cmd.Parameters.AddWithValue("@apptid", _aptid);
                cmd.ExecuteNonQuery();
                CloseConnection();
            }
            catch(Exception ex)
            {
                CloseConnection();
                throw new Exception("Error on Saving Blood Result Status to Appointment DB..." + ex.Message);
            }
        }

        /*-----------------------------------------------FEEDBACKS------------------------------------------ */
        public List<Feedback> GetFeedbacks()
        {
            try
            {
                OpenConnection();
                cmd = new SqlCommand("", myConnection);
                cmd.CommandText = @"SELECT * FROM [Feedback];";
                SqlDataReader reader = cmd.ExecuteReader();

                List<Feedback> fbObjList = new List<Feedback>();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Feedback fbData = new Feedback();
                        fbData.firstname = reader["FirstName"].ToString();
                        fbData.lastname = reader["LastName"].ToString();
                        fbData.email = reader["Email"].ToString();
                        fbData.message = reader["Message"].ToString();
                        fbData.date = Convert.ToDateTime(reader["Date"]).ToString("MMMM dd, yyyy");

                        fbObjList.Add(fbData);
                    }
                }

                CloseConnection();
                return fbObjList;
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw new Exception("Error on Getting Authority Listing" + ex.Message);
            }
        }
        public void SaveFeedbacks(string firstname, string lastname, string email, string message)
        {
            try
            {
                OpenConnection();
                cmd = new SqlCommand("", myConnection);
                cmd.CommandText = @"INSERT INTO [Feedback] (FirstName, LastName, Email, Message, Date) VALUES (@FirstName, @LastName, @Email, @Message, @Date);";
                cmd.Parameters.AddWithValue("@FirstName", firstname);
                cmd.Parameters.AddWithValue("@LastName", lastname);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Message", message);
                cmd.Parameters.AddWithValue("@Date", DateTime.Now.ToString("MM/dd/yyyy"));

                cmd.ExecuteNonQuery();
                CloseConnection();
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw new Exception("Error on Saving Feedbacks..." + ex.Message);
            }
        }
    }
}