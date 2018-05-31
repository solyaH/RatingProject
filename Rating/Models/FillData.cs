using CsQuery;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rating.Models
{
    public class FillData
    {
        public FillData()
        {
            string facultyListUrl = @"http://www.lnu.edu.ua/about/faculties/";
            string collegeListUrl = @"http://www.lnu.edu.ua/about/colleges/";

            Dictionary<string, string> facultyCollegeUrlDict = GetFacultyCollegeUrlDict(RequestGetter.GetRequestByUrl(facultyListUrl));
            foreach (var kvp in GetFacultyCollegeUrlDict(RequestGetter.GetRequestByUrl(collegeListUrl)))
                facultyCollegeUrlDict.Add(kvp.Key, kvp.Value);

            facultyCollegeUrlDict.Remove("економічний факультет");
            facultyCollegeUrlDict.Remove("біологічний факультет");
            facultyCollegeUrlDict.Remove("факультет електроніки та комп’ютерних технологій");
            facultyCollegeUrlDict.Remove("механіко-математичний факультет");
            facultyCollegeUrlDict.Remove("факультет управління фінансами та бізнесу");

            // Dictionary<string, List<string>> facultyStaffUrlDict = GetFacultyStaffUrlDict(GetRequestByUrl(url));
            //Dictionary<string, string> departmentUrlDict = GetDepartmnetUrlDict(GetRequestByUrl(url1));

            FillStaffInfoDict(facultyCollegeUrlDict);
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

        public void FillStaffInfoDict(Dictionary<string, string> facultyCollegeUrlDict)
        {
            //string facultyCollegeUrl = "http://intrel.lnu.edu.ua";
            ////http://ami.lnu.edu.ua/about/staff
            //< provider invariantName = "System.Data.SqlClient" type = "System.Data.Entity.SqlServer.SqlProviderServices, EntityFramework.SqlServer" />
            //< add name = "IndividualContext" providerName = "Npgsql" connectionString = "Server=127.0.0.1;User Id=postgres;Password=mydell2017;Port=5432;Database=Rating;" />

            string connStr = "Host = localhost; Username = postgres; Password = mydell2017; Database = Rating; ";
            string LastFaculty = "";
            string LastDepartment = "";
            //string LastLink = "";

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
                        //LastLink = rdr.GetString(2);
                    }
                }
                //}
            }

            int startFacultyIndex = facultyCollegeUrlDict.Keys.ToList().IndexOf(LastFaculty);

            foreach (string facultyCollegeUrl in facultyCollegeUrlDict.Values.Skip(startFacultyIndex))
            {
                string requestUrl = (facultyCollegeUrl + "/about/staff");
                Dictionary<string, List<string>> facultyStaffUrl = GetFacultyStaffUrlDict(RequestGetter.GetRequestByUrl(requestUrl));
                int startStaffIndex = facultyStaffUrl.Keys.ToList().IndexOf("кафедра " + LastDepartment);
                if (startStaffIndex == -1)
                {
                    startStaffIndex = facultyStaffUrl.Keys.ToList().IndexOf(LastDepartment.Replace("лабораторії ", ""));
                }
                if (startStaffIndex == -1)
                {
                    startStaffIndex = facultyStaffUrl.Keys.ToList().IndexOf(LastDepartment.Replace("лабораторії", "лабораторія"));
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
                            departmentStaff.Add(newProfessor);
                            db.Professors.Add(newProfessor);
                        }

                        //facultyStaffDict.Add(department.Key, departmentStaff);//title-get department title
                        db.SaveChanges();
                    }
                }
            }
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