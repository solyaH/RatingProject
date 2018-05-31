using CsQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rating.Models
{
    public class Professor
    {
        public int id { get; set; }
        public string link { get; set; }
        public string pib { get; set; }
        public string position { get; set; }
        public string faculty { get; set; }
        public string department { get; set; }
        public string scientific_degree { get; set; }
        public string academic_status { get; set; }
        //public  string other_status { get; set; }
        public string email { get; set; }
        public string contacts { get; set; }
        public string biography { get; set; }
        public string interests { get; set; }
        public List<string> course_description { get; set; }
        public List<string> course_materials { get; set; }
        public List<string> course_curriculum { get; set; }
        public List<string> publications { get; set; }
        public List<string> publications_references { get; set; }
        public string schedule { get; set; }

        #region GetProffesorInfo
        private string GetProfessorsName(string html)
        {
            string name = "";

            CQ cq = CQ.Create(html);

            foreach (IDomObject block in cq.Find("h1"))
            {
                if (block.GetAttribute("class") == "page-title")

                { name = block.Cq().Text().ToString(); }
            }
            return name;
        }
        private string GetProfessorsFaculty(string html)
        {
            string professorsFaculty = "";
            CQ cq = CQ.Create(html);
            foreach (IDomObject block in cq.Find("meta"))
            {
                if (block.GetAttribute("property") == "og:site_name")
                {
                    professorsFaculty = block.GetAttribute("content");
                }
            }
            return professorsFaculty;

        }
        private string GetProfessorsDepartment(string html)
        {
            string department = "";
            CQ cq = CQ.Create(html);
            foreach (IDomObject block in cq.Find("div"))
            {
                if (block.GetAttribute("class") == "info")
                {
                    foreach (IDomObject line in block.Cq().Find("p"))
                    {
                        if (line.Cq().Text().Contains("Посада"))
                        { department = line.FirstChild.NextElementSibling.FirstElementChild.FirstChild.Cq().Text(); break; }

                    }
                }
            }

            if (department.Contains("кафедри")) { department = department.Remove(0, 8); }

            return department;
        }
        private string GetProfessorsPosition(string html)
        {
            string position = "";

            CQ cq = CQ.Create(html);
            foreach (IDomObject block in cq.Find("div"))
            {
                if (block.GetAttribute("class") == "info")
                {
                    foreach (IDomObject line in cq.Find("p"))
                    {
                        if (line.Cq().Text().Contains("Посада"))
                        { position = line.FirstChild.NextElementSibling.FirstChild.Cq().Text(); break; }

                    }
                }
            }
            return position;
        }
        private string GetProfessorsScientificDegree(string html)
        {
            string scientific_degree = "";
            CQ cq = CQ.Create(html);
            foreach (IDomObject block in cq.Find("div"))
            {
                if (block.GetAttribute("class") == "info")
                {
                    foreach (IDomObject line in cq.Find("p"))
                    {
                        if (line.Cq().Text().Contains("Науковий ступінь"))
                        {
                            scientific_degree = line.FirstChild.NextElementSibling.FirstChild.ToString();
                            break;
                        }

                    }
                }
            }
            return scientific_degree;
        }
        private string GetProfessorsAcademicStatus(string html)
        {
            string academic_status = "";
            CQ cq = CQ.Create(html);
            foreach (IDomObject block in cq.Find("div"))
            {
                if (block.GetAttribute("class") == "info")
                {
                    foreach (IDomObject line in cq.Find("p"))
                    {
                        if (line.Cq().Text().Contains("Вчене звання"))
                        {
                            academic_status = line.Cq().Text(); break;
                        }

                    }
                }
            }
            academic_status = academic_status.Replace("Вчене звання :", "");

            return academic_status;
        }
        private string GetProfessorsEmail(string html)
        {
            string email = "";
            CQ cq = CQ.Create(html);
            foreach (IDomObject block in cq.Find("div"))
            {
                if (block.GetAttribute("class") == "info")
                {
                    foreach (IDomObject line in cq.Find("p"))
                    {
                        if (line.Cq().Text().Contains("Електронна пошта"))
                        { email = line.LastChild.Cq().Text(); break; }

                    }
                }
            }
            return email;
        }
        private string GetProfessorsContacts(string html)
        {
            string contacts = "";
            CQ cq = CQ.Create(html);
            foreach (IDomObject block in cq.Find("div"))
            {
                if (block.GetAttribute("class") == "info")
                {
                    foreach (IDomObject line in cq.Find("p"))
                    {
                        if (line.Cq().Text().Contains("Телефон (робочий)"))
                        { contacts = line.LastChild.Cq().Text(); break; }

                    }
                }
            }
            return contacts;
        }
        private string GetProfessorsInterests(string html)
        {
            string interests = "";

            CQ cq = CQ.Create(html);
            foreach (IDomObject block in cq.Find("section"))
            {
                if (block.GetAttribute("class") == "section")
                {
                    if (block.Cq().Text().Contains("Наукові інтереси"))
                    { interests = block.LastElementChild.Cq().Text(); }
                }
            }

            return interests;
        }
        private string GetProfessorsBiography(string html)
        {
            string biography = "";

            CQ cq = CQ.Create(html);
            foreach (IDomObject block in cq.Find("section"))
            {
                if (block.GetAttribute("class") == "section")
                {
                    if (block.Cq().Text().Contains("Біографія"))
                    { biography = block.LastElementChild.Cq().Text(); }
                }
            }

            return biography;
        }
        private string GetProfessorsSchedule(string html)
        {
            List<string> myListOfScheduleHrefs = new List<string>();
            string schedule = "";
            CQ cq = CQ.Create(html);

            foreach (IDomObject block in cq.Find("section"))
            {

                cq = CQ.Create(block);

                foreach (IDomObject href in block.Cq().Find("h2"))
                {
                    if (href.Cq().Text().Contains("Розклад"))
                    {
                        foreach (IDomObject url in block.Cq().Find("a"))
                        {
                            myListOfScheduleHrefs.Add(url.GetAttribute("href"));
                        }
                    }
                }

            }

            if (myListOfScheduleHrefs.Count > 0)
            { schedule = "розклад є на сторінці"; }
            return schedule;
        }
        #endregion

        private List<string> GetProfessorsCourseHrefs(string html)
        {
            List<string> courseHref = new List<string>();
            CQ cq = CQ.Create(html);
            foreach (IDomObject block in cq.Find("section"))
            {
                if (block.GetAttribute("class") == "section")
                {
                    if (block.Cq().Text().Contains("Курси"))
                    {
                        foreach (IDomObject line in block.Cq().Find("a"))
                        {
                            courseHref.Add(line.GetAttribute("href"));
                        }
                    }
                }
            }
            List<string> courseHrefs = new List<string>();
            foreach (var item in courseHref) {
                if ((item.Contains("http"))&& !(item.Contains("pdf"))) courseHrefs.Add(item);


                        }
            return courseHrefs;
        }
        private List<string> GetProfessorsCourseDescription(string html)
        {
            List<string> course_desription = new List<string>();
            List<string> course_hrefs = GetProfessorsCourseHrefs(html);
            List<string> descriptive_pages = new List<string>();
            List<CQ> cqList = new List<CQ>();

            foreach (var href in course_hrefs)
            {

                cqList.Add(CQ.Create(RequestGetter.GetRequestByUrl(href)));
            }
            foreach (CQ cq in cqList)
            {
                foreach (IDomObject page in cq.Find("section"))
                {
                    if (page.GetAttribute("class") == "description")
                    {
                        if (page.Cq().Text().Contains("Опис курсу"))
                        {
                            course_desription.Add(page.Cq().Text());
                        }
                    }
                }

            }
            return course_desription;
        }
        private List<string> GetProfessorsCourseMaterials(string html)
        {
            List<string> course_materials = new List<string>();
            List<string> course_hrefs = GetProfessorsCourseHrefs(html);

            List<CQ> cqList = new List<CQ>();

            foreach (var href in course_hrefs)
            {

                cqList.Add(CQ.Create(RequestGetter.GetRequestByUrl(href)));
            }
            foreach (CQ cq in cqList)
            {
                foreach (IDomObject page in cq.Find("section"))
                {
                    if (page.GetAttribute("class") == "documents")
                    {

                        course_materials.Add(page.Cq().Text());

                    }
                }

            }
            return course_materials;
        }
        private List<string> GetProfessors_CourseCurriculum(string html)
        {
            List<string> curiculum = new List<string>();
            List<string> course_hrefs = GetProfessorsCourseHrefs(html);

            List<CQ> cqList = new List<CQ>();

            foreach (var href in course_hrefs)
            {

                cqList.Add(CQ.Create(RequestGetter.GetRequestByUrl(href)));
            }
            foreach (CQ cq in cqList)
            {
                foreach (IDomObject page in cq.Find("article"))
                {
                    if (page.GetAttribute("class") == "content course")
                    {

                        curiculum.Add(page.Cq().Text());

                    }
                }

            }
            return curiculum;
        }
        private List<string> GetProfessorsPublications(string html)
        {
            List<string> publications = new List<string>();
            CQ cq = CQ.Create(html);
            foreach (IDomObject block in cq.Find("section"))
            {
                if (block.GetAttribute("class") == "section")
                {
                    if ((block.Cq().Text().Contains("Публікації")) || (block.Cq().Text().Contains("публікації")))
                    {
                        foreach (IDomObject line in block.Cq().Find("div"))
                        {
                            publications.Add(line.Cq().Text());
                        }
                    }
                }
            }
            return publications;
        }
        private List<string> GetProfessorsHrefsToPublications(string html)
        {
            List<string> hyperLinks = new List<string>();
            CQ cq = CQ.Create(html);
            foreach (IDomObject block in cq.Find("section"))
            {
                if (block.GetAttribute("class") == "section")
                {
                    if ((block.Cq().Text().Contains("Публікації")) || (block.Cq().Text().Contains("публікації")))
                    {
                        foreach (IDomObject line in block.Cq().Find("a"))
                        {
                            hyperLinks.Add(line.GetAttribute("href"));
                        }
                    }
                }
            }
            return hyperLinks;
        }
        public void FillProfessorInfo(string html)

        {
           pib = GetProfessorsName(html);
            position = GetProfessorsPosition(html);
           scientific_degree = GetProfessorsScientificDegree(html);
           academic_status = GetProfessorsAcademicStatus(html);
            email = GetProfessorsEmail(html);
            contacts = GetProfessorsContacts(html);
            interests = GetProfessorsInterests(html);
            biography = GetProfessorsBiography(html);
            //professor.course_description = GetProfessorsCourse(html);
            schedule = GetProfessorsSchedule(html);
            faculty = GetProfessorsFaculty(html);
            department = GetProfessorsDepartment(html);
            publications = GetProfessorsPublications(html);
            publications_references = GetProfessorsHrefsToPublications(html);
            course_description = GetProfessorsCourseDescription(html);
           course_materials = GetProfessorsCourseMaterials(html);
            course_curriculum = GetProfessors_CourseCurriculum(html);
        }
    }
}