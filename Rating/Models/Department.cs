using CsQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rating.Models
{
    public class Department
    {
        // About department
        public string Contacts { get; set; }
        public string Photo { get; set; }
        public string HistorySpecialtiesAchievement { get; set; }

        // Employees
        public string Employees { get; set; }

        //Teaching
        public string TeachingCourseMaterials { get; set; }
        //public string SubjectsListOfCourses { get; set; }
        //public string CourseMaterials { get; set; }

        //ScientificActivityOfTheDepartment
        public string ScientificActivityOfTheDepartment { get; set; }
        //public string ScientificActivityOfTheDepartment { get; set; }
        //public string Research { get; set; }
        //public string ScientificSeminarsConferencesRoundTables { get; set; }
        //public string Collaboration { get; set; }

        //Publications
        public string PublicationsLibraryMethodicalMaterials { get; set; }

        //Schedule
        public string Schedule { get; set; }

        //News
        public List<string> News { get; set; }

        public void FillDepartmentInfo(string html)
        {
            Contacts = GetDepartmentModelForContacts(html);
            Photo= GetDepartmentModelForPhoto(html);
            //HistorySpecialtiesAchievement= GetDepartmentModelForHistorySpecialtiesAchievement(html);
            Employees=GetDepartmentModelForEmployees(html);
            TeachingCourseMaterials = GetDepartmentModelForTeachingCourseMaterials(html);
            PublicationsLibraryMethodicalMaterials = GetDepartmentModelForPublicationsLibraryMethodicalMaterials(html);
            Schedule = GetDepartmentModelForSchedule(html);
            News=GetDepartmentModelForNews(html);
        }

        private string GetDepartmentModelForContacts(string html)
        {

            string contacts = "";
            CQ cq = CQ.Create(html);

            foreach (IDomObject sections in cq.Find("section"))
            {
                if (sections.GetAttribute("class") == "about visible")
                {
                    cq = CQ.Create(sections);
                    foreach (IDomObject divContact in cq.Find("div"))
                    {
                        if (divContact.GetAttribute("class") == "box")
                        {
                            cq = CQ.Create(divContact);

                            foreach (IDomObject aContact in cq.Find("a"))
                            {

                                contacts += aContact.GetAttribute("href") + "\n";
                            }
                        }


                    }
                }
            }
            return contacts;
        }
        private string GetDepartmentModelForPhoto(string html)
        {
            string photo = "";
            CQ cq = CQ.Create(html);

            foreach (IDomObject sections in cq.Find("section"))
            {
                if (sections.GetAttribute("class") == "about visible")
                {
                    cq = CQ.Create(sections);
                    foreach (IDomObject aPhoto in cq.Find("img"))
                    {
                        photo = aPhoto.GetAttribute("src");
                    }

                }
            }
            return photo;
        }
        private string GetDepartmentModelForHistorySpecialtiesAchievement(string html)
        {
            string historySpecialtiesAchievement = "";

            CQ cq = CQ.Create(html);

            foreach (IDomObject sections in cq.Find("section"))
            {
                if (sections.GetAttribute("class") == "about visible")
                {
                    cq = CQ.Create(sections);
                    historySpecialtiesAchievement = (cq.Find("p")[4]).Cq().Text();
                    //foreach (IDomObject history in cq.Find("p"))
                    //{
                    //    historySpecialtiesAchievement = history.Cq().Text()+"\n";
                    //}
                }
            }
            return historySpecialtiesAchievement;
        }
        private string GetDepartmentModelForEmployees(string html)
        {
            string employees = "";
            CQ cq = CQ.Create(html);

            foreach (IDomObject staff in cq.Find("section"))
            {
                if (staff.GetAttribute("class") == "staff")
                {
                    cq = CQ.Create(staff);
                    foreach (IDomObject aStaff in cq.Find("a"))
                    {
                        employees += aStaff.GetAttribute("href") + "\n";
                    }

                }
            }
            return employees;
        }
        private string GetDepartmentModelForTeachingCourseMaterials(string html)
        {
            string courses = "";
            CQ cq = CQ.Create(html);

            foreach (IDomObject staff in cq.Find("section"))
            {
                if (staff.GetAttribute("class") == "courses")
                {
                    cq = CQ.Create(staff);
                    foreach (IDomObject aStaff in cq.Find("a"))
                    {
                        courses += aStaff.GetAttribute("href") + "\n";
                    }

                }
            }
            return courses;
        }
        //ScientificActivityOfTheDepartment ?
        private string GetDepartmentModelForPublicationsLibraryMethodicalMaterials(string html)
        {
            string publicationsMaterials = "";
            CQ cq = CQ.Create(html);

            foreach (IDomObject staff in cq.Find("section"))
            {
                if (staff.GetAttribute("class") == "materials")
                {
                    cq = CQ.Create(staff);
                    foreach (IDomObject aStaff in cq.Find("a"))
                    {
                        publicationsMaterials += aStaff.GetAttribute("href") + "\n";
                    }

                }
            }
            return publicationsMaterials;
        }
        private string GetDepartmentModelForSchedule(string html) // ???
        {
            string schedule = "";
            CQ cq = CQ.Create(html);

            foreach (IDomObject section in cq.Find("section"))
            {
                if (section.GetAttribute("class") == "schedule")
                {
                    cq = CQ.Create(section);
                    foreach (IDomObject buttonSchedule in cq.Find("button"))
                    {
                        schedule += buttonSchedule.GetAttribute("onclick") + "\n";
                    }

                }
            }
            return schedule;
        }
        private List<string> GetDepartmentModelForNews(string html)
        {
            List<string> news = new List<string>();
            CQ cq = CQ.Create(html);

            foreach (IDomObject section in cq.Find("section"))
            {
                if (section.GetAttribute("class") == "news")
                {
                    cq = CQ.Create(section);
                    foreach (IDomObject aNews in cq.Find("article"))
                    {
                        if (aNews.GetAttribute("class") == "post")
                        {
                            cq = CQ.Create(aNews);
                            foreach (IDomObject h3News in cq.Find("h3"))
                            {
                                cq = CQ.Create(h3News);
                                foreach (IDomObject News in cq.Find("a"))
                                {
                                    news.Add(News.GetAttribute("href"));
                                }
                            }
                            cq = CQ.Create(aNews);
                            foreach (IDomObject divNews in cq.Find("div"))
                            {
                                if (divNews.GetAttribute("class") == "meta")
                                {
                                    news.Add((cq.Find("div")[0]).Cq().Text());
                                }
                            }
                        }

                    }
                }
            }
            return news;
        }
    }
}