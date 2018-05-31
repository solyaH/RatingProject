using CsQuery;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Rating.Models
{
    public class FillData
    {
        bool startNewDB;
        string connStr = "Host = localhost; Username = postgres; Password = password; Database = Rating; ";
        Dictionary<string, string> facultyCollegeUrlDict;

        public FillData(bool startNewDB = false)
        {
            this.startNewDB = startNewDB;

            string facultyListUrl = @"http://www.lnu.edu.ua/about/faculties/";
            string collegeListUrl = @"http://www.lnu.edu.ua/about/colleges/";

            facultyCollegeUrlDict = GetFacultyCollegeUrlDict(RequestGetter.GetRequestByUrl(facultyListUrl));
            foreach (var kvp in GetFacultyCollegeUrlDict(RequestGetter.GetRequestByUrl(collegeListUrl)))
                facultyCollegeUrlDict.Add(kvp.Key, kvp.Value);

            facultyCollegeUrlDict.Remove("економічний факультет");
            facultyCollegeUrlDict.Remove("біологічний факультет");
            facultyCollegeUrlDict.Remove("факультет електроніки та комп’ютерних технологій");
            facultyCollegeUrlDict.Remove("механіко-математичний факультет");
            facultyCollegeUrlDict.Remove("факультет управління фінансами та бізнесу");

            // Dictionary<string, List<string>> facultyStaffUrlDict = GetFacultyStaffUrlDict(GetRequestByUrl(url));
            //Dictionary<string, string> departmentUrlDict = GetDepartmnetUrlDict(GetRequestByUrl(url1));

            //FillDepartmentsInfoDict(facultyCollegeUrlDict);
        }

        public Dictionary<string, string> GetFacultyCollegeUrlDict(string Facultyhtml)
        {
            Dictionary<string, string> facultyCollegeUrl = new Dictionary<string, string>();
            CQ cq = CQ.Create(Facultyhtml);

            foreach (IDomObject facultyBlock in cq.Find("div"))
            {
                if (facultyBlock.GetAttribute("class") == "thumb")
                {
                    cq = CQ.Create(facultyBlock);
                    foreach (IDomObject facultyCollegeLink in cq.Find("a"))
                    {
                        facultyCollegeUrl.Add(cq.Find("img")[0].GetAttribute("alt").ToLower(), EditFacultyCollegeUrl(facultyCollegeLink.GetAttribute("href")));
                    }
                }
            }
            return facultyCollegeUrl;
        }

        public string EditFacultyCollegeUrl(string url)
        {
            if (url.EndsWith("/"))
                return url.Substring(0, url.Length - 1); ;
            return url;
        }

        public Dictionary<string, string> GetDepartmnetUrlDict(string Facultyhtml)
        {
            Dictionary<string, string> departmentUrl = new Dictionary<string, string>();
            CQ cq = CQ.Create(Facultyhtml);

            foreach (IDomObject departmentBlock in cq.Find("h2"))
            {
                if (departmentBlock.GetAttribute("class") == "title")
                {
                    cq = CQ.Create(departmentBlock);
                    foreach (IDomObject departmentLink in cq.Find("a"))
                    {
                        departmentUrl.Add(departmentLink.Cq().Text().ToLower(), departmentLink.GetAttribute("href"));
                    }
                }
            }
            return departmentUrl;
        }

        public Dictionary<string, List<string>> GetFacultyStaffUrlDict(string Facultyhtml)//
        {
            Dictionary<string, List<string>> facultyStaffUrl = new Dictionary<string, List<string>>();
            CQ cq = CQ.Create(Facultyhtml);

            foreach (IDomObject section in cq.Find("section"))
            {
                cq = CQ.Create(section);
                if ((cq.Find("h2").Find("a")).Length == 0)
                {
                    continue;
                }
                string departmentName = (cq.Find("h2").Find("a")[0]).Cq().Text();

                foreach (IDomObject tables in cq.Find("table"))
                {
                    List<string> department = new List<string>();
                    cq = CQ.Create(tables);

                    foreach (IDomObject tableLine in cq.Find("td"))
                    {
                        if (tableLine.GetAttribute("class") == "name")
                        {
                            cq = CQ.Create(tableLine);
                            foreach (IDomObject staffLink in cq.Find("a"))
                            {
                                department.Add(staffLink.GetAttribute("href"));
                            }
                        }
                    }
                    facultyStaffUrl.Add(departmentName.ToLower(), department);
                }
            }
            return facultyStaffUrl;
        }

        public void FillGapsInBD()
        {
            List<string> linksToFill = new List<string>();
            using (var con = new NpgsqlConnection(connStr))
            {
                con.Open();
                NpgsqlCommand comd = con.CreateCommand();
                //comd.CommandText = "SELECT to_regclass('dbo.\"Professors\"');";
                //bool exist = comd.ExecuteScalar() == null ? true : false;
                //if (exist)
                //{
                NpgsqlCommand cmd = con.CreateCommand();

                cmd.CommandText = "Select link from dbo.\"Professors\" where pib =''";
                using (NpgsqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        linksToFill.Add(rdr.GetString(0));
                    }
                }
            }

            //using (var db = new IndividualContext())
            //{
            //    foreach (string lecturerUrl in linksToFill)
            //    {
            //        Professor newProfessor = new Professor();
            //        newProfessor.FillProfessorInfo(RequestGetter.GetRequestByUrl(lecturerUrl));
            //        newProfessor.link = lecturerUrl;
            //        db.Professors.Add(newProfessor);
            //        db.SaveChanges();
            //    }
            //}
        }


        public void FillStaffBD()
        {
            string LastFaculty = "";
            string LastDepartment = "";
            int startFacultyIndex = 0;

            if (!startNewDB)
            {
                using (var con = new NpgsqlConnection(connStr))
                {
                    con.Open();
                    NpgsqlCommand comd = con.CreateCommand();
                    //comd.CommandText = "SELECT to_regclass('dbo.\"Professors\"');";
                    //bool exist = comd.ExecuteScalar() == null ? true : false;
                    //if (exist)
                    //{
                    NpgsqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "Select department,faculty from dbo.\"Professors\" order by id desc limit 1";
                    using (NpgsqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            LastDepartment = rdr.GetString(0).ToLower();
                            LastFaculty = rdr.GetString(1).ToLower();
                        }
                    }
                    //}
                }
                startFacultyIndex = facultyCollegeUrlDict.Keys.ToList().IndexOf(LastFaculty);
            }
            //else{
            //    using (var con = new NpgsqlConnection(connStr))
            //    {
            //        con.Open();
            //        NpgsqlCommand comd = con.CreateCommand();

            //        NpgsqlCommand cmd = con.CreateCommand();
            //        cmd.CommandText = "delete from dbo.\"Professors\"";
            //        cmd.ExecuteNonQuery();
            //    }
            //}


            bool restart;
            do
            {
                restart = false;
                try
                {
                    foreach (string facultyCollegeUrl in facultyCollegeUrlDict.Values.Skip(startFacultyIndex))
                    {
                        string requestUrl = (facultyCollegeUrl + "/about/staff");
                        Dictionary<string, List<string>> facultyStaffUrl = GetFacultyStaffUrlDict(RequestGetter.GetRequestByUrl(requestUrl));
                        int startStaffIndex = -1;
                        if (!startNewDB)
                        {
                            startStaffIndex = facultyStaffUrl.Keys.ToList().IndexOf("кафедра " + LastDepartment);
                            if (startStaffIndex == -1)
                            {
                                startStaffIndex = facultyStaffUrl.Keys.ToList().IndexOf(LastDepartment.Replace("лабораторії ", ""));
                            }
                            if (startStaffIndex == -1)
                            {
                                startStaffIndex = facultyStaffUrl.Keys.ToList().IndexOf(LastDepartment.Replace("лабораторії", "лабораторія"));
                            }
                        }


                        using (var db = new IndividualContext())
                        {
                            foreach (KeyValuePair<string, List<string>> department in facultyStaffUrl.Skip(startStaffIndex + 1))
                            {
                                List<Professor> departmentStaff = new List<Professor>();
                                foreach (string lecturerUrl in department.Value)
                                {
                                    Professor newProfessor = new Professor();
                                    newProfessor.FillProfessorInfo(RequestGetter.GetRequestByUrl(lecturerUrl));
                                    newProfessor.link = lecturerUrl;
                                    //departmentStaff.Add(newProfessor);
                                    db.Professors.Add(newProfessor);
                                }

                                //facultyStaffDict.Add(department.Key, departmentStaff);//title-get department title
                                db.SaveChanges();
                            }
                        }
                    }
                }
                catch (System.Net.WebException)
                {
                    restart = true;
                }
            } while (restart);
        }

        public void FillDepartmentsInfoDict(Dictionary<string, string> facultyCollegeUrlDict)
        {
            foreach (string facultyCollegeUrl in facultyCollegeUrlDict.Values)
            {
                string requestUrl = (facultyCollegeUrl + "/about/departments");
                if (facultyCollegeUrl == "http://natcollege.lnu.edu.ua")
                    requestUrl = (facultyCollegeUrl + "/about/labs");
                Dictionary<string, string> departmentUrlDict = GetDepartmnetUrlDict(RequestGetter.GetRequestByUrl(requestUrl));
                Dictionary<string, Department> departmentDict = new Dictionary<string, Department>();
                foreach (KeyValuePair<string, string> departmentUrl in departmentUrlDict)
                {
                    Department newDepartment = new Department();
                    newDepartment.FillDepartmentInfo(RequestGetter.GetRequestByUrl(departmentUrl.Value));
                    departmentDict.Add(departmentUrl.Key, newDepartment);
                }
            }
        }
    }
}