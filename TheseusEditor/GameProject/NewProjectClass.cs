using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.IO;
using TheseusEditor.Utilities;

namespace TheseusEditor.GameProject
{
    [DataContract]
    public class ProjectTemplate
    {
        [DataMember]
        public string ProjectType { get; set; }
        [DataMember]
        public string ProjectFile { get; set; }
        [DataMember]
        public List<string> Folders { get; set; }

        public byte[] Icon { get; set; }
        public byte[] Screenshot { get; set; }
        public string IconFilePath { get; set; }
        public string ScreenshotFilePath { get; set; }
        public string ProjectFilePath { get; set; }

    }

    public class NewProjectClass : ViewModelBase
    {
        //TODO get the path from the instalation location
        private readonly string _templatePath = @"..\..\TheseusEditor\ProjectTemplates";
        private string _projectname = "New Project";
        public string ProjectName
        {
            get => _projectname;
            set
            {
                if (_projectname != value)
                {
                    _projectname = value;
                    ValidateProjectPath();
                    OnPropertyChanged(nameof(ProjectName));
                }
            }
        }

        private string _projectpath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\TheseusProjects\";
        public string ProjectPath
        {
            get => _projectpath;
            set
            {
                if (_projectpath != value)
                {
                    _projectpath = value;
                    ValidateProjectPath();
                    OnPropertyChanged(nameof(ProjectPath));
                }
            }
        }

        private ObservableCollection<ProjectTemplate> _projectTemplates = new ObservableCollection<ProjectTemplate>();
        public ReadOnlyObservableCollection<ProjectTemplate> ProjectTemplates { get; }

        private bool _isValid;
        public bool IsValid
        {
            get => _isValid;
            set
            {
                if(_isValid != value)
                {
                    _isValid = value;
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }
        private string _errorMsg;
        public string ErrorMsg
        {
            get => _errorMsg;
            set
            {
                if (value != _errorMsg)
                {
                    _errorMsg = value;
                    OnPropertyChanged(nameof(ErrorMsg));
                }
            }
        }

        private bool ValidateProjectPath()
        {
            var path = ProjectPath;
            path += $@"{ProjectName}/";

            IsValid = false;
            if (string.IsNullOrEmpty(ProjectName.Trim()))
            {
                ErrorMsg = "Type in a project name";
            }
            else if(ProjectName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
            {
                ErrorMsg = "Invaid Character(s) in project name";
            }
            else if (string.IsNullOrEmpty(ProjectPath.Trim()))
            {
                ErrorMsg = "Type in a project path";
            }
            else if (ProjectPath.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                ErrorMsg = "Invaid Character(s) in project path";
            }
            else if (Directory.Exists(path) && Directory.EnumerateFileSystemEntries(path).Any())
            {
                ErrorMsg = "Selected project path already exists and is not empty";
            }
            else
            {
                ErrorMsg = String.Empty;
                IsValid = true;
            }
            return IsValid;
        }

        public string CreateProject(ProjectTemplate template)
        {
            var path = ProjectPath;
            char c = '/';
            if (path[path.Length - 1] != c) path += c;
            path += $@"{ProjectName}/";
            try
            {
                if (Directory.Exists(path)) Directory.CreateDirectory(path);
                foreach (var folder in template.Folders)
                {
                    Directory.CreateDirectory(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path), folder)));
                }
                var dirinfo = new DirectoryInfo(path + @".Theseus\");
                dirinfo.Attributes |= FileAttributes.Hidden;
                File.Copy(template.IconFilePath, Path.GetFullPath(Path.Combine(dirinfo.FullName, "Icon.png")));
                File.Copy(template.ScreenshotFilePath, Path.GetFullPath(Path.Combine(dirinfo.FullName, "Screenshot.png")));

                var projectXML = File.ReadAllText(template.ProjectFilePath);
                projectXML = string.Format(projectXML, ProjectName, ProjectPath);
                var projectPath = Path.GetFullPath(Path.Combine(path, $"{ProjectName}{Project.Extension}"));
                File.WriteAllText(projectPath, projectXML); 

                return path;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.Log(MessageType.Error, $"Failed to create {ProjectName}");
                throw;
            }
         }
        
        public NewProjectClass()
        {
            ProjectTemplates = new ReadOnlyObservableCollection<ProjectTemplate>(_projectTemplates);
            try
            {
                var templatesFiles = Directory.GetFiles(_templatePath, "template.xml", SearchOption.AllDirectories);
                Debug.Assert(templatesFiles.Any());
                foreach (var file in templatesFiles)
                {
                    var template = Utilities.Serializer.FromFile<ProjectTemplate>(file);
                    template.IconFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "Icon.png"));
                    template.Icon = File.ReadAllBytes(template.IconFilePath);
                    template.ScreenshotFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "Screenshot.png"));
                    template.Screenshot = File.ReadAllBytes(template.ScreenshotFilePath);
                    template.ProjectFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), template.ProjectFile));

                    _projectTemplates.Add(template);
                }
                ValidateProjectPath();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.Log(MessageType.Error, $"Failed to read project templates");
                throw;
            }
        }
    }
}
