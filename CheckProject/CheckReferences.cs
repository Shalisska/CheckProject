using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace CheckProject
{
    public class CheckReferences
    {
        public Dictionary<string, List<string>> GetResult()
        {
            var pathToSolution = @"D:\projects\AgeOfClones\";
            var projectName = "AgeOfClones";

            List<string> references;
            var projectsToCheck = new List<string>
            {
                projectName
            };

            var dict = new Dictionary<string, List<string>>();

            while (projectName != null)
            {
                references = FindReferencesFromProject(pathToSolution, projectName);

                dict.Add(projectName, references);

                UpdateProjectsToCheck(projectsToCheck, dict, projectName, references);
                projectName = projectsToCheck.FirstOrDefault();
            }

            return dict;
        }

        private void UpdateProjectsToCheck(List<string> projectsToCheck, Dictionary<string, List<string>> dict, string mainProject, List<string> references)
        {
            projectsToCheck.Remove(mainProject);

            if (references == null || references.Count == 0)
                return;

            foreach (var reference in references)
            {
                if (!dict.ContainsKey(reference) && !projectsToCheck.Contains(reference))
                    projectsToCheck.Add(reference);
            }
        }

        private List<string> FindReferencesFromProject(string pathToSolution, string projectName)
        {
            var fullPath = $"{pathToSolution}{projectName}\\{projectName}.csproj";

            try
            {
                XDocument xdoc = XDocument.Load(fullPath);

                var proj = xdoc.Elements().Where(e => e.Name.LocalName == "Project");
                var iGroups = proj.Elements().Where(e => e.Name.LocalName == "ItemGroup");
                var refs = iGroups.Elements().Where(e => e.Name.LocalName == "ProjectReference");

                var references = refs.Select(r => GetProjectName(r.Attribute("Include").Value)).ToList();
                return references;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private string GetProjectName(string path)
        {
            var pattern = @"([\w|.]*).csproj";
            var res = Regex.Match(path, pattern).Groups[1];
            return res.Value;
        }
    }
}
